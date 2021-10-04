namespace DGCore.Helpers
{
    public class Color
    {
        #region =========  Static section  ============
        // private static readonly Color _totalLineColor = new Color(0xDC, 0xDC, 0xDC); // Gainsboro

        public static readonly Color[] GroupColors =
        {
            new Color(0xDC, 0xDC, 0xDC), new Color(255, 153, 204), new Color(255, 204, 153), new Color(255, 255, 153),
            new Color(204, 255, 204), new Color(204, 255, 255), new Color(153, 204, 255), new Color(204, 153, 255)
        };

        public static Color GetTotalLineColor => GroupColors[0];
        // var rowBrush = isGroupRow ? _groupBrushes[groupItem.Level == 0 ? 0 : ((groupItem.Level - 1) % (_groupBrushes.Length - 1)) + 1] : null;
        public static Color GetGroupColor(int level)
        {
            var k = ((level - 1) % (GroupColors.Length - 1)) + 1;
            return GroupColors[level == 0 ? 0 : ((level - 1) % (GroupColors.Length - 1)) + 1];
        }

        public static int GetExcelColor(Color color) => (color.B << 16) + (color.G << 8) + color.R;

        #endregion

        #region =========  Instance section  ============

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public Color(byte r, byte g, byte b)
        {
            R = r; G = g; B = b;
        }

        #endregion

    }
}
