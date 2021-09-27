namespace DGWnd
{
  public class Refactoring_2019_12_WiP
  {
    // frmDGV дає команди DGVList, DGVList дає events => DGVCube оновлюється

    // Open tasks (2019-12-31):
    // 5. Bug: open already opened DataDefinition with blank setting (no groups) => Error in DGVCube.OnCellEnter(GetCellDataStatus)
    //     передивитися політика привязки datagrid до даних. Можливо, окрема форма повинна мати окремий dataset.
    //     Один із варіантів: статичний клас використовується для зберігання datasource instances, data dictionary(for compressed data),
    //     а окремі набори даних мають свої data source instances.
    // 7. Bug: TimeSpan datatype (sql example 'select top 1000000 CONVERT (time, CURRENT_TIMESTAMP) timed, * from gldocline'), export to Excel -> error (TimeSpan does not supported)
    // 7.2 Develop: export to text file: do not show records in group mode, remove 'NaN' for numbers
    // 10. Develop: RemoveFilter in group mode with expanded group => refresh resets grid to start view and do not save expanded groups (example based on MastCoA)
    // 19. Check catch blocks, 'Lovushka' exception.
    // 1. Remove flashing on DGVCube closing??? 
    // 4. ?++Не показується SortGlyph після Requery і якщо для групи робиться ExpandItems (те ж і для режиму без груп).
    // 5. ?++Після Requery зовсім не показуються SortGlyph, навіть для колонок груп.


    // ==========================================================
    // ToDo (2019-12-27):
    // ++1. Remove ColumnInfo
    // ++2. Refactoring: remove set for properties in DGVList
    // ++3. Freeze in 15 sces when GLDOCLINE is loading in hot mode
    // ++4. Bug: no sort glyph for second sort column (column 'Cost element') in MastCoA detail area (versions: DGVCube 2.0. && DGVCube 2.1)
    //      fixed: increase column width (glyph is hidden because column width is narrow)  
    // 5. Bug: open already opened DataDefinition with blank setting (no groups) => Error in DGVCube.OnCellEnter(GetCellDataStatus)
    //     передивитися політика привязки datagrid до даних. Можливо, окрема форма повинна мати окремий dataset.
    //     Один із варіантів: статичний клас використовується для зберігання datasource instances, data dictionary(for compressed data),
    //     а окремі набори даних мають свої data source instances.
    // ++6. Bug: dgv.DataSource.WhereFilter doesn't work UC_DGVLayout (not saved after changing)
    // 7. Bug: TimeSpan datatype (sql example 'select top 1000000 CONVERT (time, CURRENT_TIMESTAMP) timed, * from gldocline'), export to Excel -> error (TimeSpan does not supported)
    // 7.2 Develop: export to text file: do not show records in group mode, remove 'NaN' for numbers
    // ++8. Actions
    //    Filters, Sorts, Find
    //    ++remove DGVCube.Requery method
    //    ++remove IDGVList.Requeries methods
    // ++9. Refactoring: remove DGVCube._cellLast_... variables support, change SortGlyph (DGVCube.OnCellEnter method), ..
    // 10. Develop: RemoveFilter in group mode with expanded group => refresh resets grid to start view and do not save expanded groups (example based on MastCoA)
    // ++11. Bug: Error on clone gldocline
    // ++12. Bug: error on refresh sorted (ascendingby amount) GLDOCLINE
    //        Index was out of range. Must be non-negative and less than the size of the collection. Parameter name: index
    // ++13. DGVCube clone: apply current settings to clone
    // ++14. Bug: Click close button while data is loading => frmDGV is hangup
    // ++15. Develop: Requery action: add cancel action support
    // ++16: Develop: clone - сохранять текущие настройки (see 13 item)
    // ++17. Remove frmDGV.MULTI_THREAD_MODE variable.
    // ++18. Remove BackgroundWorker in DbDataSource.Extension.cs
    // 19. Check catch blocks, 'Lovushka' exception.
    // ++20. Check duplicates for Lookup tables

    // ToDo (2019-12-02):
    // +1. To fix LoadData bug (одночасне завантаження) (see ToDo (2019-12-27))
    // ++2. Зробити кнопку "Clear database cache" + протетсувати завантаження (GlDocline)
    // ++3. Перебудувати LoadData на Tasks. Remove BackgroundWorker
    // ++4. Це краще зробити після переносу команд з DGVCube to DGVList
    // DGVCube - саме відповідає на взаємодію з DGVList. Вилучити з frmDGV непотрібну взаємодію з DGVCube +
    //      перенести Data bind from DGVCube to frmDGV.
    // 5. Після переводу LoadData on Task remove непотрібні InvokeRequreid і LoadData for BO_LookupTable. Вичистити код.
    // ++6. RequeryData => повинні також бути оновлені Lookup Tables

    // ToDo:
    // 1. Remove flashing on DGVCube closing??? 
    // ++2. FastFilter: що робити з VisibleColumnInfo, яка потрібна для FastFilter
    //  - можливо в конструктор передавати силку на функцію, яка вертає список видимих колонок

    // ++3. Add Settings for DGVList (split ApplySettings for DGVList/DGVCube); remove public sets for some DGVList properties
    // ++4. Переробити на DoAction. Стерти в DGVCube RefreshData, DGVList убрать усі Refresh..., можливо тільки залишити один RefreshData.
    // ++5. Повторне Requery data - після завантаження у рядку статуса вказати час завантаження.

    // Bug:
    // ++1. Total column щезає після сортування всередині групи на рівні items
    // ++(for one thread mode, ? multithread mode) 2. After повторного REqueryData курсор = waiting (GLDocline)
    // ++(for one thread mode, ? multithread mode) 3. В кінці завантаження не оновлється статус. К-сть завантажених рядків - не 1'000'000, a 990'000. (GLDocline)
    // 4. ?++Не показується SortGlyph після Requery і якщо для групи робиться ExpandItems (те ж і для режиму без груп).
    // 5. ?++Після Requery зовсім не показуються SortGlyph, навіть для колонок груп.
    // ++6. Make background mode for Refresh (sort, apply filter)
    // +7. Одночасне завантаження даних - виникає помилка (see ToDo (2019-12-27))

  }
}
