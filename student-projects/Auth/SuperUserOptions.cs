namespace student_projects.Auth;

public sealed class SuperUserOptions
{
    public const string SectionName = "SuperUser";
    public const string DefaultUserName = "teacher";
    public const string DefaultPassword = "Teacher123!";

    public string UserName { get; set; } = DefaultUserName;

    public string Password { get; set; } = DefaultPassword;
}
