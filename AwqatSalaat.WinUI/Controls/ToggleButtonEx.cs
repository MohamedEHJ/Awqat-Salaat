using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace AwqatSalaat.WinUI.Controls
{
    internal class ToggleButtonEx : ToggleButton
    {
        public ToggleButtonEx()
        {
            DefaultStyleKey = typeof(ToggleButtonEx);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }
    }
}
