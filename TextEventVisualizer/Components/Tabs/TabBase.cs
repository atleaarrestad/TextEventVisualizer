using Microsoft.AspNetCore.Components;

namespace TextEventVisualizer.Components.Tabs;
public class TabBase : ComponentBase
{
    public bool IsActive { get; set; }
    public string ActiveClass => IsActive ? "active" : null;
}
