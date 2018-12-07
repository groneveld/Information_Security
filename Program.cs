using CryptoLib;
using System;
using System.IO;

namespace Hamilton_Cycle
{
    class Program : Crypto
    {
        static int Bob_ask_me()
        {
            Console.WriteLine("Вопрос Боба: ");
            Console.WriteLine("1: Каков гамильтонов цикл для графа H?");
            Console.WriteLine("2: Действительно ли граф H изоморфен G?");
            string answer;
            while (true) {
                answer = Console.ReadLine();
                if (answer == "1" || answer == "2")
                    return int.Parse(answer);
                else
                {
                    MessageBad("Выберите один из предложенных вопросов, введя '1' или '2'");
                }
            }
        }
        static void Bob_check_1 (long [,] F, string cycle_file, long D, long N, int n)
        {
            string[] strings = File.ReadAllLines(cycle_file);
            int[] cycle_tops = new int[n];
            for (int i = 0; i < n; i++)
                cycle_tops[i] = -1;
            for (int i = 0; i < n; i++)
            {
                string[] line = strings[i].Split(' ');
                long edge = long.Parse(line[2]);
                long result = SimpleMod(edge, D, N, false);
                int a = int.Parse(line[0]);
                //Боб делает проверку на повтор вершин в предоставленном Алисой цикле
                for (int j = 0; j < n; j++)
                {
                    if (a == cycle_tops[j])
                    {
                        MessageBad("Вершины в цикле повторяются! Алиса врёт!");
                        return;
                    }
                }
                //Боб проверяет соответствие указанных в списке вершин матрице F
                cycle_tops[i] = a;
                int b = int.Parse(line[1]);
                if (result != F[a, b])
                {
                    MessageBad("Результаты не совпали! Алиса врёт!");
                    return;
                }
            }
           //Боб проверяет, через все ли вершины проходит предоставленный Алисой цикл
            for (int i = 0; i < n; i++)
            {
                bool flag = false;
                for (int j = 0; j < n; j++)
                    if (cycle_tops[j] == i)
                        flag = true;
                if (flag == false)
                {
                    MessageBad("Цикл проходит не через все вершины! Алиса врёт!");
                    return;
                }
            }
            MessageOK("Алиса права! Доказательства подтверждены!");
        }

        static void Bob_check_2 (long[,] H_encode, long[,] F, long[,] G, int[] iso_tops, long D, long N, int n)
        {
            long[,] H = new long[n, n];
            for (int i = 0; i < n; i++)
            {
                int str = iso_tops[i];
                for (int j = 0; j < n; j++)
                {
                    //Боб проверяет соответствие матрицы H_encode матрице F
                    if (SimpleMod(H_encode[i, j], D, N, false) != F[i, j])
                    {
                        MessageBad("Результаты не совпали! Алиса врёт!");
                        return;
                    }
                    //Боб получает матрицу H, изоморфную матрице G, путем отбрасывания старшие десятки
                    H[i, j] = H_encode[i, j] % 10;
                    int col = iso_tops[j];
                    //Боб проверяет являются ли графы G и H изомофными
                    if (H[i, j] != G[str, col])
                    {
                        MessageBad("Вершины H и G не совпадают согласно представленной перестановке! Алиса врёт!");
                        return;
                    }
                }
            }
            MessageOK("Алиса права! Доказательства подтверждены!");
        }

        static void Main(string[] args)
        {
            string[] strings = File.ReadAllLines("graph.data");
            string[] strings_2 = File.ReadAllLines("isomorph.num");
            string[] strings_3 = File.ReadAllLines("hamilton.cycle");
            string[] line_2 = strings_2[0].Split(' ');
            string[] line_3 = strings_3[0].Split(' ');
            int n = int.Parse(strings[0].Split(' ')[0]);
            int m = int.Parse(strings[0].Split(' ')[1]);
            long[,] G = new long[n, n];
            long[,] H = new long[n, n];
            long[,] r = new long[n, n];
            long[,] H_encode = new long[n, n];
            long[,] F = new long[n, n];
            //считывание рандомного порядка вершин для изоморфной матрицы H
            int[] iso_tops = new int[n];
            for (int i = 0; i < n; i++)
                iso_tops[i] = int.Parse(line_2[i]);
            //считывание гамильтонова цикла в исходном графе
            int[] ham_cycle = new int[n];
            int[] new_cycle = new int[n];
            for (int i = 0; i < n; i++)
            {
                ham_cycle[i] = int.Parse(line_3[i]);
                //нахождение гамильтоного цикла в графе H по циклу графа G
                for (int j = 0; j < n; j++)
                    if (ham_cycle[i] == iso_tops[j])
                        new_cycle[i] = j;
            }
            //считывание связностей вершин графа в матрицу смежности G
            for (int i = 0; i < m; i++)
            {
                string[] line = strings[i + 1].Split(' ');
                int a = int.Parse(line[0]);
                int b = int.Parse(line[1]);
                G[a, b] = 1;
            }
            //создание матрицы изоморфного графа H, матрицы случайных чисел r и их конкатенации 
            for (int i = 0; i < n; i++)
            {
                int str = iso_tops[i];
                for (int j = 0; j < n; j++)
                {
                    int col = iso_tops[j];
                    H[i, j] = G[str, col];
                    r[i, j] = rand.Next(100);
                    H_encode[i, j] = long.Parse(r[i, j].ToString() + H[i, j].ToString());
                }
            }
            //RSA-encoding
            long P = SimpleRandom(2);
            long Q = SimpleRandom(2);
            long N = P * Q;
            long fi = (P - 1) * (Q - 1);
            long C = SimpleRandom(2);
            long D = Euclidean(fi, C, false)[2];
            while ((C * D) % fi != 1)
            {
                C = SimpleRandom(2);
                D = Euclidean(fi, C, false)[2];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    F[i, j] = SimpleMod(H_encode[i, j], D, N, false);
                }
            }
            Console.WriteLine("F-matrix :");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write("{0, 7:0.##} ", F[i, j]);
                }
                Console.WriteLine();
            }
            //выбор одного из вопросов Боба
            int answer = Bob_ask_me();
            if (answer == 1)
            {
                using (StreamWriter stream = new StreamWriter("cycle.list"))
                {
                    //stream.WriteLine("{0}", n);
                    for (int i = 0; i < n; i++)
                    {
                        if (i != n - 1)
                           stream.WriteLine("{0} {1} {2}", new_cycle[i], new_cycle [i + 1], H_encode[new_cycle[i], new_cycle[i + 1]]);
                        else
                            stream.WriteLine("{0} {1} {2}", new_cycle[i], new_cycle[0], H_encode[new_cycle[i], new_cycle[0]]);
                    }
                }
                //Алиса передает Бобу матрицу F, гамильтонов цикл в графе H и ключи для шифровки/дешифровки RSA
                Bob_check_1(F, "cycle.list", D, N, n);
            }
            else if (answer == 2)
            {
                Bob_check_2(H_encode, F, G, iso_tops, D, N, n);
            }
            Console.Read();
        }
    }
}
