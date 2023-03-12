﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Data
{
    public class LoaderItem: NotifyPropertyChangedAbstract
    {
        public enum ItemStatus { Disabled, None, Working, Done, Error }

        private static Dictionary<string, Bitmap> _imgResources;
        private static string[] _itemStatusImageName = new[] {"Blank", "Blank", "Wait", "Done", "Error"};

        private static Bitmap GetImage(ItemStatus status)
        {
            if (_imgResources == null)
            {
                _imgResources=new Dictionary<string, Bitmap>();
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var rm = new System.Resources.ResourceManager("Data.Images", asm);
                foreach (var o in rm.GetResourceSet(CultureInfo.InvariantCulture, true, false))
                {
                    if (o is DictionaryEntry de && de.Key is string key && de.Value is Bitmap value)
                        _imgResources.Add(key, value);
                }
            }

            return _imgResources[_itemStatusImageName[(int)status]];
        }

        public static Image GetAnimatedImage() => GetImage(ItemStatus.Working);
        public static BindingList<LoaderItem> GetItems()
        {
            var items = new BindingList<LoaderItem>
            {
                new LoaderItem {Id = "ScreenerTradingView", Name = "TradingView Screener", Status = ItemStatus.Disabled},
                new LoaderItem {Id = "ScreenerNasdaqStock", Name = "Nasdaq Stock Screener", Status = ItemStatus.None},
                new LoaderItem {Id = "ScreenerNasdaqEtf", Name = "Nasdaq Etf Screener", Status = ItemStatus.None}
            };
            return items;
        }

        public string Id;

        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertiesChanged(nameof(Checked));
            }
        }

        public System.Drawing.Bitmap Image => GetImage(Status);

        public DateTime? Started { get; private set; }
        private DateTime? _finished;
        public long? Duration => Started.HasValue && _finished.HasValue ? Convert.ToInt64((_finished.Value - Started.Value).TotalSeconds) : (long?)null;

        public string Name { get; private set; }
        public Action Action;
        public ItemStatus Status { get; set; }

        public void Reset()
        {
            Started = null;
            _finished = null;
            Status = ItemStatus.None;
            UpdateUI();
        }
        public void Start()
        {
            Started = DateTime.Now;
            _finished = null;
            Status = ItemStatus.Working;
            UpdateUI();
        }
        public void Finished()
        {
            _finished = DateTime.Now;
            Status = ItemStatus.Done;
            UpdateUI();
        }

        public override void UpdateUI()
        {
            OnPropertiesChanged(nameof(Started), nameof(ItemStatus), nameof(Duration), nameof(Image));
        }
    }
}
