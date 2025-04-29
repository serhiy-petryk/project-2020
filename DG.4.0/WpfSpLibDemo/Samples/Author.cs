using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Samples
{
    public class Author: INotifyPropertyChanged
    {
        public enum Level { Low, Medium, High }

        public static BindingList<Author> Authors =>
            new()
            {
                new Author()
                {
                    ID = 101, Name = "Mahesh Chand", BookTitle = "Graphics Programming with GDI+",
                    DOB = new DateTime(1975, 2, 23), IsMVP = false
                },
                new Author()
                {
                    ID = 201, Name = "Mike Gold", BookTitle = "Programming C#",
                    DOB = new DateTime(1982, 4, 12), IsMVP = true
                },
                new Author()
                {
                    ID = 244, Name = "Mathew Cochran", BookTitle = "LINQ in Vista",
                    DOB = new DateTime(1985, 9, 11), IsMVP = true
                }
            };

        public Author()
        {
            LocalizationHelper.RegionChanged += LocalizationHelper_LanguageChanged;
        }

        private Level _enumV;
        public Level EnumV
        {
            get => _enumV;
            set
            {
                _enumV = value;
                RefreshUI();
            }
        }

        private Level? _nEnumV;
        public Level? NEnumV
        {
            get => _nEnumV;
            set
            {
                _nEnumV = value;
                RefreshUI();
            }
        }

        private bool _boolV;
        public bool BoolV
        {
            get => _boolV;
            set
            {
                _boolV = value;
                RefreshUI();
            }
        }

        private bool? _nBoolV;
        public bool? NBoolV
        {
            get => _nBoolV;
            set
            {
                _nBoolV = value;
                RefreshUI();
            }
        }

        private int _id;
        public int ID
        {
            get => _id;
            set
            {
                _id = value;
                RefreshUI();
            }
        }

        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string BookTitle { get; set; }
        public bool IsMVP { get; set; }

        private void LocalizationHelper_LanguageChanged(object sender, EventArgs e)
        {
            Name = "1" + Name;
        }

        internal static string[] _propertyNames;
        internal virtual void RefreshUI()
        {
            _propertyNames ??= typeof(Author)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Select(a => a.Name).ToArray();
            OnPropertiesChanged(_propertyNames);
        }

        //===========  INotifyPropertyChanged  =======================
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
