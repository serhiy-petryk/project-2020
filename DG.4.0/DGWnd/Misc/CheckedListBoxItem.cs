namespace DGWnd.Misc
{
  public class CheckedListBoxItem
  {
    public string _id;
    string _displayName;
    public bool _checkState;

    /*public CheckedListBoxItem(DGCore.UserSettings.Column column)
    {
      _id = column.Id; _displayName = column.DisplayName; _checkState = column.IsHidden;
    }*/
    public CheckedListBoxItem(string id, string displayName, bool checkState)
    {
      _id = id; _displayName = displayName; _checkState = checkState;
    }
    public override string ToString() => _displayName;

    public override int GetHashCode() => _id.GetHashCode();

    public override bool Equals(object obj) => obj is CheckedListBoxItem && _id == ((CheckedListBoxItem) obj)._id;
  }
}
