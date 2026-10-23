
namespace XLS_Export_C_Sharp
{
    public class TableScheme
    {
        public string Name { get; set; }
        public List<string> Columns { get; set; }
        public List<string> Rows { get; set; }
        public TableScheme(string name)
        {
            Name = name;
            Columns = new List<string>();
            Rows = new List<string>();
        }
    }
}
