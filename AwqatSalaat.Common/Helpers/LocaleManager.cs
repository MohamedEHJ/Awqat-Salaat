using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace AwqatSalaat.Helpers
{
    public class LocaleManager : INotifyPropertyChanged
    {
        private static readonly List<string> AvailableLocales = new List<string>() { "ar", "en", "tr", "ku" };

        public static LocaleManager Default { get; } = new LocaleManager();


        private string _current;

        public string Current { get => _current; set => SetLocale(value); }
        public CultureInfo CurrentCulture { get; private set; }

        public event EventHandler CurrentChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private LocaleManager()
        {
            var lang = Properties.Settings.Default.DisplayLanguage;

            if (string.IsNullOrEmpty(lang))
            {
                lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            SetLocale(lang);

            Properties.Settings.Realtime.PropertyChanged += Settings_PropertyChanged;
        }

        public string Get(string key)
        {
            var culture = CurrentCulture.ThreeLetterISOLanguageName == "ckb"
                ? CultureInfo.GetCultureInfo("ku")
                : Properties.Resources.Culture;
            return Properties.Resources.ResourceManager.GetString(key, culture);
        }

        public string GetWithFallback(string key, string fallbackKey)
        {
            var culture = CurrentCulture.ThreeLetterISOLanguageName == "ckb"
                ? CultureInfo.GetCultureInfo("ku")
                : Properties.Resources.Culture;
            var manager = Properties.Resources.ResourceManager;
            var rs = manager.GetResourceSet(culture, false, false);
            return rs.GetString(key) ?? manager.GetString(fallbackKey, culture);
        }

        private void SetLocale(string locale)
        {
            Log.Information($"Setting locale: {locale}");

            if (string.IsNullOrEmpty(locale) || locale.Equals(_current, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (locale.Length != 2 || locale.ToLower().Any(c => c < 'a' || c > 'z'))
            {
                throw new FormatException("The locale argument must be a two-letter string.");
            }

            if (!AvailableLocales.Contains(locale))
            {
                if (_current == "en")
                {
                    return;
                }

                // Default to English
                locale = "en";
            }

            _current = locale;
            CurrentCulture = new CultureInfo(locale == "ku" ? "ckb" : locale);
            Properties.Resources.Culture = CurrentCulture;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.DisplayLanguage))
            {
                var lang = Properties.Settings.Realtime.DisplayLanguage;

                SetLocale(lang);
            }
        }
    }
}