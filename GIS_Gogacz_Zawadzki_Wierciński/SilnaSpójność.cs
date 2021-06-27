using GIS.KlasyQuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIS_Gogacz_Zawadzki_Wierciński
{
    public partial class SilnaSpójność : Form
    {
        Form Obrazek = new Form();
        int X = 1;
        int[,] macierzSasiedztwa = new int[1, 1];
        string tmacierz;
        public SilnaSpójność()
        {
            InitializeComponent();
        }

        private void SilnaSpójność_GPZPWP_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
            int ile = (int)numericUpDown1.Value;
            X = ile;
            if (ile == 0)
            {
                goto ZerowaMacierz;
            }
            button2.Visible = true;
            Macierz.Controls.Clear();
            macierzSasiedztwa = new int[ile,ile];
            Macierz.Visible = true;
            label2.Visible = true;
            int y = 20;
            for (int i = 1; i <= ile; i++)
            {
                for (int j = 1; j <= ile; j++)
                {
                    TextBox tbx = new TextBox();
                    tbx.Text = "0";
                    tbx.Name = i + " "+ j;
                    tbx.Font = new Font("Arial",10,FontStyle.Bold);
                    tbx.Location = new Point(10+(j * 20), y);
                    tbx.Size = new Size(20,20);
                    Macierz.Controls.Add(tbx);
                }
                y += 25;
            }
        ZerowaMacierz:;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            int a = 0;
            int b = 0;
            int ileb = 0;
            string[] tab = new string[2];
            foreach  (Control c in Macierz.Controls)
            {
                tab = c.Name.Split(' ');
                a = Convert.ToInt32(tab[0])-1;
                b = Convert.ToInt32(tab[1])-1;
                if (c.Text =="1"|| c.Text == "0")
                {
                    macierzSasiedztwa[a, b] = Convert.ToInt32(c.Text);
                    c.BackColor = Color.White;
                }
                else
                {
                    c.BackColor = Color.Pink;
                    ileb++;
                    
                }
                
            }
            if (ileb > 0)
                {
                    MessageBox.Show("Wprowadż Prawidłowe Dane do Macierzy");
                    goto Koniec;
                }
            button4.Visible = true;
            button3.Visible = true;
            
            string tekst = "";
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < X; j++)
                {
                    tekst += macierzSasiedztwa[i, j] + " ";
                }
                tekst += "\n";
            }
            tmacierz = tekst;
            Koniec:;
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox4.Visible = true;
            string tekst=SilnieSpojneSkladowe(macierzSasiedztwa);
            label6.Text = tekst;
            RysowanieGrafow rysowanie = new RysowanieGrafow();
            rysowanie.wygenerujObrazGrafu(rysowanie.convertToGraph(macierzSasiedztwa));
            Obrazek.AutoSize = true;
            Obrazek.Text = @"Graf";
            PictureBox pb = new PictureBox();
            pb.ImageLocation = @".\external\gprz.dt.jpg";
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            Obrazek.Controls.Add(pb);
            Obrazek.Show();

        }
        public static void DFS(int[,] macierz, bool[] visited, int u, Stack<int> stosPost)
        {
            visited[u] = true;//Odwieedzony wierzchołek u
            for (int i = 0; i < macierz.GetLength(0); i++) //Sprawdzamy sąsiadów u 
            {
                if (macierz[u, i] > 0 && visited[i] == false)//Sprawdza czy sąsiad odwiedzony 
                {
                    DFS(macierz, visited, i, stosPost);//Przechodzimy do tego wierzchołka
                }
            }
            stosPost.Push(u);//Wrzucamy na stos wierzchołek gdy ma wszystkich sąsiadów odwiedzonych 
        }
        public static void Transpozycja(int[,] macierz)
        {
            int n = macierz.GetLength(0);
            for (int x = 0; x < n; x++)//pętla po wierszach macierzy
            {
                for (int y = x; y < n; y++)//pętla po kolumnach macierzy
                {
                    //Zamiana macierzy czyli (Macierz^T)
                    int temp = macierz[x, y];
                    macierz[x, y] = macierz[y, x];
                    macierz[y, x] = temp;
                }
            }
        }
        public static string SilnieSpojneSkladowe(int[,] macierz)
        {
            string wynik=" ";
            int n = macierz.GetLength(0);//Pobiera rozmiar macierzy 0 wiersza
            bool[] visited = new bool[n];//Tworzy Tablicę do sprawdzania czy odwiedzony
            for (int i = 0; i < n; i++)//Pętla uzupełnia Tablice 
            {
                visited[i] = false;
            }
            Stack<int> stosPost = new Stack<int>();//Stoso do Pre/Post
            DFS(macierz, visited, 0, stosPost);//Wywołanie DFS dla punktu 0 czyli pierwszego wierzchołka
            //na stosie mamy jako 1-największy post ostatni - najmniejszy post
            Transpozycja(macierz);//Transpozycja grafu
            bool[] visited2 = new bool[n];//Twprzenie macierz visited 2 
            for (int i = 0; i < n; i++)
            {
                visited2[i] = false;//Do ponownego przejścia ustawiamy False
            }
            Stack<int> SilnieSpój = new Stack<int>();//Stos do 
            int licznik = 1;//Liczy ile mamy silnie spójnech
            while (stosPost.Count > 0)//Dopuki jest coś na Stosie Post
            {
                int wierzchołek = stosPost.Pop();//Pobieramy Wierzchołek z Największym POST
                if (visited2[wierzchołek] == false)//Fdy Wierzchołek nie odwiedzony
                {

                    DFS(macierz, visited2, wierzchołek, SilnieSpój);//Przechodzimy DFS
                    wynik+="Spójnie silnie składowa nr " + licznik + " to weirzchołki nr: ";
                    licznik++;
                    while (SilnieSpój.Count > 0)//Dopuki jest coś na SilnieSpój
                    {
                        wynik+=SilnieSpój.Pop()+" ";//Wypisanie
                    }

                    wynik += "\n";
                }
            }
            return wynik;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = true;
            label4.Text = tmacierz;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
            tmacierz = "";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        

        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
