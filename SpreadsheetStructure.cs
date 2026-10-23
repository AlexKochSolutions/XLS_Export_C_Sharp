<<<<<<< HEAD
﻿
namespace XLS_Export_C_Sharp
{
    internal class SpreadsheetStructure
    {
        internal string tablename { get; set; }
        internal SortedDictionary<int, SpreadsheetSubstructure> capters { get; set; } = new SortedDictionary<int, SpreadsheetSubstructure> ();
        internal int[] headerleft { get; set; } = { -1, -1 };
        internal int[] headerright { get; set; } = { -1, -1 };
        internal int[] tabletopleft { get; set; } = { -1, -1 };
        internal int[] tabletopright { get; set; } = { -1, -1 };
        internal int[] tablebottomleft { get; set; } = { -1, -1 };
        internal int[] tablebottomright { get; set; } = { -1, -1 };

        public SpreadsheetStructure(string tablename)
        {
            this.tablename = tablename;
        }
    }
}
=======
﻿
namespace XLS_Export_C_Sharp
{
    internal class SpreadsheetStructure
    {
        internal string tablename { get; set; }
        internal SortedDictionary<int, SpreadsheetSubstructure> capters { get; set; } = new SortedDictionary<int, SpreadsheetSubstructure> ();
        internal int[] headerleft { get; set; } = { -1, -1 };
        internal int[] headerright { get; set; } = { -1, -1 };
        internal int[] tabletopleft { get; set; } = { -1, -1 };
        internal int[] tabletopright { get; set; } = { -1, -1 };
        internal int[] tablebottomleft { get; set; } = { -1, -1 };
        internal int[] tablebottomright { get; set; } = { -1, -1 };

        public SpreadsheetStructure(string tablename)
        {
            this.tablename = tablename;
        }
    }
}
>>>>>>> 1bd0500c0ee3e006632d2dec01c8deda2a29f099
