
using Microsoft.Extensions.Configuration;

namespace XLS_Export_C_Sharp
{
    class Program
    {
        private static Databaseoperations dbo = new Databaseoperations();
        private static Spreadsheetoperations sso = new Spreadsheetoperations();

        static void Main(string[] args)
        {
            PrintOption.StartLog();
            var config = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
            if (args.Length > 0)
            {
                dbo.databasereadout(args[0], "");
                dbo.tablesreadout();
                if (args.Length > 1)
                {
                    sso.readout(args[1]);
                }
            }
            else
            {
                if (config != null)
                {
                    PrintOption.Print($"config[\"AppSettings:Databasepath\"] {config["AppSettings:Databasepath"]}");
                    PrintOption.Print($"config[\"AppSettings:XLS_Read\"] {config["AppSettings:XLS_Read"]}");
                    PrintOption.Print($"config[\"AppSettings:XLS_Write\"] {config["AppSettings:XLS_Write"]}");
                    PrintOption.Print($"config[\"AppSettings:DB_Table_Structure\"] {config["AppSettings:DB_Table_Structure"]}");
                    PrintOption.Print($"config[\"AppSettings:XLS_Table_Structure\"] {config["AppSettings:XLS_Table_Structure"]}");
                    dbo.databasereadout(config["AppSettings:Databasepath"], config["AppSettings:DB_Table_Structure"]);
                    dbo.tablesreadout();
                    dbo.tablestructurereadout();
                    sso.readout(config["AppSettings:XLS_Read"]);
                    sso.determinetablestructure(config["AppSettings:XLS_Table_Structure"]);
                    sso.tablestructurereadout();
                    sso.writein(config["AppSettings:XLS_Write"]);
                }
                else
                {
                    PrintOption.Print("appsettings.json sould not be opened");
                }
            }

            Console.WriteLine("\nPress a key to exit...");
            Console.ReadKey();
            PrintOption.Dispose();
        }
    }
}
