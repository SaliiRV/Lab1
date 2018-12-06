using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SROMLab1 {
    class Program {
        static void Main(string[] args) {
            string a = "16D4CA8"; //"95D0AC765C6D01F15A75CEA154AA5BC3F636459F925D6602255FF75DD3AD78D9";
            string b = "81";//"DA9CEA567FAF76EFA1920FB35E1238AE8728B7B2EEE03797BAA757B06A45B8F";
            string sone = "1";
            a = CorrLength(a);
            b = CorrLength(b);
            sone = CorrLength(sone);
            ulong[] one = new ulong[sone.Length / 8];
            ToArr(sone, one);
            var a_32 = new ulong[a.Length / 8];
            var b_32 = new ulong[b.Length / 8];
            a_32 = ToArr(a, a_32);
            b_32 = ToArr(b, b_32);

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

            Console.WriteLine("\nDivision:");
            ToStr(Division(a_32, b_32));
            Console.WriteLine("Need:");
            Console.WriteLine("A");

            Console.WriteLine("\nGorner:");
            ToStr(Gorner(a_32, b_32, one, b));
            Console.WriteLine("Need:");

            
            Console.ReadKey();
            Console.ReadKey();
        }

        public static string CorrLength(string a) { 
            string z = "0";
            while (a.Length % 8 != 0) {
                a = z + a;
            }
            return a;
        }

        public static ulong[] RHZ(ulong[] a_32) {
            int l = a_32.Length;
            int i = l - 1;
            while (a_32[i] == 0) { i--; }
            var r = new ulong[i + 1];
            Array.Copy(a_32, r, i + 1);
            return r;
        }

        public static ulong[] ToArr(string a, ulong[] a_32) {
            var p_32 = new ulong[a.Length / 8];
            for (int i = 0; i < a.Length; i += 8) {
                p_32[i / 8] = Convert.ToUInt64(a.Substring(i, 8), 16);
                a_32[i / 8] = p_32[i / 8];
            }
            Array.Reverse(a_32);
            Array.Reverse(p_32);

            return a_32;
        }

        public static string ToStr(ulong[] a_32) {
            string result = null;
            for (int i = 0; i < a_32.Length; i++) {
                result = (a_32[i].ToString("X").PadLeft(8, '0')) + result;
            }
            Console.WriteLine(result);
            return result;
        }

        public static ulong[] Addition(ulong[] a_32, ulong[] b_32) {
            var maxlenght = Math.Max(a_32.Length, b_32.Length);
            var c = new ulong[maxlenght];
            Array.Resize(ref a_32, maxlenght);
            Array.Resize(ref b_32, maxlenght);
            ulong carry = 0;
            for (var i = 0; i < maxlenght; i++) {
                ulong t = a_32[i] + b_32[i] + carry;
                carry = t >> 32;
                c[i] = t & 0xffffffff;
            }
            RHZ(c);
            return c;
        }

        public static ulong[] Subtraction(ulong[] a_32, ulong[] b_32) {
            ulong borrow = 0;
            var maxlenght = Math.Max(a_32.Length, b_32.Length);
            Array.Resize(ref a_32, maxlenght);
            Array.Resize(ref b_32, maxlenght);
            var c = new ulong[maxlenght];
            for (var i = 0; i < maxlenght; i++) {
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
            //RHZ(c);
            return c;
        }

        public static ulong[] Multiply(ulong[] a_32, ulong[] b_32) {
            ulong carry = 0;
            var maxlenght = Math.Max(a_32.Length, b_32.Length);
            Array.Resize(ref a_32, maxlenght);
            Array.Resize(ref b_32, maxlenght);
            var c = new ulong[2 * maxlenght];
            for (int i = 0; i < maxlenght; i++) {
                carry = 0;
                for (int j = 0; j < maxlenght; j++) {
                    ulong temp = c[i + j] + a_32[j] * b_32[i] + carry;
                    c[i + j] = temp & 0xFFFFFFFF;
                    carry = temp >> 32;
                }
                c[i + a_32.Length] = carry;
            }
            RHZ(c);
            return c;
        }

        public static ulong[] Division(ulong[] a_32, ulong[] b_32) {
            var k = BitLength(b_32);
            var r = a_32;
            ulong[] q = new ulong[a_32.Length];
            ulong[] Temp = new ulong[a_32.Length];
            ulong[] c = new ulong[a_32.Length];
            Temp[0] = 0x1;

            while (LongCmp(r, b_32) >= 0) {
                var t = BitLength(r);
                c = ShiftBitsToHigh(b_32, t - k);
                if (LongCmp(r, c) == -1) {
                    t = t - 1;
                    c = ShiftBitsToHigh(b_32, t - k);
                }
                r = Subtraction(r, c);
                q = Addition(q, ShiftBitsToHigh(Temp, t - k));
            }
            return q;
        }

        public static ulong[] Gorner(ulong[] a_32, ulong[] b_32, ulong[] one, string b) {
            ulong[][] D = new ulong[16][];
            int m = b.Length;
            int k = 0;
            ulong[] C = new ulong[1];
            C[0] = 0x1;
            D[0] = new ulong[1] { 1 };
            D[1] = a_32;
            for (int i = 2; i < 16; i++) {
                D[i] = Multiply(D[i - 1], a_32);
                D[i] = RHZ(D[i]);
            }
            for (int j = 0; j < m; j++) {
                var qwer = b[j].ToString();
                int w = Convert.ToInt32(qwer, 16);
                var v = D[w];
                C = Multiply(C, v);
                if (j != m - 1) {
                    for ( k = 1; k<= 4; k++) {
                        C = Multiply(C, C);
                        C = RHZ(C);
                    }
                }
            }
            RHZ(C);
            return C;
        }

        public static ulong[] ShiftBitsToHigh(ulong[] a, int b) {
            int t = b / 32;
            int s = b - t * 32;
            ulong n, carry = 0;
            ulong[] C = new ulong[a.Length + t + 1];
            for (int i = 0; i < a.Length; i++) {
                n = a[i];
                n = n << s;
                C[i + t] = (n & 0xFFFFFFFF) + carry;
                carry = (n & 0xFFFFFFFF00000000) >> 32;
            }
            C[C.Length - 1] = carry;
            return C;
        }

        public static int BitLength(ulong[] a_32) {
            int bits = 0;
            int i = a_32.Length - 1;
            while (a_32[i] == 0) {
                if (i < 0)
                    return 0;
                i--;
            }
            var n = a_32[i];
            while (n > 0) {
                bits++;
                n = n >> 1;
            }
            bits = bits + 32 * i;
            return bits;
        }

        static int LongCmp(ulong[] a_32, ulong[] b_32) {
            var maxlenght = Math.Max(a_32.Length, b_32.Length);
            Array.Resize(ref a_32, maxlenght);
            Array.Resize(ref b_32, maxlenght);
            for (int i = a_32.Length - 1; i > -1; i--) {
                if (a_32[i] < b_32[i]) { return -1; }
                if (a_32[i] > b_32[i]) { return 1; }
            }
            return 0;
        }
    }
}
