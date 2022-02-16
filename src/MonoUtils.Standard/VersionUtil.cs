using System;
using System.Text.RegularExpressions;

namespace MonoUtilities.Validation
{
    public class VersionFormatException: Exception
    {
        public VersionFormatException(string message): base(message){}
    }
    /// <summary>
    /// Class of helpers to perform checks and operations on versions with a major.minor.patch-suffix format where the suffix is optional.
    /// </summary>
    public class VersionUtil
    {

        /// <summary>
        /// Checks if version is newer than compareAgainst
        /// </summary>
        /// <param name="version">The version to check</param>
        /// <param name="compareAgainst">The version to check against</param>
        /// <returns>true if the version is newer. A non suffixed version will always be considered newer than a suffixed one with the same version number</returns>
        /// <exception cref="VersionFormatException">The format of version or compareAgainst was of the wrong format</exception>
        public static bool IsVersionNewer(string version, string compareAgainst)
        {
            bool compareEmpty = string.IsNullOrEmpty(compareAgainst);
            bool versionEmpty = string.IsNullOrEmpty(version);

            if (!versionEmpty && compareEmpty)
            {
                return true;
            }

            if (!compareEmpty && versionEmpty)
            {
                return false;
            }
            
           return IsVersionNewer(version.ToVersion(), compareAgainst.ToVersion());
        }

        /// <summary>
        /// Checks if version is newer than compareAgainst
        /// </summary>
        /// <param name="version">The version to check</param>
        /// <param name="compareAgainst">The version to check against</param>
        /// <returns>true if the version is newer. A non suffixed version will always be considered newer than a suffixed one with the same version number</returns>
        public static bool IsVersionNewer(VersionInfo version, VersionInfo compareAgainst)
        {
            return version > compareAgainst;
        }

        /// <summary>
        /// Creates a new version object. Identical to calling new VersionInfo(version)
        /// </summary>
        /// <param name="version">version string</param>
        /// <returns>VersionInfo object</returns>
        public static VersionInfo MakeVersionInfo(string version)
        {
            return new VersionInfo(version);
        }

        /// <summary>
        /// Returns the major part of the version string with format major.minor.patch-suffix
        /// </summary>
        /// <param name="version">The version to extract the major part from</param>
        /// <returns>The major part of the version</returns>
        /// <exception cref="VersionFormatException">The format of the version string was not of major.minor.patch-suffix</exception>
        public static string ExtractMajor(string version)
        {
            Regex regex = new Regex("^[0-9]+.[0-9]+.[0-9]+(-[a-z-A-Z-0-9]+)*$");

            if (!regex.IsMatch(version))
            {
                throw new VersionFormatException($"version {version} is not a valid version number");
            }

            string[] versionArray = version.Split('.', '-');            

            if (versionArray.Length < 3 || versionArray.Length > 4)
            {
                throw new VersionFormatException($"Got array size of {versionArray.Length} for {version} (should be 3 or 4)");
            }

            return versionArray[0];
        }

        /// <summary>
        /// Returns the minor part of the version string with format major.minor.patch-suffix
        /// </summary>
        /// <param name="version">The version to extract the minor part from</param>
        /// <returns>The minor part of the version</returns>
        /// <exception cref="VersionFormatException">The format of the version string was not of major.minor.patch-suffix</exception>
        public static string ExtractMinor(string version)
        {
            Regex regex = new Regex("^[0-9]+.[0-9]+.[0-9]+(-[a-z-A-Z-0-9]+)*$");

            if (!regex.IsMatch(version))
            {
                throw new VersionFormatException($"version {version} is not a valid version number");
            }

            string[] versionArray = version.Split('.', '-');

            if (versionArray.Length < 3 || versionArray.Length > 4)
            {
                throw new VersionFormatException($"Got array size of {versionArray.Length} for {version} (should be 3 or 4)");
            }

            return versionArray[1];
        }

        /// <summary>
        /// Returns the patch part of the version string with format major.minor.patch-suffix
        /// </summary>
        /// <param name="version">The version to extract the patch part from</param>
        /// <returns>The patch part of the version</returns>
        /// <exception cref="VersionFormatException">The format of the version string was not of major.minor.patch-suffix</exception>
        public static string ExtractPatch(string version)
        {
            Regex regex = new Regex("^[0-9]+.[0-9]+.[0-9]+(-[a-z-A-Z-0-9]+)*$");

            if (!regex.IsMatch(version))
            {
                throw new VersionFormatException($"version {version} is not a valid version number");
            }

            string[] versionArray = version.Split('.', '-');

            if (versionArray.Length < 3 || versionArray.Length > 4)
            {
                throw new VersionFormatException($"Got array size of {versionArray.Length} for {version} (should be 3 or 4)");
            }

            return versionArray[2];
        }

        /// <summary>
        /// Returns the suffix part of the version string with format major.minor.patch-suffix
        /// </summary>
        /// <param name="version">The version to extract the suffix part from</param>
        /// <returns>The suffix part of the version, or empty string if version is missing a suffix</returns>
        /// <exception cref="VersionFormatException">The format of the version string was not of major.minor.patch-suffix</exception>
        public static string ExtractSuffix(string version)
        {
            Regex regex = new Regex("^[0-9]+.[0-9]+.[0-9]+(-[a-z-A-Z-0-9]+)*$");

            if (!regex.IsMatch(version))
            {
                throw new VersionFormatException($"version {version} is not a valid version number");
            }

            string[] versionArray = version.Split('.', '-');

            if (versionArray.Length < 3 || versionArray.Length > 4)
            {
                throw new VersionFormatException($"Got array size of {versionArray.Length} for {version} (should be 3 or 4)");
            }

            if (versionArray.Length < 4)
            {
                return "";
            }
            else
            {
                return versionArray[3];
            }
        }
    }
}
