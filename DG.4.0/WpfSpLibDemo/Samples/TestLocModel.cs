using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace WpfSpLibDemo.Samples
{
    public class TestLocModel: INotifyPropertyChanged
    {
        public static class NestedStaticClasses
        {
            public static readonly string StaticString = "NestedClass";

        }

        public static string TestString = "TestString";
        public static string TestStringProperty { get; set; } = "TestStringProperty";

        public static TestLocModel[] TestLocModelData;

        static TestLocModel()
        {
            WpfSpLib.Helpers.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
        }
        private static void LocalizationHelper_LanguageChanged(object sender, System.EventArgs e)
        {
            if (TestLocModelData == null)
            {
                TestLocModelData = (TestLocModel[])Application.Current.Resources["Loc:TestLocModelData"];
                return;
            }
            var newData = (TestLocModel[])Application.Current.Resources["Loc:TestLocModelData"];
            foreach (var item in TestLocModelData)
            {
                var newItem = newData.FirstOrDefault(a => a.Id == item.Id);
                if (newItem != null && !string.Equals(item.Name, newItem.Name))
                    item.Name = newItem.Name;
            }
        }

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertiesChanged("Name");
            }
        }

        #region ===========  INotifyPropertyChanged  ===============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}