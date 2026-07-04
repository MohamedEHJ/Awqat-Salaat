using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace AwqatSalaat.WinUI.Media
{
    internal class DesktopAcrylicSystemBackdrop : SystemBackdrop
    {
        private DesktopAcrylicController acrylicController;
        private int count = 0;

        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            // Call the base method to initialize the default configuration object.
            base.OnTargetConnected(connectedTarget, xamlRoot);

            if (acrylicController is null)
            {
                acrylicController = new DesktopAcrylicController();
                // Set configuration.
                SystemBackdropConfiguration defaultConfig = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
                defaultConfig.IsInputActive = true;
                acrylicController.SetSystemBackdropConfiguration(defaultConfig);
            }

            // Add target.
            bool success = acrylicController.AddSystemBackdropTarget(connectedTarget);

            if (success)
            {
                count++;
            }
        }

        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);

            bool success = acrylicController.RemoveSystemBackdropTarget(disconnectedTarget);

            if (success && --count == 0)
            {
                acrylicController = null;
            }
        }
    }
}
