using Microsoft.Extensions.Configuration;

namespace XLS_Export_C_Sharp
{
    class Program
    {
        private static Databaseoperations dbo = new Databaseoperations();
        private static Spreadsheetoperations sso = new Spreadsheetoperations();

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
            if (args.Length > 0)
            {
                dbo.databasereadout(args[0]);
                dbo.tablesreadout();
                if (args.Length > 1)
                {
                    sso.readin(args[1]);
                }
            }
            else
            {
                if (config != null)
                {
                    Console.WriteLine($"config[\"AppSettings:Databasepath\"] {config["AppSettings:Databasepath"]}");
                    Console.WriteLine($"config[\"AppSettings:XLS_Read\"] {config["AppSettings:XLS_Read"]}");
                    Console.WriteLine($"config[\"AppSettings:XLS_Write\"] {config["AppSettings:XLS_Write"]}");
                    dbo.databasereadout(config["AppSettings:Databasepath"]);
                    dbo.tablesreadout();
                    sso.readin(config["AppSettings:XLS_Read"]);
                    sso.writein(config["AppSettings:XLS_Write"]);
                }
                else
                {
                    Console.WriteLine("appsettings.json sould not be opened");
                }
            }

            Console.WriteLine("\nPress a key to exit...");
            Console.ReadKey();
        }
    }
}