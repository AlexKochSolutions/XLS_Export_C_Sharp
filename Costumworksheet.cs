<<<<<<< HEAD
﻿using NPOI.SS.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Costumworksheet
    {
        internal string Name { get; set; }
        //internal List<TableCell> Cells { get; set; }
        // key value $"{cell.row},{cell.col}"
        internal SortedDictionary<string, TableCell> Cells { get; set; }
        internal List<Costumstyle> Styles { get; set; }
        internal List<Costumshape> Shapes { get; set; }
        internal int maxrow { get; set; }
        internal int maxcol { get; set; }

        public Costumworksheet(string name)
        {
            Name = name;
            //Cells = new List<TableCell>();
            Cells = new SortedDictionary<string, TableCell>();
            Styles = new List<Costumstyle>();
            Shapes = new List<Costumshape>();
            maxrow = 0;
            maxcol = 0;
        }

        public void AddCell(TableCell cell)
        {
            if (cell.row > maxrow)
            {
                maxrow = cell.row;
            }
            if (cell.col > maxcol)
            {
                maxcol = cell.col;
            }
            Cells.Add($"{cell.row},{cell.col}", cell);
        }

        public int GetStyleNumber(short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout)
        {
            int counter = 0;
            foreach (Costumstyle s in Styles) {
                if (s == null) {continue;}
                if (s.IsSameStyle(fillBackColor, fillpattern, borders, alignment, fontname, fontcolor,
                    typeOffset, height, isBold, isItalic, underline, isStrikeout))
                {
                    return s.number;
                }
                counter++;
            }
            int number = Styles.Count + 1;
            Costumstyle s2 = new Costumstyle(number, fillBackColor, fillpattern, borders, alignment, fontname, fontcolor,
                    typeOffset, height, isBold, isItalic, underline, isStrikeout);
            Styles.Add(s2);
            return number;
        }
    }
}
=======
﻿using NPOI.SS.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Costumworksheet
    {
        internal string Name { get; set; }
        //internal List<TableCell> Cells { get; set; }
        internal SortedDictionary<string, TableCell> Cells { get; set; }
        internal List<Costumstyle> Styles { get; set; }
        internal int maxrow { get; set; }
        internal int maxcol { get; set; }

        // Der Konstruktor stellt sicher, dass die Listen niemals 'null' sind
        public Costumworksheet(string name)
        {
            Name = name;
            //Cells = new List<TableCell>();
            Cells = new SortedDictionary<string, TableCell>();
            Styles = new List<Costumstyle>();
            maxrow = 0;
            maxcol = 0;
        }

        public void AddCell(TableCell cell)
        {
            if (cell.row > maxrow)
            {
                maxrow = cell.row;
            }
            if (cell.col > maxcol)
            {
                maxcol = cell.col;
            }
            Cells.Add($"{cell.row},{cell.col}", cell);
        }

        public int GetStyleNumber(short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout)
        {
            int counter = 0;
            foreach (Costumstyle s in Styles) {
                if (s == null) {continue;}
                if (s.IsSameStyle(fillBackColor, fillpattern, borders, alignment, fontname, fontcolor,
                    typeOffset, height, isBold, isItalic, underline, isStrikeout))
                {
                    return s.number;
                }
                counter++;
            }
            int number = Styles.Count + 1;
            Costumstyle s2 = new Costumstyle(number, fillBackColor, fillpattern, borders, alignment, fontname, fontcolor,
                    typeOffset, height, isBold, isItalic, underline, isStrikeout);
            Styles.Add(s2);
            return number;
        }
    }
}
>>>>>>> bb2a77754ddfb50b72dfe360a703998a3ea8112c
