<<<<<<< HEAD
﻿using NPOI.SS.Formula.Functions;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;

namespace XLS_Export_C_Sharp
{
    internal class Databaseoperations
    {
        public List<TableScheme> Tables { get; private set; } = new List<TableScheme>();
        public SortedDictionary<string, DB_Table_Structure>? Tablestructures{ get; private set; } = null;

        private void databasequery(string query, OleDbConnection connection, TableScheme table, int mode)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            PrintOption.Print("start method databasequery");
            // SQL-Befehl vorbereiten
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                try
                {
                    // Daten über einen Reader auslesen
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        // Prüfen, ob Daten vorhanden sind
                        if (reader.HasRows)
                        {

                            int columnCount = reader.FieldCount;
                            string[] columnNames = new string[columnCount];

                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames[i] = reader.GetName(i);
                                if (mode != 2)
                                {
                                    PrintOption.Print($"i: {i} | columnNames: {columnNames[i]}");
                                }
                            }
                            if (mode > 0)
                            {
                                foreach (string c in columnNames)
                                {
                                    table.Columns.Add(c);
                                }
                            }
                            object[] rowValues = new object[columnCount];
                            int rownumber = 0;
                            while (reader.Read())
                            {
                                switch (mode)
                                {
                                    case 0:
                                        var id = reader[0];
                                        var abschnitt = reader[$"{table.Columns[0]}"].ToString();
                                        var tabelle = reader[$"{table.Columns[1]}"].ToString();
                                        PrintOption.Print($"ID: {id} | abschnitt: {abschnitt} | tabelle: {tabelle}");
                                        break;
                                    case 1:
                                        reader.GetValues(rowValues);
                                        string rowString = string.Join(";", rowValues.Select(v => v?.ToString() ?? ""));
                                        PrintOption.Print($"rownumber: {rownumber} | columnNames: {rowString}");
                                        table.Rows.Add(rowString);
                                        break;
                                    case 2:
                                        reader.GetValues(rowValues);
                                        string rowString2 = string.Join(";", rowValues.Select(v => v?.ToString() ?? ""));
                                        table.Rows.Add(rowString2);
                                        break;
                                }
                                rownumber++;
                            }
                        }
                        else
                        {
                            PrintOption.Print("Keine Daten in der Tabelle gefunden.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung (z.B. falscher Pfad oder fehlender Treiber)
                    PrintOption.Print("Fehler beim Zugriff auf die Datenbank:");
                    PrintOption.Print(ex.Message);
                }
            }
        }

        private void databaseconnection(string connectionString, TableScheme? table, int mode)
        {

            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            // 'using' stellt sicher, dass die Verbindung am Ende automatisch geschlossen wird
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {

                // Verbindung öffnen
                connection.Open();
                PrintOption.Print("Verbindung zur Access-Datenbank erfolgreich hergestellt!");

                if (connection != null)
                {
                    if (table != null)
                    {
                        string query = $"SELECT * FROM {table.Name}";
                        databasequery(query, connection, table, mode);
                    }
                    else
                    {
                        DataTable schemaTable = connection.GetSchema("Tables");

                        PrintOption.Print("Tables in the MDB database:");
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string? tableName = row["TABLE_NAME"].ToString();
                            string? tableType = row["TABLE_TYPE"].ToString();

                            // Only list actual tables (exclude system tables, views, etc.)
                            if (tableType != null && tableType.Equals("TABLE", StringComparison.OrdinalIgnoreCase) && tableName != null)
                            {
                                if (mode != 2)
                                {
                                    PrintOption.Print($"- {tableName}");
                                }
                                TableScheme ts2 = new TableScheme(tableName);
                                string query = $"SELECT * FROM {ts2.Name}";
                                databasequery(query, connection, ts2, mode);
                                Tables.Add(ts2);
                            }
                        }
                    }
                }
            }
        }

        public void tablesreadout()
        {
            PrintOption.Print("start method tablesreadout");
            foreach (var t in Tables)
            {
                PrintOption.Print($" table name: {t.Name}");
                foreach (string column in t.Columns)
                {
                    PrintOption.Print($" column name: {column}");
                }
                foreach (string row in t.Rows)
                {
                    PrintOption.Print($" row content: {row}");
                }
            }
        }

        public void tablestructurereadout()
        {
            if (Tablestructures == null)
            {
                return;
            }
            PrintOption.Print("start method tablestructurereadout");
            foreach (var ts in Tablestructures)
            {
                PrintOption.Print($"ts.Key: {ts.Key}");
                foreach (var sts in ts.Value.sub_table)
                {
                    PrintOption.Print($"sts.Key: {sts.Key}");
                }
            }
        }

        public void dbtableexamination(string? tablestructure_str)
        {
            if (tablestructure_str == null || tablestructure_str.Length == 0) return;
            PrintOption.Print("start method dbtableexamination");

            string tablename = string.Empty;
            int column_structure = -1;
            int column_sub_structure = -1;
            char[] delimiters = ['_'];
            string[] tokens = tablestructure_str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 3)
            {
                tablename = tokens[0];

                if (int.TryParse(tokens[1], out int result))
                {
                    PrintOption.Print($"Success! Integer: {result}");
                    column_structure = result;
                }
                else
                {
                    PrintOption.Print("Conversion failed. Invalid format.");
                }
                if (int.TryParse(tokens[2], out int result2))
                {
                    PrintOption.Print($"Success! Integer: {result2}");
                    column_sub_structure = result2;
                }
                else
                {
                    PrintOption.Print("Conversion failed. Invalid format.");
                }
            }
            if (string.IsNullOrEmpty(tablename) || column_structure == -1 || column_sub_structure == -1) return;
            Tablestructures = new SortedDictionary<string, DB_Table_Structure>();
            foreach (var t in Tables)
            {
                if (t == null) continue;
                if (t.Name.Equals(tablename))
                {
                    foreach (var item in t.Rows)
                    {
                        char[] delimiters2 = [';'];
                        string[] tokens2 = item.Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens2.Length < 3) continue;
                        PrintOption.Print($"tokens2[1].Trim(): {tokens2[1].Trim()} tokens2[2].Trim(): {tokens2[2].Trim()}");
                        if (Tablestructures.ContainsKey(tokens2[1].Trim()))
                        {
                            PrintOption.Print("Key existed, so the value was updated.");
                            Tablestructures[tokens2[1].Trim()].sub_table[tokens2[2].Trim()] = new TableScheme(tokens2[2].Trim());
                        }
                        else
                        {
                            PrintOption.Print("Added a brand new entry.");
                            Tablestructures[tokens2[1].Trim()] = new DB_Table_Structure();
                            Tablestructures[tokens2[1].Trim()].sub_table[tokens2[2].Trim()] = new TableScheme(tokens2[2].Trim());
                        }
                        
                    }
                }
            }
        }

        public void databasereadout(string? databasePath, string? tablestructure)
        {
            long startTime = Stopwatch.GetTimestamp();
            if (databasePath == null) {
                PrintOption.Print("databasePath == null");
                return; 
            }
            // Für .accdb Dateien (Access 2007 und neuer): Microsoft.ACE.OLEDB.12.0 oder 16.0
            // Für ältere .mdb Dateien: Microsoft.Jet.OLEDB.4.0
            if (!File.Exists(databasePath))
            {
                PrintOption.Print("!File.Exists(databasePath)");
                return;
            }
            PrintOption.Print("start method databasereadout");
            string connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;";

            //TabellenSchema ts = new TabellenSchema("Tabellen");
            //ts.Columns.Add("Abschnitt");
            //ts.Columns.Add("Tabelle");

            //databaseconnection(connectionString, ts, 0);

            //TabellenSchema ts2 = new TabellenSchema("Results");

            //databaseconnection(connectionString, ts2, 1);

            databaseconnection(connectionString, null, 2);

            dbtableexamination(tablestructure);

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTime);

            PrintOption.Print($"\nExecution time: {elapsed.TotalMilliseconds} ms\n");
        }

    }
}
=======
﻿using System.Data;
using System.Data.OleDb;

namespace XLS_Export_C_Sharp
{
    internal class Databaseoperations
    {
        public List<TabellenSchema> Tables { get; private set; } = new List<TabellenSchema>();

        private void databasequery(string query, OleDbConnection connection, TabellenSchema table, int mode)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }
            // SQL-Befehl vorbereiten
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                try
                {
                    // Daten über einen Reader auslesen
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        // Prüfen, ob Daten vorhanden sind
                        if (reader.HasRows)
                        {

                            int columnCount = reader.FieldCount;
                            string[] columnNames = new string[columnCount];

                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames[i] = reader.GetName(i);
                                if (mode != 2)
                                {
                                    Console.WriteLine($"i: {i} | columnNames: {columnNames[i]}");
                                }
                            }
                            if (mode > 0)
                            {
                                foreach (string c in columnNames)
                                {
                                    table.Columns.Add(c);
                                }
                            }
                            object[] rowValues = new object[columnCount];
                            int rownumber = 0;
                            while (reader.Read())
                            {
                                switch (mode)
                                {
                                    case 0:
                                        var id = reader[0];
                                        var abschnitt = reader[$"{table.Columns[0]}"].ToString();
                                        var tabelle = reader[$"{table.Columns[1]}"].ToString();
                                        Console.WriteLine($"ID: {id} | abschnitt: {abschnitt} | tabelle: {tabelle}");
                                        break;
                                    case 1:
                                        reader.GetValues(rowValues);
                                        string rowString = string.Join(";", rowValues.Select(v => v?.ToString() ?? ""));
                                        Console.WriteLine($"rownumber: {rownumber} | columnNames: {rowString}");
                                        table.Rows.Add(rowString);
                                        break;
                                    case 2:
                                        reader.GetValues(rowValues);
                                        string rowString2 = string.Join(";", rowValues.Select(v => v?.ToString() ?? ""));
                                        table.Rows.Add(rowString2);
                                        break;
                                }
                                rownumber++;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Keine Daten in der Tabelle gefunden.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung (z.B. falscher Pfad oder fehlender Treiber)
                    Console.WriteLine("Fehler beim Zugriff auf die Datenbank:");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void databaseconnection(string connectionString, TabellenSchema? table, int mode)
        {

            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            // 'using' stellt sicher, dass die Verbindung am Ende automatisch geschlossen wird
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {

                // Verbindung öffnen
                connection.Open();
                Console.WriteLine("Verbindung zur Access-Datenbank erfolgreich hergestellt!");

                if (connection != null)
                {
                    if (table != null)
                    {
                        string query = $"SELECT * FROM {table.Name}";
                        databasequery(query, connection, table, mode);
                    }
                    else
                    {
                        DataTable schemaTable = connection.GetSchema("Tables");

                        Console.WriteLine("Tables in the MDB database:");
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string? tableName = row["TABLE_NAME"].ToString();
                            string? tableType = row["TABLE_TYPE"].ToString();

                            // Only list actual tables (exclude system tables, views, etc.)
                            if (tableType != null && tableType.Equals("TABLE", StringComparison.OrdinalIgnoreCase) && tableName != null)
                            {
                                if (mode != 2)
                                {
                                    Console.WriteLine($"- {tableName}");
                                }
                                TabellenSchema ts2 = new TabellenSchema(tableName);
                                string query = $"SELECT * FROM {ts2.Name}";
                                databasequery(query, connection, ts2, mode);
                                Tables.Add(ts2);
                            }
                        }
                    }
                }
            }
        }

        public void tablesreadout()
        {
            foreach (var t in Tables)
            {
                Console.WriteLine($" table name: {t.Name}");
                foreach (string column in t.Columns)
                {
                    Console.WriteLine($" column name: {column}");
                }
                foreach (string row in t.Rows)
                {
                    Console.WriteLine($" row content: {row}");
                }
            }
        }

        public void databasereadout(string? databasePath)
        {
            if (databasePath == null) {
                Console.WriteLine("databasePath == null");
                return; 
            }
            // Für .accdb Dateien (Access 2007 und neuer): Microsoft.ACE.OLEDB.12.0 oder 16.0
            // Für ältere .mdb Dateien: Microsoft.Jet.OLEDB.4.0
            if (!File.Exists(databasePath))
            {
                Console.WriteLine("!File.Exists(databasePath)");
                return;
            }
            string connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;";

            //TabellenSchema ts = new TabellenSchema("Tabellen");
            //ts.Columns.Add("Abschnitt");
            //ts.Columns.Add("Tabelle");

            //databaseconnection(connectionString, ts, 0);

            //TabellenSchema ts2 = new TabellenSchema("Results");

            //databaseconnection(connectionString, ts2, 1);

            databaseconnection(connectionString, null, 2);
        }

    }
}
>>>>>>> bb2a77754ddfb50b72dfe360a703998a3ea8112c
