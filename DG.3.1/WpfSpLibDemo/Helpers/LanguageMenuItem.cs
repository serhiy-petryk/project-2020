using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Helpers
{
    public class LanguageMenuItem
    {
        public static Dictionary<string, LanguageMenuItem> LanguageMenuItems = new Dictionary<string, LanguageMenuItem>
            { {"EN", new LanguageMenuItem("en")}, {"UK", new LanguageMenuItem("uk")}};

        //========================
        public string Id { get; }
        public CultureInfo Culture { get; }
        public string Label => Culture.DisplayName + (Culture.DisplayName == Culture.NativeName ? "" : $" ({Culture.NativeName})");
        public Canvas Icon => LocalizationHelper.GetLanguageIcon(Culture);
        public bool IsSelected => string.Equals(LocalizationHelper.CurrentCulture.IetfLanguageTag, Id, StringComparison.InvariantCultureIgnoreCase);
        public RelayCommand CmdSetLanguage { get; }

        public LanguageMenuItem(string id)
        {
            Id = id;
            Culture = new CultureInfo(Id);
            CmdSetLanguage = new RelayCommand(o => LocalizationHelper.SetLanguage(Culture), o => !IsSelected);
        }
    }
}
