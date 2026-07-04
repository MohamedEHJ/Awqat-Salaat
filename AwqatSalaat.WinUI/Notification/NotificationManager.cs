using AwqatSalaat.Helpers;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Serilog;
using System;

namespace AwqatSalaat.WinUI.Notification
{
    internal static class NotificationManager
    {
        private const string ToastClickAction = "toast_click";
        private const string DismissReminderAction = "dismiss_remider";
        private const string StopReminderSoundAction = "mute_reminder";
        private const string DismissTimeEnteredNotificationAction = "dismiss_time_entered";
        private const string StopAdhanAction = "mute_adhan";

        private static bool m_isRegistered;

        public static bool IsRegistered => m_isRegistered;

        public static event Action ShowWidgetRequested;
        public static event Action DismissReminderRequested;
        public static event Action StopReminderSoundRequested;
        public static event Action StopAdhanRequested;

        public static void Init()
        {
            if (m_isRegistered) return;

            Log.Information("Initializing NotificationManager");

            // To ensure all Notification handling happens in this process instance, register for
            // NotificationInvoked before calling Register(). Without this a new process will
            // be launched to handle the notification.
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

#if PACKAGED
            AppNotificationManager.Default.Register();
#else
            string iconPath = App.GetFullPathToAsset("app_icon_32.png");
            AppNotificationManager.Default.Register(LocaleManager.Default.Get("Data.AppName"), new Uri(iconPath));
#endif
            m_isRegistered = true;

            Log.Information("NotificationManager is registered successfully");
        }

        public static void Unregister()
        {
            if (m_isRegistered)
            {
                AppNotificationManager.Default.Unregister();
                m_isRegistered = false;

                Log.Information("NotificationManager is unregistered successfully");
            }
        }

        public static bool SendReminderToast(string prayer, DateTime time, bool soundEnabled)
        {
            if (!m_isRegistered)
                return false;

#if DEBUG
            if (time < DateTime.Now)
                time = DateTime.Now.AddMinutes(1);
#endif

            Log.Information($"Sending reminder toast notification for {prayer}. SoundEnabled={soundEnabled}");

            var prayer_loc = LocaleManager.Default.Get("Data.Salaat." + prayer);
            var message = string.Format(LocaleManager.Default.Get("Notification.PrayerTimeSoonFormat"), prayer_loc);

            var appNotificationBuilder = new AppNotificationBuilder()
                .SetTag("reminder" + prayer)
                .SetScenario(AppNotificationScenario.Reminder)
                .AddArgument("action", ToastClickAction)
                .AddText(message)
                .AddButton(new AppNotificationButton(LocaleManager.Default.GetWithFallback("Notification.DismissReminder", "Notification.Dismiss"))
                    .AddArgument("action", DismissReminderAction));

            if (soundEnabled)
            {
                appNotificationBuilder
                    .AddButton(new AppNotificationButton(LocaleManager.Default.Get("Notification.StopSound"))
                    .AddArgument("action", StopReminderSoundAction));
            }

            var appNotification = appNotificationBuilder.BuildNotification();
            appNotification.Expiration = time;

            AppNotificationManager.Default.Show(appNotification);

            Log.Information($"Notification ID: {appNotification.Id}");

            return appNotification.Id != 0;
        }

        public static bool SendTimeEnteredToast(string prayer, bool adhanEnabled)
        {
            if (!m_isRegistered)
                return false;

            Log.Information($"Sending 'time entered' toast notification for {prayer}. AdhanEnabled={adhanEnabled}");

            var prayer_loc = LocaleManager.Default.Get("Data.Salaat." + prayer);
            var message = string.Format(LocaleManager.Default.Get("Notification.PrayerTimeEnteredFormat"), prayer_loc);

            var appNotificationBuilder = new AppNotificationBuilder()
                .SetTag("timeentered" + prayer)
                .AddArgument("action", ToastClickAction)
                .AddText(message)
                .AddButton(new AppNotificationButton(LocaleManager.Default.GetWithFallback("Notification.DismissTimeEntered", "Notification.Dismiss"))
                    .AddArgument("action", DismissTimeEnteredNotificationAction));

            if (adhanEnabled)
            {
                appNotificationBuilder
                    .AddButton(new AppNotificationButton(LocaleManager.Default.Get("Notification.StopSound"))
                    .AddArgument("action", StopAdhanAction));
            }

            var appNotification = appNotificationBuilder.BuildNotification();

            AppNotificationManager.Default.Show(appNotification);

            Log.Information($"Notification ID: {appNotification.Id}");

            return appNotification.Id != 0;
        }
        public static bool SendWidgetStillRunningToast()
        {
            if (!m_isRegistered)
                return false;

            Log.Information("Sending toast notification about tray icon");

            var appNotificationBuilder = new AppNotificationBuilder()
                .SetTag("widgetstillrunning")
                .SetScenario(AppNotificationScenario.Default)
                .AddText(LocaleManager.Default.Get("Data.AppName"))
                .AddText(LocaleManager.Default.Get("Notification.WidgetStillRunning"));

            var appNotification = appNotificationBuilder.BuildNotification();

            AppNotificationManager.Default.Show(appNotification);

            Log.Information($"Notification ID: {appNotification.Id}");

            return appNotification.Id != 0;
        }

        private static void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            Log.Information("Notification invoked");

            if (args.Arguments.TryGetValue("action", out string action))
            {
                Log.Information($"Notification's action: {action}");

                switch (action)
                {
                    case ToastClickAction:
                        ShowWidgetRequested?.Invoke();
                        break;
                    case DismissReminderAction:
                        DismissReminderRequested?.Invoke();
                        break;
                    case StopReminderSoundAction:
                        StopReminderSoundRequested?.Invoke();
                        break;
                    case DismissTimeEnteredNotificationAction:
                        // do nothing
                        break;
                    case StopAdhanAction:
                        StopAdhanRequested?.Invoke();
                        break;
                }
            }
        }
    }
}
