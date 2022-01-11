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

        //Struktura przechowująca momenty figury
        Moments momenty = new Moments();

        //Lista obrazów z poszczególnymi obiektami po segmentacji
        List<Image<Bgr, byte>> obiekty_biale = new List<Image<Bgr, byte>>();
        List<Image<Bgr, byte>> obiekty_czarne = new List<Image<Bgr, byte>>();

        //Lista z polami szachownicy
        List<Image<Bgr, byte>> checkerboard = new List<Image<Bgr, byte>>();
        List<string> chess_notation_list = new List<string>();

        //Lista zawierajaca notacje szachowa
        List<IChessBoard> chessboardnotation = new List<IChessBoard>();
        


        //Lista ognisk do segmentacji - pomocniczo
        List<Point> ogniska = new List<Point>();

        //Początkowy kolor w procedurze segmentacji
        MCvScalar kolor = new MCvScalar(0, 0, 0);

        //Tablica sygnatury, rozmiar tablicy jest zmieniany przez procedurę
        double[] sygnaturaTab = new double[360];

        //Środek ciężkości obiektu
        Point srodek = new Point(0, 0);



        public Chess_App()
        {
            InitializeComponent();
        }



        private void button_From_File_Click(object sender, EventArgs e)
        {
            Mat zPliku;
            zPliku = CvInvoke.Imread(@"C:\Users\zbign\OneDrive - Politechnika Łódzka\V SEMESTR\Studia\V SEMESTR\Systemy wizyjne\Laboratorium\PROJEKT_MOJEGO_ZYCIA\a.bmp");
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
            obraz_binarny = obiekty_biale[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
            //obraz_binarny.CopyTo(obraz3);
            pictureBox3.Image = obraz_binarny.AsBitmap();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Zabezpieczenie przed wybraniem obiektu, którego nie ma na liście
            //if (pola_czarne.Count <= listBox_Black_Chess_Notation.SelectedIndex || listBox_Black_Chess_Notation.SelectedIndex < 0) return;
            //if (obiekty_czarne.Count <= listBox_Black_Chess_Notation.SelectedIndex || listBox_Black_Chess_Notation.SelectedIndex < 0) return;

            int index = listBox_Black_Chess_Notation.SelectedIndex;

            obraz_binarny = obiekty_czarne[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
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
            MouseEventArgs M = e as MouseEventArgs;

            // Pobranie koloru w miejscu klikniecia
             Bitmap b = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(b, pictureBox1.ClientRectangle);
            Color kolorKlikniecia = b.GetPixel(M.X, M.Y);
            b.Dispose();
            listBox_Black_Chess_Notation.Items.Add("Kolor: " + kolorKlikniecia.ToString());
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

            // label7.Text = b.ToString();
            //  label6.Text = g.ToString();
            //      label5.Text = r.ToString();

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
            int xa, ya, rozmiar_obiektu;
            double F = 0;
            int licznik_pom = 0;
            // pobranie progu dla dużych obiektów

            rozmiar_obiektu = 300
                ;
            Image<Bgr, byte> obiektczarny = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> czarnepole = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> obiektbialy = new Image<Bgr, byte>(obraz1.Size);
            Image<Bgr, byte> bialepole = new Image<Bgr, byte>(obraz1.Size);

            ChessBoard board = new ChessBoard("sss",2);

            MCvScalar kolor = new MCvScalar(10, 0, 0);

            Image<Bgr, byte> tempObraz1 = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz1);

            //Stworzenie tymczasowej kopii obrazu zrodlowego
            Image<Bgr, byte> tempObraz2 = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz2);

            Image<Bgr, byte> tempObraz3 = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz3);

            Image<Bgr, byte> tempObraz4 = new Image<Bgr, byte>(obraz1.Size);
            obraz1.CopyTo(tempObraz4);


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
                        F = calculate_area(temp);

                        if (F > 400)
                        {
                            if (F > 400 && F < 500)
                                listBox_Black_Chess_Notation.Items.Add("Black Pawn " + " F =" + F.ToString());

                            else if (F > 700 && F < 755)
                                listBox_Black_Chess_Notation.Items.Add("Black Bishop " + " F =" + F.ToString());

                            else if (F > 756 && F < 840)
                                listBox_Black_Chess_Notation.Items.Add("Black Rook " + " F =" + F.ToString());

                            else if (F > 850 && F < 900)
                                listBox_Black_Chess_Notation.Items.Add("Black Knight " + " F =" + F.ToString());

                            else if (F > 900 && F < 950)
                                listBox_Black_Chess_Notation.Items.Add("Black Queen " + " F =" + F.ToString());

                            else if (F > 951 && F < 999)
                                listBox_Black_Chess_Notation.Items.Add("Black King " + " F =" + F.ToString());

                            else
                                listBox_Black_Chess_Notation.Items.Add("Black" + licznik_duzych.ToString() + "    (" + licznik.ToString() + ")   F =" + F.ToString());

                            obiekty_czarne.Add(obiektczarny);
                        }

                        obraz2 = obraz2.Add(obiektczarny);
                        //pictureBox2.Image = obraz2.AsBitmap();
                        pictureBox2.Image = obiektczarny.AsBitmap();
                        licznik++;
                    }

                    //SEGMENTACJA BIAŁE FIGURY
                    else if (tempObraz1.Data[ya, xa, 0] == 255 && tempObraz1.Data[ya, xa, 1] == 255 && tempObraz1.Data[ya, xa, 2] == 255)
                    {
                        obiektbialy = segmentacja_1_obiektu(new Point(xa, ya));
                        CvInvoke.Subtract(tempObraz1, obiektbialy, tempObraz1);

                        byte[,,] temp = obiektbialy.Data;
                        F = calculate_area(temp);

                        if (F > 300)
                        {
                            if (F > 300 && F < 450)
                                listBox_White_Chess_Notation.Items.Add("White Pawn " + " F =" + F.ToString());

                            else if (F > 590 && F < 660)
                                listBox_White_Chess_Notation.Items.Add("White Rook " + " F =" + F.ToString());

                            else if (F > 690 && F < 725)
                                listBox_White_Chess_Notation.Items.Add("White Knight " + " F =" + F.ToString());

                            else if (F > 500 && F < 560)
                                listBox_White_Chess_Notation.Items.Add("White Bishop " + " F =" + F.ToString());

                            else if (F > 765 && F < 850)
                                listBox_White_Chess_Notation.Items.Add("White Queen " + " F =" + F.ToString());

                            else if (F > 726 && F < 764)
                                listBox_White_Chess_Notation.Items.Add("White King " + " F =" + F.ToString());


                            else listBox_White_Chess_Notation.Items.Add("White" + licznik_duzych.ToString() + "    (" + licznik.ToString() + ")   F =" + F.ToString());
                            obiekty_biale.Add(obiektbialy);
                        }

                        obraz2 = obraz2.Add(obiektbialy);
                        pictureBox2.Image = obraz2.AsBitmap();
                        licznik++;
                    }
                }
            }
                    
            for (xa = 1; xa < obraz1.Width; xa = xa + 50)
                    {
                        for (ya = 1; ya < obraz1.Height; ya++)
                        {
                            // SEGMENTACJA BIAŁE POLA
                            if (tempObraz3.Data[ya, xa, 0] > 200 && tempObraz3.Data[ya, xa, 0] < 220 && tempObraz3.Data[ya, xa, 1] > 230 && tempObraz3.Data[ya, xa, 1] < 245 && tempObraz3.Data[ya, xa, 2] > 230 && tempObraz3.Data[ya, xa, 2] < 245)
                            {
                                bialepole = segmentacja_1_obiektu(new Point(xa, ya));
                                CvInvoke.Subtract(tempObraz3, bialepole, tempObraz3);

                                byte[,,] temp = bialepole.Data;
                                F = calculate_area(temp);

                                if (F > 500)
                                {
                                    board.checkerboard_field_name = chess_notation_list.ElementAt(licznik_pom);
                                    board.checkerboard_field_area = (int)F;
                                    chessboardnotation.Add(board);
                                    licznik_pom++;

                                    listBox1.Items.Add(board.checkerboard_field_name + "   F =" + F.ToString());
                                    checkerboard.Add(bialepole);
                                }

                                obraz2 = obraz2.Add(bialepole);
                                pictureBox2.Image = obraz2.AsBitmap();
                                licznik++;
                            }

                            // SEGMENTACJA CZARNE POLA
                            else if (tempObraz4.Data[ya, xa, 0] == 86 && (tempObraz4.Data[ya, xa, 1] == 150 || tempObraz4.Data[ya, xa, 1] == 0) && tempObraz4.Data[ya, xa, 2] == 118)
                            {
                                czarnepole = segmentacja_1_obiektu(new Point(xa, ya));
                                CvInvoke.Subtract(tempObraz4, czarnepole, tempObraz4);

                                byte[,,] temp = czarnepole.Data;
                                F = calculate_area(temp);

                                if (F > 500)
                                {
                                    board.checkerboard_field_name = chess_notation_list.ElementAt(licznik_pom);
                                    board.checkerboard_field_area = (int)F;
                                    chessboardnotation.Add(board);
                                    licznik_pom++;

                                    listBox1.Items.Add(board.checkerboard_field_name + "   F =" + F.ToString());
                                    checkerboard.Add(czarnepole);

                                }

                                obraz2 = obraz2.Add(czarnepole);
                                pictureBox2.Image = obraz2.AsBitmap();
                                licznik++;
                            }
                        
                    
                }
                if (licznik > 1000) break;  // max. wszystkich wykrytych obiektów = 1000 - wyjscie z pętli for
            }
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

            obraz_binarny = checkerboard[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
          //  obraz_binarny = obiekty_biale[index].ThresholdBinary(new Bgr(0, 0, 0), new Bgr(255, 255, 255));
            //obraz_binarny.CopyTo(obraz3);
            pictureBox3.Image = obraz_binarny.AsBitmap();
        }

        private void Chess_App_Load(object sender, EventArgs e)
        {
            //inicjalizacja obrazów
            obraz1 = new Image<Bgr, byte>(pictureBox1.Size);
            obraz2 = obraz1.Clone();
            obraz3 = obraz1.Clone();

            obraz_binarny = obraz1.Clone();
        }

        private double calculate_area(byte[,,] temp)
        {
            double F = 0;
            
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

        //Procedura Pojedynczego Kroku Segmentacji dla danego ogniska

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

            //zmiana koloru przy wywołaniu - lub opcja 2 - stały kolor

            //kolor.V0++;
            // if (kolor.V0 == 255) kolor.V0 = 10;  // ograniczenie koloru

             kolor.V0 = 120;
             kolor.V1 = 150;
             kolor.V2 = 100;

            //Funkcja FloodFill wypełnia kolorem (parametr 'kolor') obiekt na obrazie 'tempObraz'
            //Punkt startowy segmentacji (należący do obiektu) określa parametr 'ognisko'
            //Dwa ostatnie argumenty funkcji to odpowiednio maksymalna różnica koloru sąsiadujących pikseli obiektu w dół i w górę

            CvInvoke.FloodFill(tempObraz, maska, ognisko, kolor, out rect, new MCvScalar(5, 5, 5), new MCvScalar(5, 5, 5),
                Emgu.CV.CvEnum.Connectivity.FourConnected, Emgu.CV.CvEnum.FloodFillType.FixedRange);

            // Connectivity.EightConnected

            //tempObraz zawiera zmodyfikowany obraz wejściowy ze wszystkimi obiektami, konieczne jest odfiltowanie dodatkowych obiektów

            tempObraz = tempObraz.ThresholdToZero(new Bgr(kolor.V0 - 1, kolor.V1 - 1, kolor.V2 - 1));
            tempObraz = tempObraz.ThresholdToZeroInv(new Bgr(kolor.V0, kolor.V1, kolor.V2));

            // tempObraz   na zakończenie progowania powinien zawierać obraz z pojedynczym obiektem

            CvInvoke.Resize(maska, maska, new Size(pictureBox3.Width, pictureBox3.Height));
            return tempObraz;
            
        }

        #endregion


    }

    interface IChessBoard
    {

    }

    class ChessBoard : IChessBoard
    {
        public ChessBoard(string s, int x)
        {
            checkerboard_field_name = s;
            checkerboard_field_area = x;
        }

        public int checkerboard_field_area { get; set; }
        public string checkerboard_field_name { get; set; }
    }

}
