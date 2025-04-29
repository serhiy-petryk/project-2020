using System;
using System.Collections;
using System.ComponentModel;

namespace WpfSpLibDemo.Samples
{
    public class AuthorINotifyDataErrorInfo: Author, INotifyDataErrorInfo
    {

        public static BindingList<AuthorINotifyDataErrorInfo> Authors =>
            new()
            {
                new AuthorINotifyDataErrorInfo()
                {
                    ID = 101, Name = "Mahesh Chand", BookTitle = "Graphics Programming with GDI+",
                    DOB = new DateTime(1975, 2, 23), IsMVP = false
                },
                new AuthorINotifyDataErrorInfo()
                {
                    ID = 201, Name = "Mike Gold", BookTitle = "Programming C#",
                    DOB = new DateTime(1982, 4, 12), IsMVP = true
                },
                new AuthorINotifyDataErrorInfo()
                {
                    ID = 244, Name = "Mathew Cochran", BookTitle = "LINQ in Vista",
                    DOB = new DateTime(1985, 9, 11), IsMVP = true
                }
            };

        internal override void RefreshUI()
        {
            base.RefreshUI();

            foreach (var p in _propertyNames)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p));
            }
        }

        #region INotifyDataErrorInfo Members
        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == "ID" && ID < 200) return new[] { "ID must >= 200!!!" };
            return null;
            // throw new NotImplementedException();
        }
        public bool HasErrors => ID < 200;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion
    }
}
