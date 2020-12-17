using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoUtilities.Http
{
    public class MonoHttpClient : IDisposable
    {
        /*
        * TODO:
        * Add method to add headers (referer, etc)
        */

        private List<KeyValuePair<string, string>> postParams;
        private List<KeyValuePair<string, string>> getParams;
        private bool clearAuth;
        private bool disposedValue;

        public JsonSerializerOptions SerializerOptions { get; set; }
        public HttpClient HttpClient { get; private set; }
        public HttpClientHandler ClientHandler { get; private set; }

        public CookieContainer CookieContainer { get; private set; }

        public struct JsonResult<TJsonObject>
        {
            public HttpResponseMessage Response { get; private set; }
            public TJsonObject JsonObject { get; private set; }
            public JsonResult(HttpResponseMessage httpResponse, TJsonObject jsonObject)
            {
                Response = httpResponse;
                JsonObject = jsonObject;
            }
        }

        public MonoHttpClient(TimeSpan requestTimeout, bool allowAutoRedirect = true)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer };
            HttpClient = new HttpClient(ClientHandler) { Timeout = requestTimeout };
            HttpClient.Timeout = requestTimeout;
            SetupDefaults();
        }
        public MonoHttpClient(double requestTimeoutInMs, bool allowAutoRedirect = true)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer };
            HttpClient = new HttpClient(ClientHandler) { Timeout = TimeSpan.FromMilliseconds(requestTimeoutInMs) };
            SetupDefaults();
            
        }

        private void SetupDefaults()
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "MonoHTTP Client C# .NET Standard");
            postParams = new List<KeyValuePair<string, string>>();
            getParams = new List<KeyValuePair<string, string>>();
            clearAuth = false;
            SerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
        }

        public void AddPostParameter(string paramName, object paramValue)
        {
            postParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearPostParameters()
        {
            postParams.Clear();
        }

        public void AddGetParameter(string paramName, object paramValue)
        {
            getParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearGetParameters()
        {
            getParams.Clear();
        }

        public void AddBasicAuth(string username, string password, bool clearAfterCall)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public void AddCookie(string url, string key, object value)
        {
            Cookie c = new Cookie(key, value.ToString());
            CookieContainer.Add(new Uri(url), c);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = await HttpClient.PostAsync(url, content);
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                HttpClient.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<JsonResult<T>> PostAsync<T>(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = await HttpClient.PostAsync(url, content);
            string json = await result.Content.ReadAsStringAsync();
            T obj = JsonSerializer.Deserialize<T>(json, SerializerOptions);
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                HttpClient.DefaultRequestHeaders.Authorization = null;
            }
            return new JsonResult<T>(result, obj);
        }

        public async Task<HttpResponseMessage> GetAsync(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await HttpClient.GetAsync(url);
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                HttpClient.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<JsonResult<T>> GetAsync<T>(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await HttpClient.GetAsync(url);
            string json = await result.Content.ReadAsStringAsync();
            T obj = JsonSerializer.Deserialize<T>(json, SerializerOptions);
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                HttpClient.DefaultRequestHeaders.Authorization = null;
            }
            
            return new JsonResult<T>(result, obj);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ClientHandler.Dispose();
                    HttpClient.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
