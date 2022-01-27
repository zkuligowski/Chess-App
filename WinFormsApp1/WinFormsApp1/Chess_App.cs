using Chess_App;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Chess_App : Form
    {
        Image<Bgr, byte> image1, image2, image3, image4, image5;
        Image<Gray, byte> mask;

        // segmentation counter
        int counter = 1;
        // area field
        int area = 0;

        //Lists with figures after segmentation process
        List<Image<Bgr, byte>> white_objects = new List<Image<Bgr, byte>>();
        List<Image<Bgr, byte>> black_objects = new List<Image<Bgr, byte>>();

        //List with checkerboard fields
        List<Image<Bgr, byte>> checkerboard = new List<Image<Bgr, byte>>();

        //List with chessnotation
        List<string> chess_notation_list = new List<string>();

        //ChessBoard Class list
        List<ChessBoard> chessboardnotation = new List<ChessBoard>();

        //ChessPiece Class list
        List<ChessPiece> chessPiece = new List<ChessPiece>();

        //Color used in segmentation process
        MCvScalar color = new(0, 0, 0);

        //Rectangle surrounding objects
        Rectangle rectangle = new();


        public Chess_App()
        {
            InitializeComponent();
            image1 = new Image<Bgr, byte>(pictureBox1.Size);
            image2 = image1.Clone();
            image3 = image1.Clone();
            image4 = image1.Clone();
            image5 = image1.Clone();
        }


        private void button_From_File_Click(object sender, EventArgs e)
        {
            Mat from_file;
            // from_file = CvInvoke.Imread(@"C:\Users\zbign\OneDrive - Politechnika Łódzka\V SEMESTR\Studia\V SEMESTR\Systemy wizyjne\Laboratorium\PROJEKT_MOJEGO_ZYCIA\a23.bmp");
            string path = textBox_Path.Text;
            from_file = CvInvoke.Imread(path);
            CvInvoke.Resize(from_file, from_file, new Size(pictureBox1.Width, pictureBox1.Height));
            image1 = from_file.ToImage<Bgr, byte>();
            pictureBox1.Image = image1.AsBitmap();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            image1.SetZero();
            image2.SetZero();
            image3.SetZero();
            image5.SetZero();
            pictureBox1.Image = image1.AsBitmap();
            pictureBox2.Image = image1.AsBitmap();
            pictureBox3.Image = image1.AsBitmap();

            white_objects.Clear();
            listBox_White_Chess_Notation.Items.Clear();

            black_objects.Clear();
            listBox_Black_Chess_Notation.Items.Clear();

            listBox1.Items.Clear();
            checkerboard.Clear();
            chessPiece.Clear();
            chess_notation_list.Clear();
            chessboardnotation.Clear();
            counter = 0;
            area = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (white_objects.Count <= listBox_White_Chess_Notation.SelectedIndex || listBox_White_Chess_Notation.SelectedIndex < 0) return;
            int index = listBox_White_Chess_Notation.SelectedIndex;

            image5 = white_objects[index];
            pictureBox3.Image = image5.AsBitmap();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (black_objects.Count <= listBox_Black_Chess_Notation.SelectedIndex || listBox_Black_Chess_Notation.SelectedIndex < 0) return;
            int index = listBox_Black_Chess_Notation.SelectedIndex;

            image5 = black_objects[index];
            pictureBox3.Image = image5.AsBitmap();
        }

        private void button_Analyze_Click(object sender, EventArgs e)
        {
            chess_notation_list = generate_chess_notation(chess_notation_list);
            full_segmentation();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            image4 = image1.Clone();
            pictureBox1.Image = image1.AsBitmap();
            MouseEventArgs? M = e as MouseEventArgs;

            foreach (ChessBoard element in chessboardnotation)
            {
                int rect_X = element.rectangle.X;
                int rect_Y = element.rectangle.Y;
                int rect_X_plus_width = element.rectangle.X + element.rectangle.Width;
                int rect_Y_plus_height = element.rectangle.Y + element.rectangle.Height;


                if (M.X > rect_X && M.X < rect_X_plus_width && M.Y > rect_Y && M.Y < rect_Y_plus_height)
                {
                    //PAWN
                    if (element.whichfigure == "Pawn")
                    {
                        draw_Pawn_Movement(element);
                    }

                    //ROOK   
                    else if (element.whichfigure == "Rook")
                    {
                        // Horizontal Right Movement // int move = 8
                        draw_Vertical_or_Horizontal_Movement(element, 8, "HORIZONTAL");

                        // Horizontal Left Movement // int move = -8
                        draw_Vertical_or_Horizontal_Movement(element, -8, "HORIZONTAL");

                        //Vertical up Movement // int move = -1
                        draw_Vertical_or_Horizontal_Movement(element, -1, "VERTICAL");

                        //Vertical Down Movement // int move = 1
                        draw_Vertical_or_Horizontal_Movement(element, 1, "VERTICAL");               
                    }

                    //BISHOP
                    else if (element.whichfigure == "Bishop")
                    {
                        //Diagonal Left Down Movement // int move = -7
                        draw_Diagonal_Movement(element, -7, "DOWN");

                        //Diagonal Right Down Movement // int move = 9
                        draw_Diagonal_Movement(element, 9, "DOWN");

                        //Diagonal Right Up Movement // int move = 7
                        draw_Diagonal_Movement(element, 7, "UP");

                        //Diagonal Left Up Movement // int move = -9
                        draw_Diagonal_Movement(element, -9, "UP");
                    }

                    //KNIGHT
                    else if (element.whichfigure == "Knight")
                    {
                        //Draw Move 2xUp -> 1xLeft
                        if (element.row < 7) draw_Knight_Movements(element, -10);

                        //Draw Move 2xUp -> 1xRight
                        if (element.row < 7) draw_Knight_Movements(element, 6);

                        //Draw Move 2xRight -> 1xUp
                        if (element.row < 8) draw_Knight_Movements(element, 15);

                        //Draw Move 2xRight -> 1xDown
                        if (element.row > 1) draw_Knight_Movements(element, 17);

                        //Draw Move 2xDown -> 1xLeft
                        if (element.row > 2) draw_Knight_Movements(element, -6);

                        //Draw Move 2xDown -> 1xRight
                        if (element.row > 2) draw_Knight_Movements(element, 10);

                        //Draw Move 2xLeft -> 1xUp
                        if (element.row < 8) draw_Knight_Movements(element, -17);

                        //Draw Move 2xLeft -> 1xDown
                        if (element.row > 1) draw_Knight_Movements(element, -15);

                    }

                    //QUEEN
                    else if (element.whichfigure == "Queen")
                    {
                        // Horizontal Right Movement // int move = 8
                        draw_Vertical_or_Horizontal_Movement(element, 8, "HORIZONTAL");

                        // Horizontal Left Movement // int move = -8
                        draw_Vertical_or_Horizontal_Movement(element, -8, "HORIZONTAL");

                        //Vertical up Movement // int move = -1
                        draw_Vertical_or_Horizontal_Movement(element, -1, "VERTICAL");

                        //Vertical Down Movement // int move = 1
                        draw_Vertical_or_Horizontal_Movement(element, 1, "VERTICAL");

                        //Diagonal Left Down Movement // int move = -7
                        draw_Diagonal_Movement(element, -7, "DOWN");

                        //Diagonal Right Down Movement // int move = 9
                        draw_Diagonal_Movement(element,  9, "DOWN");
                        
                        //Diagonal Right Up Movement // int move = 7
                        draw_Diagonal_Movement(element,  7, "UP");

                        //Diagonal Left Up Movement // int move = -9
                        draw_Diagonal_Movement(element, -9, "UP");

                    }

                    //King
                    else if (element.whichfigure == "King")
                    {
                        if (element.row < 8)
                        {
                            
                            // Up movement int move = -1
                            draw_King_Movements(element, -1);

                            // Diagonal Right Up movement int move = 7
                            draw_King_Movements(element, 7);

                            // Diagonal Left Up movement int move = -9
                            draw_King_Movements(element, -9);
                        }

                        if (element.row > 1)
                        {
                            // Down movement int move = 1
                            draw_King_Movements(element, 1);

                            // Diagonal Left Down movement int move = -7
                            draw_King_Movements(element, -7);

                            // Diagonal Right Down movement int move = 9
                            draw_King_Movements(element, 9);
                        }

                        // Left movement int move = -8
                        draw_King_Movements(element, -8);

                        // Right movement int move = 8
                        draw_King_Movements(element, 8);

                    }
                    pictureBox1.Image = image4.AsBitmap();
                }
            }
        }


        #region drawing methods
        private void draw_King_Movements(ChessBoard element, int move)
        {
            ChessBoard el;
            bool attack_on_white = false;
            int index = chessboardnotation.IndexOf(element);

            if (element.is_white == true)
            {
                attack_on_white = false;
            }
            else if (element.is_white == false)
            {
                attack_on_white = true;
            }
            for (int i = 1; i <= 1; i++)
            {
                index = index + move;
                if (index < 0 || index > 63) break;

                
                    el = chessboardnotation.ElementAt(index);
                    Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));

                    //if (el.column != element.column) break;
                    if (el.is_figure == true && el.is_white == !attack_on_white) break;

                    else if (el.is_figure == true && el.is_white == false)
                    {
                        CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                    }
                    else if (el.is_figure == false)
                    {
                        CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                    }
            }
        }
        
        private void draw_Knight_Movements(ChessBoard element, int move)
        {
            ChessBoard el;
            int row = element.row;
            int column = element.column;
            int index = chessboardnotation.IndexOf(element);
            bool attack_on_white = false;

            if (element.is_white == true)
            {
                attack_on_white = false;
            }
            else if (element.is_white == false)
            {
                attack_on_white = true;
            }

            for (int i = 1; i <= 1; i++)
            {
                index = index + move;
                if (index >= 0 && index <= 63)
                {
                    el = chessboardnotation.ElementAt(index);
                    Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));

                    if (el.is_figure == true && el.is_white == !attack_on_white) break;
                    else if (el.is_figure == true && el.is_white == attack_on_white)
                    {
                        CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                        break;
                    }
                    else if (el.is_figure == false)
                    {
                        CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                    }
                }
            }
        }

        private void draw_Diagonal_Movement(ChessBoard element, int move, string up_or_down_diagonal)
        {
            ChessBoard el;
            int column = element.column;
            int index = chessboardnotation.IndexOf(element);
            bool attack_on_white = false;

                if (element.is_white == true)
            {
                attack_on_white = false;
            }
            else if (element.is_white == false)
            {
                attack_on_white = true;
            }

            for (int i = 1; i <= 8; i++)
            {
                index = index + move;
                if (index < 0 || index > 63) break;

                if (element.row == 8 && up_or_down_diagonal == "UP")
                {
                    break;
                }
                else if (element.row == 1 && up_or_down_diagonal == "DOWN")
                {
                    break;
                }

                el = chessboardnotation.ElementAt(index);
                Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                if (el.is_figure == true && el.is_white == !attack_on_white) break;

                else if (el.is_figure == true && el.is_white == attack_on_white)
                {
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                    break;
                }
                else if (el.is_figure == false)
                {
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                }
                if (el.row == 1 || el.column == 1 || el.row == 8 || el.column == 8) break;

            }
        }

        private void draw_Vertical_or_Horizontal_Movement(ChessBoard element, int move, string vertical_or_horizontal)
        {
            ChessBoard el;
            int column = element.column;
            int index = chessboardnotation.IndexOf(element);
            bool attack_on_white = false;

            if (element.is_white == true)
            {
                attack_on_white = false;
            }
            else if (element.is_white == false)
            {
                attack_on_white = true;
            }

            for (int i = column; i <= 8; i++)
            {
                index = index + move;
                if (index > 63 || index < 0) break;
                el = chessboardnotation.ElementAt(index);
                Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));

                if (element.column != el.column && vertical_or_horizontal == "VERTICAL") break;
                if (el.is_figure == true && el.is_white == !attack_on_white) break;

                else if (el.is_figure == true && el.is_white == attack_on_white)
                {
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                    break;
                }
                else if (el.is_figure == false)
                {
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                }
            }
        }

        private void draw_Pawn_Movement(ChessBoard element)
        {
            ChessBoard el;
            int row = element.row;

            int leftattack, rightattack, forward1, forward2, double_jump_row, cant_attack_on_column_left, cant_attack_on_column_right;
            leftattack = rightattack = forward1 = forward2 = double_jump_row = cant_attack_on_column_left = cant_attack_on_column_right = 0;
            bool attack_on_white = false;

            if (element.is_white == true)
            {
                leftattack = -9;
                rightattack = 7;
                forward1 = -1;
                forward2 = -2;
                attack_on_white = false;
                double_jump_row = 2;
                cant_attack_on_column_left = 1;
                cant_attack_on_column_right = 8;
            }
            else if (element.is_white == false)
            {
                leftattack = 9;
                rightattack = -7;
                forward1 = 1;
                forward2 = 2;
                attack_on_white = true;
                double_jump_row = 7;
                cant_attack_on_column_left = 8;
                cant_attack_on_column_right = 1;
            }

            // Forward movement
            el = chessboardnotation.ElementAt(chessboardnotation.IndexOf(element) + forward1);
            if (el.is_figure == false)
            {
                Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);

                if (row == double_jump_row)
                {
                    el = chessboardnotation.ElementAt(chessboardnotation.IndexOf(element) + forward2);
                    if (el.is_figure == false)
                    {
                        p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                        CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                    }
                }
                else
                {
                    el = chessboardnotation.ElementAt(chessboardnotation.IndexOf(element) + forward1);
                    p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(0, 0, 255), 5);
                }
            }
            // Left attack
            if (element.column != cant_attack_on_column_left)
            {
                el = chessboardnotation.ElementAt(chessboardnotation.IndexOf(element) + leftattack);

                if (el.is_figure == true && el.is_white == attack_on_white)
                {
                    Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                }
            }

            //Right attack
            if (element.column != cant_attack_on_column_right)
            {
                el = chessboardnotation.ElementAt(chessboardnotation.IndexOf(element) + rightattack);

                if (el.is_figure == true && el.is_white == attack_on_white)
                {
                    Point p = new Point((el.rectangle.X + el.rectangle.Width / 2), (el.rectangle.Y + el.rectangle.Height / 2));
                    CvInvoke.Circle(image4, p, 20, new MCvScalar(255, 0, 0), 5);
                }
            }
        }
#endregion
        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        #region Segmentation Methods

        private void full_segmentation()
        {
            int xa, ya;
            int F = 0;
            int temp_counter = 0;
            int cols = 1;
            int rows = 8;

            Image<Bgr, byte> blackobject = new Image<Bgr, byte>(image1.Size);
            Image<Bgr, byte> blackfield = new Image<Bgr, byte>(image1.Size);
            Image<Bgr, byte> whiteobject = new Image<Bgr, byte>(image1.Size);
            Image<Bgr, byte> whitefield = new Image<Bgr, byte>(image1.Size);

            //Creating a temp copy of the source image
            Image<Bgr, byte> temp_image = new Image<Bgr, byte>(image1.Size);
            image1.CopyTo(temp_image);

            //Clear Lists with objects
            black_objects.Clear();
            black_objects.Clear();
            checkerboard.Clear();
            chessboardnotation.Clear();

            for (xa = 1; xa < image1.Width; xa++)
            {
                for (ya = 1; ya < image1.Height; ya++)
                {
                    // SEGMENTATION BLACK FIGURES
                    if (temp_image.Data[ya, xa, 0] == 50 && temp_image.Data[ya, xa, 1] == 50 && temp_image.Data[ya, xa, 2] == 50)
                    {
                        blackobject = part_segmentation(new Point(xa, ya));
                        CvInvoke.Subtract(temp_image, blackobject, temp_image);

                        byte[,,] temp = blackobject.Data;

                        F = area;
                        if (F > 50)
                        {
                            string figure = "";
                            bool king = false;      //1
                            bool queen = false;     //2
                            bool knight = false;    //3
                            bool bishop = false;    //4
                            bool rook = false;      //5
                            bool pawn = false;      //6

                            king = check_king(temp);
                            queen = check_queen(temp);
                            knight = check_knight(temp);
                            bishop = check_bishop(temp);
                            rook = check_rook(temp);
                            pawn = check_pawn(temp);

                            if (king == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black King " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "King";
                            }
                            else if (queen == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Queen " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Queen";
                            }
                            else if (knight == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Knight " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Knight";
                            }
                            else if (bishop == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Bishop " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Bishop";
                            }
                            else if (rook == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Rook " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Rook";
                            }
                            else if (pawn == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Pawn " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Pawn";
                            }
                            else
                                listBox_Black_Chess_Notation.Items.Add("Black NON" + "    (" + counter.ToString() + ")   F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + rectangle.Location);


                            ChessPiece piece = new ChessPiece(F, false, figure, rectangle.X, rectangle.Y);
                            chessPiece.Add(piece);
                            black_objects.Add(blackobject);
                        }
                        counter++;
                    }

                    // SEGMENTATION WHITE FIGURES   
                    else if (temp_image.Data[ya, xa, 0] == 255 && temp_image.Data[ya, xa, 1] == 255 && temp_image.Data[ya, xa, 2] == 255)
                    {
                        whiteobject = part_segmentation(new Point(xa, ya));
                        CvInvoke.Subtract(temp_image, whiteobject, temp_image);

                        byte[,,] temp = whiteobject.Data;
                        F = area;

                        if (F > 50)
                        {
                            string figure = "";
                            bool king = false;      //1
                            bool queen = false;     //2
                            bool knight = false;    //3
                            bool bishop = false;    //4
                            bool rook = false;      //5
                            bool pawn = false;      //6

                            king = check_king(temp);
                            queen = check_queen(temp);
                            knight = check_knight(temp);
                            bishop = check_bishop(temp);
                            rook = check_rook(temp);
                            pawn = check_pawn(temp);

                            if (king == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White King " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "King";
                            }
                            else if (queen == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Queen " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Queen";
                            }
                            else if (knight == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Knight " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Knight";
                            }
                            else if (bishop == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Bishop " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Bishop";
                            }
                            else if (rook == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Rook " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Rook";
                            }
                            else if (pawn == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Pawn " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                figure = "Pawn";
                            }
                            else listBox_White_Chess_Notation.Items.Add("White NON" + "    (" + counter.ToString() + ")   F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + rectangle.Location);


                            ChessPiece piece2 = new ChessPiece(F, true, figure, rectangle.X, rectangle.Y);
                            chessPiece.Add(piece2);
                            white_objects.Add(whiteobject);
                        }
                        counter++;
                    }
                }
            }

            for (xa = 1; xa < image1.Width; xa = xa + 40)
            {
                for (ya = 1; ya < image1.Height; ya++)
                {
                    // SEGMENTATION WHITE FIELDS
                    if (temp_image.Data[ya, xa, 0] > 200 && temp_image.Data[ya, xa, 0] < 220 && temp_image.Data[ya, xa, 1] > 230 && temp_image.Data[ya, xa, 1] < 245 && temp_image.Data[ya, xa, 2] > 230 && temp_image.Data[ya, xa, 2] < 245)
                    {
                        whitefield = part_segmentation(new Point(xa, ya));
                        CvInvoke.Subtract(temp_image, whitefield, temp_image);


                        byte[,,] temp = whitefield.Data;
                        F = area;
                        bool isfig = false;

                        decimal scale = 4000 / 78;
                        decimal F_decimal = F;
                        decimal rect_widht_decimnal = rectangle.Width;
                        decimal a = F_decimal / rect_widht_decimnal;

                        if (F > 150)
                        {
                            if (a < scale)
                            {
                                isfig = true;
                            }
                            else
                            {
                                isfig = false;
                            }

                            ChessBoard board = new(chess_notation_list.ElementAt(temp_counter), F, isfig, true, rectangle, "", cols, rows);
                            chessboardnotation.Add(board);
                            temp_counter++;

                            listBox1.Items.Add(board.field_name + "   F =" + F.ToString() + " " + rectangle.Location + " " + rectangle.Width);
                            checkerboard.Add(whitefield);

                            rows--;
                            if (rows < 1)
                            {
                                cols++;
                                rows = 8;
                            }
                        }
                        counter++;
                    }

                    // SEGMENTAION BLACK FIELDS
                    else if (temp_image.Data[ya, xa, 0] == 86 && (temp_image.Data[ya, xa, 1] == 150 || temp_image.Data[ya, xa, 1] == 0) && temp_image.Data[ya, xa, 2] == 118)
                    {
                        blackfield = part_segmentation(new Point(xa, ya));
                        CvInvoke.Subtract(temp_image, blackfield, temp_image);

                        byte[,,] temp = blackfield.Data;
                        F = area;
                        bool isfig = false;

                        decimal scale = 4000 / 78;
                        decimal F_decimal = F;
                        decimal rect_widht_decimnal = rectangle.Width;
                        decimal a = F_decimal / rect_widht_decimnal;

                        if (F > 150)
                        {
                            if (a < scale)
                            {
                                isfig = true;
                            }
                            else
                            {
                                isfig = false;
                            }

                            ChessBoard board = new(chess_notation_list.ElementAt(temp_counter), F, isfig, false, rectangle, "", cols, rows);
                            chessboardnotation.Add(board);
                            temp_counter++;

                            listBox1.Items.Add(board.field_name + "   F =" + F.ToString() + " " + rectangle.Location + " " + rectangle.Width);
                            checkerboard.Add(blackfield);

                            rows--;
                            if (rows < 1)
                            {
                                cols++;
                                rows = 8;
                            }
                        }
                        counter++;
                    }
                }
                if (counter > 200) break;
            }

            foreach (ChessBoard element in chessboardnotation)
            {

                int rect_X = element.rectangle.X;
                int rect_Y = element.rectangle.Y;
                int rect_X_plus_width = element.rectangle.X + element.rectangle.Width;
                int rect_Y_plus_height = element.rectangle.Y + element.rectangle.Height;


                foreach (ChessPiece cp in chessPiece)
                {
                    if (cp.piecepoint_X > rect_X && cp.piecepoint_X < rect_X_plus_width && cp.piecepoint_Y > rect_Y && cp.piecepoint_Y < rect_Y_plus_height)
                    {
                        element.whichfigure = cp.Figure;
                        element.is_white = cp.isWhite;
                    }
                }
            }
        }




        #region FigureDetection
        private bool check_pawn(byte[,,] temp)
        {
            // Pawn Detection
            decimal scale_Y1, scale_Y2, scale_X1, scale_X2, condition_Y1, condition_Y2, condition_X1, condition_X2;
            scale_Y1 = scale_X1 = scale_X2 = condition_Y1 = condition_Y2 = condition_X1 = condition_X2 = 0;
            decimal c = 22;
            decimal d = 6;
            decimal e = 38;

            scale_Y1 = 2;
            scale_X1 = 2;
            scale_Y2 = c / e;
            scale_X2 = d / e;

            condition_Y1 = rectangle.Bottom - scale_Y1;
            condition_X1 = rectangle.X + scale_X1;
            condition_Y2 = rectangle.Y + (scale_Y2 * rectangle.Height);
            condition_X2 = rectangle.X + (scale_X2 * rectangle.Width);


            if (temp[(int)condition_Y1, (int)condition_X1, 0] == 120 && temp[(int)condition_Y2, (int)condition_X2, 0] == 0)
            {
                return true;
            }

            else return false;
        }

        private bool check_rook(byte[,,] temp)
        {
            // Rook Detection
            decimal scale_Y1, scale_Y2, scale_X1, scale_X2, condition_Y1, condition_Y2, condition_X1, condition_X2;
            scale_Y1 = scale_X1 = scale_X2 = condition_Y1 = condition_Y2 = condition_X1 = condition_X2 = 0;
            decimal c = 5;
            decimal d = 7;
            decimal e = 44;



            scale_Y1 = 2;
            scale_X1 = 2;
            scale_Y2 = c / e;
            scale_X2 = d / e;

            condition_Y1 = rectangle.Bottom - scale_Y1;
            condition_X1 = rectangle.X + scale_X1;
            condition_Y2 = rectangle.Y + (scale_Y2 * rectangle.Height);
            condition_X2 = rectangle.X + (scale_X2 * rectangle.Width);


            if (temp[(int)condition_Y1, (int)condition_X1, 0] == 120 && temp[(int)condition_Y2, (int)condition_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool check_bishop(byte[,,] temp)
        {
            // Bishop Detection
            decimal scale_Y1, scale_Y2, scale_X1, scale_X2, condition_Y1, condition_Y2, condition_X1, condition_X2;
            scale_Y1 = scale_X1 = scale_X2 = condition_Y1 = condition_Y2 = condition_X1 = condition_X2 =  0;
            decimal a = 1;
            decimal b = 2;
            decimal c = 7;
            decimal d = 45;

            scale_Y1 = 2;
            scale_X1 = 2;
            scale_Y2 = a / b;
            scale_X2 = c / d;

            condition_Y1 = rectangle.Bottom - scale_Y1;
            condition_X1 = rectangle.X + scale_X1;
            condition_Y2 = rectangle.Y + (scale_Y2 * rectangle.Height);
            condition_X2 = rectangle.X + (scale_X2 * rectangle.Width);

            if (temp[(int)condition_Y1, (int)condition_X1, 0] == 120 && temp[(int)condition_Y2, (int)condition_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool check_knight(byte[,,] temp)
        {
            // Knight Detection
            decimal scale_Y, scale_X1, scale_X2, condition_Y, condition_X1, condition_X2;
            scale_Y = scale_X1 = scale_X2 = condition_Y = condition_X1 = condition_X2 = 0;

            scale_Y = 2;
            scale_X1 = 2;
            condition_Y = rectangle.Bottom - scale_Y;
            condition_X1 = rectangle.X + scale_X1;
            condition_X2 = rectangle.Right - scale_X1;

            if (temp[(int)condition_Y, (int)condition_X1, 0] == 0 && temp[(int)condition_Y, (int)condition_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool check_queen(byte[,,] temp)
        {
            // Queen Detection
            decimal scale_Y, scale_X1, scale_X2, condition_Y, condition_X1, condition_X2;
            scale_Y = scale_X1 = scale_X2 = condition_Y = condition_X1 = condition_X2 = 0;
            decimal b = 13;
            decimal c = 47;

            scale_Y = b / c;
            scale_X1 = 2;
            condition_Y = rectangle.Y + (rectangle.Height * scale_Y);
            condition_X1 = rectangle.X + scale_X1;
            condition_X2 = rectangle.Right - scale_X1;


            if (temp[(int)condition_Y, (int)condition_X1, 0] == 120 && temp[(int)condition_Y, (int)condition_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool check_king(byte[,,] temp)
        {
            // Rozpoznanie krola
            decimal scale_Y, scale_X1, scale_X2, condition_Y, condition_X1, condition_X2, warunek_polozenia_X3;
            scale_Y = scale_X1 = scale_X2 = condition_Y = condition_X1 = condition_X2 = warunek_polozenia_X3 = 0;
            decimal a = 25;
            decimal b = 48;
            decimal c = 16;
            decimal d = 39;
            decimal e = 57;


            scale_Y = a / b;
            scale_X1 = c / e;
            scale_X2 = d / e;
            condition_Y = rectangle.Y + (rectangle.Height * scale_Y);
            condition_X1 = rectangle.X + (rectangle.Width * scale_X1);
            condition_X2 = rectangle.X + (rectangle.Width * scale_X2);
            warunek_polozenia_X3 = rectangle.X + 2;


            if (temp[(int)condition_Y, (int)condition_X1, 0] == 0 && temp[(int)condition_Y, (int)condition_X2, 0] == 0 && temp[(int)condition_Y, (int)warunek_polozenia_X3, 0] == 120)
            {
                return true;
            }

            else return false;
        }
        #endregion




        private void button_Clear_White_Chess_Notation_Click(object sender, EventArgs e)
        {
            white_objects.Clear();
            listBox_White_Chess_Notation.Items.Clear();
        }


        private void button_Clear_Black_Chess_Notation_Click(object sender, EventArgs e)
        {
            black_objects.Clear();
            listBox_Black_Chess_Notation.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (ChessBoard c in chessboardnotation)
            {
                if (c.is_figure)
                {
                    if (c.is_white == true)
                    {
                        listBox1.Items.Add("White " + c.whichfigure + " " + c.field_name.ToString());
                    }
                    else if (c.is_white == false)
                    {
                        listBox1.Items.Add("Black " + c.whichfigure + " " + c.field_name.ToString());
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox_Path.Text = get_image_path();
        }

        private string get_image_path()
        {
            string ret = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Obrazy|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog1.Title = "Wybierz obrazek.";
            //Jeśli wszystko przebiegło ok to pobiera nazwę pliku
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ret = openFileDialog1.FileName;
            }

            return ret;
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (checkerboard.Count <= listBox1.SelectedIndex || listBox1.SelectedIndex < 0) return;

            int index = listBox1.SelectedIndex;

            pictureBox3.Image = checkerboard[index].AsBitmap();
        }

        private int calculate_area(byte[,,] temp)
        {
            int F = 0;

            for (int X = 0; X < image1.Width; X++)
            {
                for (int Y = 0; Y < image1.Height; Y++)
                {
                    if (temp[Y, X, 0] == 120 && temp[Y, X, 1] == 150 && temp[Y, X, 2] == 100)
                    {
                        F = F + 1;
                    }
                }
            }
            return F;
        }
      
        private List<string> generate_chess_notation(List<string> L)
        {
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 8; j >= 1; j--)
                {
                    if (i == 1) L.Add("A" + j.ToString());
                    else if (i == 2) L.Add("B" + j.ToString());
                    else if (i == 3) L.Add("C" + j.ToString());
                    else if (i == 4) L.Add("D" + j.ToString());
                    else if (i == 5) L.Add("E" + j.ToString());
                    else if (i == 6) L.Add("F" + j.ToString());
                    else if (i == 7) L.Add("G" + j.ToString());
                    else if (i == 8) L.Add("H" + j.ToString());
                }
            }
            return L;
        }

        private Image<Bgr, byte> part_segmentation(Point ognisko)
        {
            // procedura zwraca obraz z pojedynczym obrazem
            Image<Bgr, byte> tempObraz = new Image<Bgr, byte>(image1.Size);
            image1.CopyTo(tempObraz);

            //Maska operacyjna - wymagana przez funkcję FloodFill

            mask = new Image<Gray, byte>(image2.Width + 2, image2.Height + 2, new Gray(0));
            mask.SetZero();

            //prostokąt opisujący wysegmentowany obiekt

            Rectangle rect = new Rectangle();
            color.V0 = 120;
            color.V1 = 150;
            color.V2 = 100;

            //Funkcja FloodFill wypełnia kolorem (parametr 'kolor') obiekt na obrazie 'tempObraz'
            //Punkt startowy segmentacji (należący do obiektu) określa parametr 'ognisko'
            //Dwa ostatnie argumenty funkcji to odpowiednio maksymalna różnica koloru sąsiadujących pikseli obiektu w dół i w górę

            area = CvInvoke.FloodFill(tempObraz, mask, ognisko, color, out rect, new MCvScalar(2, 2, 2), new MCvScalar(2, 2, 2),
                Emgu.CV.CvEnum.Connectivity.FourConnected, Emgu.CV.CvEnum.FloodFillType.FixedRange);

            // Connectivity.EightConnected

            //tempObraz zawiera zmodyfikowany obraz wejściowy ze wszystkimi obiektami, konieczne jest odfiltowanie dodatkowych obiektów

            tempObraz = tempObraz.ThresholdToZero(new Bgr(color.V0 - 1, color.V1 - 1, color.V2 - 1));
            tempObraz = tempObraz.ThresholdToZeroInv(new Bgr(color.V0, color.V1, color.V2));

            //Narysowanie prostokąta otaczającego wysegmentowany obiekt
            CvInvoke.Rectangle(tempObraz, rect, new MCvScalar(0, 0, 255), 1);
            rectangle = rect;
            // tempObraz   na zakończenie progowania powinien zawierać obraz z pojedynczym obiektem

            CvInvoke.Resize(mask, mask, new Size(pictureBox3.Width, pictureBox3.Height));
            return tempObraz;

        }
        #endregion


    }
}
