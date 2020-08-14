using System;

namespace DGWnd.ThirdParty.Oli
{
	public enum DropOperation
	{
		Reorder,
		MoveToHere,
		CopyToHere,
		MoveFromHere,
		CopyFromHere
	}

	public class DroppedEventArgs : EventArgs
	{
    DropOperation _operation;
    IDragDropSource _source;
    IDragDropSource _target;
    object[] _droppedItems;

    public DropOperation Operation { get { return this._operation; } set { this._operation = value; } }
    public IDragDropSource Source { get { return this._source; } set { this._source = value; } }
    public IDragDropSource Target { get { return _target; } set { _target = value; } }
    public object[] DroppedItems { get { return _droppedItems; } set { _droppedItems = value; } }
	}
}
