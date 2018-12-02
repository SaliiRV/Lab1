using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SROMLab1 {
    class Program {
        static void Main(string[] args) {
            string a = "2"; //"95D0AC765C6D01F15A75CEA154AA5BC3F636459F925D6602255FF75DD3AD78D9";
            string b = "A";//"DA9CEA567FAF76EFA1920FB35E1238AE8728B7B2EEE03797BAA757B06A45B8F";
            string c = "BFAFB3728B85B300F5AC85C52198659F903E1DAC8DF57600B0955C300AB850AC";
            string qwe = "1";
            ulong[] one = new ulong[32];
            ToArr(qwe, one);
            var a_32 = new ulong[32];
            var b_32 = new ulong[32];
            var c_32 = new ulong[32];
            ToArr(a, a_32);
            ToArr(b, b_32);
            ToArr(c, c_32);

            Console.WriteLine("Addition:");
            ToStr(Addition(a_32, b_32));
            Console.WriteLine("Need:");
            Console.WriteLine("A37A7B1BC467F960548EEF9C8A8B7F4EDEA8D11AC14B697BA10A6CD8DA51D468");

            Console.WriteLine("\nSubtraction:");
            ToStr(Subtraction(a_32, b_32));
            Console.WriteLine("Need:");
            Console.WriteLine("8826DDD0F4720A82605CADA61EC938390DC3BA24636F6288A9B581E2CD091D4A");
            
            Console.WriteLine("\nMultiply:");
            ToStr(Multiply(a_32, b_32));
            Console.WriteLine("Need:");
            Console.WriteLine("7FEF87293F4C7B226F213FC1F514F757467D8D2A4709F6C2487829662D3DEA0DD9A194814A28DD80E3E65F8F21EAEDBEFB85C28F3023F69284970E168DFA437");

            /*Console.WriteLine("\nDivision:");
            ToStr(Division(a_32, b_32));
            Console.WriteLine("Need:");
            Console.WriteLine("A");*/

            Console.WriteLine("\nGorner:");
            ToStr(Gorner(a_32, b_32, one, b));
            Console.WriteLine("Need:");
            //Console.WriteLine("A");
            //var num = Gorner(a_32, b_32, one, b);
            //string gh = "1242";
            
            Console.ReadKey();
            Console.ReadKey();
        }

        public static void ToArr(string a, ulong[] a_32) {
            string z = "0";
            while (a.Length % 8 != 0) {
                a = z + a;
            }
            var p_32 = new ulong[a.Length/8];
            for (int i = 0; i < a.Length; i += 8) {
                p_32[i / 8] = Convert.ToUInt64(a.Substring(i, 8), 16);
                a_32[i / 8] = p_32[i / 8];
            }

            Array.Reverse(a_32);
            Array.Reverse(p_32);

            return;
        }

        public static string ToStr(ulong[] a) {
            string result = null;
            for (int i = 0; i < a.Length; i++) {
                result = (a[i].ToString("X").PadLeft(8, '0')) + result;
            }
            Console.WriteLine(result);
            return result;
        }

        public static ulong[] Addition(ulong[] a_32, ulong[] b_32) {
            var c = new ulong[a_32.Length];
            ulong carry = 0;
            for (var i = 0; i < a_32.Length; i++) {
                var t = a_32[i] + b_32[i] + carry;
                carry = (t >> 32) & 1;
                c[i] = t & 0xffffffff;
            }
            return c;
        }

        public static ulong[] Subtraction(ulong[] a_32, ulong[] b_32) {
            ulong borrow = 0;
            var c = new ulong[32];
            for (var i = 0; i < a_32.Length; i++) {
                var temp = a_32[i] - b_32[i] - borrow;
                if (temp > a_32[i]) {
                    c[i] = 0xFFFFFFFF & temp;
                    borrow = 1;
                }
                else {
                    c[i] = (0xFFFFFFFF & temp);
                    borrow = 0;
                }
            }
            return c;
        }

        public static ulong[] Multiply(ulong[] a_32, ulong[] b_32) {
            ulong carry = 0;
            var c = new ulong[64];
            for (int i = 0; i < a_32.Length; i++) {
                carry = 0;
                for (int j = 0; j < b_32.Length; j++) {
                    ulong temp = c[i + j] + a_32[j] * b_32[i] + carry;
                    c[i + j] = temp & 0xFFFFFFFF;
                    carry = temp >> 32;
                }
                c[i + a_32.Length] = carry;
            }
            return c;
        }

        /*public static ulong[] Division(ulong[] a_32, ulong[] b_32) {
            var b = new ulong[32];
            int k = BitLength(b_32);
            var x = a_32;
            var q = new ulong[128];
            while (Cmp(x, b_32) >= 0) {
                int t = BitLength(x);
                b = ShiftBitsToHigh(b_32, t - k);
                if (Cmp(x, b) == -1) {
                    t--;
                    b = ShiftBitsToHigh(b_32, t - k);
                }
                x = Subtraction(x, b); // ostacha
                q = SetBit(q, t - k); // result
            }
            return q;
        }*/


        public static ulong[] Gorner(ulong[] a_32, ulong[] b_32, ulong[] one, string b) {
            ulong[][] D = new ulong[15][];
            int m = b.Length;
            ulong[] C = new ulong[64];
            //for (i=0, i<one.Length, i++)
            D[0] = one;
            D[1] = a_32;
            for (int i = 2; i <= 15; i++) {
                D[i] = Multiply(D[i - 1], a_32);
            }
            for (int j = 0; j <= m; j++) {
                int w = Convert.ToInt32(b[j]);
                var v = D[w];
                C = Multiply(v, C);
                for (int k = 0; k < 4; k++) {
                    C = Multiply(C, C);
                }
            }

            return a_32;
        }


        // for DIVISION
        /*public static int BitLength(ulong[] b_32) {
            var bits = 0;
            var index = HighNotZero(b_32);
            var temp = b_32[index];
            while (temp > 0) {
                temp >>= 1;
                bits++;
            }
            return bits + sizeof(ulong) * 8 * index;
        }

        public static int HighNotZero(ulong[] a_32) {
            for (var i = a_32.Length - 1; i >= 0; i--) {
                if (a_32[i] > 0) { return i; }
            }
            return 0;
        }


        public static ulong[] ShiftBitsToHigh(ulong[] b_32, int shift_num) {
            if (shift_num == 0) { return b_32; };
            var c = new ulong[128];
            var surrogate = new ulong[32];
            Array.Copy(b_32, surrogate, b_32.Length);
            int shift;
            while (shift_num > 0) {
                c = new ulong[surrogate.Length + 1];
                var carriedBits = 0UL;
                if (shift_num < 32) { shift = shift_num; }
                else { shift = 31; }
                int i = 0;
                for (; i < surrogate.Length; i++) {
                    var temp = surrogate[i];
                    c[i] = (temp << shift) | carriedBits;
                    carriedBits = temp >> (32 - shift);
                }
                c[i] = surrogate[i - 1] >> (32 - shift);
                shift_num -= 31;
                surrogate = c;
            }
            return c;
        }

        public static int Cmp(ulong[] a_32, ulong[] b_32) {
            for (int i = a_32.Length - 1; i > -1; i--) {
                if (a_32[i] > b_32[i]) return 1;
                if (a_32[i] < b_32[i]) return -1;
            }
            return 0;
        }

        public static ulong[] SetBit(ulong[] a_32, int position) {
            var temp = new ulong[32];
            temp[0] = 1;
            temp = ShiftBitsToHigh(temp, position);
            return Addition(a_32, temp);
        }*/

    }
}
