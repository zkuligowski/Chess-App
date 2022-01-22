using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    internal class ChessPiece
    {
        public ChessPiece(int area, bool isWhite, string Figure, int piecepoint_X, int piecepoint_Y)
        {
            this.area = area;
            this.isWhite = isWhite;
            this.Figure = Figure;
            this.piecepoint_X = piecepoint_X;
            this.piecepoint_Y = piecepoint_Y;
        }

        public int area { get; set; }
        public bool isWhite { get; set; }
        public string Figure { get; set; }
        public int piecepoint_X { get; set; }
        public int piecepoint_Y { get; set; }
    }
}
