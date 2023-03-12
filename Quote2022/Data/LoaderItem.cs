using System;
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
        private static Dictionary<ItemStatus, Bitmap> _images;

        private static Bitmap GetImage(ItemStatus status)
        {
            if (_images == null)
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var rm = new System.Resources.ResourceManager("Data.Images", asm);
                _images = new Dictionary<ItemStatus, Bitmap>
                {
                    {ItemStatus.Disabled, Get("Blank")}, {ItemStatus.None, Get("Blank")},
                    {ItemStatus.Working, Get("Wait")}, {ItemStatus.Done, Get("Done")}, {ItemStatus.Error, Get("Error")}
                };
                var a1 = rm.GetObject("Done");

                //=================
                Bitmap Get(string name) => (Bitmap)rm.GetObject(name);
            }

            return _images[status];
        }

        public enum ItemStatus { Disabled, None, Working, Done, Error}

        public static Image GetAnimatedImage() => GetImage(ItemStatus.Working);
        public static BindingList<LoaderItem> GetItems()
        {
            var items = new BindingList<LoaderItem>
            {
                new LoaderItem {Id = "ScreenerTradingView", Name = "TradingView Screener", Status = ItemStatus.Disabled},
                new LoaderItem {Id = "ScreenerNasdaqStock", Name = "Nasdaq Stock Screener", Status = ItemStatus.None},
                new LoaderItem {Id = "ScreenerNasdaqEtf", Name = "Nasdaq Etf Screener", Status = ItemStatus.Working}
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
