using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;
using MyClass.Core.Options;
using ClassEntity = MyClass.Core.Data.Entities.Class;

namespace MyClass.Core.Services;

public sealed class SchoolClassService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IOptions<TeacherOptions> teacherOptions) : ISchoolClassService
{
    public async Task<SchoolClassListResult> GetSchoolClassesAsync(
        LoginState? loginState,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassListResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var schools = await dbContext.Schools
            .AsNoTracking()
            .OrderBy(school => school.Name)
            .Select(school => new SchoolClassSchoolItem(
                school.Id,
                school.Name,
                school.Classes.Count,
                school.Classes
                    .OrderBy(@class => @class.Name)
                    .ThenBy(@class => @class.Code)
                    .Select(@class => new SchoolClassClassItem(
                        @class.Id,
                        @class.SchoolId,
                        @class.Name,
                        @class.Code,
                        @class.Students.Count))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return SchoolClassListResult.Success(schools);
    }

    public async Task<SchoolClassClassListResult> GetClassesForSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassClassListResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var schoolExists = await dbContext.Schools
            .AnyAsync(school => school.Id == schoolId, cancellationToken);

        if (!schoolExists)
        {
            return SchoolClassClassListResult.Failure("The selected school was not found.");
        }

        var classes = await dbContext.Classes
            .AsNoTracking()
            .Where(@class => @class.SchoolId == schoolId)
            .OrderBy(@class => @class.Name)
            .ThenBy(@class => @class.Code)
            .Select(@class => new SchoolClassClassItem(
                @class.Id,
                @class.SchoolId,
                @class.Name,
                @class.Code,
                @class.Students.Count))
            .ToListAsync(cancellationToken);

        return SchoolClassClassListResult.Success(classes);
    }

    public async Task<SchoolClassActionResult> CreateSchoolAsync(
        LoginState? loginState,
        string name,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return SchoolClassActionResult.Failure("School name is required.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var school = new School
        {
            Name = normalizedName
        };

        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync(cancellationToken);

        return SchoolClassActionResult.Success("School added.", school.Id);
    }

    public async Task<SchoolClassActionResult> UpdateSchoolAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return SchoolClassActionResult.Failure("School name is required.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var school = await dbContext.Schools
            .SingleOrDefaultAsync(school => school.Id == schoolId, cancellationToken);

        if (school is null)
        {
            return SchoolClassActionResult.Failure("The selected school was not found.");
        }

        school.Name = normalizedName;
        await dbContext.SaveChangesAsync(cancellationToken);

        return SchoolClassActionResult.Success("School updated.", school.Id);
    }

    public async Task<SchoolClassActionResult> DeleteSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var school = await dbContext.Schools
            .SingleOrDefaultAsync(school => school.Id == schoolId, cancellationToken);

        if (school is null)
        {
            return SchoolClassActionResult.Failure("The selected school was not found.");
        }

        var hasClasses = await dbContext.Classes
            .AnyAsync(@class => @class.SchoolId == schoolId, cancellationToken);

        if (hasClasses)
        {
            return SchoolClassActionResult.Failure("Remove this school's classes before deleting it.");
        }

        dbContext.Schools.Remove(school);
        await dbContext.SaveChangesAsync(cancellationToken);

        return SchoolClassActionResult.Success("School deleted.");
    }

    public async Task<SchoolClassActionResult> CreateClassAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        string code,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        var validationMessage = ValidateClassInput(name, code, out var normalizedName, out var normalizedCode);

        if (validationMessage is not null)
        {
            return SchoolClassActionResult.Failure(validationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var schoolExists = await dbContext.Schools
            .AnyAsync(school => school.Id == schoolId, cancellationToken);

        if (!schoolExists)
        {
            return SchoolClassActionResult.Failure("A valid school is required before adding a class.");
        }

        if (await ClassCodeExistsAsync(dbContext, normalizedCode, null, cancellationToken))
        {
            return SchoolClassActionResult.Failure("That class code is already in use.");
        }

        var newClass = new ClassEntity
        {
            SchoolId = schoolId,
            Name = normalizedName,
            Code = normalizedCode
        };

        dbContext.Classes.Add(newClass);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return SchoolClassActionResult.Failure("That class code is already in use.");
        }

        return SchoolClassActionResult.Success("Class added.", newClass.Id);
    }

    public async Task<SchoolClassActionResult> UpdateClassAsync(
        LoginState? loginState,
        int classId,
        string name,
        string code,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        var validationMessage = ValidateClassInput(name, code, out var normalizedName, out var normalizedCode);

        if (validationMessage is not null)
        {
            return SchoolClassActionResult.Failure(validationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingClass = await dbContext.Classes
            .SingleOrDefaultAsync(@class => @class.Id == classId, cancellationToken);

        if (existingClass is null)
        {
            return SchoolClassActionResult.Failure("The selected class was not found.");
        }

        if (await ClassCodeExistsAsync(dbContext, normalizedCode, classId, cancellationToken))
        {
            return SchoolClassActionResult.Failure("That class code is already in use.");
        }

        existingClass.Name = normalizedName;
        existingClass.Code = normalizedCode;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return SchoolClassActionResult.Failure("That class code is already in use.");
        }

        return SchoolClassActionResult.Success("Class updated.", existingClass.Id);
    }

    public async Task<SchoolClassActionResult> DeleteClassAsync(
        LoginState? loginState,
        int classId,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingClass = await dbContext.Classes
            .SingleOrDefaultAsync(@class => @class.Id == classId, cancellationToken);

        if (existingClass is null)
        {
            return SchoolClassActionResult.Failure("The selected class was not found.");
        }

        var hasStudents = await dbContext.Students
            .AnyAsync(student => student.ClassId == classId, cancellationToken);

        if (hasStudents)
        {
            return SchoolClassActionResult.Failure("Remove this class's students before deleting it.");
        }

        dbContext.Classes.Remove(existingClass);
        await dbContext.SaveChangesAsync(cancellationToken);

        return SchoolClassActionResult.Success("Class deleted.");
    }

    public async Task<SchoolClassStudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        int classId,
        string? searchText = null,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassStudentListResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var classExists = await dbContext.Classes
            .AnyAsync(@class => @class.Id == classId, cancellationToken);

        if (!classExists)
        {
            return SchoolClassStudentListResult.Failure("The selected class was not found.");
        }

        var query = dbContext.Students
            .AsNoTracking()
            .Where(student => student.ClassId == classId);

        var normalizedSearchText = searchText?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearchText))
        {
            query = query.Where(student => student.DisplayName.Contains(normalizedSearchText));
        }

        var students = await query
            .OrderBy(student => student.DisplayName)
            .ThenBy(student => student.UserName)
            .Select(student => new SchoolClassStudentItem(
                student.Id,
                student.UserName,
                student.DisplayName,
                student.IsActive,
                student.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return SchoolClassStudentListResult.Success(students);
    }

    public async Task<SchoolClassActionResult> RemoveStudentFromClassAsync(
        LoginState? loginState,
        int classId,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState);

        if (authorizationMessage is not null)
        {
            return SchoolClassActionResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var student = await dbContext.Students
            .SingleOrDefaultAsync(
                student =>
                    student.Id == studentId &&
                    student.ClassId == classId,
                cancellationToken);

        if (student is null)
        {
            return SchoolClassActionResult.Failure("The selected student was not found in this class.");
        }

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(cancellationToken);

        return SchoolClassActionResult.Success($"{student.DisplayName} was removed.", student.Id);
    }

    private static string? ValidateClassInput(
        string name,
        string code,
        out string normalizedName,
        out string normalizedCode)
    {
        normalizedName = name.Trim();
        normalizedCode = NormalizeClassCode(code);

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return "Class name is required.";
        }

        return string.IsNullOrWhiteSpace(normalizedCode)
            ? "Class code is required."
            : null;
    }

    private static string NormalizeClassCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    private static Task<bool> ClassCodeExistsAsync(
        ApplicationDbContext dbContext,
        string normalizedCode,
        int? excludedClassId,
        CancellationToken cancellationToken)
    {
        return dbContext.Classes.AnyAsync(
            @class =>
                @class.Code == normalizedCode &&
                (!excludedClassId.HasValue || @class.Id != excludedClassId.Value),
            cancellationToken);
    }

    private string? ValidateTeacherAccess(LoginState? loginState)
    {
        if (loginState is null)
        {
            return "Sign in as the teacher to manage schools and classes.";
        }

        if (!loginState.IsTeacher)
        {
            return "Only teachers can manage schools and classes.";
        }

        var teacher = teacherOptions.Value;

        return string.Equals(loginState.UserName, teacher.UserName, StringComparison.OrdinalIgnoreCase)
            ? null
            : "Teacher login is required to manage schools and classes.";
    }
}
