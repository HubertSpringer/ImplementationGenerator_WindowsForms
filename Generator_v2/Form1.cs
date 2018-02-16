using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generator_v2
{
    public partial class Form1 : Form
    {  
        samoobcinajacy samoobcinajacy;
        String wynik;
        String tekst_jawny;
        String szyfrogram;
        String szyfrogram_do_zapisu;
        Tests testy;
        bool czy_wczytano_juz_ciag;

        public Form1()
        {
            InitializeComponent();
            wynik = "";
            tekst_jawny = "";
             szyfrogram = "";
        }

        #region generator
        private void t_odczepy_TextChanged(object sender, EventArgs e)
        {
            String[] substrings = t_odczepy.Text.Split(',');
            int s = 0;

            foreach (var substring in substrings)
            {
                if (substring == "") break;

                if (int.TryParse(substring, out s))
                {
                    if (s >= t_poczatkowy.TextLength || s < 0)
                    {
                        MessageBox.Show("Zle indeksy odczepow");
                    }
                }  
            }
                
        }
        private void t_poczatkowy_TextChanged(object sender, EventArgs e)
        {
            if (t_poczatkowy.TextLength < 2)
            {
                b_generuj.Enabled = false;
            }
            else
            {
                b_generuj.Enabled = true;
            }
            l_poczatkowy.Text = "stan początkowy rejestru (" + t_poczatkowy.TextLength + ")";

        }
        private void b_generuj_Click(object sender, EventArgs e)
        {
            if (t_ilosc.Text=="")
            {
                MessageBox.Show("Nie wpisano długości generowanego ciągu");
                return;
            }
            if (t_odczepy.Text == "")
            {
                MessageBox.Show("Nie wpisano indexów odczepów");
                return;
            }
            if (t_ilosc.Text == "0")
            {
                MessageBox.Show("Nie można wygenerować ciągu o długości: 0");
                return;
            }
            samoobcinajacy = new samoobcinajacy(t_poczatkowy.Text, t_odczepy.Text);
            String result = new String(samoobcinajacy.generate(Int32.Parse(t_ilosc.Text)));
            wynik = result;
            radioButton1.Checked = true;
            t_wynik.Text = wynik;
        }

        #endregion generator

        #region szyfrator



        //szyfrowanie
        private void button1_Click(object sender, EventArgs e)
        {
            if (wynik == ""&&r_u_ciagu.Checked==true)
            {
                MessageBox.Show("Nie wczytano ciągu szyfrującego (Kliknij Wczytaj -> Ciąg szyfrujący)");
                return;
            }
            if (t_tekst_jawny.Text =="")
            {
                MessageBox.Show("Nie wpisano tekstu jawnego");
                return;
            }
            int l = t_tekst_jawny.Text.Length * 8;

            byte[] byteArray = Encoding.GetEncoding(28592).GetBytes(t_tekst_jawny.Text.ToCharArray());

            var bits = new BitArray(byteArray);

            if (r_u_generatora.Checked)
            {
                if (t_poczatkowy.Text==""|| t_odczepy.Text=="")
                {
                    MessageBox.Show("Generator nie został utworzony.");
                    return;
                }

                samoobcinajacy = new samoobcinajacy(t_poczatkowy.Text, t_odczepy.Text);

                for (int i = 0; i < l; i++)
                {
                    bits[i] ^= samoobcinajacy.step();
                }
            }
            else if (r_u_ciagu.Checked)
            {
                bool[] array = wynik.Select(c => c == '1').ToArray();
                if (l >= array.Length)
                {
                    MessageBox.Show("Wczytany ciąg jest za krótki.");
                    return;
                }
                for (int i = 0; i < l; i++)
                {
                    bits[i] ^= array[i];

                }
            }

            StringBuilder s = new StringBuilder();

            foreach (bool bit in bits) {
                if (bit)
                {
                    s.Append("1");
                }
                else
                {
                    s.Append("0");
                }
            } 
    
            szyfrogram_do_zapisu = s.ToString();

            byte[] byteresult = BitArrayToByteArray(bits);

            t_szyfrogram.Text = Encoding.GetEncoding(28592).GetString(byteresult);
        }

        //deszyfrowanie
        private void b_deszyfruj_Click(object sender, EventArgs e)
        {
            if (wynik == "" && r_u_ciagu.Checked == true)
            {
                MessageBox.Show("Nie wczytano ciągu szyfrującego (Kliknij Wczytaj -> Ciąg szyfrujący)");
                return;
            }
            if (t_szyfrogram.Text=="")
            {
                MessageBox.Show("Nie wpisano szyfrogramu.");
                return;
            }

            int l = t_szyfrogram.Text.Length*8;

            byte[] byteArray = Encoding.GetEncoding(28592).GetBytes(t_szyfrogram.Text.ToCharArray());

            var bits = new BitArray(byteArray);

            if (r_u_generatora.Checked)
            {
                if (t_poczatkowy.Text == "" || t_odczepy.Text == "")
                {
                    MessageBox.Show("Generator nie został utworzony.");
                    return;
                }
                samoobcinajacy = new samoobcinajacy(t_poczatkowy.Text, t_odczepy.Text);

                for (int i = 0; i < l; i++)
                {
                    bits[i] ^= samoobcinajacy.step();
                }
            }
            else if (r_u_ciagu.Checked)
            {
                bool[] array = wynik.Select(c => c == '1').ToArray();
                if (l >= array.Length)
                {
                    MessageBox.Show("Wczytany ciąg jest za krótki.");
                    return;
                }
                for (int i = 0; i < l; i++)
                {
                    bits[i] ^= array[i];

                }
            }

            byte[] byteresult = BitArrayToByteArray(bits);
            t_tekst_jawny.Text = Encoding.GetEncoding(28592).GetString(byteresult);
        }

        #endregion szyfrator

        #region menu

        #region domyslne
        private void domyślneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t_poczatkowy.Text = "01101010";
            t_ilosc.Text = "255";
            t_odczepy.Text = "0,1,6,7";
        }

        #endregion domyslne

        #region loadsave

        //ciąg   load /save
        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wynik = load_File();
            radioButton1.Checked = true;
            t_wynik.Text = wynik;
            t_ilosc.Text = "";
            t_odczepy.Text = "";
            t_poczatkowy.Text = "";
        }
        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_File(wynik);
        }


        //tekst jawny  load / save
        private void teksJawnyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t_tekst_jawny.Text = load_File();
        }
        private void tekstJawnyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_File(tekst_jawny);
        }


        //szyfrogram load / save 
        private void szyfrogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t_szyfrogram.Text = load_File();
        }
        private void szyfrogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            save_File(szyfrogram_do_zapisu);
        }


        //ziarno load / save
        private void ziarnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t_poczatkowy.Text = load_File();
        }
        private void ziarnoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            save_File(t_poczatkowy.Text);
        }

        //load save
        private string load_File()
        {
            Stream myStream = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory = "C:\\Users\\Hubi\\Desktop\\Ten semestr\\Pody\\lab7";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|Binary File (*.bin)|*.bin";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            if (openFileDialog.FilterIndex == 2) {

                                byte[] fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                                BitArray bits = new BitArray(fileBytes);
                                StringBuilder s = new StringBuilder();

                                for (int i = 0; i < bits.Length / 8; i++)
                                {
                                    for(int j = 7; j >= 0; j--)
                                    {
                                        if (bits[i*8+j])
                                        {
                                            s.Append("1");
                                        }
                                        else
                                        {
                                            s.Append("0");
                                        }
                                    }
                                }
                                return s.ToString();
                            }
                            else
                            {
                                byte[] buffer = new byte[myStream.Length];//+10
                                myStream.Read(buffer, 0, (int)myStream.Length);

                                return System.Text.Encoding.Default.GetString(buffer);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            return null;   
        }
        private void save_File(String message)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save As...";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|Binary File (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = "C:\\Users\\Hubi\\Desktop\\Ten semestr\\Pody\\lab7";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.FilterIndex == 2)
                {
                    bool[] array = message.Select(c => c == '1').ToArray();
                    Byte[] toBytes = ToByteArray(array);

                    FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);

                    bw.Write(toBytes);

                    fs.Close();
                    bw.Close();
                }
                else
                {
                    File.WriteAllText(saveFileDialog.FileName, message);
                }
            }
        }

        #endregion loadsave

        #endregion menu

        #region pomocnicze
        public static string BinaryStringToHexString(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }
        private byte[] ToByteArray(bool[] input)
        {
            if (input.Length % 8 != 0)
            {
                byte[] ret = new byte[(input.Length / 8)];
                for (int i = 0; i < input.Length - input.Length % 8; i += 8)
                {
                    int value = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        if (input[i + j])
                        {
                            value += 1 << (7 - j);
                        }
                    }
                    ret[i / 8] = (byte)value;
                }
                return ret;

            }
            else
            {
                byte[] ret = new byte[input.Length / 8];
                for (int i = 0; i < input.Length; i += 8)
                {
                    int value = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        if (input[i + j])
                        {
                            value += 1 << (7 - j);
                        }
                    }
                    ret[i / 8] = (byte)value;
                }
                return ret;
            }
        }
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        #endregion pomocnicze

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (wynik != "")
            {
                t_wynik.Text = wynik;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (wynik != "")
            {
                String n = wynik;

                bool[] array = n.Select(c => c == '1').ToArray();
                Byte[] toBytes = ToByteArray(array);
                StringBuilder result = new StringBuilder(toBytes.Length);

                for (int i = 0; i < toBytes.Length; i++)
                {
                    result.Append(toBytes[i]);
                }
                t_wynik.Text = result.ToString();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (wynik != "")
            {
                string result = "";
                String n = wynik;

                bool[] array = n.Select(c => c == '1').ToArray();

                result = BinaryStringToHexString(n);

                t_wynik.Text = result;
            }
        }

        static string ToBinaryString(Encoding encoding, string text)
        {
            return string.Join("", encoding.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }

        private void t_tekst_jawny_TextChanged(object sender, EventArgs e)
        {
            tekst_jawny = ToBinaryString(Encoding.UTF8, t_tekst_jawny.Text);
        }

        private void t_szyfrogram_TextChanged(object sender, EventArgs e)
        {
            szyfrogram = ToBinaryString(Encoding.UTF8, t_szyfrogram.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!czy_wczytano_juz_ciag)
            {
                MessageBox.Show("Proszę najpierw wczytać ciąg.");
                return;
            }
            testy.Clear();
            label15.Text = "";

            if (checkBox1.Checked)
            {
                if (testy.T_pojedzynczych_bitow())
                {
                    label20.ForeColor = Color.Lime;
                    label20.Text = "POZYTYWNY";
                }
                else
                {
                    label20.ForeColor = Color.Red;
                    label20.Text = "NEGATYWNY";
                }
                label29.Text = testy.liczba_jedynek.ToString();
            }
            else
            {
                label20.ForeColor = Color.Black;
                label20.Text = "----------";
                label29.Text = "";
            }

            if (checkBox2.Checked)
            {
                if (testy.T_serii())
                {
                    label21.ForeColor = Color.Lime;
                    label21.Text = "POZYTYWNY";
                }
                else
                {
                    label21.ForeColor = Color.Red;
                    label21.Text = "NEGATYWNY";
                }
                String zero="";
                foreach(int zmienna in testy.series0)
                {
                    zero += zmienna.ToString() + "  ";
                }
                label24.Text = zero;
                String jeden = "";
                foreach (int zmienna in testy.series1)
                {
                    jeden += zmienna.ToString() + "  ";
                }
                label26.Text = jeden;
            }
            else
            {
                label21.ForeColor = Color.Black;
                label21.Text = "----------";
                label26.Text = "";
                label24.Text = "";
            }

            if (checkBox3.Checked)
            {
                if (testy.T_dlugiej_serii())
                {
                    label22.ForeColor = Color.Lime;
                    label22.Text = "POZYTYWNY";
                }
                else
                {
                    label22.ForeColor = Color.Red;
                    label22.Text = "NEGATYWNY";
                }
                label30.Text = testy.dluga_seria.ToString();
            }
            else
            {
                label22.ForeColor = Color.Black;
                label22.Text = "----------";
                label30.Text = "";
            }

            if (checkBox4.Checked)
            {
                if (testy.T_pokerowy())
                {
                    label23.ForeColor = Color.Lime;
                    label23.Text = "POZYTYWNY";
                }
                else
                {
                    label23.ForeColor = Color.Red;
                    label23.Text = "NEGATYWNY";
                }
                label31.Text = testy.wynik.ToString();
            }
            else
            {
                label23.ForeColor = Color.Black;
                label23.Text = "----------";
                label31.Text = "";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
            String wczytany = load_File();
            if (wczytany == null)
            {
                return;
            }
            if (wczytany.Length < 20000)
            {
                MessageBox.Show("     Ciąg jest za krótki");
                return;
            }
            if (wczytany.Length !=20000)
            {
                MessageBox.Show("     Ciąg był zbyt długi, został obcięty do długośći 20000");
                wczytany = new string(wczytany.Take(20000).ToArray());
            }



            testy = new Tests(wczytany);

            label15.Text = "   Ciąg został Wczytany. \n\n  Można uruchomić Testy.";
            czy_wczytano_juz_ciag = true;
        }

    }
}
