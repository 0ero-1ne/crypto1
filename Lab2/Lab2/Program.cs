using System.Text;

namespace Lab2
{
    class Program
    {
        public static void Main()
        {
            string fileContent = File.ReadAllText("input.txt");
            byte[] fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            string base64EncodedData = new(Base64Encoding(fileContentBytes));
            File.WriteAllText("output.txt", base64EncodedData);

            Dictionary<char, int> alphabetInput = new();
            Dictionary<char, int> alphabetOutput = new();

            Console.WriteLine("INPUT ALPHABET PROPERTIES");

            foreach (var ch in fileContent)
            {
                if (!alphabetInput.ContainsKey(ch))
                {
                    alphabetInput[ch] = fileContent.Count(c => c == ch);
                }
            }

            //Console.WriteLine("Chars frequency:");
            //PrintCharsFrequency(alphabetInput);

            Console.WriteLine($"Shannon entropy: {CalculateShannonEntropy(alphabetInput, fileContent.Length)}");
            Console.WriteLine($"Hartley entropy: {CalculateHartleyEntropy(alphabetInput)}");
            Console.WriteLine($"Redundancy: {CalculateRedundancy(alphabetInput, fileContent.Length)}\n");

            Console.WriteLine("OUTPUT ALPHABET PROPERTIES");
            
            foreach (var ch in base64EncodedData)
            {
                if (!alphabetOutput.ContainsKey(ch))
                {
                    alphabetOutput[ch] = base64EncodedData.Count(c => c == ch);
                }
            }

            //Console.WriteLine("Chars frequency:");
            //PrintCharsFrequency(alphabetOutput);

            Console.WriteLine($"Shannon entropy: {CalculateShannonEntropy(alphabetOutput, base64EncodedData.Length)}");
            Console.WriteLine($"Hartley entropy: {CalculateHartleyEntropy(alphabetOutput)}");
            Console.WriteLine($"Redundancy: {CalculateRedundancy(alphabetOutput, base64EncodedData.Length)}\n");

            string name = "Dmitry";
            string lastname = "Volikov";

            int maxLength = Math.Max(name.Length, lastname.Length);

            string asciiName = name.PadRight(maxLength, '\0');
            string asciiLastname = lastname.PadRight(maxLength, '\0');

            Console.WriteLine("a XOR b XOR b: a = Dmitry, b = Volikov");

            Console.WriteLine($"XOR ASCII: {XOR(XOR(asciiName, asciiLastname), asciiLastname)}");

            string base64Name = new(Base64Encoding(Encoding.UTF8!.GetBytes(asciiName)));
            string base64Lastname = new(Base64Encoding(Encoding.UTF8!.GetBytes(asciiLastname)));

            Console.WriteLine($"XOR BASE64: {XOR(XOR(base64Name, base64Lastname), base64Lastname)}");
        }

        private static string XOR(string name, string lastname)
        {
            byte[] arr1 = Encoding.ASCII.GetBytes(name);
            byte[] arr2 = Encoding.ASCII.GetBytes(lastname);

            string result = "";

            for (int i = 0; i < arr1.Length; i++)
            {
                result += (char)(arr1[i] ^ arr2[i]);
            }

            return result;
        }

        private static double CalculateShannonEntropy(Dictionary<char, int> dict, double length)
        {
            double result = 0;

            foreach (var item in dict)
            {
                result += Math.Log2(item.Value / length) * (item.Value / length);
            }

            return -result;
        }

        private static double CalculateHartleyEntropy(Dictionary<char, int> dict)
        {
            return Math.Log2(dict.Count);
        }

        private static double CalculateRedundancy(Dictionary<char, int> dict, double length)
        {
            var shannonEntropy = CalculateShannonEntropy(dict, length);
            var hartleyEntropy = CalculateHartleyEntropy(dict);

            Console.WriteLine(shannonEntropy / hartleyEntropy);

            return (1 - (shannonEntropy / hartleyEntropy)) * 100;
        }

        private static void PrintCharsFrequency(Dictionary<char, int> dict)
        {
            foreach (var item in dict)
            {
                string ch = item.Key == '\n' ? "\\n" :
                    item.Key == '\r' ? "\\r" :
                    item.Key.ToString();

                Console.WriteLine($"Char - {ch} : {item.Value}");
            }
        }

        public static char[] Base64Encoding(byte[] data)
        {
            int length, length2;
            int blockCount;
            int paddingCount;

            length = data.Length;

            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);
                blockCount = (length + paddingCount) / 3;
            }

            length2 = length + paddingCount;

            byte[] source2;
            source2 = new byte[length2];

            for (int x = 0; x < length2; x++)
            {
                if (x < length)
                {
                    source2[x] = data[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }

            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];

            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];

                temp1 = (byte)((b1 & 252) >> 2);

                temp = (byte)((b1 & 3) << 4);
                temp2 = (byte)((b2 & 240) >> 4);
                temp2 += temp;

                temp = (byte)((b2 & 15) << 2);
                temp3 = (byte)((b3 & 192) >> 6);
                temp3 += temp;

                temp4 = (byte)(b3 & 63);

                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;

            }

            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = SixBitToChar(buffer[x]);
            }

            switch (paddingCount)
            {
                case 0:
                    break;
                case 1:
                    result[blockCount * 4 - 1] = '=';
                    break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default:
                    break;
            }

            return result;
        }

        private static char SixBitToChar(byte b)
        {
            char[] lookupTable = new char[64] {
                'A','B','C','D','E','F','G','H','I','J','K','L','M',
                'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                'a','b','c','d','e','f','g','h','i','j','k','l','m',
                'n','o','p','q','r','s','t','u','v','w','x','y','z',
                '0','1','2','3','4','5','6','7','8','9','+','/'
            };

            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[(int)b];
            }
            else
            {
                return ' ';
            }
        }
    }
}