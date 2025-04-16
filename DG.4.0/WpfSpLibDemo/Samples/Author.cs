using System;
using System.Collections.Generic;
using System.ComponentModel;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Samples
{
    public class Author: IDataErrorInfo
    {
        public static List<Author> Authors =>
            new List<Author>
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

        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string BookTitle { get; set; }
        public bool IsMVP { get; set; }

        public Author()
        {
            LocalizationHelper.RegionChanged += LocalizationHelper_LanguageChanged;
        }

        private void LocalizationHelper_LanguageChanged(object sender, EventArgs e)
        {
            Name = "1" + Name;
        }


        #region IDataErrorInfo Members

        public string Error => ID<200 ? "ID must >= 200!!!": null;

        public string this[string columnName] => Error;

        #endregion

    }
}
