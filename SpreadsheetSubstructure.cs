namespace XLS_Export_C_Sharp
{
    internal class SpreadsheetSubstructure
    {
        internal string? substructurename { get; set; } = "";
        internal int[] position { get; set; } = { 0, 0 };

        public SpreadsheetSubstructure(string? substructurename, int[] position)
        {
            this.substructurename = substructurename;
            this.position = position;
        }
    }
}
