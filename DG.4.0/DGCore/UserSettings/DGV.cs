using System.Collections.Generic;
using System.ComponentModel;
using DGCore.Common;

namespace DGCore.UserSettings
{
  public class DGV
  {
    public int ExpandedGroupLevel { get; set; }
    public bool ShowGroupsOfUpperLevels { get; set; }
    public List<Filter> WhereFilter { get; set; } = new List<Filter>();
    public List<Filter> FilterByValue { get; set; } = new List<Filter>();
    public List<Column> AllColumns { get; set; } = new List<Column>();
    public List<string> FrozenColumns { get; set; } = new List<string>();
    public List<Sorting> Groups { get; set; } = new List<Sorting>();
    public List<Sorting> Sorts { get; set; } = new List<Sorting>();
    public List<List<Sorting>> SortsOfGroup { get; set; } = new List<List<Sorting>>();
    public List<TotalLine> TotalLines { get; set; } = new List<TotalLine>();
    public bool ShowTotalRow { get; set; } = false;
    public bool IsGridVisible { get; set; } = true;
    public Common.Enums.DGRowViewMode RowViewMode { get; set; } = Enums.DGRowViewMode.OneRow;
    public string BaseFont { get; set; } = null;
    public string TextFastFilter { get; set; } = null;
  }

  public class Filter
  {
    private string _name;
    public string Name
    {
      get => _name;
      set => _name = value?.Replace(".", Constants.MDelimiter);
    }
    public bool Not { get; set; }
    public bool? IgnoreCase { get; set; }
    public List<FilterLine> Lines { get; set; } = new List<FilterLine>();
  }

  public class FilterLine
  {
    public Common.Enums.FilterOperand Operand { get; set; }
    public object Value1 { get; set; }
    public object Value2 { get; set; }
  }

  public class Column
  {
    private string _id;
    public string Id
    {
      get => _id;
      set => _id = value?.Replace(".", Constants.MDelimiter);
    }

    // public string DisplayName { get; set; }
    public bool IsHidden { get; set; }
    public int? Width { get; set; }
    public override string ToString() => $"Id={Id}, IsHidden={IsHidden}, Width={Width}";
  }

  public class Sorting
  {
    private string _id;
    public string Id
    {
      get => _id;
      set => _id = value?.Replace(".", Constants.MDelimiter);
    }
    public ListSortDirection SortDirection { get; set; }
    public override string ToString() => $"Id={Id}, Direction={SortDirection}";
  }

  public class TotalLine: ITotalLine
  {
    private string _id;
    public string Id
    {
      get => _id;
      set => _id = value?.Replace(".", Constants.MDelimiter);
    }
    public int? DecimalPlaces { get; set; }
    public Enums.TotalFunction TotalFunction { get; set; }
    public override string ToString() => $"Id={Id}, DecimalPlaces={DecimalPlaces}, Function={TotalFunction}";
  }
}