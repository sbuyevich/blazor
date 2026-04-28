using Microsoft.AspNetCore.Components;
using MyClass.Core.Services.ClassContext;

namespace MyClass.Web.Components.Pages;

public partial class QuizAnswer
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;
}