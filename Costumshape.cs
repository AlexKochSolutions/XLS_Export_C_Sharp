
namespace XLS_Export_C_Sharp
{
    internal class Costumshape
    {
        internal int[] startcell {  get; set; }
        internal int[] endcell { get; set; }
        internal string stringcontent { get; set; } = "";
        internal string stringshapename { get; set; } = "";
        internal int shapetype { get; set; }

        public Costumshape(int[] startcell, int[] endcell)
        {
            this.startcell = startcell;
            this.endcell = endcell;
        }

        public Costumshape(int[] startcell, int[] endcell, string content, int shapetype)
        {
            this.startcell = startcell;
            this.endcell = endcell;
            this.stringcontent = content;
            this.shapetype = shapetype;
        }
    }
}
