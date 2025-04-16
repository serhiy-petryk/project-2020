using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Helpers
{
    public class LanguageMenuItem
    {
        public static Dictionary<string, LanguageMenuItem> LanguageMenuItems = new Dictionary<string, LanguageMenuItem>
            { {"EN-US", new LanguageMenuItem("en-US")}, {"EN-GB", new LanguageMenuItem("en-GB")}, {"UK", new LanguageMenuItem("uk")}};

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
