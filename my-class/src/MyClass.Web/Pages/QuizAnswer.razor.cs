using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace  MyClass.Web.Pages;

public partial class QuizAnswer
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;
}