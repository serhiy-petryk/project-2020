﻿using System;
using System.Collections.Generic;

namespace Quote2022.Models
{
    class LoaderItem
    {
        public enum ItemStatus { Disabled, None, Done, Error, Working}
        public static LoaderItem[] GetItems()
        {
            var items = new []
            {
                new LoaderItem {Id = "ScreenerTradingView", Name = "TradingView Screener", Status = ItemStatus.Disabled},
                new LoaderItem {Id = "ScreenerNasdaqStock", Name = "Nasdaq Stock Screener", Status = ItemStatus.None},
                new LoaderItem {Id = "ScreenerNasdaqEtf", Name = "Nasdaq Etf Screener", Status = ItemStatus.Done}
            };
            return items;
        }


        public string Id;
        public bool Checked;
        public string Name;
        public Action Action;
        public ItemStatus Status;
    }
}
