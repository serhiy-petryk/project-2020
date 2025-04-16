using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace DGView.Helpers
{
    public class LanguageMenuItem
    {
        private static readonly string[] CultureList =
            { "en-AU", "en-CA", "en-IN", "en-IE", "en-NZ", "en-SG", "en-GB", "en-US", "uk-UA" };

        private static readonly Dictionary<string, LanguageMenuItem> _regionMenuItems = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures).Where(a => CultureList.Contains(a.IetfLanguageTag))
            .OrderBy(a => a.DisplayName).ToDictionary(a => a.IetfLanguageTag,
                a => new LanguageMenuItem(a.IetfLanguageTag), System.StringComparer.OrdinalIgnoreCase);

        public static readonly Dictionary<string, LanguageMenuItem> RegionMenuItems = _regionMenuItems;

        //========================
        public CultureInfo Culture { get; }
        public string Label { get; }
        public ImageSource Icon { get; }
        public bool IsSelected => string.Equals(LocalizationHelper.CurrentCulture.IetfLanguageTag, Culture.IetfLanguageTag);
        public RelayCommand CmdSetLanguage { get; }

        public LanguageMenuItem(string id)
        {
            Culture = new CultureInfo(id ?? "");
            Label = Culture.DisplayName + (Culture.DisplayName == Culture.NativeName ? "" : $" ({Culture.NativeName})");
            Icon = LocalizationHelper.GetRegionIcon(Culture.IetfLanguageTag);
            CmdSetLanguage = new RelayCommand(o => LocalizationHelper.SetRegion(Culture), o => !IsSelected);
        }
    }
}
