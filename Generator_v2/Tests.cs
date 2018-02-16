using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator_v2
{
    class Tests
    {
        String CiagTestowy;
        public int liczba_jedynek = 0;
        public int[] series1 = new int[6] { 0, 0, 0, 0, 0, 0 };
        public int[] series0 = new int[6] { 0, 0, 0, 0, 0, 0 };
        public bool dluga_seria = false;
        public double wynik = 0;

        public void Clear()
        {
         liczba_jedynek = 0;
         series1 = new int[6] { 0, 0, 0, 0, 0, 0 };
         series0 = new int[6] { 0, 0, 0, 0, 0, 0 };
         dluga_seria = false;
         wynik = 0;
         }

        public Tests(string ciagTestowy)
        {
            CiagTestowy = ciagTestowy;
        }

        public Boolean T_pojedzynczych_bitow()
        {
            foreach (char c in CiagTestowy)
            {
                if (c == '1') liczba_jedynek++;
            }

            return 9725 < liczba_jedynek && liczba_jedynek < 10275;
        }

        public Boolean T_serii()
        {
            int i = 0;
            int j = 0;

            foreach (char c in CiagTestowy)
            {
                if (c == '1')
                {
                    if (j != 0)
                    {
                        if (j > 6) j = 6;
                        series0[j - 1] += 1;
                    }
                    i++;
                    j = 0;
                }else if(c == '0')
                {
                    if (i != 0)
                    {
                        if (i > 6) i = 6;
                        series1[i - 1] += 1;
                    }
                    j++;
                    i = 0;
                }
            }

            return (series1[0] >= 2315 && series1[0] <= 2685
                && series1[1] >= 1114 && series1[1] <= 1386
                && series1[2] >= 527 && series1[2] <= 723
                && series1[3] >= 240 && series1[3] <= 384
                && series1[4] >= 103 && series1[4] <= 209
                && series1[5] >= 103 && series1[5] <= 209

                && series0[0] >= 2315 && series0[0] <= 2685
                && series0[1] >= 1114 && series0[1] <= 1386
                && series0[2] >= 527 && series0[2] <= 723
                && series0[3] >= 240 && series0[3] <= 384
                && series0[4] >= 103 && series0[4] <= 209
                && series0[5] >= 103 && series0[5] <= 209);
        }

        public Boolean T_dlugiej_serii()
        {
            int i = 0;
            int j = 0;

            foreach (char c in CiagTestowy)
            {
                if (c == '1')
                {
                    i++;
                    j = 0;
                }
                else if (c == '0')
                {
                    j++;
                    i = 0;
                }
                if (i >= 26 || j >= 26)
                {
                    dluga_seria = true;
                    return false;
                }
            }
            return true;
        }

        public Boolean T_pokerowy()
        {
            int[] serie = new int[16];
            int i = 0;
            BitArray bitArray = new BitArray(32);

            int[] index = new int[1];
            bitArray.CopyTo(index, 0);



            foreach (char c in CiagTestowy)
            {
                if (c == '1') bitArray[i] = true;
                if (c == '0') bitArray[i] = false;
                i++;

                if (i > 3)
                {
                    i = 0;
                    bitArray.CopyTo(index, 0);
                    serie[index[0]]+=1;
                }
            }

            foreach (int x in serie)
            {
                wynik += x * x;
            }

            double a = 16;
            double b = 5000;
            double d = a / b;
            wynik = wynik *  d;
            wynik = wynik - 5000;

            if(2.16<wynik && wynik < 46.17){
                return true;
            }else{
                return false;
            }    
        }
    
    }
}
