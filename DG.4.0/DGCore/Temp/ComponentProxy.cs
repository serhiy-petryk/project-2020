namespace DGCore.Temp
{
    public class ComponentProxy
    {
        public decimal? COST_COMPONENT { get; set; }
        public string NAME  => "NAME" + COST_COMPONENT?.ToString();
        public override string ToString()
        {
            return 456.ToString();
        }
    }
}
