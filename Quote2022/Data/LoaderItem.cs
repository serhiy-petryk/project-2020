using System;
using System.ComponentModel;

namespace Data
{
    public class LoaderItem: NotifyPropertyChangedAbstract
    {
        public enum ItemStatus { Disabled, None, Done, Error, Working}
        public static BindingList<LoaderItem> GetItems()
        {
            var items = new BindingList<LoaderItem>
            {
                new LoaderItem {Id = "ScreenerTradingView", Name = "TradingView Screener", Status = ItemStatus.Disabled},
                new LoaderItem {Id = "ScreenerNasdaqStock", Name = "Nasdaq Stock Screener", Status = ItemStatus.None},
                new LoaderItem {Id = "ScreenerNasdaqEtf", Name = "Nasdaq Etf Screener", Status = ItemStatus.Done}
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

        public System.Drawing.Bitmap Image { get; private set; }

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
            OnPropertiesChanged(nameof(Started), nameof(ItemStatus), nameof(Duration));
        }
        public void Start()
        {
            Started = DateTime.Now;
            _finished = null;
            Status = ItemStatus.Working;
            OnPropertiesChanged(nameof(Started), nameof(ItemStatus), nameof(Duration));
        }
        public void Finished()
        {
            _finished = DateTime.Now;
            Status = ItemStatus.Done;
            OnPropertiesChanged(nameof(ItemStatus), nameof(Duration));
        }
    }
}
