namespace student_projects.Auth;

public static class AuthNavigation
{
    public static string BuildLoginPath(string? returnUrl)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl, fallback: null);
        return string.IsNullOrWhiteSpace(normalizedReturnUrl)
            ? AuthenticationConstants.LoginPath
            : $"{AuthenticationConstants.LoginPath}?returnUrl={Uri.EscapeDataString(normalizedReturnUrl)}";
    }

    public static string NormalizeReturnUrl(string? returnUrl, string? fallback = "/projects")
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return fallback ?? string.Empty;
        }

        if (!returnUrl.StartsWith("/", StringComparison.Ordinal) ||
            returnUrl.StartsWith("//", StringComparison.Ordinal) ||
            returnUrl.StartsWith("/\\", StringComparison.Ordinal) ||
            returnUrl.Equals(AuthenticationConstants.LoginPath, StringComparison.OrdinalIgnoreCase) ||
            returnUrl.Equals(AuthenticationConstants.LogoutPath, StringComparison.OrdinalIgnoreCase))
        {
            return fallback ?? string.Empty;
        }

        return returnUrl;
    }
}
