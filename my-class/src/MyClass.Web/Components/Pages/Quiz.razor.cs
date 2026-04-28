using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace MyClass.Web.Components.Pages;

public partial class Quiz
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;
}