using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator_v2
{
    class samoobcinajacy
    {
        private LSFR myLSFR;

        public samoobcinajacy(String rejestr, String feedback)
        {
            this.myLSFR = new LSFR(rejestr,feedback);
        }

        public bool step()
        {
            bool[] tablica = { false, false };

            while (tablica[0] == false)
            {
                tablica[0] = myLSFR.MSB();
                myLSFR.tact();
                tablica[1] = myLSFR.MSB();
                myLSFR.tact();
            }
            return tablica[1];
        }

        public char[] generate (int n)
        {
            char[] result = new char[n];

            for (int i = 0; i < n; i++)
            {
                result[i] = (step() ? '1' : '0');
            }
            return result;
        }
    
    }
}
