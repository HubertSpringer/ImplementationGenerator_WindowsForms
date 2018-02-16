using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator_v2
{
    class LSFR
    {
        private BitArray register;
        private int length;
        Random rnd;
        String feedback;

        public LSFR(String register,String feedback)
        {
            this.length = register.Length;
            this.register = string_to_bitarray(register);
            this.rnd = new Random(System.DateTime.Now.Millisecond);
            this.feedback = feedback;
        }

        private BitArray string_to_bitarray(String message)
        {
            BitArray bitArray = new BitArray(message.Length);

            for (int i = 0; i < length; i++)
            {
                if (message[i] == '1')
                {
                    bitArray.Set(i, true);
                }
            }

            return bitArray;
        }

        private Boolean feedback_function()
        {
            Boolean result = true;
            String[] substrings = feedback.Split(',');
            
            foreach (var substring in substrings)
                result ^=  register [Int32.Parse(substring)];

            return result;
 
        }

        public Boolean MSB()
        {
            return register[0];
        }

        public void tact()
        {
            Boolean feedback = feedback_function();
            for (int i = 1; i < length; i++)
            {
                register[i - 1] = register[i];
            }
            register[length - 1] = feedback;
        }

        public char[] generate (int n)
        {
            char[] result = new char[n];

            for (int i = 0; i < n;i++)
            {
                result[i] = (MSB() ? '1' : '0');
                tact();
            }
            
            return result;
        }
    }
}
