using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    internal class ChessPiece
    {
        public ChessPiece(int area, bool isWhite)
        {
            this.area = area;
            this.isWhite = isWhite;
        }

        public int area { get; set; }
        public bool isWhite { get; set; }
    }
}
