using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace DGWnd.Useful {

  public class DbReaderEnumerable<T> : IEnumerable<T>, IEnumerable {

      DGCore.DB.DbCmd _cmd;
    IEnumerable<DGCore.DB.DbColumnMapElement> _columnMap;
    Func<DbDataReader, T> _delegate;

    public DbReaderEnumerable(DGCore.DB.DbCmd cmd, IEnumerable<DGCore.DB.DbColumnMapElement> columnMap) {
      this._cmd = cmd; this._columnMap = columnMap;
      this._delegate = DGCore.DB.DbUtils.Reader.GetDelegate_FromDataReaderToObject<T>(cmd, this._columnMap);
    }

    /*    
        public DBReaderEnumerable(DbConnection conn, string sql)
          : this(conn, sql, null, null) {
        }
        public DBReaderEnumerable(DbConnection conn, string sql, IList paramValues)
          : this(conn, sql, paramValues, null) {
        }
        public DBReaderEnumerable(DbConnection conn, string sql, IList paramValues, IList<string> paramNames) {
          this._conn = conn; this._sql = sql; this._paramValues = paramValues; this._paramNames = paramNames;
           this._delegate = DBConnStat.GetDelegate_FromDataReaderToObject<T>(conn, sql);
        }*/

    public IEnumerator<T> GetEnumerator() {
      this._cmd.Connection_Open();
      DbDataReader reader = this._cmd._dbCmd.ExecuteReader();
      return new DBReaderEnumerator(reader, this._delegate);
    }
    IEnumerator IEnumerable.GetEnumerator() {
      DbDataReader reader = this._cmd._dbCmd.ExecuteReader();
      return new DBReaderEnumerator(reader, this._delegate);
    }

    // ============================================
    class DBReaderEnumerator : IEnumerator<T> {

      DbDataReader _reader;
      Func<DbDataReader, T> _dataProxy;
      public DBReaderEnumerator(DbDataReader reader, Func<DbDataReader, T> dataProxy) {
        this._reader = reader;
        this._dataProxy = dataProxy;
      }
      public T Current {
        get { return _dataProxy(_reader); }
      }

      public void Dispose() {
        _reader.Dispose();
        _reader = null;
//        if (Disposed != null) Disposed.Invoke(this, new EventArgs());
      }
      object IEnumerator.Current {
        get { return _dataProxy(_reader); }
      }

      public bool MoveNext() {
        return _reader.Read();
      }

      public void Reset() {
        ((IEnumerator)_reader).Reset();
      }
    }

  }
}
