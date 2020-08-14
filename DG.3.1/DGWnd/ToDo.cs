namespace DGWnd
{
  internal partial class ToDo
  {
    // Settings in 2 dbs: dbAssessment, dbOneSAP_DW
    // +Todo: remove old user settings + check comments
    // Todo: remove ColumnInfo
    // +Todo: remove csFastSerializer
    // +Todo: check new user settings
    // columns, sort, group, sort on groups, total, filters, db filters
    // checharda with group and sorts (there are sort marker on bad columns, available 'remove sorting' on group columns)
    // no settings for sort on group of 2-nd level (may be other levels)
    // +Todo: done: smaller size of save user settings form
    // +Todo: done: bug - column header is blank after apply setting with new column
    // +Todo: column width settings, mode of columns width
    // Bug: Can not load MasterData/Partners
    //    - first load ~30-40secs (after SQL Server restart)
    //    - second load ~0.5 secs
    // Todo: remove the ability to sort on external group column+complex property (like group.column1, ..) of that group
    // Bug: refresh dgv after column list was changed in settings
    // -- add visible columns in v_sys_columns menu option
    // +Todo: remove unused group columns in mode 'не показувати рядки вищого рівня'
    // +Todo: print mode: remove unused group columns in mode 'не показувати рядки вищого рівня'
    // +Todo: Excel mode: remove unused group columns in mode 'не показувати рядки вищого рівня'
    // Todo: comments for config.json
    // Bug: sort glyphs are dissapers after change group columns mode from the same level with 'не показувати рядки вищого рівня'
    // -- Todo: long string in json: is it possible to use multiline
    // Todo: config.json - columns for lookups
    // Todo: config.json - how to implement global parameters. Example - districtId
    // Todo: config.json - deserialize to case-insensitive dictionary (example: columns)
    // Bug: search form: do not search in group description columns (example: data-AssessmentSessions, settings:SCC-71, level:3(no headers), search:"Camerin" or "James" -- не знаходить)
    // + Bug: Go to dgv form (data: AssessmentSessions), change settings from 'default' to 'SCC-71'. The column structure is destroyed.
    // ToDo: init dgv column structure after data loaded
    //        disable DGV object while data loading    
    // Bug: (low priority) incorrect color of top-left pixel of group border 
    // ToDo: (low priority) ??? is needed. 3 mode for grid borders (Yes, No, Only for Group )  
    // +ToDo: export to PDF. Use print to PDF.
    // +Bug: export to Excel: total cells with Null values are visible with 65535 value
    // Bug: export to Excel: AM/PM date part
    // ToDo: (low priority) ??? is needed. editable label (TreeView: editlabel = true)
    // Bug: or ToDo: read csv files (TestCsvDataSource connection in config.json)
  }
}
