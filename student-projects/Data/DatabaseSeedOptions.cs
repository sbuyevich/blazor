namespace student_projects.Data;

public sealed class DatabaseSeedOptions
{
    public const string SectionName = "SeedUser";
    public const string DefaultUserName = "student";
    public const string DefaultPassword = "Password123!";

    public string UserName { get; set; } = DefaultUserName;

    public string Password { get; set; } = DefaultPassword;
}
