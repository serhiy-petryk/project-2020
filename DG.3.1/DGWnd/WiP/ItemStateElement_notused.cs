using System.Collections.Generic;

namespace DGWnd.WiP {

  public class ItemStateElement {

    public static ItemStateElement[] GetState(IEnumerable<DGCore.DB.DbColumnMapElement> map, object item) {
      List<ItemStateElement> state = new List<ItemStateElement>();
        foreach (DGCore.DB.DbColumnMapElement e in map) {
          if (e.IsValid) {
//            if (item == null) state.Add(new ItemStateElement(e, e._dbNullValue));
  //          else state.Add(new ItemStateElement(e, e.GetValue(item)));
            state.Add(new ItemStateElement(e, e.MemberDescriptor.GetValue(item)));
          }
      }
      return state.ToArray();
    }
    public static ItemStateElement[] GetChanges(object item, IEnumerable<ItemStateElement> oldState) {
      List<ItemStateElement> oo = new List<ItemStateElement>();
      foreach (ItemStateElement elem in oldState) {
        if (elem._mapElement != null) {
          if (elem.IsDifferentValues(elem._mapElement.MemberDescriptor.GetValue(item))) oo.Add(elem);
        }
      }
      return oo.ToArray();
    }

    //==============================================
    public readonly DGCore.DB.DbColumnMapElement _mapElement; 
    readonly object _oldValue;
    object _newValue;

    public ItemStateElement(DGCore.DB.DbColumnMapElement mapElement, object oldValue) {
      this._mapElement = mapElement;
      this._oldValue = oldValue;
    }

    public object OldValue {
      get { return this._oldValue; }
    }
    public object NewValue {
      get { return this._newValue; }
    }
    public bool IsDifferentValues(object newValue) {
      this._newValue = newValue;
      return !DGCore.Utils.Tips.IsValueEquals(this._oldValue, this._newValue);
    }

    public override string ToString() {
      return this._mapElement.ToString() + "\t" + (this._oldValue == null ? "(null)" : this._oldValue.ToString()) + "\t" + 
        (this._newValue == null ? "(null)" : this._newValue.ToString());
    }
  }
}
