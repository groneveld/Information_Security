using System;
using System.Collections;

namespace CryptoLib
{
    class Crypto
    {

        public static Random rand = new Random();
        static Hashtable hash = new Hashtable();

        public static long stBstG(long a, long p, long y, bool debug)
        {
            long m = 0, k;
            long x = 0, i = 0, j = 0;
            m = k = (long)Math.Sqrt(p) + 1;
            long[] mMas = new long[m];
            for (long z = 0; z < m; z++)
            {
                mMas[z] = (SimpleMod(a, z, p, false) * (y % p)) % p;
                if (debug) Console.Write($"{mMas[z]} ");
            }
            if (debug) Console.WriteLine();

            for (long z = 1; z <= k; z++)
            {
                try { hash.Add(SimpleMod(a, z * m, p, false), z); }
                catch { }
            }

            if (debug)
            {
                foreach (DictionaryEntry entry in hash)
                    Console.Write($"{entry.Key} ");
                Console.WriteLine();
            }

            for (long z = 0; z < m; z++)
                if (hash.Contains(mMas[z]))
                {
                    i = (long)hash[mMas[z]];
                    j = z;
                    break;
                }
            x = i * m - j;
            if (debug) Console.WriteLine($"m = {m}, i = {j}, j = {i}");
            return x;
        }

        public static long DiffHell(bool debug)
        {
            long q = SimpleRandom(1);
            long p = 2 * q + 1;
            while (!isSimple(p))
            {
                q = SimpleRandom(1);
                p = 2 * q + 1;
            }
            long g = (long)rand.Next((int)p);
            while (SimpleMod(g, q, p, false) == 1)
                g = (long)rand.Next((int)p);
            long Xa = SimpleRandom(1), Xb = SimpleRandom(1);
            long Ya = SimpleMod(g, Xa, p, false);
            long Yb = SimpleMod(g, Xb, p, false);
            long Zab = SimpleMod(Yb, Xa, p, false);
            long Zba = SimpleMod(Ya, Xb, p, false);
            if (debug)
                Console.WriteLine($"q = {q}\np = {p}\ng = {g}\nXa = {Xa}\nXb = {Xb}\nYa = {Ya}\nYb = {Yb}\nZab = {Zab}\nZba = {Zba}");
            return Zab;
        }

        public static long[] Euclidean(long a, long b, bool debug)
        {
            if (a < b)
            {
                long temp = b;
                b = a;
                a = temp;
            }

            long[] U = { a, 1, 0 }, V = { b, 0, 1 }, T = new long[3];
            long q;
            int numIteration = 0;
            while (V[0] > 0)
            {
                q = (long)Math.Floor((decimal)(U[0] / V[0]));
                for (int i = 0; i < 3; i++)
                    T[i] = U[i] - q * V[i];
                Array.Copy(V, U, 3);
                Array.Copy(T, V, 3);
                numIteration++;
            }
            for (int i = 0; i < U.Length; i++)
                if (U[i] < 0) U[i] += a;

            if (debug)
                Console.WriteLine($"iter = {numIteration}");
            return U;
        }

        public static long SimpleRandom(int size)
        {
            long x = (long)rand.Next(100, 1000000000);
            if (size == 0)
                return x;
            bool exit = false;
            while (!exit)
            {
                exit = false;
                x = (long)rand.Next(100, (size == 2) ? 1000 : 1000000000);
                if (isSimple(x))
                    exit = true;
            }
            return x;
        }

        public static bool isSimple(long x)
        {
            for (long i = 2; i * i <= x; i++)
                if (x % i == 0)
                    return false;
            return true;
        }

        public static long SimpleMod(long a, long x, long p, bool debug)
        {   //y = a ^ x % p
            int numIteration = 0;
            long y = 1;
            long s = a;
            while (x != 0)
            {
                if ((x & 1) == 1)
                    y = (y % p * s % p) % p;
                s = (s % p * s % p) % p;
                x >>= 1;
                numIteration++;
            }
            if (debug)
                Console.WriteLine($"iter = {numIteration}");
            return y;
        }

        public static void MessageOK(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        public static void MessageBad(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ResetColor();
        }

        public static void MessageInfo(string str)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(str);
            Console.ResetColor();
        }
    }
}
