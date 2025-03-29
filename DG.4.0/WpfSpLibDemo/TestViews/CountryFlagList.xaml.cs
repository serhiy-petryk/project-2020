﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for CountryFlagList.xaml
    /// </summary>
    public partial class CountryFlagList : Window
    {

        public class FlagItem
        {
            public string Name { get; set; }
            public ImageSource Image { get; set; }
        }

        public List<FlagItem> FlagItems = new List<FlagItem>();

        public CountryFlagList()
        {
            InitializeComponent();

            foreach (var dict in Resources.MergedDictionaries)
            foreach (DictionaryEntry entry in dict)
            {
                var key = entry.Key as string;
                var image = entry.Value as ImageSource;
                if (image != null && (key.StartsWith("Region", StringComparison.CurrentCultureIgnoreCase) ||
                                      key.StartsWith("Country", StringComparison.CurrentCultureIgnoreCase)))
                    FlagItems.Add(new FlagItem { Name = key, Image = image });
            }
            ImageList.ItemsSource = FlagItems.OrderByDescending(a=>a.Name);
        }
    }
}
