using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    internal class ChessBoard
    {
        public ChessBoard(string field_name, int field_area, bool is_figure, bool is_white, Rectangle rectangle, string whichfigure, int column, int row)
        {
            this.field_name = field_name;
            this.field_area = field_area;
            this.is_figure = is_figure;
            this.is_white = is_white;
            this.rectangle = rectangle;
            this.whichfigure = whichfigure;
            this.column = column;
            this.row = row;
            
        }

    public int field_area { get; set; }
    public bool is_figure { get; set; }
    public string field_name { get; set; }
    public bool is_white { get; set; }
    public Rectangle rectangle { get; set; }
    public string whichfigure { get; set; }
    public int column { get; set; }
    public int row { get; set; }
}
}
