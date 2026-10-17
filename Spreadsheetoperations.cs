using NPOI.HSSF.UserModel; // Required for legacy .xls files
using NPOI.SS.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Spreadsheetoperations
    {
        public List<Costumworksheet> costumworksheets { get; private set; } = new List<Costumworksheet>();

        public void readin(string? filePath)
        {
            if (filePath == null) {
                Console.WriteLine("readin filePath == null");
                return; 
            }
            if (!File.Exists(filePath))
            {
                Console.WriteLine("readin !File.Exists(filePath)");
                return;
            }
            Console.WriteLine("start method readin");
            // Required for .NET Core / .NET 5+ to support older code pages used by .xls
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // HSSFWorkbook opens the legacy binary format (.xls)
                    IWorkbook workbook = new HSSFWorkbook(file);
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        ISheet sheet = workbook.GetSheetAt(i); // Get the first worksheet
                        if (sheet == null) continue;
                        Console.WriteLine($"Sheet({i}) Name: {sheet.SheetName}");
                        Costumworksheet ws = new Costumworksheet(sheet.SheetName);
                        for (int j = 0; j < sheet.PhysicalNumberOfRows; j++)
                        {
                            IRow row = sheet.GetRow(j);
                            if (row != null)
                            {
                                Console.WriteLine($"row number: {j}");
                                for (int k = 0; k < row.LastCellNum - 1; k++)
                                {
                                    ICell cell = row.GetCell(k);
                                    if (cell != null)
                                    {
                                        Console.WriteLine($"cell number: {k} string: {cell.StringCellValue}");

                                        if (cell.Hyperlink != null)
                                        {
                                            Console.WriteLine($"Hyperlink label: {cell.Hyperlink.Label} Address: {cell.Hyperlink.Address}");
                                        }

                                        ICellStyle style = cell.CellStyle;
                                        if (style == null) continue;

                                        // Note: FillForegroundColor represents the main fill color in NPOI
                                        short fillBackColor = style.FillForegroundColor;
                                        Console.WriteLine($"Fill Color Index: {fillBackColor}");
                                        Console.WriteLine($"Fill Color fill pattern: {style.FillPattern}");

                                        Console.Write($"Top Border Style: {style.BorderTop}\t");
                                        Console.Write($"Left Border Style: {style.BorderLeft}\t");
                                        Console.Write($"Right Border Style: {style.BorderRight}\t");
                                        Console.Write($"Bottom Border Style: {style.BorderBottom}\n");

                                        Console.Write($"Horizontal Alignment: {style.Alignment}\n");

                                        IFont font = style.GetFont(workbook);
                                        if (font == null) continue;
                                        Console.Write($"Font Name: {font.FontName}\t");
                                        Console.Write($"Font Size (Points): {font.FontHeightInPoints}");
                                        Console.Write($"Is Bold: {font.IsBold}\t");
                                        Console.Write($"Is Italic: {font.IsItalic}\t");
                                        Console.Write($"Is IsStrikeout: {font.IsStrikeout}\t");
                                        Console.Write($"Is TypeOffset: {font.TypeOffset}\t");
                                        Console.Write($"Font Color Index: {font.Color}\n");
                                        TableCell ts = new TableCell(j, k, cell.StringCellValue,
                                            ws.GetStyleNumber(fillBackColor,
                                            style.FillPattern,
                                            [style.BorderTop, style.BorderLeft, style.BorderRight, style.BorderBottom],
                                            style.Alignment,
                                            font.FontName,
                                            font.Color,
                                            font.TypeOffset,
                                            font.FontHeightInPoints,
                                            font.IsBold,
                                            font.IsItalic,
                                            font.Underline,
                                            font.IsStrikeout));
                                        ws.AddCell(ts);
                                        //ws.Cells.Add(ts);
                                    }
                                }
                            }
                        }
                        costumworksheets.Add(ws);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: The file does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: You do not have permission to access this file.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: The file is locked by another process or a disk error occurred. Details: {ex.Message}");
            }
        }

        public void writein(string? filePath)
        {
            if (filePath == null) return;
            Console.WriteLine("start method writein");

            IWorkbook workbook = new HSSFWorkbook();
            foreach(Costumworksheet ws in costumworksheets)
            {
                ISheet sheet = workbook.CreateSheet(ws.Name);

                SortedDictionary<int, ICellStyle> styles = new SortedDictionary<int, ICellStyle>();

                foreach(Costumstyle cws in ws.Styles)
                {
                    ICellStyle iCellStyle = workbook.CreateCellStyle();
                    IFont iFont = workbook.CreateFont();
                    iFont.FontName = cws.fontname;
                    iFont.FontHeightInPoints = cws.height;
                    iFont.IsBold = cws.isBold;
                    iFont.Color = cws.fontcolor;
                    iCellStyle.SetFont(iFont);

                    iCellStyle.FillForegroundColor = cws.fillBackColor;
                    iCellStyle.FillPattern = cws.fillpattern;
                    iCellStyle.Alignment = cws.alignment;

                    iCellStyle.BorderBottom = cws.borders[0];
                    iCellStyle.BorderLeft = cws.borders[1];
                    iCellStyle.BorderRight = cws.borders[2];
                    iCellStyle.BorderTop = cws.borders[3];

                    styles.Add(cws.number, iCellStyle);
                }
                int realmaxc = 0;
                Dictionary<int, int> widths = new Dictionary<int, int>();
                for (int r = 0; r < ws.maxrow; r++)
                {
                    IRow row = sheet.CreateRow(r);
                    for (int c = 0; c < ws.maxcol; c++)
                    {
                        ICell cell = row.CreateCell(c);
                        TableCell? tc = null;
                        if (ws.Cells.TryGetValue($"{r},{c}", out var value))
                        {
                            Console.WriteLine($"Key'{r},{c}' found! The value is: {value}");
                            tc = value;
                            realmaxc = c;
                        }
                        else
                        {
                            Console.WriteLine($"Key'{r},{c}' does not exist.");
                        }
                        //TableCell tc = ws.Cells[$"{r + 1},{c}"];
                        if (tc == null) { continue; }
                        cell.CellStyle = styles[tc.stylenumber];

                        string? measure = "";
                        if (tc.name is int intVal)
                        {
                            cell.SetCellValue(intVal);
                            measure = $"{intVal}";
                        }
                        else if (tc.name is double dblVal)
                        {
                            cell.SetCellValue(dblVal);
                            measure = $"{dblVal}";
                        }
                        else
                        {
                            if (tc.name != null && tc.name.ToString() != null)
                            {
                                cell.SetCellValue(tc.name.ToString());
                                measure = tc.name.ToString();
                            }
                        }
                        if (measure == null)
                        {
                            continue;
                        }
                        if (widths.TryGetValue(c, out var value2))
                        {
                            Console.WriteLine($"Key'{c}' found! The value is: {value2}");
                            if (measure.Length > value2)
                            {
                                widths[c] = measure.Length;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Key'{r},{c}' does not exist.");
                            widths.Add(c, measure.Length);
                        }
                    }
                }

                int padding = 5;
                
                for (int i = 0; i < realmaxc; i++)
                {
                    // Math: (Length of longest text + padding) * 256
                    sheet.SetColumnWidth(i, (widths[i] + padding) * 256);
                }
            }

            Console.WriteLine($"FileStream fs {filePath} FileMode {FileMode.Create} FileAccess {FileAccess.Write}");
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: The file does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: You do not have permission to access this file.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: The file is locked by another process or a disk error occurred. Details: {ex.Message}");
            }

            Console.WriteLine("Successfully created the .xls file!");
        }
    }
}
