using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DGCore.DGVList
{
    public interface IDGVList_GroupItem
    {
        int Level { get; }
        int ItemCount { get; }
        int ExpandedItemCount { get; }
        bool IsExpanded { get; set; }
    }

    //=============  DGVList_GroupItem<T>  ==============
    public class DGVList_GroupItem<T> : CustomTypeDescriptor, IDGVList_GroupItem, Common.IGetValue
    {
        public override PropertyDescriptorCollection GetProperties() => PD.MemberDescriptorUtils.GetTypeMembers(typeof(T));
        private PropertyDescriptorCollection _pdc;

        private DGVList_GroupItem<T> _parent;
        public List<DGVList_GroupItem<T>> ChildGroups;
        public List<T> ChildItems;

        private string _propertyName;
        internal object _propertyValue;

        private decimal?[] _totalValues;
        private Misc.TotalLine[] _totalDefinitions;
        private int[] _totalItemCount;

        public DGVList_GroupItem() { }

        // Create wrapper for group element
        public DGVList_GroupItem<T> CreateChildGroup(string groupValueName, object groupValue, bool isExpanded, PropertyDescriptorCollection pdc)
        {
            if (this.ChildGroups == null) this.ChildGroups = new List<DGVList_GroupItem<T>>();
            DGVList_GroupItem<T> newItem = new DGVList_GroupItem<T>();
            newItem._parent = this;
            newItem._pdc = pdc;
            newItem._propertyName = groupValueName;
            newItem._propertyValue = groupValue;
            newItem._totalDefinitions = this._totalDefinitions;
            newItem.IsExpanded = isExpanded;// Level 1 == already initially expanded
            this.ChildGroups.Add(newItem);
            return newItem;
        }
        //============
        public decimal?[] TotalValues
        {
            get
            {
                if (this._totalValues == null) GetTotals();
                return this._totalValues;
            }
        }
        public void ResetTotals()
        {
            if (_totalValues != null) _totalValues = null;
        }

        public bool IsVisible => _parent == null || (_parent.IsVisible && _parent.IsExpanded);

        public int Level => _parent?.Level + 1 ?? 0;

        public int ItemCount
        {
            get
            {
                if (ChildItems != null) return ChildItems.Count;
                try
                {
                    return System.Linq.Enumerable.Sum<DGVList_GroupItem<T>>(ChildGroups,
                      (Func<DGVList_GroupItem<T>, int>)delegate (DGVList_GroupItem<T> grItem) { return grItem.ItemCount; });
                }
                catch (Exception ex)
                {
                    throw new Exception("LOVUSHKA!!!" + Environment.NewLine + ex);
                }
            }
        }
        public bool IsEmpty
        {
            get
            {
                if (this.ChildGroups == null) return this.ChildItems.Count == 0;
                foreach (DGVList_GroupItem<T> item in this.ChildGroups)
                {
                    if (!item.IsEmpty) return false;
                }
                return true;
            }
        }
        public bool IsExpanded { get; set; } = true;
        public int ExpandedItemCount
        {
            get
            {
                if (this.IsExpanded)
                {
                    if (this.ChildGroups == null) return this.ChildItems.Count + 1;
                    int cnt = 1;
                    foreach (DGVList_GroupItem<T> o in this.ChildGroups) cnt += o.ExpandedItemCount;
                    return cnt;
                }
                else return 1;
            }
        }

        object GetPropertyValue(string propertyName)
        {
            if (_propertyName == propertyName) return _propertyValue;
            if (_propertyValue != null && _pdc != null && propertyName.StartsWith(_propertyName + "."))
            {
                var s = propertyName.Substring(this._propertyName.Length + 1);
                var pd = this._pdc[s];
                if (pd != null) return pd.GetValue(this._propertyValue);
            }

            return _parent?.GetPropertyValue(propertyName);
        }

        public object GetValue(string propertyName)
        {
            object value = GetPropertyValue(propertyName);
            if (value == null)
            {
                if (_totalDefinitions != null)
                {
                    var propertyNameWithDot = propertyName + ".";
                    var nestedTotalValues = new List<decimal?>();
                    var nestedTotalDefinitions = new List<Misc.TotalLine>();
                    for (var i = 0; i < _totalDefinitions.Length; i++)
                    {
                        if (_totalDefinitions[i].Id == propertyName)
                        {
                            if (_totalValues == null) GetTotals();// Refresh totals if they do not exist
                            return _totalValues[i];
                        }

                        if (_totalDefinitions[i].Id.StartsWith(propertyNameWithDot))
                        {
                            if (_totalValues == null) GetTotals();// Refresh totals if they do not exist
                            nestedTotalDefinitions.Add(_totalDefinitions[i]);
                            nestedTotalValues.Add(_totalValues[i]);
                        }

                    }

                    if (nestedTotalDefinitions.Count > 0)
                    {
                        var propertyType = PD.MemberDescriptorUtils.GetTypeMembers(typeof(T))[propertyName].PropertyType;
                        return new DGVGroupTotalValueProxy
                        {
                            pdc = PD.MemberDescriptorUtils.GetTypeMembers(propertyType),
                            Prefix = propertyNameWithDot,
                            TotalDefinitions = nestedTotalDefinitions.ToArray(),
                            TotalValues = nestedTotalValues.ToArray()
                        };
                    }
                }
            }
            return value;
        }

        // === totals =======
        public void SetTotalsProperties(Misc.TotalLine[] totalLines)
        {// Call only for root
            this._totalDefinitions = totalLines;
        }

        decimal?[] GetTotals()
        {
            if (this._totalDefinitions == null || this._totalValues != null) return this._totalValues;
            this._totalValues = new decimal?[this._totalDefinitions.Length];
            this._totalItemCount = new int[this._totalDefinitions.Length];
            // Init total values
            for (int i = 0; i < this._totalDefinitions.Length; i++)
            {
                if (this._totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Minimum)
                {
                    this._totalValues[i] = decimal.MaxValue;
                }
                else if (this._totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Maximum)
                {
                    this._totalValues[i] = decimal.MinValue;
                }
                else this._totalValues[i] = 0;
                this._totalItemCount[i] = 0;
            }
            if (this.ChildGroups != null && this.ChildGroups.Count > 0)
            {
                for (int k = 0; k < this.ChildGroups.Count; k++)
                {
                    DGVList_GroupItem<T> child = this.ChildGroups[k];
                    var dd = child.GetTotals().ToArray();
                    for (var i = 0; i < dd.Length; i++)
                    {
                        // if (double.IsNaN(dd[i])) continue;
                        if (!dd[i].HasValue) continue;

                        this._totalItemCount[i] += child._totalItemCount[i];
                        switch (this._totalDefinitions[i].TotalFunction)
                        {
                            case Common.Enums.TotalFunction.First:
                                if (k == 0) this._totalValues[i] = dd[i];
                                break;
                            case Common.Enums.TotalFunction.Last:
                                if (k == (this.ChildGroups.Count - 1)) this._totalValues[i] = dd[i];
                                break;
                            case Common.Enums.TotalFunction.Count:
                                this._totalValues[i] += child._totalItemCount[i];
                                break;
                            case Common.Enums.TotalFunction.Average:
                                this._totalValues[i] += dd[i] * child._totalItemCount[i];
                                break;
                            case Common.Enums.TotalFunction.Sum:
                                this._totalValues[i] += dd[i];
                                break;
                            case Common.Enums.TotalFunction.Maximum:
                                this._totalValues[i] = Math.Max(this._totalValues[i].Value, dd[i].Value);
                                break;
                            case Common.Enums.TotalFunction.Minimum:
                                this._totalValues[i] = Math.Min(this._totalValues[i].Value, dd[i].Value);
                                break;
                        }
                    }
                }
            }
            if (this.ChildItems != null && this.ChildItems.Count > 0)
            {
                bool[] notFirstFlags = new bool[this._totalDefinitions.Length];
                foreach (T item in this.ChildItems)
                {
                    for (int i = 0; i < this._totalDefinitions.Length; i++)
                    {
                        object o = this._totalDefinitions[i].PropertyDescriptor.GetValue(item);
                        if (o != null)
                        {
                            this._totalItemCount[i]++;
                            switch (this._totalDefinitions[i].TotalFunction)
                            {
                                case Common.Enums.TotalFunction.First:
                                    if (!notFirstFlags[i]) this._totalValues[i] = Convert.ToDecimal(o);
                                    break;
                                case Common.Enums.TotalFunction.Last:
                                    this._totalValues[i] = Convert.ToDecimal(o);
                                    break;
                                case Common.Enums.TotalFunction.Count:
                                    this._totalValues[i] += decimal.One;
                                    break;
                                case Common.Enums.TotalFunction.Average:
                                case Common.Enums.TotalFunction.Sum:
                                    this._totalValues[i] += Convert.ToDecimal(o);
                                    break;
                                case Common.Enums.TotalFunction.Maximum:
                                    this._totalValues[i] = Math.Max(this._totalValues[i].Value, Convert.ToDecimal(o));
                                    break;
                                case Common.Enums.TotalFunction.Minimum:
                                    this._totalValues[i] = Math.Min(this._totalValues[i].Value, Convert.ToDecimal(o));
                                    break;
                            }
                            notFirstFlags[i] = true;
                        }
                    }
                }
            }
            // Rounding rezult
            for (int i = 0; i < this._totalDefinitions.Length; i++)
            {
                if (_totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Average)
                {
                    // this._totalValues[i] = Math.Round(this._totalValues[i] / this._totalItemCount[i], this._totalDefinitions[i].DecimalPlaces);
                    _totalValues[i] = _totalValues[i] / _totalItemCount[i];
                }
                if (_totalItemCount[i] == 0)
                    _totalValues[i] = null;

                /*if (this._totalItemCount[i] == 0)
                {
                    this._totalValues[i] = double.NaN;
                }
                else
                {
                    if (this._totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Average)
                    {
                        this._totalValues[i] = Math.Round(this._totalValues[i] / this._totalItemCount[i], this._totalDefinitions[i].DecimalPlaces);
                    }
                    else
                    {
                        this._totalValues[i] = Math.Round(this._totalValues[i], this._totalDefinitions[i].DecimalPlaces);
                    }

                    // Remove leading minus in grid (sometimes 0.0 can be shown as -0 in wpf datagrid)
                    if (_totalValues[i] == 0.0)
                        _totalValues[i] = 0.0;
                }*/
            }

            return this._totalValues;
        }
    }
}

