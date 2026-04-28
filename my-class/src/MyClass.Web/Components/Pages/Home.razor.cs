using Microsoft.AspNetCore.Components;
using MyClass.Core.Services.ClassContext;

namespace MyClass.Web.Components.Pages;

public partial class Home
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;
}