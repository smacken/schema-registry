namespace SchemaRegistry
{
    using System;
    using System.Linq;

    public static class VersionParser
    {
        public static string GetLatestVersion(string[] versions)
        {
            if (versions.Length == 0) throw new ArgumentException("no versions");
            if (versions.Length == 1) return versions[0];
            if (versions.All(VersionIsNumber))
            {
                var maxVersion = versions
                    .Select(int.Parse)
                    .OrderByDescending(i => i);
                return maxVersion.First().ToString();
            }

            var latest = Parse(versions[0]);
            for (var i = 1; i < versions.Length; i++)
            {
                var current = Parse(versions[i]);
                if (current.CompareTo(latest) > 0)
                {
                    latest = current;
                }
            }

            return $"{latest.Major}.{latest.Minor}.{latest.Patch}";
        }
        
        public static bool VersionIsSemver(string versionText) => StringIsSemver(versionText);
        
        public static bool VersionIsNumber(string versionText) => int.TryParse(versionText, out _);
        
        public static bool StringIsSemver(string versionText)
        {
            try
            {
                Parse(versionText);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        
        public static SemanticVersion Parse(string versionText)
        {
            if(string.IsNullOrEmpty(versionText)) throw new ArgumentException("invalid version");
            var versionParts = versionText.Split('.');
            if(versionParts.Length < 2) throw new ArgumentException("invalid version");
            
            var major = int.Parse(versionParts[0]);
            var minor = int.Parse(versionParts[1]);
            
            if (versionParts.Length == 2) return new SemanticVersion(major, minor, 0);
            var patch = int.Parse(versionParts[2]);
            return new SemanticVersion(major, minor, patch);
        }
    }
    
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string? Label { get; }
        
        public SemanticVersion(int major, int minor, int patch, string? label = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Label = label;
        }

        public int CompareTo(SemanticVersion? other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
            {
                return majorComparison;
            }

            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
            {
                return minorComparison;
            }

            var patchComparison = Patch.CompareTo(other.Patch);
            if (patchComparison != 0)
            {
                return patchComparison;
            }

            return string.Compare(Label, other.Label, StringComparison.Ordinal);
        }
    }
}