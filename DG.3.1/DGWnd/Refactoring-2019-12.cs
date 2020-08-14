namespace DGWnd
{
  public class Refactoring_2019_12
  {
    // Головна ціль:
    // Зменшити залежність модулів, в першу чергу з DGVList прибрати залежність від DGVCube
    // Побудувати все на командах:
    // frmDGV дає команди DGVCube або DGVList, DGVCube дає команди DGVList і т.п.
    // Зробити перехід з WindowsForm to WPF. 

    // Modules / Dependencies:
    // 1. PD - PropertyDescription (depends on Utils.Emit/Tips/Types)
    // 1. Misc.AppSettings (depends on Utils.ExcelApp)
    // 1. Misc.DependentObjectManager - depends on Utils.Events.RemoveEventSubsriptions/RemoveAllEventSubsriptions
    // 1. CSV.TestCsvConnection - no dependencies
    // 2. UserSettings (depends on Misc.AppSettings.settingsStorage, Utils.Dgv/Tips, Common.Enums.DGCellViewMode/TotalFunction)
    // 2. DB - depends on PD.MemberDescriptorUtils/MemberDescriptor/MemberKind/Converter_Dictionary, CSV.TestCsvConnection, Misc.DependentObjectManager
    // 3. Filter - depend on UserSettings, DB.DbCmd/DbSchemaTable/.., 
    // 3. Misc.TotalLine - depend on UserSettings.TotalLine
    // 4. Sql(DataSource) - Misc.DependentObjectManager, PD.MemberDescriptorUtils, DB.DbCmd/DbUtils.Reader, Filters.DbWhereFilter
    // 5. Misc.DataDefinition - depends on DB.DbCmd/DbDynamicType, Filters.FilterList/.., Sql.ParameterCollection/DbDataSource, 
    // 6. Menu - depends on Misc.DataDefinition, Sql.Parameter/ParameterCollection, DB.DbCmd/DB.DbDynamicType.GetDynamicType, Misc.AppSettings

    // Utils.DGVColumnHelper - depends on PD.IMemberDescriptor
    // Utils.DGVClipboard - depends on DGVSelection.GetSelectedArea, Utils.DGVColumnHelper
    // Utils.DGVSave - depends on DGVSelection.GetSaveArea, DGV.DGVCube.IDGVList_GroupItem, ((DGV.DGVCube)dgv)._groupPens
    // Utils.DGVSelection - depends on ((DGV.DGVCube)dgv)._IsItemVisible


    // 5. DGVList / DGVList_GroupItem, DGVList, DGVCube
    // 6. DGV
    // 7. UI

    // Dependencies:

    // DGVList_GroupItem - depends on Misc.TotalLine, 
    // DGVList - depends on DGVCube, DGVList_GroupItem, IDGVList_GroupItem, PD.MemberDescriptor/.., Misc.TotalLine, Sql.DataSourceBase, Utils.DGVColumnHelper
    // 5. DGVList
    // 6. DGV
    // 7. UI
    // 11. Utils.Dgv, Utils.ExcelApp - no dependencies

    // CONTROL DIGITS (GLDOCLINE, 1'000'000 recs):
    // Used RAM before first run: 2MB
    // 1. First run - 31.2 secs, used RAM - 679.5MB, sort on DOCKEY - 2.93 secs
    // Used RAM after close first run: 5.3MB
    // 2. Second run - 10.3 secs, used RAM - 679.5MB, sort on DOCKEY - 2.95 secs
    // Used RAM after close second run: 5.3MB
    // 3. FastFilter
    // - "1":     3.4 secs (1'000'000 recs)
    // - "12":    7.9 secs (383'411 recs)
    // - "123":   9.4 secs (73'520 recs)
    // - "1234":  9.6 secs (368 recs)

    // Todo: 2019-11-25
    // ++1. Move totals, sort, .. from dgv to DGVList. Remove owner(dgv) property in DGVList
    // 2. May be: supply delegates for filters into DGVList instead of filter objects (flexibility for filters in UI)
    // 3. Refactoring the DGVList: 'DoAction' approach.

    // ToDo: 2019-11-20
    // 1. Common.IDGVList_GroupItem interface: SetTotalsProperties(Misc.TotalLine[] totals) method =>
    //    - move totals in DGVList class
    //    - new signature: SetTotalsProperties() method
    // ++ 2. Split Filter on 3 part: common/dbCmd/dgv
    // 3. Move totals, sort, .. from dgv to DGVList. Remove owner(dgv) property in DGVList
    // 4. Переробити деякі класи (модуля PropertyDescription, ...) на Generic
    // 5. SortsOfGroups union with Groups (DGVList)

    // Bug:
    // 1. TextFastFilter:
    //   - open MastCoA
    //  - set text filters: 3->30->300->3000->... поки не буде пустого набору даних (recs=0)
    //  - remove from filters '0': set filters from 3000000->300000->30000->3000->300 - фільтр спрацьовує тільки якщо довжина TextFastFilter невелика (десь 2-3 символи) 
    //  - причина: до уваги беруться тільки колонки групи
    // ++2. Зміна записаних налаштувань для MastCoA.
    //  - default налаштування з групами
    //  - якщо міняємо налаштування, то збиваються колонки
    // 3. Якщо було записано Settings з групой, а після цього в запиті даних Sql добавлено нове поле,
    //  то це поле зявляється в таблиці, хоча цього не повинно бути: нове поле не звязане з групою 
    //  і не може бути відображене в режимі групи. Приклад: sap.mastcoa("План рахунків") (група по полю "Account group")
    // ++4. If the settings are changed from group mode to non-group mode there are some group columns in datagrid with non-group mode
    //  - example sap.mastcoa("План рахунків")
    // 5. MastCoa - group by type, account, altacc, 3 level with no top levels: can't sort by altacc
    // 6. MastCoa - group by type, account, altacc, 3 level with no top levels:
    //  - a lof of bugs, for example, can't sort by altacc
  }
}
