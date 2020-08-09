namespace DGView.Common
{
    public class CellViewModeClass
    {
        public enum CellViewMode { NotSet, OneRow, WordWrap }

        public static CellViewModeClass[] CellViewModeValues = new[]
        {
            new CellViewModeClass(CellViewMode.NotSet, "Не встановлено"),
            new CellViewModeClass(CellViewMode.OneRow, "1 рядок"),
            new CellViewModeClass(CellViewMode.WordWrap, "Переніс слів")
        };
        //================================
        public CellViewMode Id { get; }
        public string Name { get; }
        public CellViewModeClass(CellViewMode id, string name)
        {
            Id = id; Name = name;
        }

        public override string ToString() => Id.ToString();
    }
}
