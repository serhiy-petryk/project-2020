using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Samples
{
    public class AuthorIDataErrorInfo : INotifyPropertyChanged, IDataErrorInfo, IEditableObject
    {

        private static BindingList<AuthorIDataErrorInfo> _authors = null;

        public static BindingList<AuthorIDataErrorInfo> Authors
        {
            get
            {
                if (_authors == null)
                {
                    _authors = new BindingList<AuthorIDataErrorInfo>
                    {
                        new()
                        {
                            ID = 101,
                            Name = "Mahesh Chand",
                            BookTitle = "Graphics Programming with GDI+",
                            DOB = new DateTime(1975, 2, 23),
                            IsMVP = false
                        },
                        new()
                        {
                            ID = 201,
                            Name = "Mike Gold",
                            BookTitle = "Programming C#",
                            DOB = new DateTime(1982, 4, 12),
                            IsMVP = true
                        },
                        new()
                        {
                            ID = 244,
                            Name = "Mathew Cochran",
                            BookTitle = "LINQ in Vista",
                            DOB = new DateTime(1985, 9, 11),
                            IsMVP = true
                        }
                    };
                }
                return _authors;
            }
        }

        public AuthorIDataErrorInfo()
        {
            LocalizationHelper.RegionChanged += LocalizationHelper_LanguageChanged;
        }

        private Author.Level _enumV;
        public Author.Level EnumV
        {
            get => _enumV;
            set
            {
                _enumV = value;
                RefreshUI();
            }
        }

        private Author.Level? _nEnumV;
        public Author.Level? NEnumV
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
            _propertyNames ??= typeof(AuthorIDataErrorInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public| BindingFlags.FlattenHierarchy).Select(a => a.Name).ToArray();
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

        #region IDataErrorInfo Members
        public string Error => ID<200 ? "ID must >= 200!!!": null;
        public string this[string columnName] => columnName == "ID" ? Error : null;
        #endregion

        #region ===========  IEditableObject  ===========
        private AuthorIDataErrorInfo backupCopy;
        private bool inEdit;
        public void BeginEdit()
        {
            if (inEdit) return;
            inEdit = true;
            backupCopy = MemberwiseClone() as AuthorIDataErrorInfo;
        }

        public void EndEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            backupCopy = null;
        }

        public void CancelEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            ID = backupCopy.ID;
            // Value1 = backupCopy.Value1;
            // Value2 = backupCopy.Value2;
        }
        #endregion
    }
}
