using AwqatSalaat.WinUI.Helpers;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;

namespace AwqatSalaat.WinUI.Controls
{
    public class FlyoutEx : Flyout
    {
        private bool isFirstTime = true;
        private Control flyoutPresenter;
        private Popup popup;

        public FlyoutEx()
        {
            this.Opened += FlyoutEx_Opened;
            this.Opening += FlyoutEx_Opening;
            ThemeHelper.ThemeChanged += ThemeHelper_ThemeChanged;
        }

        private void ThemeHelper_ThemeChanged()
        {
            if (DispatcherQueue.HasThreadAccess)
            {
                UpdateTheme();
            }
            else
            {
                DispatcherQueue.TryEnqueue(UpdateTheme);
            }
        }
        
        private void UpdateTheme()
        {
            if (popup is not null)
            {
                popup.RequestedTheme = ThemeHelper.GeneralTheme;
                flyoutPresenter.RequestedTheme = popup.ActualTheme;
            }
        }

        public void DisableLightDismissTemporarily()
        {
            if (flyoutPresenter?.Parent is Popup popup)
            {
                popup.IsLightDismissEnabled = false;

                Task.Delay(100).ContinueWith((task, state) =>
                {
                    if (state is FlyoutEx flyout)
                    {
                        flyout.DispatcherQueue.TryEnqueue(() => (flyout.flyoutPresenter.Parent as Popup).IsLightDismissEnabled = true);
                    }
                },
                this);
            }
        }

        public Control GetPresenter() => flyoutPresenter;

        protected override Control CreatePresenter()
        {
            var presenter = base.CreatePresenter();

            var displayArea = DisplayArea.GetFromWindowId(XamlRoot.ContentIslandEnvironment.AppWindowId, DisplayAreaFallback.Primary);
            double maxPresenterHeight = displayArea.WorkArea.Height / XamlRoot.RasterizationScale - 8;
            double maxPresenterWidth = displayArea.WorkArea.Width / XamlRoot.RasterizationScale - 4;

            if (presenter.MaxHeight > maxPresenterHeight)
            {
                presenter.MaxHeight = maxPresenterHeight;
            }

            if (presenter.MaxWidth > maxPresenterWidth)
            {
                presenter.MaxWidth = maxPresenterWidth;
            }

            flyoutPresenter = presenter;

            return presenter;
        }

        private void FlyoutEx_Opening(object sender, object e)
        {
            ThemeHelper_ThemeChanged();
        }
        
        private void FlyoutEx_Opened(object sender, object e)
        {
            if (isFirstTime)
            {
                popup = flyoutPresenter.Parent as Popup;
                popup.GotFocus += (_, _) => flyoutPresenter?.Focus(FocusState.Programmatic);
                isFirstTime = false;

                ThemeHelper_ThemeChanged();
            }
        }
    }
}
