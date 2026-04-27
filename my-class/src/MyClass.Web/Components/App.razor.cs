using Microsoft.AspNetCore.Components;
using MyClass.Core.Services.ClassContext;

namespace MyClass.Components;

public partial class App : ComponentBase
{
    public static ClassContext? CurrentClass { get; set; }
}
