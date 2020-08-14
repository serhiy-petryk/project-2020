using System;
using System.Collections;
using System.Windows.Forms;

namespace DGWnd.ThirdParty {
  public partial class FindAndReplaceForm : Form {
    private DataGridView _dgv;
    static int _lastSearchRegion = 0;

    public FindAndReplaceForm(DataGridView datagridview) {
      InitializeComponent();
      InitializeForm(datagridview);
    }

    public void InitializeForm(DataGridView datagridview) {
      _dgv = datagridview;
      // Init combo
      rbSelection1.Enabled = (_dgv.SelectedCells.Count > 1);
      rbSelection2.Enabled = (_dgv.SelectedCells.Count > 1);
      if (_lastSearchRegion == 1 && rbSelection1.Enabled) {
        rbSelection1.Checked = true;
        rbSelection2.Checked = true;
      }
      else if (_lastSearchRegion == 2) {
        rbActiveColumn1.Checked = true;
        rbActiveColumn2.Checked = true;
      }
      else {
        rbAllTable1.Checked = true;
        rbAllTable2.Checked = true;
      }

      if (_dgv.CurrentCell != null) {
        if (_dgv.CurrentCell.Value != null) {
          object o = _dgv.CurrentCell.FormattedValue;
          this.FindWhatTextBox1.Text = (o == null ? "" : o.ToString());
        }
        else this.FindWhatTextBox1.Text = "";
      }
      if (_dgv.ReadOnly) {
        tabControl1.TabPages.Remove(ReplacePage);
      }
    }

    // ===================  Events ======================
    void FindWhatTextBox1_TextChanged(object sender, System.EventArgs e) {
      this.FindWhatTextBox2.Text = this.FindWhatTextBox1.Text;
    }

    void FindWhatTextBox2_TextChanged(object sender, System.EventArgs e) {
      this.FindWhatTextBox1.Text = this.FindWhatTextBox2.Text;
    }

    void MatchCaseCheckBox1_CheckedChanged(object sender, System.EventArgs e) {
      this.MatchCaseCheckBox2.Checked = this.MatchCaseCheckBox1.Checked;
    }

    void MatchCaseCheckBox2_CheckedChanged(object sender, System.EventArgs e) {
      this.MatchCaseCheckBox1.Checked = this.MatchCaseCheckBox2.Checked;
    }

    void MatchCellCheckBox1_CheckedChanged(object sender, System.EventArgs e) {
      this.MatchCellCheckBox2.Checked = this.MatchCellCheckBox1.Checked;
    }

    void MatchCellCheckBox2_CheckedChanged(object sender, System.EventArgs e) {
      this.MatchCellCheckBox1.Checked = this.MatchCellCheckBox2.Checked;
    }

    void SearchUpCheckBox1_CheckedChanged(object sender, System.EventArgs e) {
      this.SearchUpCheckBox2.Checked = this.SearchUpCheckBox1.Checked;
    }

    void SearchUpCheckBox2_CheckedChanged(object sender, System.EventArgs e) {
      this.SearchUpCheckBox1.Checked = this.SearchUpCheckBox2.Checked;
    }

    void UseCheckBox1_CheckedChanged(object sender, System.EventArgs e) {
      this.UseCheckBox2.Checked = this.UseCheckBox1.Checked;
      if (this.UseCheckBox1.Checked) {
        this.UseComboBox1.Enabled = true;
      }
      else {
        this.UseComboBox1.Enabled = false;
        this.UseComboBox1.SelectedItem = null;
      }
    }

    void UseCheckBox2_CheckedChanged(object sender, System.EventArgs e) {
      this.UseCheckBox1.Checked = this.UseCheckBox2.Checked;
      if (this.UseCheckBox2.Checked) {
        this.UseComboBox2.Enabled = true;
      }
      else {
        this.UseComboBox2.Enabled = false;
        this.UseComboBox2.SelectedItem = null;
      }
    }

    void UseComboBox1_SelectedIndexChanged(object sender, System.EventArgs e) {
      this.UseComboBox2.SelectedIndex = this.UseComboBox1.SelectedIndex;
    }

    void UseComboBox2_SelectedIndexChanged(object sender, System.EventArgs e) {
      this.UseComboBox1.SelectedIndex = this.UseComboBox2.SelectedIndex;
    }

    void FindButton2_Click(object sender, System.EventArgs e) {
      FindButton1_Click(sender, e);
    }

    void ReplaceAllButton_Click(object sender, System.EventArgs e) {
      throw new Exception("Procedure is not ready!");
    }

    void ReplaceButton_Click(object sender, System.EventArgs e) {
      throw new Exception("Procedure is not ready!");
    }

    DataGridViewCell _lastFindCell = null;
    void FindButton1_Click(object sender, System.EventArgs e) {
      DataGridViewCell FindCell = null;
      if (_lastFindCell != this._dgv.CurrentCell) {
        _lastFindCell = null;
      }
      if (this.rbAllTable1.Checked ) {
        FindCell = sp_FindInTable();
//        if (FindCell != null) _dgv.CurrentCell = FindCell;
        if (FindCell != null) {
          if (FindCell == _lastFindCell) {
            MessageBox.Show("Текст більше не знайдено");
          }
          else {
              DGCore.Utils.Dgv.SetNewCurrentCell(_dgv, FindCell);
            _lastFindCell = FindCell;
          }
        }
      }
      else if (this.rbSelection1.Checked) {
        FindCell = sp_FindInSelection();
        if (FindCell != null) {
          DataGridViewSelectedCellCollection selectedCells = this._dgv.SelectedCells;
          DGCore.Utils.Dgv.SetNewCurrentCell(_dgv, FindCell);
//          _dgv.CurrentCell = FindCell;
          foreach (DataGridViewCell cell in selectedCells) cell.Selected = true;
        }
      }
      else if (this.rbActiveColumn1.Checked) {
        FindCell = sp_FindInColumn();
//        if (FindCell != null) _dgv.CurrentCell = FindCell;
        if (FindCell != null) DGCore.Utils.Dgv.SetNewCurrentCell(_dgv, FindCell);
      }
      if (FindCell == null) {
        MessageBox.Show("Текст не знайдено");
      }
    }

/*    void SetCurrentCell(DataGridViewCell newCurrentCell) {
      _dgv.CurrentCell = newCurrentCell;
      if (!_dgv.CurrentCell.OwningColumn.Displayed) {
        int colIndex = _dgv.CurrentCell.OwningColumn.Index;
        int rowIndex = _dgv.CurrentCell.OwningRow.Index;
        _dgv.CurrentCell = _dgv[colIndex - 1, rowIndex];
        _dgv.CurrentCell = _dgv[colIndex, rowIndex];
      }
//      MessageBox.Show(_dgv.CurrentCell.OwningColumn.Displayed.ToString());
    }*/

    // ============  Private methods  ==================
    DataGridViewCell sp_FindInTable() {
      if (_dgv.CurrentCell == null) return null;

      // Search criterions
      String sFindWhat = this.FindWhatTextBox1.Text;
      bool bMatchCase = this.MatchCaseCheckBox1.Checked;
      bool bMatchCell = this.MatchCellCheckBox1.Checked;
      bool bSearchUp = this.SearchUpCheckBox1.Checked;
      int iSearchMethod = -1; // No regular repression or wildcard
      if (this.UseCheckBox1.Checked) {
        iSearchMethod = this.UseComboBox1.SelectedIndex;
      }

      DataGridViewColumn[] cols = DGCore.Utils.Dgv.GetColumnsInDisplayOrder(this._dgv, true);
      DGCore.Utils.DGVColumnHelper[] colHelpers = new DGCore.Utils.DGVColumnHelper[cols.Length];
      for (int i = 0; i < cols.Length; i++) colHelpers[i] = new DGCore.Utils.DGVColumnHelper(cols[i]);

      // Start of search            
      int iSearchStartRow = _dgv.CurrentCell.RowIndex;
      int iLastRowNumber = _dgv.Rows.Count;
      if (iLastRowNumber > 0 && _dgv.Rows[iLastRowNumber - 1].IsNewRow) iLastRowNumber--;
      if (iLastRowNumber < 0) return null;

      // Find startup column number
      int iSearchStartColumn = -1;
      for (int i = 0; i < cols.Length; i++) {
        if (this._dgv.CurrentCell.ColumnIndex == cols[i].Index) {
          iSearchStartColumn = i;
          break;
        }
      }
      if (iSearchStartColumn < 0) return null;

      int iRowIndex = iSearchStartRow;
      int iColIndex = iSearchStartColumn;
      IList data = (IList)ListBindingHelper.GetList(this._dgv.DataSource, this._dgv.DataMember);
      do {
        // Get next Column & Row numbers
        if (bSearchUp) iColIndex--;
        else iColIndex++;
        if (iColIndex >= cols.Length) {
          iColIndex = 0;
          iRowIndex++;
        }
        else if (iColIndex < 0) {
          iColIndex = cols.Length - 1;
          iRowIndex--;
        }
        if (iRowIndex >= iLastRowNumber) iRowIndex = 0;
        else if (iRowIndex < 0) iRowIndex = iLastRowNumber - 1;
        // find value
        if (sp_FindBase(colHelpers[iColIndex].GetFormattedValueFromItem(data[iRowIndex], false), sFindWhat, bMatchCase, bMatchCell, iSearchMethod)) {
          return _dgv[cols[iColIndex].Index, iRowIndex];
        }
      } while (!(iRowIndex == iSearchStartRow && iColIndex == iSearchStartColumn));
      return null;
    }

    //sp_FindInSelection
    DataGridViewCell sp_FindInSelection() {
      if (_dgv.CurrentCell == null) return null;

      // Search criterions
      String sFindWhat = this.FindWhatTextBox1.Text;
      bool bMatchCase = this.MatchCaseCheckBox1.Checked;
      bool bMatchCell = this.MatchCellCheckBox1.Checked;
      bool bSearchUp = this.SearchUpCheckBox1.Checked;
      int iSearchMethod = -1; // No regular repression or wildcard
      if (this.UseCheckBox1.Checked) {
        iSearchMethod = this.UseComboBox1.SelectedIndex;
      }

      int[] selectedRows;
      DataGridViewColumn[] selectedColumns;
      DGCore.Utils.Dgv.GetSelectedArea(this._dgv, out selectedRows, out selectedColumns);
      DGCore.Utils.DGVColumnHelper[] colHelpers = new DGCore.Utils.DGVColumnHelper[selectedColumns.Length];
      for (int i = 0; i < selectedColumns.Length; i++) colHelpers[i] = new DGCore.Utils.DGVColumnHelper(selectedColumns[i]);

      // Start of search            
      int iTmp = _dgv.CurrentCell.RowIndex;
      int iSearchStartRow = -1;
      for (int i = 0; i < selectedRows.Length; i++) {
        if (selectedRows[i] == iTmp) {
          iSearchStartRow = i;
          break;
        }
      }
      if (iSearchStartRow < 0) return null;

      // Find startup column number
      int iSearchStartColumn = -1;
      for (int i = 0; i < selectedColumns.Length; i++) {
        if (this._dgv.CurrentCell.ColumnIndex == selectedColumns[i].Index) {
          iSearchStartColumn = i;
          break;
        }
      }
      if (iSearchStartColumn < 0) return null;

      int iRowIndex = iSearchStartRow;
      int iColIndex = iSearchStartColumn;
      IList data = (IList)ListBindingHelper.GetList(this._dgv.DataSource, this._dgv.DataMember);

      do {
        // Get next row & column numbers
        if (bSearchUp) iColIndex--;
        else iColIndex++;
        if (iColIndex >= selectedColumns.Length) {
          iColIndex = 0;
          iRowIndex++;
        }
        else if (iColIndex < 0) {
          iColIndex = selectedColumns.Length - 1;
          iRowIndex--;
        }
        if (iRowIndex >= selectedRows.Length) iRowIndex = 0;
        else if (iRowIndex < 0) iRowIndex = selectedRows.Length - 1;
        // find
        if (sp_FindBase(colHelpers[iColIndex].GetFormattedValueFromItem(data[selectedRows[iRowIndex]], false), sFindWhat, bMatchCase, bMatchCell, iSearchMethod)) {
          return _dgv[selectedColumns[iColIndex].Index, selectedRows[iRowIndex]];
        }
      } while (!(iRowIndex == iSearchStartRow && iColIndex == iSearchStartColumn));
      return null;
    }

    //sp_FindInColumn
    DataGridViewCell sp_FindInColumn() {
      if (_dgv.CurrentCell == null) return null;

      // Search criterions
      String sFindWhat = this.FindWhatTextBox1.Text;
      bool bMatchCase = this.MatchCaseCheckBox1.Checked;
      bool bMatchCell = this.MatchCellCheckBox1.Checked;
      bool bSearchUp = this.SearchUpCheckBox1.Checked;
      int iSearchMethod = -1; // No regular repression or wildcard
      if (this.UseCheckBox1.Checked) {
        iSearchMethod = this.UseComboBox1.SelectedIndex;
      }

      DataGridViewColumn col = this._dgv.Columns[_dgv.CurrentCell.ColumnIndex];
      DGCore.Utils.DGVColumnHelper colHelper = new DGCore.Utils.DGVColumnHelper(col);

      // Start of search            
      int iSearchStartRow = _dgv.CurrentCell.RowIndex;
      int iLastRowNumber = _dgv.Rows.Count;
      if (iLastRowNumber > 0 && _dgv.Rows[iLastRowNumber - 1].IsNewRow) iLastRowNumber--;
      if (iLastRowNumber < 0) return null;

      int iRowIndex = _dgv.CurrentCell.RowIndex;

//      if (bSearchUp) iRowIndex--;
  //    else iRowIndex++;

      if (iRowIndex >= iLastRowNumber) iRowIndex = 0;
      else if (iRowIndex < 0) {
        iRowIndex = iLastRowNumber - 1;
      }

      IList data = (IList)ListBindingHelper.GetList(this._dgv.DataSource, this._dgv.DataMember);
      do {
        // Get next row number
        if (bSearchUp) iRowIndex--;
        else iRowIndex++;
        if (iRowIndex >= iLastRowNumber) iRowIndex = 0;
        else if (iRowIndex < 0) iRowIndex = iLastRowNumber - 1;
        //find
        if (sp_FindBase(colHelper.GetFormattedValueFromItem(data[iRowIndex], false), sFindWhat, bMatchCase, bMatchCell, iSearchMethod)) {
          return _dgv[col.Index, iRowIndex];
        }
      } while (!(iRowIndex == iSearchStartRow));
      return null;
    }

    //sp_FindBase
    bool sp_FindBase(object cellValue, String FindString, bool bMatchCase, bool bMatchCell, int iSearchMethod) {
      if (cellValue == null || !(cellValue is string)) return false;
      String SearchString = ((string)cellValue).Replace((char)160, (char)32);
      // Regular string search
      if (iSearchMethod == -1) {
        // Match Cell
        if (bMatchCell) {
          if (bMatchCase)
            return String.Equals(FindString, SearchString, StringComparison.Ordinal);
          else
            return String.Equals(FindString, SearchString, StringComparison.OrdinalIgnoreCase);
        }
        // No Match Cell
        else {
          if (bMatchCase) return SearchString.IndexOf(FindString, StringComparison.Ordinal) >= 0;
          else return SearchString.IndexOf(FindString, StringComparison.OrdinalIgnoreCase) >= 0;
          //if (bMatchCase) return FastIndexOf(SearchString, FindString) >= 0; 
          //else return FastIndexOf(SearchString.ToLower(), FindString.ToLower()) >= 0;
        }
      }
      else {
        // Regular Expression
        string RegexPattern = FindString;
        // Wildcards
        if (iSearchMethod == 1) {
          // Convert wildcard to regex:
          RegexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(FindString).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }
        System.Text.RegularExpressions.RegexOptions strCompare = System.Text.RegularExpressions.RegexOptions.None;
        if (!bMatchCase) {
          strCompare = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
        }
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(RegexPattern, strCompare);
        if (regex.IsMatch(SearchString)) return true;
        return false;
      }
    }

    static int FastIndexOf(string source, string pattern)
    {
      // From https://www.codeproject.com/Articles/43726/Optimizing-string-operations-in-C
      if (pattern == null) throw new ArgumentNullException();
      if (pattern.Length == 0) return 0;
      if (pattern.Length == 1) return source.IndexOf(pattern[0]);
      bool found;
      int limit = source.Length - pattern.Length + 1;
      if (limit < 1) return -1;
      // Store the first 2 characters of "pattern"
      char c0 = pattern[0];
      char c1 = pattern[1];
      // Find the first occurrence of the first character
      int first = source.IndexOf(c0, 0, limit);
      while (first != -1)
      {
        // Check if the following character is the same like
        // the 2nd character of "pattern"
        if (source[first + 1] != c1)
        {
          first = source.IndexOf(c0, ++first, limit - first);
          continue;
        }
        // Check the rest of "pattern" (starting with the 3rd character)
        found = true;
        for (int j = 2; j < pattern.Length; j++)
          if (source[first + j] != pattern[j])
          {
            found = false;
            break;
          }
        // If the whole word was found, return its index, otherwise try again
        if (found) return first;
        first = source.IndexOf(c0, ++first, limit - first);
      }
      return -1;
    }
    private void FindAndReplaceForm_FormClosed(object sender, FormClosedEventArgs e) {
      this._dgv = null;
      if (rbAllTable1.Checked) _lastSearchRegion = 0;
      else if (rbSelection1.Checked) _lastSearchRegion = 1;
      else _lastSearchRegion = 2;

    }

    private void FindAndReplaceForm_Activated(object sender, EventArgs e) {
      this.FindWhatTextBox1.Focus();

    }

    private void FindWhatTextBox1_KeyPress(object sender, KeyPressEventArgs e) {
      if (!String.IsNullOrEmpty(this.FindWhatTextBox1.Text) && ((int)e.KeyChar == 13)) {
        e.Handled = true;
        FindButton1_Click(null, null);
      }
      else if (e.KeyChar == (char)27) this.Close();
    }

  }
}