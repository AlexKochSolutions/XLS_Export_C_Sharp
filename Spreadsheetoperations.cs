<<<<<<< HEAD
﻿using NPOI.HSSF.UserModel; // Required for legacy .xls files
using NPOI.SS.UserModel;
using System.Text;
using static NPOI.SS.Formula.PTG.AttrPtg;
//using NPOI.XSSF.Streaming;
//using NPOI.XSSF.Streaming.Values;
//using NPOI.XSSF.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Spreadsheetoperations
    {
        public List<Costumworksheet> costumworksheets { get; private set; } = new List<Costumworksheet>();
        public List<SpreadsheetStructure> worksheetstructure { get; private set; } = new List<SpreadsheetStructure>();

        // Helper method to decode common HSSF Object Type integer flags
        static string GetHSSFShapeName(int typeId)
        {
            return typeId switch
            {
                1 => "Line",
                2 => "Rectangle",
                3 => "Oval",
                5 => "Polygon",
                // Other esoteric Microsoft BIFF8 drawing indices fall back here
                _ => "Generic Shape/Object"
            };
        }
        public void readout(string? filePath)
        {
            if (filePath == null) {
                PrintOption.Print("readin filePath == null");
                return; 
            }
            if (!File.Exists(filePath))
            {
                PrintOption.Print("readin !File.Exists(filePath)");
                return;
            }
            PrintOption.Print("start method readin");
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
                        PrintOption.Print($"Sheet({i}) Name: {sheet.SheetName}");
                        Costumworksheet ws = new Costumworksheet(sheet.SheetName);
                        for (int j = 0; j < sheet.PhysicalNumberOfRows; j++)
                        {
                            IRow row = sheet.GetRow(j);
                            if (row != null)
                            {
                                PrintOption.Print($"row number: {j}");
                                for (int k = 0; k < row.LastCellNum - 1; k++)
                                {
                                    NPOI.SS.UserModel.ICell cell = row.GetCell(k);
                                    if (cell != null)
                                    {
                                        PrintOption.Print($"cell number: {k} string: {cell.StringCellValue}");

                                        if (cell.Hyperlink != null)
                                        {
                                            PrintOption.Print($"Hyperlink label: {cell.Hyperlink.Label} Address: {cell.Hyperlink.Address}");
                                        }

                                        ICellStyle style = cell.CellStyle;
                                        if (style == null) continue;

                                        // Note: FillForegroundColor represents the main fill color in NPOI
                                        short fillBackColor = style.FillForegroundColor;
                                        PrintOption.Print($"Fill Color Index: {fillBackColor}");
                                        PrintOption.Print($"Fill Color fill pattern: {style.FillPattern}");

                                        StringBuilder printline = new StringBuilder();
                                        printline.Append($"Top Border Style: {style.BorderTop}\t");
                                        printline.Append($"Left Border Style: {style.BorderLeft}\t");
                                        printline.Append($"Right Border Style: {style.BorderRight}\t");
                                        printline.Append($"Bottom Border Style: {style.BorderBottom}\n");

                                        printline.Append($"Horizontal Alignment: {style.Alignment}\n");

                                        IFont font = style.GetFont(workbook);
                                        if (font == null) continue;
                                        printline.Append($"Font Name: {font.FontName}\t");
                                        printline.Append($"Font Size (Points): {font.FontHeightInPoints}");
                                        printline.Append($"Is Bold: {font.IsBold}\t");
                                        printline.Append($"Is Italic: {font.IsItalic}\t");
                                        printline.Append($"Is IsStrikeout: {font.IsStrikeout}\t");
                                        printline.Append($"Is TypeOffset: {font.TypeOffset}\t");
                                        printline.Append($"Font Color Index: {font.Color}\n");
                                        PrintOption.Print(printline.ToString());
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
                        HSSFPatriarch? drawing = sheet.DrawingPatriarch as HSSFPatriarch;

                        if (drawing != null)
                        {
                            PrintOption.Print("Found HSSFPatriarch drawing");
                            foreach (HSSFShape shape in drawing.Children)
                            {
                                // Filter for standard geometric entities (Rectangles, lines, etc.)
                                if (shape is HSSFSimpleShape simpleShape)
                                {
                                    // Get coordinates pinned to the spreadsheet grid layout
                                    HSSFClientAnchor anchor = (HSSFClientAnchor)simpleShape.Anchor;

                                    int startRow = anchor.Row1;
                                    int startCol = anchor.Col1;
                                    int endRow = anchor.Row2;
                                    int endCol = anchor.Col2;

                                    // Identify the structural shape definition code
                                    int shapeType = simpleShape.ShapeType;
                                    string shapeTypeName = GetHSSFShapeName(shapeType);

                                    PrintOption.Print($"Found Shape Type ID: {shapeType} ({shapeTypeName})");
                                    PrintOption.Print($"-> Located from Cell Grid ({startRow},{startCol}) to ({endRow},{endCol})");

                                    Costumshape cs = new Costumshape(new int[] { startRow, startCol }, new int[] { endRow, endCol });

                                    cs.shapetype = shapeType;
                                    cs.stringshapename = shapeTypeName;

                                    // Extract text metadata inside the shape boundaries (if applicable)
                                    if (simpleShape.String != null && !string.IsNullOrEmpty(simpleShape.String.String))
                                    {
                                        PrintOption.Print($"-> Contained Label Text: {simpleShape.String.String}");
                                        cs.stringcontent = simpleShape.String.String;
                                    }
                                    PrintOption.Print("");
                                    ws.Shapes.Add(cs);
                                }
                            }
                        }
                        else
                        {
                            PrintOption.Print("No classic drawing layer discovered on this worksheet index.");
                        }
                        costumworksheets.Add(ws);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                PrintOption.Print("Error: The file does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                PrintOption.Print("Error: You do not have permission to access this file.");
            }
            catch (IOException ex)
            {
                PrintOption.Print($"Error: The file is locked by another process or a disk error occurred. Details: {ex.Message}");
            }
        }

        public void determinetablestructure(string? tablename)
        {
            if (tablename == null)
            {
                return;
            }
            PrintOption.Print("start method determinetablestructure");
            foreach (var ws in costumworksheets)
            {
                if (ws.Name.Equals(tablename))
                {
                    int thinlinecounter = 0;
                    SpreadsheetStructure spshsust = new SpreadsheetStructure(tablename);
                    for (int i = 0; i < ws.maxrow; i++)
                    {
                        
                        for (int j = 0; j < ws.maxcol; j++)
                        {
                            if (ws.Cells.TryGetValue($"{i},{j}", out var value)){
                                if (spshsust.headerleft[0] == -1)
                                {
                                    if (value != null && value.name != null  && value.name.ToString()?.Length > 0)
                                    {
                                        PrintOption.Print($"{i},{j} -> value.name {value.name.ToString()}");
                                        int[] position = { i, j };
                                        spshsust.capters.Add(spshsust.capters.Count + 1, new SpreadsheetSubstructure(value.name.ToString(), position));
                                    }
                                }
                                if (value == null || value.stylenumber < 1 || ws.Styles.Count < value.stylenumber - 1)
                                {
                                    continue;
                                }
                                if (j == 0)
                                {
                                    PrintOption.Print($"ws.Styles[value.stylenumber - 1].borders[0] {ws.Styles[value.stylenumber - 1].borders[0]} borders[3] {ws.Styles[value.stylenumber - 1].borders[3]}");
                                    if (ws.Styles[value.stylenumber - 1].borders[0] == BorderStyle.Thin && ws.Styles[value.stylenumber - 1].borders[3] == BorderStyle.Thin)
                                    {
                                        thinlinecounter++;
                                    }
                                    else
                                    {
                                        thinlinecounter = 0;
                                    }
                                    PrintOption.Print($"thinlinecounter {thinlinecounter}");
                                }
                                if (ws.Styles[value.stylenumber - 1].borders[0] > BorderStyle.Thin)
                                {
                                    if ((spshsust.headerleft[0] == -1 || (spshsust.headerleft[0] == i && spshsust.headerleft[1] > j)))
                                    {
                                        spshsust.headerleft[0] = i;
                                        spshsust.headerleft[1] = j;
                                    }
                                    if ((spshsust.headerright[0] == -1 || (spshsust.headerright[0] == i && spshsust.headerright[1] < j)))
                                    {
                                        spshsust.headerright[0] = i;
                                        spshsust.headerright[1] = j;
                                    }
                                }
                                if ((spshsust.headerleft[0] != -1 && spshsust.headerleft[0] < i) && ws.Styles[value.stylenumber - 1].borders[3] > BorderStyle.Thin)
                                {
                                    if ((spshsust.tabletopleft[0] == -1 || (spshsust.tabletopleft[0] == i && spshsust.tabletopleft[1] > j)))
                                    {
                                        spshsust.tabletopleft[0] = i;
                                        spshsust.tabletopleft[1] = j;
                                    }
                                    if ((spshsust.tabletopright[0] == -1 || (spshsust.tabletopright[0] == i && spshsust.tabletopright[1] < j)))
                                    {
                                        spshsust.tabletopright[0] = i;
                                        spshsust.tabletopright[1] = j;
                                    }
                                }
                                //if (thinlinecounter > 1 && (spshsust.tabletopleft[0] != -1 && spshsust.tabletopleft[0] < i) && ws.Styles[value.stylenumber - 1].borders[0] > BorderStyle.Thin)
                                if (thinlinecounter > 0 && ws.Styles[value.stylenumber - 1].borders[0] > BorderStyle.Thin)
                                {
                                    if ((spshsust.tablebottomleft[0] == -1 || (spshsust.tablebottomleft[0] == i && spshsust.tablebottomleft[1] > j)))
                                    {
                                        spshsust.tablebottomleft[0] = i;
                                        spshsust.tablebottomleft[1] = j;
                                    }
                                    if ((spshsust.tablebottomright[0] == -1 || (spshsust.tablebottomright[0] == i && spshsust.tablebottomright[1] < j)))
                                    {
                                        spshsust.tablebottomright[0] = i;
                                        spshsust.tablebottomright[1] = j;
                                    }
                                }
                            }
                            
                        }
                    }
                    worksheetstructure.Add(spshsust);
                }
            }
        }

        public void tablestructurereadout()
        {
            if (worksheetstructure == null) return;
            PrintOption.Print("start method tablestructurereadout");
            foreach (var wsstruc in worksheetstructure)
            {
                foreach (var capter in wsstruc.capters)
                {
                    PrintOption.Print($"capter.Key: {capter.Key} name: {capter.Value.substructurename} position: {capter.Value.position[0]}, {capter.Value.position[1]}"); 
                }
                PrintOption.Print($"headerleft: {wsstruc.headerleft[0]}, {wsstruc.headerleft[1]}");
                PrintOption.Print($"headerright: {wsstruc.headerright[0]}, {wsstruc.headerright[1]}");
                PrintOption.Print($"tabletopleft: {wsstruc.tabletopleft[0]}, {wsstruc.tabletopleft[1]}");
                PrintOption.Print($"tabletopright: {wsstruc.tabletopright[0]}, {wsstruc.tabletopright[1]}");
                PrintOption.Print($"tablebottomleft: {wsstruc.tablebottomleft[0]}, {wsstruc.tablebottomleft[1]}");
                PrintOption.Print($"tablebottomright: {wsstruc.tablebottomright[0]}, {wsstruc.tablebottomright[1]}");
            }
        }

        public void writein(string? filePath)
        {
            if (filePath == null) return;
            PrintOption.Print("start method writein");

            IWorkbook workbook = new HSSFWorkbook();
            foreach(Costumworksheet ws in costumworksheets)
            {
                ISheet sheet = workbook.CreateSheet(ws.Name);

                SortedDictionary<int, ICellStyle> styles = new SortedDictionary<int, ICellStyle>();

                foreach (Costumstyle cws in ws.Styles)
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
                        NPOI.SS.UserModel.ICell cell = row.CreateCell(c);
                        TableCell? tc = null;
                        if (ws.Cells.TryGetValue($"{r},{c}", out var value))
                        {
                            PrintOption.Print($"Key'{r},{c}' found! The value is: {value}");
                            tc = value;
                            realmaxc = c;
                        }
                        else
                        {
                            PrintOption.Print($"Key'{r},{c}' does not exist.");
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
                            PrintOption.Print($"Key'{c}' found! The value is: {value2}");
                            if (measure.Length > value2)
                            {
                                widths[c] = measure.Length;
                            }
                        }
                        else
                        {
                            PrintOption.Print($"Key'{r},{c}' does not exist.");
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

            PrintOption.Print($"FileStream fs {filePath} FileMode {FileMode.Create} FileAccess {FileAccess.Write}");
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
            catch (FileNotFoundException)
            {
                PrintOption.Print("Error: The file does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                PrintOption.Print("Error: You do not have permission to access this file.");
            }
            catch (IOException ex)
            {
                PrintOption.Print($"Error: The file is locked by another process or a disk error occurred. Details: {ex.Message}");
            }

            PrintOption.Print("Successfully created the .xls file!");
        }
    }
}
=======
﻿using NPOI.HSSF.UserModel; // Required for legacy .xls files
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
>>>>>>> bb2a77754ddfb50b72dfe360a703998a3ea8112c
