using System.Globalization;

namespace Memorio.Api.AppVersion;

/// <summary>
/// An app version "major.minor.patch". Pre-release and build suffixes ("-beta", "+12") are ignored,
/// so "1.4.0+12" equals "1.4.0".
/// </summary>
internal readonly record struct AppVersionNumber(int Major, int Minor, int Patch) : IComparable<AppVersionNumber>
{
    public static bool TryParse(string? text, out AppVersionNumber version)
    {
        version = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var parts = text.Trim().Split('+', '-')[0].Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        var numbers = new int[3];
        for (var i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[i]))
            {
                return false;
            }
        }

        version = new AppVersionNumber(numbers[0], numbers[1], numbers[2]);
        return true;
    }

    public int CompareTo(AppVersionNumber other) =>
        (Major, Minor, Patch).CompareTo((other.Major, other.Minor, other.Patch));

    public static bool operator <(AppVersionNumber left, AppVersionNumber right) => left.CompareTo(right) < 0;

    public static bool operator >(AppVersionNumber left, AppVersionNumber right) => left.CompareTo(right) > 0;

    public static bool operator <=(AppVersionNumber left, AppVersionNumber right) => left.CompareTo(right) <= 0;

    public static bool operator >=(AppVersionNumber left, AppVersionNumber right) => left.CompareTo(right) >= 0;

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
