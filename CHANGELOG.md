### v4.2.1

- Avoid the port exhaustion problem in service clients.
- Implemented a fix for the re-rentrancy bug caused by TileNotification in the widget's button Loaded event. (WinUI through Microsoft Store only)

### v4.2

- Support multi-display desktops. (WinUI only)
- Show/hide the location in the prayers panel.
- Show prayer time and countdown on the lock screen. (WinUI through Microsoft Store only)
- Control the widget's button background. (Only solid color is supported for now) (WinUI only)
- Added a button to auto-detect the location from the system at any time.
- Ignore text size scale from Windows Accessibility settings.
- Show a toast notification after hiding the widget. (WinUI only)
- Updated dismiss button text in reminder notification and in time entered notification. (WinUI only)
- Show clearer process architecture in the **About** page. (WinUI only)
- Improved the performance of AcrylicWindow when dragging the window. (Deskband only)
- Fixed a bug in QCH service when the widget starts between Isha and midnight.
- Fixed re-entrancy bug in ToggleButton. (Not confirmed yet) (WinUI only)
- Fixed a bug in DesktopAcrylicSystemBackdrop. (WinUI only)
- Fixed a bug in LocaleManager during settings loading.
- Renamed CustomizedFlyout and CustomizedMenuFlyout to FlyoutEx and MenuFlyoutEx respectively. (WinUI only)

### v4.1.2

- Implement a workaround for QCH service to show fake data after Isha before getting fresh data at midnight.
- Ensure early initialization of LocaleManager to show localized error messages at startup.
- Solve the User-Agent issue in the Salah Hour service.
- Fix crashing when failing to create en-SA culture info. Mostly on Windows 7/8/8.1.
- Fix the pop-up transparency issue on Windows 7/8/8.1. (Deskband only)
- Fix issue with eastern Arabic numbers when the OS uses western Arabic numbers. (WinUI through Microsoft Store only)
- Rename L11nExtension to I18nExtension. Oops!

### v4.1.1

- Add Kurdish translation.
- Add supported languages to package manifest. (WinUI only)
- Fix MessageBox crash before widget initialization. (WinUI only)

### v4.1

- Support ARM64 architecture. (Untested!) (WinUI only)
- Add a new service to import times from a CSV file.
- Add a new service to scrape times from Qatar Calendar House (QCH) website. (Limited)
- Add Turkish translation.
- Implement manual theme selection.
- Implement manual accent color selection.
- Implement auto-geolocation at the first use of the widget.
- Per-prayer-time adhan sound.
- Implement toast notifications. (WinUI only)
- Implement auto-alignment for taskbar button elements when placed near the corner. (WinUI only)
- Add a new style for the prayer times panel. (WinUI only)
- Control ayah visibility in the prayer times panel.
- Control the opacity of the notification indicator in the taskbar button.
- Control the repetition of the notification sound.
- Control the time format (12h or 24h).
- Show/hide Gregorian date in prayers panel.
- Increase the limit of prayer time adjustment to 60 minutes.
- Make **Location** tab selected when the widget is opened for the first time.
- Validate service-related settings before saving.
- Improve the times column in the prayer times panel.
- Improve searching for places using Nominatim.
- Keep Settings panel open when changing the settings **Show countdown** and **Compact mode**. (WinUI only)
- Add two beautiful adhan voices:
  - Sheikh Nasser Al-Qatami
  - Abdul Majeed Al-Suraihi
- Fix a bug in switching to the next day after _Isha_ when offline.
- Fix hang issue in UIAutomation when the taskbar is hidden and the Widgets button is enabled. (WinUI only)

### v4.0.1

- Cancel updates checking when closing Settings panel.
- Fix crashing due to negative prayer time adjustment.
- Fix volume decrease after playing an audio the first time. (Deskband only)
- Enable caching the content of the pages in **More info** window. (WinUI only)
- Implement workarounds to the issues caused by Start11 v2. (WinUI only)

### v4.0

- Add an offline service to calculate prayer times.
- Add adhan.
- Add per-time settings.
- Add prayers calendar which can be printed or exported as a PNG image.
- Add an informational page to learn how prayers times are determined.
- Add logging. (Disabled by default)
- Rename **Islamic Finder** service to **Salah Hour**.
- Adapt to taskbar color when it uses accent color.
- Avoid infecting current settings while making changes in Settings panel.
- Allow the user to retry injecting the widget if that failed. (WinUI only)
- Improve highlighted text color for the light theme.
- Make the widget close itself during update/uninstall and restart after update. (WinUI only)
- Improve interaction with `UIAutomation`. (WinUI only)
- Enable animations in Settings panel. (WinUI only)
- Improve UX of radio button. (Deskband only)
- Fix translation issue in dropdowns. (WinUI only)
- Fix updates detection from Microsoft Store. (WinUI only)

### v3.3.1

- Fix crashing after failing to inject the widget into the taskbar. (WinUI only)
- Fix bug of wrong detection of Windows display language. (WinUI only)

### v3.3

- Make the widget available on Microsoft Store. (WinUI only)
- Allow dragging the widget to custom position. (WinUI only)
- Add a setting to control Hijri date.
- Add updates checking in About page.
- Enable Acrylic background. (WinUI only)
- Improve widget positioning. (WinUI only)
- Fix navigation using Tab key. (Deskband only)
- Make MessageBox dialogs respect current language direction.
- Show a hint when the current service is down to suggest a potential solution.
- Improve location search using Nominatim.
- Various bug-fixes.

### v3.2

- Add **Compact mode**.
- Show Shuruq time.
- Add option for showing elapsed time.
- Add option for playing sound during notification period.
- Make widget's pane on the taskbar display the texts correctly.
- Improve widget positioning. (WinUI only)
- Fix the issue of time jumps when Windows wake up from sleep/hibernation.
- Minor bug-fixes.

### v3.1

- Add the option of selecting juristic school for Asr prayer time.
- Display seconds in countdown.
- Make time format respect system configuration (12h or 24h).
- Unify calculation methods.
- Redesign the Settings panel.
- Add About tab in Settings panel.
- Hide Gregorian date in main panel.
- Avoid overlapping other widgets injected into the taskbar. (WinUI only)
- Make context-menu and system tray icon menu respect language direction. (WinUI only)
- Make widget's pane on the taskbar respects DPI scale. (WinUI only)
- Fix widget's vertical position for 2-in-1 devices. (WinUI only)
- Minor bug-fixes.

### v3.0

- Add Awqat Salaat WinUI app to support Windows 11.
- Fix a critical error in date serialization on systems that use different calendars other than Gregorian.
- Fix settings loss after the installation of Windows Updates (such as cumulative updates).
- Show Hijri date in English when Display language is not set to Arabic.

### v2.0.1

- Fix a typo in **Fajr** prayer English name.
- Stabilize version numbers calculation

### v2.0

- Add Al-Adhan's Prayer Times API.

### v1.1.3

- Show clearer error message when Islamic Finder's Prayer Times API is down.

### v1.1

- Fix compatibility issues with Windows 7.

### v1.0

- Initial release.