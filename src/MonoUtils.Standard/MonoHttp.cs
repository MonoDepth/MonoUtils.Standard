using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;
using System.Reflection;

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
        public HttpClient Client { get; private set; }
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
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer, UseCookies = true};
            Client = new HttpClient(ClientHandler) { Timeout = requestTimeout };
            Client.Timeout = requestTimeout;
            SetupDefaults();
        }
        public MonoHttpClient(double requestTimeoutInMs, bool allowAutoRedirect = true)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer, UseCookies = true};
            Client = new HttpClient(ClientHandler) { Timeout = TimeSpan.FromMilliseconds(requestTimeoutInMs) };
            SetupDefaults();
            
        }

        private void SetupDefaults()
        {
            Client.DefaultRequestHeaders.Add("User-Agent", "MonoHTTP Client C# .NET Standard");
            postParams = new List<KeyValuePair<string, string>>();
            getParams = new List<KeyValuePair<string, string>>();
            clearAuth = false;
            SerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public void AddCookie(string url, string key, object value)
        {
            Cookie c = new Cookie(key, value.ToString());
            CookieContainer.Add(new Uri(url), c);
        }

        #region Async_Calls
        public async Task<HttpResponseMessage> PostAsync(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = await Client.PostAsync(url, content);
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
               jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await Client.PostAsync(url, content);
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<JsonResult<T>> PostAsync<T>(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
                jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");  
            HttpResponseMessage result = await Client.PostAsync(url, content);
            string json = await result.Content.ReadAsStringAsync();
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return new JsonResult<T>(result, obj);
        }

        public async Task<JsonResult<T>> PostAsync<T>(string url, bool clearParamsAfterResponse = true, JsonSerializerOptions serializerOptions = null)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = await Client.PostAsync(url, content);
            string json = await result.Content.ReadAsStringAsync();
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
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
            HttpResponseMessage result = await Client.GetAsync(url);
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<JsonResult<T>> GetAsync<T>(string url, bool clearParamsAfterResponse = true, JsonSerializerOptions serializerOptions = null)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await Client.GetAsync(url);
            string json = await result.Content.ReadAsStringAsync();
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            
            return new JsonResult<T>(result, obj);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await Client.DeleteAsync(url);
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
                jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");            
            HttpResponseMessage result = await Client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = content
            });

            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }
        #endregion

        #region Sync_Calls
        public HttpResponseMessage Post(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = Client.PostAsync(url, content).Result;
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public HttpResponseMessage Post(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
                jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = Client.PostAsync(url, content).Result;
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public JsonResult<T> Post<T>(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
                jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = Client.PostAsync(url, content).Result;
            string json = result.Content.ReadAsStringAsync().Result;
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return new JsonResult<T>(result, obj);
        }

        public JsonResult<T> Post<T>(string url, bool clearParamsAfterResponse = true, JsonSerializerOptions serializerOptions = null)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = Client.PostAsync(url, content).Result;
            string json = result.Content.ReadAsStringAsync().Result;
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearParamsAfterResponse)
            {
                postParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return new JsonResult<T>(result, obj);
        }

        public HttpResponseMessage Get(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = Client.GetAsync(url).Result;
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public JsonResult<T> Get<T>(string url, bool clearParamsAfterResponse = true, JsonSerializerOptions serializerOptions = null)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = Client.GetAsync(url).Result;
            string json = result.Content.ReadAsStringAsync().Result;
            T obj;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, serializerOptions ?? SerializerOptions);
            }
            catch
            {
                obj = default;
            }
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }

            return new JsonResult<T>(result, obj);
        }

        public HttpResponseMessage Delete(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i < getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 < getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = Client.DeleteAsync(url).Result;
            if (clearParamsAfterResponse)
            {
                getParams.Clear();
            }
            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        public HttpResponseMessage Delete(string url, object body, JsonSerializerOptions serializerOptions = null)
        {
            string jsonBody;
            if (typeof(object).IsPrimitive)
            {
                jsonBody = body.ToString();
            }
            else
            {
                jsonBody = JsonSerializer.Serialize(body, serializerOptions ?? SerializerOptions);
            }

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = Client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = content
            }).Result;

            if (clearAuth)
            {
                clearAuth = false;
                Client.DefaultRequestHeaders.Authorization = null;
            }
            return result;
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ClientHandler.Dispose();
                    Client.Dispose();
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
