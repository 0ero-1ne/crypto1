namespace Lab5
{
    class Program
    {
        public static Polynom GenerateXk(int k)
        {
            Random random = new((int)DateTime.Now.Ticks);
            int[] word = new int[k];

            for (int i = 0; i < word.Length; i++)
            {
                word[i] = random.Next() % 2;
            }

            if (word[0] == 0)
            {
                word[0] = 1;
            }

            return new Polynom(word);
        }

        public static List<string> GenerateCheckMatrix(int k, int r, Polynom Gx) // string, REALY? yeah, little cheat :)
        {
            int n = k + r;
            List<string> matrix = new();
            
            for (int i = 1; i <= k; i++)
            {
                var Xnk = new Polynom(n - i);
                var Xr = Xnk.DivideByPolynom(Gx).Item2.ToString().PadLeft(r, '0');
                matrix.Add(Xr);
            }

            for (int i = 0; i < r; i++)
            {
                string Xr = "1".PadLeft(i + 1, '0').PadRight(r, '0');
                matrix.Add(Xr);
            }

            return matrix;
        }

        public static Polynom CalculateXr(Polynom XkXr, Polynom Gx)
        {
            var result = XkXr.DivideByPolynom(Gx).Item2.GetBites();

            return new Polynom(result);
        }

        public static Polynom CalculateXn(Polynom Xk, Polynom Xr, int r)
        {
            var XnnBites = Xk.GetBites();
            var XrBites = new List<int>();
            foreach (var item in Xr.ToString().PadLeft(r, '0'))
            {
                XrBites.Add(int.Parse(item.ToString()));
            }

            return new Polynom(XnnBites.Concat(XrBites).ToArray());
        }

        public static Polynom CalculateYn(Polynom Xn, int errors = 0)
        {
            var bites = Xn.GetBites();
            var result = new int[bites.Count];
            List<int> errorsPositions = new();

            for (int i = 0; i < errors; i++)
            {
                Random random = new();
                int errorPosition = random.Next(0, bites.Count - 1);

                while (errorsPositions.Contains(errorPosition))
                {
                    errorPosition = random.Next(0, bites.Count - 1);
                }
                errorsPositions.Add(errorPosition);
            }

            for (int i = 0; i < bites.Count; i++)
            {
                if (errorsPositions.Contains(i))
                {
                    result[i] = bites[i] ^ 1;
                }
                else
                {
                    result[i] = bites[i];
                }
            }

            return new Polynom(result);
        }

        public static Polynom CalculateFixedYn(Polynom Yn, Polynom S, List<string> CheckMatrix, int r, int k)
        {
            var bites = Yn.ToString().PadLeft(k + r, '0');
            int errorPosition = CheckMatrix.IndexOf(S.ToString().PadLeft(r, '0'));
            List<int> fixedBites = new();

            for (int i = 0; i < bites.Length; i++)
            {
                int bit = int.Parse(bites[i].ToString());
                if (i == errorPosition)
                {
                    fixedBites.Add(bit ^ 1);
                }
                else
                {
                    fixedBites.Add(bit);
                }
            }

            return new Polynom(fixedBites);
        }

        public static void Main()
        {
            int r = 4; //check bites length
            int k = 11; //info word length
            int n = k + r; //code word length

            var Gx = new Polynom(4, "10011"); //Gx = x^4 + x + 1

            var Xk = new Polynom(10, "01101011011");
            Console.WriteLine("Xk:\t" + Xk);
            var XInRPower = new Polynom(r);
            var Xnn = Xk.MultiplyPolynomByPolynom(XInRPower);
            var Xr = CalculateXr(Xnn, Gx);
            Console.WriteLine("Xr:\t" + Xr.ToString().PadLeft(r, '0'));
            var Xn = CalculateXn(Xk, Xr, r);
            Console.WriteLine("Xn:\t" + Xn.ToString().PadLeft(n, '0'));
            var Yn = CalculateYn(Xn, 1);
            Console.WriteLine("Yn:\t" + Yn.ToString().PadLeft(n, '0'));
            var CheckMatrix = GenerateCheckMatrix(k, r, Gx);
            var S = Yn.DivideByPolynom(Gx).Item2;
            var FixedYn = CalculateFixedYn(Yn, S, CheckMatrix, r, k);
            Console.WriteLine("Fixed:\t" + FixedYn.ToString().PadLeft(n, '0'));
            Console.WriteLine("S:\t" + S.ToString().PadLeft(r, '0'));
            Console.WriteLine("Matrix:\n");
            CheckMatrix.ForEach(item => Console.WriteLine(item));
        }
    }
}