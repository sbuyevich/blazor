namespace student_projects.Auth;

public static class AuthenticationConstants
{
    public const string Scheme = "StudentProjectsCookie";
    public const string CookieName = "student-projects.auth";
    public const string LoginPath = "/login";
    public const string LogoutPath = "/logout";
    public const string AdminRole = "Admin";
    public const string AdminPolicy = "AdminOnly";
}
