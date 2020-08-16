using System.ComponentModel;

namespace DGCore.Common
{
  public interface ILookupTableTypeConverter
  {
    string SqlKey { get; }
    //    IEnumerable<object> GetKeyMembers(IEnumerable items);
    object GetItemByKeyValue(object keyValue);
    object GetKeyByItemValue(object item);
    void LoadData(IComponent consumer);
  }

  public interface IGetValue // For DGVCube(DGVList_GroupItem).IDGVList_GroupItem
  {
    object GetValue(string propertyName);
  }

  public interface ITotalLine
  {
    string Id { get; }
    int DecimalPlaces { get; set; }
    DGCore.Common.Enums.TotalFunction TotalFunction { get; set; }
  }

}
