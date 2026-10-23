
namespace XLS_Export_C_Sharp
{
    internal class TableCell
    {
        internal int row { get; set; }
        internal int col { get; set; }
        internal object name { get; set; }
        //internal string value { get; set; }
        internal string type { get; set; }
        internal string hyperlinklabel { get; set; }
        internal string hyperlinkaddress { get; set; }
        internal int stylenumber { get; set; }

        public TableCell(int row, int col, object name, int stylenumber)
        {
            this.row = row;
            this.col = col;
            this.name = name;
            this.stylenumber = stylenumber;
            type = "";
            hyperlinklabel = "";
            hyperlinkaddress = "";
        }
    }
}
