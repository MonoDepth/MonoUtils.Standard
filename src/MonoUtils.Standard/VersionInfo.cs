using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MonoUtilities.Validation
{
    public class VersionInfo
    {
        public string Major { get; private set; }
        public string Minor { get; private set; }
        public string Patch { get; private set; }
        public string Suffix { get; private set; }

        public VersionInfo(string version)
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

            Major = versionArray[0];
            Minor = versionArray[1];
            Patch = versionArray[2];
            Suffix = versionArray.Length == 4 ? versionArray[3] : "";            
        }

        public static bool operator >(VersionInfo a, VersionInfo b)
        {
            int majorCompare = string.Compare(a.Major, b.Major, StringComparison.InvariantCultureIgnoreCase);

            if (majorCompare == 1)
            {
                return true;
            }

            int minorCompare = string.Compare(a.Minor, b.Minor, StringComparison.InvariantCultureIgnoreCase);


            if (majorCompare == 0 && minorCompare == 1)
            {
                return true;
            }

            int patchCompare = string.Compare(a.Patch, b.Patch, StringComparison.InvariantCultureIgnoreCase);

            if (majorCompare == 0 && minorCompare == 0 && patchCompare == 1)
            {
                return true;
            }

            if (majorCompare == 0 && minorCompare == 0 && patchCompare == 0) {
                return ((a.Suffix == "" && b.Suffix != "") || string.Compare(a.Suffix.ToUpperInvariant(), b.Suffix.ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase) == 1);
            }

            return false;

        }

        public static bool operator <(VersionInfo a, VersionInfo b)
        {
            return b > a;
        }

        public static bool operator <=(VersionInfo a, VersionInfo b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(VersionInfo a, VersionInfo b)
        {
            return a > b || a == b;
        }

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            return EqualityComparer<VersionInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is VersionInfo info &&
                   Major == info.Major &&
                   Minor == info.Minor &&
                   Patch == info.Patch &&
                   Suffix.ToUpperInvariant() == info.Suffix.ToUpperInvariant();
        }

        public override int GetHashCode()
        {
            int hashCode = -261206211;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Major);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Minor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Patch);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Suffix);
            return hashCode;
        }

        public override string ToString()
        {
            string version = $"{Major}.{Minor}.{Patch}";
            if (Suffix != "")
                return version + $"-{Suffix}";
            return version;
        }
    }
    
    public static class VersionInfoExtensions
    {
        public static VersionInfo ToVersion(this string version)
        {
            return new VersionInfo(version);
        }
    }
}
