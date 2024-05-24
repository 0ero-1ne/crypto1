using System.Text;

namespace Lab6
{
    class Program
    {
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

        static int[] GenerateMessage(int bytes, int k)
        {
            int bitsLength = bytes * 8;
            int legnth = bitsLength + (k - bitsLength % k);
            int[] message = new int[legnth];
            
            for (int i = 0; i < message.Length; i++)
            {
                Random random = new();
                message[i] = random.Next() % 2;
            }
            
            return message;
        }

        public static int[] GenerateEncodedMessage(int[] message, int k, int r, Polynom Gx)
        {
            List<int> encodedMessage = new();
            var XInR = new Polynom(r);
            
            for (int i = 0; i < message.Length / k; i++)
            {
                var bites = message.Skip(i * k).Take(k).ToArray();
                var Xk = new Polynom(bites);
                var XkXinR = Xk.MultiplyPolynomByPolynom(XInR);
                var Xr = XkXinR.DivideByPolynom(Gx).Item2;
                var XkXr = Xk.ToString().PadLeft(k, '0') + Xr.ToString().PadLeft(r, '0');
                foreach (var item in XkXr)
                {
                    encodedMessage.Add(Int32.Parse(item.ToString()));
                }
            }

            return encodedMessage.ToArray();
        }

        public static int[,] GenerateInterleavingMatrix(int[] encodedMessage, int columns)
        {
            int rows = encodedMessage.Length / columns;
            int[,] matrix = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = encodedMessage[i * columns + j];
                }
            }

            return matrix;
        }

        public static int[,] GenerateDeinterleavingMatrix(int[] encodedMessage, int columns)
        {
            int rows = encodedMessage.Length / columns;
            int[,] matrix = new int[rows, columns];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    matrix[j, i] = encodedMessage[i * rows + j];
                }
            }

            return matrix;
        }

        public static int[] GetInterleavingMessage(int[,] interleavingMatrix)
        {
            int rows = interleavingMatrix.GetLength(0);
            int columns = interleavingMatrix.GetLength(1);

            List<int> interleavingMessage = new();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    interleavingMessage.Add(interleavingMatrix[j, i]);
                }
            }

            return interleavingMessage.ToArray();
        }

        public static int[] GenerateErrorsInMessage(int[] interleavingMessage, int errors)
        {
            int[] message = new int[interleavingMessage.Length];

            Random random = new();
            int startPosition = random.Next() % (message.Length - errors);

            for (int i = 0; i < message.Length; i++)
            {
                if (i >= startPosition && i - startPosition < errors)
                {
                    message[i] = interleavingMessage[i] ^ 1;
                } else
                {
                    message[i] = interleavingMessage[i];
                }
            }

            return message;
        }

        public static string GenerateFixedMessage(int[] message, Polynom Gx, List<string> checkMatrix, int k, int r)
        {
            var fixedMessage = new StringBuilder();
             
            for (int i = 0; i < message.Length / (k + r); i++)
            {
                var bites = message.ToList().Skip(i * (k + r)).Take(k + r).ToArray();
                var polynom = new Polynom(bites);
                var result = new StringBuilder(polynom.ToString().PadLeft(k + r, '0'));
                //Console.WriteLine(result);
                var S = polynom.DivideByPolynom(Gx).Item2.ToString().PadLeft(r, '0');

                if (checkMatrix.Contains(S))
                {
                    result[checkMatrix.IndexOf(S)] = result[checkMatrix.IndexOf(S)] == '1' ? '0' : '1';
                }

                fixedMessage.Append(result.ToString());
            }

            return fixedMessage.ToString();
        }

        public static int[] GetNormalMessage(int[,] interleavingMatrix)
        {
            List<int> message = new();

            for (int i = 0; i < interleavingMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < interleavingMatrix.GetLength(1); j++)
                {
                    message.Add(interleavingMatrix[i, j]);
                }
            }

            return message.ToArray();
        }

        public static void Main()
        {
            int l = 15; // message length in bytes
            int columns = 15; // columns number in interleaving matrix
            int k = 7; // length of info word
            int r = 8;
            int n = k + r;
            int errors = 3;
            int ok = 0;
            int fail = 0;
            
            Polynom Gx = new(r, "111010001"); // x^8 + x^7 + x6 + x^4 + 1
            var checkMatrix = GenerateCheckMatrix(k, r, Gx);

            var message = GenerateMessage(l, k);
            var encodedMessage = GenerateEncodedMessage(message, k, r, Gx);
            var interleavingMatrix = GenerateInterleavingMatrix(encodedMessage, columns);
            var interleavingMessage = GetInterleavingMessage(interleavingMatrix);
            var interleavingMessageWithErrors = GenerateErrorsInMessage(interleavingMessage, errors);
            var interleavingMatrixWithErrors = GenerateDeinterleavingMatrix(interleavingMessageWithErrors, columns);
            var normalMessage = GetNormalMessage(interleavingMatrixWithErrors);
            var fixedMessage = GenerateFixedMessage(normalMessage, Gx, checkMatrix, k, r);

            var encodedMessageString = "";
            encodedMessage.ToList().ForEach(item => encodedMessageString += item.ToString());

            message.ToList().ForEach(item => Console.Write(item));
            Console.WriteLine();
            Console.WriteLine();
            encodedMessage.ToList().ForEach(item => Console.Write(item));
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < interleavingMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < interleavingMatrix.GetLength(1); j++)
                {
                    Console.Write(interleavingMatrix[i,j] + " ");
                }
                Console.Write("\t");
                for (int j = 0; j < interleavingMatrixWithErrors.GetLength(1); j++)
                {
                    Console.Write(interleavingMatrixWithErrors[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();

            fixedMessage.ToList().ForEach(item => Console.Write(item));

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Is fixed message same as encoded message: " + (fixedMessage == encodedMessageString.ToString()));
        }
    }
}