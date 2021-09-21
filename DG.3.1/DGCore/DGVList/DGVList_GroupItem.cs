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
        Dictionary<string, object[]> GetTotalsForWpfDataGrid();
    }

    //=============  DGVList_GroupItem<T>  ==============
    public class DGVList_GroupItem<T> : CustomTypeDescriptor, IDGVList_GroupItem, Common.IGetValue
    {
        public override PropertyDescriptorCollection GetProperties() => PD.MemberDescriptorUtils.GetTypeMembers(typeof(T));
        PropertyDescriptorCollection _pdc;

        public List<DGVList_GroupItem<T>> ChildGroups;
        public List<T> ChildItems;

        private DGVList_GroupItem<T> _parent;
        private string _propertyName;
        internal object _propertyValue;

        private double[] _totalValues;
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
        public double[] TotalValues
        {
            get
            {
                if (this._totalValues == null) GetTotals();
                return this._totalValues;
            }
        }
        public void ResetTotals()
        {
            if (this._totalValues != null) this._totalValues = null;
        }

        public bool IsVisible
        {
            get
            {
                //if (Level == 0) return false;
                return _parent == null ? true : this._parent.IsVisible && this._parent.IsExpanded;
            }
        }
        public int Level
        {
            get { return _parent == null ? 0 : this._parent.Level + 1; }
        }
        public int ItemCount
        {
            get
            {
                if (this.ChildItems != null) return this.ChildItems.Count;
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

        private void FillChildList(List<object> itemList)
        {
            if (this.ChildItems == null)
            {
                foreach (DGVList_GroupItem<T> item in this.ChildGroups)
                {
                    itemList.Add(item);
                    if (item.IsExpanded) item.FillChildList(itemList);
                }
            }
            else
            {
                foreach (object o in this.ChildItems) itemList.Add(o);
            }
        }

        object GetPropertyValue(string propertyName)
        {
            if (this._propertyName == propertyName) return this._propertyValue;
            if (this._propertyValue != null && this._pdc != null && propertyName.StartsWith(this._propertyName + "."))
            {
                string s = propertyName.Substring(this._propertyName.Length + 1);
                PropertyDescriptor pd = this._pdc[s];
                if (pd != null) return pd.GetValue(this._propertyValue);
            }
            if (this._parent != null) return this._parent.GetPropertyValue(propertyName);
            return null;
        }

        public object GetValue(string propertyName)
        {
            object value = GetPropertyValue(propertyName);
            if (value == null)
            {
                if (this._totalDefinitions != null)
                {
                    for (int i = 0; i < this._totalDefinitions.Length; i++)
                    {
                        if (this._totalDefinitions[i].Id == propertyName)
                        {
                            if (this._totalValues == null) GetTotals();// Refresh totals if they do not exist
                            return this._totalValues[i];
                        }
                    }
                }
            }
            return value;
            /*        object value = null;

                    if (!this._properties.TryGetValue(propertyName, out value)) {
                      if (this._totalDefintions != null) {
                        for (int i = 0; i < this._totalDefintions.Length; i++) {
                          if (this._totalDefintions[i]._pd.Name == propertyName) {
                            if (this._totalValues == null) GetTotals();// Refresh totals if they do not exist
                            return this._totalValues[i];
                          }
                        }
                      }
                    }
                    return value;*/
        }

        // === totals =======
        public void SetTotalsProperties(Misc.TotalLine[] totalLines)
        {// Call only for root
            this._totalDefinitions = totalLines;
        }

        private Dictionary<string, object[]> _totalsForWpfDataGrid;

        public Dictionary<string, object[]> GetTotalsForWpfDataGrid()
        {
            if (_totalsForWpfDataGrid == null) GetTotals();
            if (_totalDefinitions == null) return null;

            _totalsForWpfDataGrid = new Dictionary<string, object[]>();
            for (var k = 0; k < _totalDefinitions.Length; k++)
                if (_totalDefinitions[k].Id.Contains("."))
                    _totalsForWpfDataGrid.Add(_totalDefinitions[k].Id, new object[] { _totalValues[k], null });
            return _totalsForWpfDataGrid.Count == 0 ? null : _totalsForWpfDataGrid;
        }

        double[] GetTotals()
        {
            if (this._totalDefinitions == null || this._totalValues != null) return this._totalValues;
            this._totalValues = new double[this._totalDefinitions.Length];
            this._totalItemCount = new int[this._totalDefinitions.Length];
            // Init total values
            for (int i = 0; i < this._totalDefinitions.Length; i++)
            {
                if (this._totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Minimum)
                {
                    this._totalValues[i] = double.MaxValue;
                }
                else if (this._totalDefinitions[i].TotalFunction == Common.Enums.TotalFunction.Maximum)
                {
                    this._totalValues[i] = double.MinValue;
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
                        if (double.IsNaN(dd[i])) continue;

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
                                this._totalValues[i] = Math.Max(this._totalValues[i], dd[i]);
                                break;
                            case Common.Enums.TotalFunction.Minimum:
                                this._totalValues[i] = Math.Min(this._totalValues[i], dd[i]);
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
                                    if (!notFirstFlags[i]) this._totalValues[i] = Convert.ToDouble(o);
                                    break;
                                case Common.Enums.TotalFunction.Last:
                                    this._totalValues[i] = Convert.ToDouble(o);
                                    break;
                                case Common.Enums.TotalFunction.Count:
                                    this._totalValues[i] += 1.0;
                                    break;
                                case Common.Enums.TotalFunction.Average:
                                case Common.Enums.TotalFunction.Sum:
                                    this._totalValues[i] += Convert.ToDouble(o);
                                    break;
                                case Common.Enums.TotalFunction.Maximum:
                                    this._totalValues[i] = Math.Max(this._totalValues[i], Convert.ToDouble(o));
                                    break;
                                case Common.Enums.TotalFunction.Minimum:
                                    this._totalValues[i] = Math.Min(this._totalValues[i], Convert.ToDouble(o));
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
                if (this._totalItemCount[i] == 0)
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
                }
            }

            return this._totalValues;
        }
    }
}

