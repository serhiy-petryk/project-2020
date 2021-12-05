using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DGCore.Misc
{

    public class TotalLine : Common.ITotalLine
    {
        #region ==========  Static section  ==============
        private static readonly Type[] _typesForTotalLines = {
            typeof(char), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)
        };

        public static bool IsTypeSupport(Type t) => _typesForTotalLines.Contains(t);

        public static void ApplySettings(IEnumerable<TotalLine> target, IEnumerable<Common.ITotalLine> source)
        {
            foreach (Common.ITotalLine tl1 in source)
                foreach (TotalLine tl2 in target)
                    if (tl2.Id == tl1.Id)
                    {
                        tl2.TotalFunction = tl1.TotalFunction;
                        tl2.DecimalPlaces = tl1.DecimalPlaces;
                        break;
                    }
        }
        #endregion

        // PropertyDescriptor _pd;// can be null; before work with pd you need to activate it (use PropertyDescriptor property set)
        // Common.Enums.TotalFunction _totalFunction = Common.Enums.TotalFunction.None;
        private int _dpTotals = 5;

        public TotalLine() { }

        public TotalLine(PropertyDescriptor pd)
        {
            Id = pd.Name;
            PropertyDescriptor = pd;
        }

        [Browsable(false)]
        public string Id { get; }

        public string DisplayName => PropertyDescriptor.DisplayName;

        public Common.Enums.TotalFunction TotalFunction { get; set; }

        public int DecimalPlaces
        {
            get => _dpTotals;
            set => _dpTotals = Math.Min(15, Math.Max(0, value));
        }

        [Browsable(false)]
        public PropertyDescriptor PropertyDescriptor { get; set; }

        public UserSettings.TotalLine ToSettingsTotalLine() => new UserSettings.TotalLine { Id = Id, DecimalPlaces = _dpTotals, TotalFunction = TotalFunction };

        public override string ToString() => Id;
    }

}




