using System.Collections.Generic;

namespace DGWnd.Test {
  internal class ThreadSafeDictionary<K, V> : Dictionary<K, V> where V : class {
    // Fields
    private bool _isComplete;

    // Methods
//    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal ThreadSafeDictionary() {
    }

    private void SetComplete() {
      List<K> list = null;
      foreach (KeyValuePair<K, V> pair in this) {
        if (pair.Value == null) {
          if (list == null) {
            list = new List<K>();
          }
          list.Add(pair.Key);
        }
      }
      if (list != null) {
        for (int i = 0; i < list.Count; i++) {
          base.Remove(list[i]);
        }
      }
      this._isComplete = true;
    }

    public V TryAdd(K name, V member) {
      lock (((ThreadSafeDictionary<K, V>)this)) {
        V local;
        if (!base.TryGetValue(name, out local)) {
          if (!this.IsComplete) {
            base.Add(name, member);
          }
          local = member;
        }
        return local;
      }
    }

    public bool TryGetValue(K name, out V member) {
      lock (((ThreadSafeDictionary<K, V>)this)) {
        return base.TryGetValue(name, out member);
      }
    }

    // Properties
    public bool IsComplete {
//      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      get {
        return this._isComplete;
      }
      set {
        lock (((ThreadSafeDictionary<K, V>)this)) {
          this.SetComplete();
        }
      }
    }
  }

}
