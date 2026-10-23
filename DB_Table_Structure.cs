
namespace XLS_Export_C_Sharp
{
    internal class DB_Table_Structure
    {
        internal SortedDictionary<string, TableScheme> sub_table { get; set; }

        public DB_Table_Structure() {
            sub_table = new SortedDictionary<string, TableScheme>();
        }
    }
}
