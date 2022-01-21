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
        Image<Bgr, byte> obraz1, obraz2, obraz3;
        Image<Bgr, byte> tab_obraz1;
        Image<Bgr, byte> obraz_binarny;
        Image<Gray, byte> maska;

        private Mat obrazMAT = new Mat();

        // liczniki segmentacji

        int licznik = 1;
        int licznik_duzych = 1;
        int pole = 0;

        //Struktura przechowująca momenty figury
        Moments momenty = new Moments();

        //Lista obrazów z poszczególnymi obiektami po segmentacji
        List<Image<Bgr, byte>> obiekty_biale = new List<Image<Bgr, byte>>();
        List<Image<Bgr, byte>> obiekty_czarne = new List<Image<Bgr, byte>>();

        //Lista z polami szachownicy
        List<Image<Bgr, byte>> checkerboard = new List<Image<Bgr, byte>>();
        List<string> chess_notation_list = new List<string>();

        //Lista zawierajaca notacje szachowa
        List<ChessBoard> chessboardnotation = new List<ChessBoard>();

        // Lista zawierająca figury
        List<ChessPiece> chessPiece = new List<ChessPiece>();

        //Początkowy kolor w procedurze segmentacji
        MCvScalar kolor = new MCvScalar(0, 0, 0);

        Rectangle rectangle = new Rectangle();


        public Chess_App()
        {
            InitializeComponent();
            //inicjalizacja obrazów
            obraz1 = new Image<Bgr, byte>(pictureBox1.Size);
            obraz2 = obraz1.Clone();
            obraz3 = obraz1.Clone();

            obraz_binarny = obraz1.Clone();
        }


        private void button_From_File_Click(object sender, EventArgs e)
        {
            Mat zPliku;
            zPliku = CvInvoke.Imread(@"C:\Users\zbign\OneDrive - Politechnika Łódzka\V SEMESTR\Studia\V SEMESTR\Systemy wizyjne\Laboratorium\PROJEKT_MOJEGO_ZYCIA\a2.bmp");
            CvInvoke.Resize(zPliku, zPliku, new Size(pictureBox1.Width, pictureBox1.Height));
            obraz1 = zPliku.ToImage<Bgr, byte>();
            pictureBox1.Image = obraz1.AsBitmap();
            pictureBox1.Image = obraz1.AsBitmap();
            pictureBox1.Image = obraz1.AsBitmap();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            obraz1.SetZero();
            obraz2.SetZero();
            obraz3.SetZero();
            obraz_binarny.SetZero();
            pictureBox1.Image = obraz1.AsBitmap();
            pictureBox2.Image = obraz1.AsBitmap();
            pictureBox3.Image = obraz1.AsBitmap();

            obiekty_biale.Clear();
            listBox_White_Chess_Notation.Items.Clear();

            obiekty_czarne.Clear();
            listBox_Black_Chess_Notation.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Zabezpieczenie przed wybraniem obiektu, którego nie ma na liście
            if (obiekty_biale.Count <= listBox_White_Chess_Notation.SelectedIndex || listBox_White_Chess_Notation.SelectedIndex < 0) return;
            //if (pola_biale.Count <= listBox_White_Chess_Notation.SelectedIndex || listBox_White_Chess_Notation.SelectedIndex < 0) return;

            int index = listBox_White_Chess_Notation.SelectedIndex;

            //obraz_binarny = pola_biale[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
            obraz_binarny = obiekty_biale[index];
            //.ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
            //obraz_binarny.CopyTo(obraz3);
            pictureBox3.Image = obraz_binarny.AsBitmap();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Zabezpieczenie przed wybraniem obiektu, którego nie ma na liście
            //if (pola_czarne.Count <= listBox_Black_Chess_Notation.SelectedIndex || listBox_Black_Chess_Notation.SelectedIndex < 0) return;
            //if (obiekty_czarne.Count <= listBox_Black_Chess_Notation.SelectedIndex || listBox_Black_Chess_Notation.SelectedIndex < 0) return;

            int index = listBox_Black_Chess_Notation.SelectedIndex;

            obraz_binarny = obiekty_czarne[index];
            //.ThresholdBinary(new Bgr(0, 150,0), new Bgr(255, 255, 255));
            obraz_binarny.CopyTo(obraz3);
            pictureBox3.Image = obraz3.AsBitmap();
        }

        private void button_Analyze_Click(object sender, EventArgs e)
        {

            chess_notation_list = generate_chess_notation(chess_notation_list);
            pelna_segmentacja();
          
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs? M = e as MouseEventArgs;
            
            // Pobranie koloru w miejscu klikniecia
             Bitmap b = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(b, pictureBox1.ClientRectangle);
#pragma warning disable CS8602 // Wyłuskanie odwołania, które może mieć wartość null.
            Color kolorKlikniecia = b.GetPixel(M.X, M.Y);
#pragma warning restore CS8602 // Wyłuskanie odwołania, które może mieć wartość null.
            b.Dispose();
            listBox_Black_Chess_Notation.Items.Add(//"Kolor: " + kolorKlikniecia.ToString() + " " +
                M.Location.ToString());
             pictureBox1.Image = obraz1.AsBitmap();
            //pictureBox1.Image = obraz1.AsBitmap();

            Image<Bgr, byte> obiekt = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(obiekt);
            int pole = 1;
            obiekt = segmentacja_1_obiektu(new Point(M.X, M.Y));
            Image<Gray, byte> obrazSzary = obiekt.Convert<Gray, byte>();
            //pictureBox1.Image = obraz1.AsBitmap();
            obiekty_biale.Add(obiekt);
            CvInvoke.Subtract(obraz1, obiekt, obraz1);
            
            obraz2 = obraz2.Add(obiekt);
           // obiekt = obiekt.ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));

           // Image<Gray, byte> obrazSzary = obiekt.Convert<Gray, byte>();
            Moments momenty2 = new Moments();
            momenty2 = CvInvoke.Moments(obrazSzary, true);
            pole = Convert.ToInt32(momenty2.M00);

            listBox_White_Chess_Notation.Items.Add("obiekt" + licznik.ToString() + "  " + pole);

            pictureBox1.Image = obraz1.AsBitmap();
            pictureBox2.Image = obraz2.AsBitmap();
          //  pictureBox3.Image = obiekt.AsBitmap();

            licznik++;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MouseEventArgs? me = e as MouseEventArgs;
            byte r, g, b;

            tab_obraz1 = obraz2.Clone();

            b = tab_obraz1.Data[me.Y, me.X, 0];
            g = tab_obraz1.Data[me.Y, me.X, 1];
            r = tab_obraz1.Data[me.Y, me.X, 2];


            Bgr kolor_bgr;
            long kolor_long;



            kolor_bgr = tab_obraz1[me.Y, me.X];
            kolor_long = (long)(tab_obraz1.Data[me.Y, me.X, 0]) + 0x100 * (long)(tab_obraz1.Data[me.Y, me.X, 1]) + 0x10000 * (long)(tab_obraz1.Data[me.Y, me.X, 2]); //0 ch-blue

            b = (byte)(kolor_long & 0xff);
            g = (byte)((kolor_long & 0xff00) / 0x100);
            r = (byte)((kolor_long & 0xff0000) / 0x10000);


            //  label32.Text = kolor_long.ToString("X6");
        }

        #region Metody Segmentacja

        private void pelna_segmentacja()
        {
            int xa, ya;
            int F = 0;
            int licznik_pom = 0;
            Figura f;
            // pobranie progu dla dużych obiektów
                
            Image<Bgr, byte> obiektczarny = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> czarnepole = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> obiektbialy = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> bialepole = new Image<Bgr, byte>(obraz1.Size);

            

            ChessPiece piece = new ChessPiece(2, true);

            MCvScalar kolor = new MCvScalar(10, 0, 0);


            //Stworzenie tymczasowej kopii obrazu zrodlowego
            Image<Bgr, byte> tempObraz2 = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz2);

              //Wyczyszczenie listy obiektbialyów obrazowych (ich obrazów)
              obiekty_czarne.Clear();
              obiekty_czarne.Clear();
              checkerboard.Clear();
              chessboardnotation.Clear();

            for (xa = 1; xa < obraz1.Width; xa++)
            {
                for (ya = 1; ya < obraz1.Height; ya++)
                {

                    // CZARNE FIGURY
                    if (tempObraz2.Data[ya, xa, 0] == 50 && tempObraz2.Data[ya, xa, 1] == 50 && tempObraz2.Data[ya, xa, 2] == 50)
                    {
                        obiektczarny = segmentacja_1_obiektu(new Point(xa, ya));
                        CvInvoke.Subtract(tempObraz2, obiektczarny, tempObraz2);

                        byte[,,] temp = obiektczarny.Data;

                        F = pole;
                        if (F > 400)
                        {
                            bool krol = false;
                            bool hetman = false;
                            bool skoczek = false;
                            bool goniec = false;
                            bool wieza = false;
                            bool pionek = false;

                            krol = sprawdz_czy_krol(temp);
                            hetman = sprawdz_czy_hetman(temp);
                            skoczek = sprawdz_czy_skoczek(temp);
                            goniec = sprawdz_czy_goniec(temp);
                            wieza = sprawdz_czy_wieza(temp);
                            pionek = sprawdz_czy_pionek(temp);

                            if (krol == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black King " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.King;
                            }
                            else if (hetman == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Queen " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.Queen;
                            }
                            else if (skoczek == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Knight " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.Knight;
                            }
                            else if (goniec == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Bishop " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.Bishop;
                            }
                            else if (wieza == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Rook " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.Rook;
                            }
                            else if (pionek == true)
                            {
                                listBox_Black_Chess_Notation.Items.Add("Black Pawn " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                                f = Figura.Pawn;
                            }
                            else
                                listBox_Black_Chess_Notation.Items.Add("Black NON" + licznik_duzych.ToString() + "    (" + licznik.ToString() + ")   F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + rectangle.Location);

                            piece.area = F;
                            piece.isWhite = false;
                            chessPiece.Add(piece);
                            obiekty_czarne.Add(obiektczarny);
                        }

                        obraz2 = obraz2.Add(obiektczarny);
                        //pictureBox2.Image = obraz2.AsBitmap();
                        pictureBox2.Image = obiektczarny.AsBitmap();
                        licznik++;
                    }

                    //SEGMENTACJA BIAŁE FIGURY
                    else if (tempObraz2.Data[ya, xa, 0] == 255 && tempObraz2.Data[ya, xa, 1] == 255 && tempObraz2.Data[ya, xa, 2] == 255)
                    {
                        obiektbialy = segmentacja_1_obiektu(new Point(xa, ya));
                        CvInvoke.Subtract(tempObraz2, obiektbialy, tempObraz2);

                        byte[,,] temp = obiektbialy.Data;
                        F = pole;

                        if (F > 400)
                        {
                            bool krol = false;
                            bool hetman = false;
                            bool skoczek = false;
                            bool goniec = false;
                            bool wieza = false;
                            bool pionek = false;

                            krol = sprawdz_czy_krol(temp);
                            hetman = sprawdz_czy_hetman(temp);
                            skoczek = sprawdz_czy_skoczek(temp);
                            goniec = sprawdz_czy_goniec(temp);
                            wieza = sprawdz_czy_wieza(temp);
                            pionek = sprawdz_czy_pionek(temp);

                            if (krol == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White King " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else if (hetman == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Queen " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else if (skoczek == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Knight " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else if (goniec == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Bishop " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else if (wieza == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Rook " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else if (pionek == true)
                            {
                                listBox_White_Chess_Notation.Items.Add("White Pawn " + " F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + " xy=" + rectangle.Location);
                            }
                            else listBox_White_Chess_Notation.Items.Add("White NON" + licznik_duzych.ToString() + "    (" + licznik.ToString() + ")   F =" + F.ToString() + " w =" + rectangle.Width + " h=" + rectangle.Height + rectangle.Location);
                         
                            obiekty_biale.Add(obiektbialy);
                        }

                        piece.area = F;
                        piece.isWhite = true;
                        chessPiece.Add(piece);
                        obraz2 = obraz2.Add(obiektbialy);
                        pictureBox2.Image = obraz2.AsBitmap();
                        licznik++;
                    }
                }
            }
                    
            for (xa = 1; xa < obraz1.Width; xa = xa + 40)
            {
                for (ya = 1; ya < obraz1.Height; ya++)
                {
                            // SEGMENTACJA BIAŁE POLA
                    if (tempObraz2.Data[ya, xa, 0] > 200 && tempObraz2.Data[ya, xa, 0] < 220 && tempObraz2.Data[ya, xa, 1] > 230 && tempObraz2.Data[ya, xa, 1] < 245 && tempObraz2.Data[ya, xa, 2] > 230 && tempObraz2.Data[ya, xa, 2] < 245)
                    {
                        bialepole = segmentacja_1_obiektu(new Point(xa, ya));
                        CvInvoke.Subtract(tempObraz2, bialepole, tempObraz2);

                        byte[,,] temp = bialepole.Data;
                        //F = calculate_area(temp);
                        F = pole;
                        // bialepole.ThresholdBinary(new Bgr(120, 150, 100), new Bgr(255, 255, 255));
                        string s = "";
                          if (F > 1200)
                          {
                            ChessBoard board = new(chess_notation_list.ElementAt(licznik_pom), F, true, true);
                            if (F > 2200) 
                            {
                                board.is_figure = false;
                                s = "";
                            } 
                            else
                            {
                                board.is_figure = true;
                                if (F > 1900 && F < 2100) s = "Pawn ";

                            }
                            chessboardnotation.Add(board);
                            licznik_pom++;

                            listBox1.Items.Add(s + board.field_name + "   F =" + F.ToString());
                            checkerboard.Add(bialepole);
                          }

                                obraz2 = obraz2.Add(bialepole);
                                pictureBox2.Image = bialepole.AsBitmap();
                                licznik++;
                    }

                            // SEGMENTACJA CZARNE POLA
                    else if (tempObraz2.Data[ya, xa, 0] == 86 && (tempObraz2.Data[ya, xa, 1] == 150 || tempObraz2.Data[ya, xa, 1] == 0) && tempObraz2.Data[ya, xa, 2] == 118)
                    {
                                czarnepole = segmentacja_1_obiektu(new Point(xa, ya));
                                CvInvoke.Subtract(tempObraz2, czarnepole, tempObraz2);

                                byte[,,] temp = czarnepole.Data;
                        //F = calculate_area(temp);
                        F = pole;
                        string s = "";
                                if (F > 1200)
                                {
                                    ChessBoard board = new(chess_notation_list.ElementAt(licznik_pom), F, true, false);
                                    if (F > 2200)
                                    {
                                        board.is_figure = false;
                                        s = "";
                                    }
                                    else
                                    {
                                        board.is_figure = true;
                                        if (F > 1900 && F < 2100) s = "Pawn ";

                                    }
                            chessboardnotation.Add(board);
                                    licznik_pom++;

                                    listBox1.Items.Add(s + board.field_name + "   F =" + F.ToString());
                                    checkerboard.Add(czarnepole);

                                }

                                obraz2 = obraz2.Add(czarnepole);
                                pictureBox2.Image = obraz2.AsBitmap();
                                licznik++;
                            }
                        
                    
                }
                if (licznik > 200) break;  
            }
        }

        public enum Figura
        {
            Pawn,
            Rook,
            Knight,
            Bishop,
            Queen,
            King
        }

        private bool sprawdz_czy_pionek(byte[,,] temp)
        {
            // Rozpoznanie wiezy
            decimal wsp_skal_krol_Y1, wsp_skal_krol_Y2, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y1, warunek_polozenia_Y2, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y1 = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y1 = warunek_polozenia_Y2 = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;
            decimal a = 1;
            decimal b = 2;
            decimal c = 22;
            decimal d = 6;
            decimal e = 38;



            wsp_skal_krol_Y1 = 2;
            wsp_skal_krol_X1 = 2;
            wsp_skal_krol_Y2 = c / e;
            wsp_skal_krol_X2 = d / e;

            warunek_polozenia_Y1 = rectangle.Bottom - wsp_skal_krol_Y1;
            warunek_polozenia_X1 = rectangle.X + wsp_skal_krol_X1;
            warunek_polozenia_Y2 = rectangle.Y + (wsp_skal_krol_Y2 * rectangle.Height);
            warunek_polozenia_X2 = rectangle.X + (wsp_skal_krol_X2 * rectangle.Width);


            if (temp[(int)warunek_polozenia_Y1, (int)warunek_polozenia_X1, 0] == 120 && temp[(int)warunek_polozenia_Y2, (int)warunek_polozenia_X2, 0] == 0)
            {
                return true;
            }

            else return false;
        }

        private bool sprawdz_czy_wieza(byte[,,] temp)
        {
            // Rozpoznanie wiezy
            decimal wsp_skal_krol_Y1, wsp_skal_krol_Y2, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y1, warunek_polozenia_Y2, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y1 = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y1 = warunek_polozenia_Y2 = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;
            decimal a = 1;
            decimal b = 2;
            decimal c = 5;
            decimal d = 7;
            decimal e = 44;



            wsp_skal_krol_Y1 = 2;
            wsp_skal_krol_X1 = 2;
            wsp_skal_krol_Y2 = c / e;
            wsp_skal_krol_X2 = d / e;

            warunek_polozenia_Y1 = rectangle.Bottom - wsp_skal_krol_Y1;
            warunek_polozenia_X1 = rectangle.X + wsp_skal_krol_X1;
            warunek_polozenia_Y2 = rectangle.Y + (wsp_skal_krol_Y2 * rectangle.Height);
            warunek_polozenia_X2 = rectangle.X + (wsp_skal_krol_X2 * rectangle.Width);


            if (temp[(int)warunek_polozenia_Y1, (int)warunek_polozenia_X1, 0] == 120 && temp[(int)warunek_polozenia_Y2, (int)warunek_polozenia_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool sprawdz_czy_goniec(byte[,,] temp)
        {
            // Rozpoznanie gonca
            decimal wsp_skal_krol_Y1, wsp_skal_krol_Y2, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y1, warunek_polozenia_Y2, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y1 = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y1 = warunek_polozenia_Y2 = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;
            decimal a = 1;
            decimal b = 2;
            decimal c = 7;
            decimal d = 45;



            wsp_skal_krol_Y1 = 2;
            wsp_skal_krol_X1 = 2;
            wsp_skal_krol_Y2 = a/b;
            wsp_skal_krol_X2 = c/d;

            warunek_polozenia_Y1 = rectangle.Bottom - wsp_skal_krol_Y1;
            warunek_polozenia_X1 = rectangle.X + wsp_skal_krol_X1;
            warunek_polozenia_Y2 = rectangle.Y + (wsp_skal_krol_Y2 * rectangle.Height);
            warunek_polozenia_X2 = rectangle.X + (wsp_skal_krol_X2 * rectangle.Width);


            if (temp[(int)warunek_polozenia_Y1, (int)warunek_polozenia_X1, 0] == 120 && temp[(int)warunek_polozenia_Y2, (int)warunek_polozenia_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool sprawdz_czy_skoczek(byte[,,] temp)
        {
            // Rozpoznanie skoczka
            decimal wsp_skal_krol_Y, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;

            wsp_skal_krol_Y = 2;
            wsp_skal_krol_X1 = 2;
            warunek_polozenia_Y = rectangle.Bottom - wsp_skal_krol_Y;
            warunek_polozenia_X1 = rectangle.X + wsp_skal_krol_X1;
            warunek_polozenia_X2 = rectangle.Right - wsp_skal_krol_X1;


            if (temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X1, 0] == 0 && temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool sprawdz_czy_hetman(byte[,,] temp)
        {
            // Rozpoznanie hetmana
            decimal wsp_skal_krol_Y, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;
            decimal a = 2;
            decimal b = 13;
            decimal c = 47;



            wsp_skal_krol_Y = b /c;
            wsp_skal_krol_X1 = 2;
            warunek_polozenia_Y = rectangle.Y + (rectangle.Height * wsp_skal_krol_Y);
            warunek_polozenia_X1 = rectangle.X + wsp_skal_krol_X1;
            warunek_polozenia_X2 = rectangle.Right - wsp_skal_krol_X1;
           // warunek_polozenia_X3 = rectangle.X + 2;


            if (temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X1, 0] == 120 && temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X2, 0] == 120)
            {
                return true;
            }

            else return false;
        }

        private bool sprawdz_czy_krol(byte[,,] temp)
        {
            // Rozpoznanie krola
            decimal wsp_skal_krol_Y, wsp_skal_krol_X1, wsp_skal_krol_X2, warunek_polozenia_Y, warunek_polozenia_X1, warunek_polozenia_X2, warunek_polozenia_X3;
            wsp_skal_krol_Y = wsp_skal_krol_X1 = wsp_skal_krol_X2 = warunek_polozenia_Y = warunek_polozenia_X1 = warunek_polozenia_X2 = warunek_polozenia_X3 = 0;
            decimal a = 25;
            decimal b = 48;
            decimal c = 16;
            decimal d = 39;
            decimal e = 57;


            wsp_skal_krol_Y = a / b;
            wsp_skal_krol_X1 = c / e;
            wsp_skal_krol_X2 = d / e;
            warunek_polozenia_Y = rectangle.Y + (rectangle.Height * wsp_skal_krol_Y);
            warunek_polozenia_X1 = rectangle.X + (rectangle.Width * wsp_skal_krol_X1);
            warunek_polozenia_X2 = rectangle.X + (rectangle.Width * wsp_skal_krol_X2);
            warunek_polozenia_X3 = rectangle.X + 2;


            if (temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X1, 0] == 0 && temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X2, 0] == 0 && temp[(int)warunek_polozenia_Y, (int)warunek_polozenia_X3, 0] == 120)
            {
                return true;
            }

            else  return false;
        }

        private void button_Clear_White_Chess_Notation_Click(object sender, EventArgs e)
        {
            obiekty_biale.Clear();
            listBox_White_Chess_Notation.Items.Clear();
        }


        private void button_Clear_Black_Chess_Notation_Click(object sender, EventArgs e)
        {
            obiekty_czarne.Clear();
            listBox_Black_Chess_Notation.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // chessboardnotation.Add()

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //Zabezpieczenie przed wybraniem obiektu, którego nie ma na liście
            //if (obiekty_biale.Count <= listBox_White_Chess_Notation.SelectedIndex || listBox_White_Chess_Notation.SelectedIndex < 0) return;
            if (checkerboard.Count <= listBox1.SelectedIndex || listBox1.SelectedIndex < 0) return;

            int index = listBox1.SelectedIndex;

          //  obraz_binarny = checkerboard[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
          //  obraz_binarny = obiekty_biale[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
            //obraz_binarny.CopyTo(obraz3);
            pictureBox3.Image = checkerboard[index].AsBitmap();
        }



        private int calculate_area(byte[,,] temp)
        {
            int F = 0;
            
            for (int X = 0; X < obraz1.Width; X++)
            {
                for (int Y = 0; Y < obraz1.Height; Y++)
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
                for(int j = 8; j>=1; j--)
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

        private Image<Bgr, byte> segmentacja_1_obiektu(Point ognisko)
        {
            // procedura zwraca obraz z pojedynczym obrazem
            Image<Bgr, byte> tempObraz = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz);

            //Maska operacyjna - wymagana przez funkcję FloodFill

            maska = new Image<Gray, byte>(obraz2.Width + 2, obraz2.Height + 2, new Gray(0));
            maska.SetZero();

            //prostokąt opisujący wysegmentowany obiekt

            Rectangle rect = new Rectangle();


             kolor.V0 = 120;
             kolor.V1 = 150;
             kolor.V2 = 100;

          //  kolor.V0 = 255;
           // kolor.V1 = 255;
          //  kolor.V2 = 255;

            //Funkcja FloodFill wypełnia kolorem (parametr 'kolor') obiekt na obrazie 'tempObraz'
            //Punkt startowy segmentacji (należący do obiektu) określa parametr 'ognisko'
            //Dwa ostatnie argumenty funkcji to odpowiednio maksymalna różnica koloru sąsiadujących pikseli obiektu w dół i w górę

            pole = CvInvoke.FloodFill(tempObraz, maska, ognisko, kolor, out rect, new MCvScalar(2,2, 2), new MCvScalar(2, 2, 2),
                Emgu.CV.CvEnum.Connectivity.FourConnected, Emgu.CV.CvEnum.FloodFillType.FixedRange);

            // Connectivity.EightConnected

            //tempObraz zawiera zmodyfikowany obraz wejściowy ze wszystkimi obiektami, konieczne jest odfiltowanie dodatkowych obiektów

            tempObraz = tempObraz.ThresholdToZero(new Bgr(kolor.V0 - 1, kolor.V1 - 1, kolor.V2 - 1));
            tempObraz = tempObraz.ThresholdToZeroInv(new Bgr(kolor.V0, kolor.V1, kolor.V2));

            //Narysowanie prostokąta otaczającego wysegmentowany obiekt
            CvInvoke.Rectangle(tempObraz, rect, new MCvScalar(0, 0, 255), 1);
            rectangle = rect;
            // tempObraz   na zakończenie progowania powinien zawierać obraz z pojedynczym obiektem

            CvInvoke.Resize(maska, maska, new Size(pictureBox3.Width, pictureBox3.Height));
            return tempObraz;
            
        }
        #endregion


    }

    
}
