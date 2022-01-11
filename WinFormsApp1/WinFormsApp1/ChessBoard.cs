using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    internal class ChessBoard
    {
        public ChessBoard(string field_name, int field_area, bool is_figure, bool is_white)
        {
            this.field_name = field_name;
            this.field_area = field_area;
            this.is_figure = is_figure;
            this.is_white = is_white;
        }

    public int field_area { get; set; }
    public bool is_figure { get; set; }
    public string field_name { get; set; }
    public bool is_white { get; set; }
}
}
