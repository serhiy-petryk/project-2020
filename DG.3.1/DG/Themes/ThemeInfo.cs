using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace DG.Themes
{
    public class ThemeInfo
    {
        //public static string StartupTheme = "Material Design";
        public static string StartupTheme = "MahApps.Metro";

        // Background color is taken from Control.BackgroundProperty Setter of typeof(ToolBar):
        //        var style = Resource[typeof(ToolBar)] as Style;
        //    var setter = style.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == Control.BackgroundProperty);
        //    if (setter != null) BackgroundColor = setter.Value as Brush;

        private const string PREFIX_LINEAR_GRADIENT_BRUSH = "<LinearGradientBrush xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ";
        private const string BRUSH_DEFAULT = "#FFFFFFFF";
        private const string BRUSH_AERO = PREFIX_LINEAR_GRADIENT_BRUSH + "EndPoint=\"0,1\" StartPoint=\"0,0\"><GradientStop Color=\"#FFFFFFFF\" Offset=\"0\" /><GradientStop Color=\"#FFFFFBFF\" Offset=\"0.5\" /><GradientStop Color=\"#FFF7F7F7\" Offset=\"1\" /></LinearGradientBrush>";
        private const string BRUSH_AERO_LITE = "#FFFFFBFF";
        private const string BRUSH_CLASSIC = PREFIX_LINEAR_GRADIENT_BRUSH + "EndPoint=\"0,1\" StartPoint=\"0,0\"><GradientStop Color=\"#FFE2E0DB\" Offset=\"0\" /><GradientStop Color=\"#FFEAE8E4\" Offset=\"0.5\" /><GradientStop Color=\"#FFD5D2CA\" Offset=\"0.9\" /><GradientStop Color=\"#FFDBD8D1\" Offset=\"1\" /></LinearGradientBrush>";
        private const string BRUSH_LUNA = PREFIX_LINEAR_GRADIENT_BRUSH + "EndPoint=\"0,1\" StartPoint=\"0,0\"><GradientStop Color=\"#FFFAF9F5\" Offset=\"0\" /><GradientStop Color=\"#FFEBE7E0\" Offset=\"0.5\" /><GradientStop Color=\"#FFC0C0A8\" Offset=\"0.9\" /><GradientStop Color=\"#FFA3A37C\" Offset=\"1\" /></LinearGradientBrush>";
        private const string BRUSH_LUNA_METALLIC = PREFIX_LINEAR_GRADIENT_BRUSH + "EndPoint=\"0,1\" StartPoint=\"0,0\"><GradientStop Color=\"#FFF3F4FA\" Offset=\"0\" /><GradientStop Color=\"#FFE1E2EC\" Offset=\"0.5\" /><GradientStop Color=\"#FF9997B5\" Offset=\"0.9\" /><GradientStop Color=\"#FF7C7C94\" Offset=\"1\" /></LinearGradientBrush>";
        private const string BRUSH_LUNA_HOMESTEAD = PREFIX_LINEAR_GRADIENT_BRUSH + "EndPoint=\"0,1\" StartPoint=\"0,0\"><GradientStop Color=\"#FFFAF9F5\" Offset=\"0\" /><GradientStop Color=\"#FFEBE7E0\" Offset=\"0.5\" /><GradientStop Color=\"#FFC0C0A8\" Offset=\"0.9\" /><GradientStop Color=\"#FFA3A37C\" Offset=\"1\" /></LinearGradientBrush>";

        // ==============   Static section  ======================
        private static Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();
        // Luna/Luna homestead/Royale are very similar themes
        public static ThemeInfo[] Themes =
        {
      new ThemeInfo("MahApps.Metro", "Стиль MahApps.Metro", null, new[]
        { "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml",
          "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml",
          "pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml",
          "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml",
          "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"
        },
        Common.WpfUtils.GetBrushFromHexColor(BRUSH_DEFAULT)),
      /*new ThemeInfo("Material Design", "Стиль Material Design", null, new[]
        { "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml",
          "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" },
        Misc.WpfUtils.GetBrushFromHexColor(BRUSH_DEFAULT)),
      new ThemeInfo("Material Design Dark", "Стиль Material Design Dark", null, new[]
          { "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml",
            "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" },
        Misc.WpfUtils.GetBrushFromHexColor(BRUSH_DEFAULT)),*/

      new ThemeInfo("Default theme", "Стиль по замовчуванню", null, null,
        Common.WpfUtils.GetBrushFromHexColor(BRUSH_DEFAULT)),

      new ThemeInfo("Aero", "Стиль Aero", "DG.Themes.PresentationFramework.Aero.dll", new []{"themes/aero.normalcolor.xaml"},
        (Brush) Common.WpfUtils.DeserializeToXaml(BRUSH_AERO)),
      
      // bad assembly new ThemeInfo("Aero2", "DG.Themes.PresentationFramework.Aero2.dll", "themes/aero2.normalcolor.xaml"),

      new ThemeInfo("Aero Lite", "Стиль Aero Lite", "DG.Themes.PresentationFramework.AeroLite.dll",
        new []{"themes/aerolite.normalcolor.xaml"}, Common.WpfUtils.GetBrushFromHexColor(BRUSH_AERO_LITE)),

      new ThemeInfo("Classic", "Стиль Класичний", "DG.Themes.PresentationFramework.Classic.dll", new []{"themes/classic.xaml"},
        (Brush) Common.WpfUtils.DeserializeToXaml(BRUSH_CLASSIC)),

      new ThemeInfo("Luna", "Стиль Luna", "DG.Themes.PresentationFramework.Luna.dll", new []{"themes/luna.normalcolor.xaml"},
        (Brush) Common.WpfUtils.DeserializeToXaml(BRUSH_LUNA)),

      new ThemeInfo("Luna metallic", "Стиль Luna metallic", "DG.Themes.PresentationFramework.Luna.dll",
        new [] {"themes/luna.metallic.xaml"}, (Brush) Common.WpfUtils.DeserializeToXaml(BRUSH_LUNA_METALLIC)),

      new ThemeInfo("Luna homestead", "Стиль Luna homestead", "DG.Themes.PresentationFramework.Luna.dll",
        new []{"themes/luna.homestead.xaml"}, (Brush) Common.WpfUtils.DeserializeToXaml(BRUSH_LUNA_HOMESTEAD))
      
      // new ThemeInfo("Royale", "DG.Themes.PresentationFramework.Royale.dll", "themes/royale.normalcolor.xaml")
    };

        private static Assembly GetAssembly(string assemblyName)
        {
            var curAsm = Assembly.GetExecutingAssembly();
            if (!_assemblyCache.ContainsKey(assemblyName))
            {
                var resource = assemblyName;
                using (var stm = curAsm.GetManifestResourceStream(resource))
                {
                    var ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    _assemblyCache.Add(assemblyName, Assembly.Load(ba));
                }
            }
            return _assemblyCache[assemblyName];
        }

        // ==============   Instance section  ======================
        public string Id { get; }
        public string Name { get; }
        public Brush Background { get; }
        // Foreground, Combobbox background

        private string _assemblyName;
        private string[] _uris;
        private ResourceDictionary[] _resources = null;
        public ThemeInfo(string id, string name, string assemblyName, string[] uris, Brush background)
        {
            Id = id;
            Name = name;
            _assemblyName = assemblyName;
            _uris = uris;
            Background = background ?? new SolidColorBrush(Colors.White);
        }

        public void ApplyTheme()
        {
            return;
            if (!string.IsNullOrEmpty(_assemblyName) && _resources == null)
            {
                var assembly = GetAssembly(_assemblyName);
                var uris = _uris.Select(uri => new Uri("/" + assembly.GetName().Name + ";component/" + uri, UriKind.Relative));
                _resources = uris.Select(uri => Application.LoadComponent(uri) as ResourceDictionary).ToArray();
            }
            else if (_resources == null && _uris != null)
            {
                _resources = _uris.Select(uri => new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) }).ToArray();
            }
            else if (_resources == null && _uris == null)
            {
                _resources = new ResourceDictionary[0];
            }

            // Clear old themes
            Application.Current.Resources.MergedDictionaries.Clear();      // Add new theme

            foreach (var r in _resources)
                Application.Current.Resources.MergedDictionaries.Add(r);
        }
        public override string ToString() => Id;
    }
}
