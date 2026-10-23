<<<<<<< HEAD
﻿using NPOI.SS.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Costumstyle
    {
        internal int number { get; set; }
        // fillBackColor is FillForegroundColor
        internal short fillBackColor { get; set; }
        internal FillPattern fillpattern { get; set; }
        //iCellStyle.BorderBottom = cws.borders[0];
        //iCellStyle.BorderLeft = cws.borders[1];
        //iCellStyle.BorderRight = cws.borders[2];
        //iCellStyle.BorderTop = cws.borders[3];
        internal BorderStyle[] borders { get; set; }
        internal HorizontalAlignment alignment { get; set; }
        internal string fontname { get; set; }
        internal short fontcolor { get; set; }
        internal FontSuperScript typeOffset { get; set; }
        internal double height { get; set; }
        internal bool isBold { get; set; }
        internal bool isItalic { get; set; }
        internal FontUnderlineType underline { get; set; }
        internal bool isStrikeout { get; set; }

        public Costumstyle(int number, short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout) {
            this.number = number;
            this.fillBackColor = fillBackColor;
            this.fillpattern = fillpattern;
            this.borders = borders;
            this.alignment = alignment;
            this.fontname = fontname;
            this.fontcolor = fontcolor;
            this.typeOffset = typeOffset;
            this.height = height;
            this.isBold = isBold;
            this.isItalic = isItalic;
            this.underline = underline;
            this.isStrikeout = isStrikeout;
        }

        public bool IsSameStyle(short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout) {
            if (this.fillBackColor != fillBackColor) { return false; }
            if (this.fillpattern != fillpattern) { return false; }
            for (int i = 0; i < borders.Length; i++) {
                if (this.borders[i] != borders[i]) { return false; }
            }
            if (this.alignment != alignment) { return false; }
            if (!this.fontname.Equals(fontname)) { return false; }
            if (this.fontcolor != fontcolor) { return false; }
            if (this.typeOffset != typeOffset) { return false; }
            if (Math.Abs(this.height - height) > 1.0) { return false; }
            if (this.isBold != isBold) { return false; }
            if (this.isItalic != isItalic) { return false; }
            if (this.underline != underline) { return false; }
            if (this.isStrikeout != isStrikeout) { return false; }
            return true;
        }

    }
}
=======
﻿using NPOI.SS.UserModel;

namespace XLS_Export_C_Sharp
{
    internal class Costumstyle
    {
        internal int number { get; set; }
        // fillBackColor is FillForegroundColor
        internal short fillBackColor { get; set; }
        internal FillPattern fillpattern { get; set; }
        internal BorderStyle[] borders { get; set; }
        internal HorizontalAlignment alignment { get; set; }
        internal string fontname { get; set; }
        internal short fontcolor { get; set; }
        internal FontSuperScript typeOffset { get; set; }
        internal double height { get; set; }
        internal bool isBold { get; set; }
        internal bool isItalic { get; set; }
        internal FontUnderlineType underline { get; set; }
        internal bool isStrikeout { get; set; }

        public Costumstyle(int number, short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout) {
            this.number = number;
            this.fillBackColor = fillBackColor;
            this.fillpattern = fillpattern;
            this.borders = borders;
            this.alignment = alignment;
            this.fontname = fontname;
            this.fontcolor = fontcolor;
            this.typeOffset = typeOffset;
            this.height = height;
            this.isBold = isBold;
            this.isItalic = isItalic;
            this.underline = underline;
            this.isStrikeout = isStrikeout;
        }

        public bool IsSameStyle(short fillBackColor, FillPattern fillpattern, BorderStyle[] borders, HorizontalAlignment alignment, string fontname, short fontcolor,
             FontSuperScript typeOffset, double height, bool isBold, bool isItalic, FontUnderlineType underline, bool isStrikeout) {
            if (this.fillBackColor != fillBackColor) { return false; }
            if (this.fillpattern != fillpattern) { return false; }
            for (int i = 0; i < borders.Length; i++) {
                if (this.borders[i] != borders[i]) { return false; }
            }
            if (this.alignment != alignment) { return false; }
            if (!this.fontname.Equals(fontname)) { return false; }
            if (this.fontcolor != fontcolor) { return false; }
            if (this.typeOffset != typeOffset) { return false; }
            if (Math.Abs(this.height - height) > 1.0) { return false; }
            if (this.isBold != isBold) { return false; }
            if (this.isItalic != isItalic) { return false; }
            if (this.underline != underline) { return false; }
            if (this.isStrikeout != isStrikeout) { return false; }
            return true;
        }

    }
}
>>>>>>> bb2a77754ddfb50b72dfe360a703998a3ea8112c
