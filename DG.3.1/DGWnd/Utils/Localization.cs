using System;
using System.Collections.Generic;
using System.Globalization;

namespace DGWnd.Utils
{
    public static class Localization
    {
        private static Dictionary<string, string> _data =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"LoadConfigError1", "Помилка у файлі конфігурації: {0}"},
                {"LoadConfigError2", "Рядок файлу: {0}. Позиція: {1}."},
                {"LoadConfigError3", "Текст помилки:"},
            };

        public static string GetMessage(string key, params object[] args) =>
            GetMessage(CultureInfo.CurrentCulture, key, args);

        public static string GetMessage(IFormatProvider provider, string key, params object[] args) =>
            _data.ContainsKey(key) ? string.Format(provider, _data[key], args) : null;
    }
}
