using System.Text;

namespace lab1
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Error: alphabet file not specified");
                return;
            }

            string filePath = @"./../../../" + args[0];
            string csvFilePath = @"./../../../data.csv";

            FileInfo alphabetFile = new(filePath);
            if (!alphabetFile.Exists)
            {
                Console.Error.WriteLine("Error: alphabet file not found");
                return;
            }

            Dictionary<char, int> alphabet = new Dictionary<char, int>();
            Dictionary<char, int> binaryAlphabet = new Dictionary<char, int>();

            string stringFileContent = File.ReadAllText(filePath);
            byte[] binaryFileContent = File.ReadAllBytes(filePath);
            StringBuilder sb = new();

            foreach (var item in binaryFileContent)
            {
                sb.Append(Convert.ToString(item, 2).PadLeft(8, '0'));
            }

            binaryAlphabet['0'] = sb.ToString().Count(c => c == '0');          
            binaryAlphabet['1'] = sb.ToString().Count(c => c == '1');          

            foreach (var item in stringFileContent)
            {
                if (!alphabet.ContainsKey(item))
                {
                    alphabet[item] = stringFileContent.Count(ch => ch == item);
                }
            }

            File.WriteAllText(csvFilePath, "Char;Frequency\n");

            foreach (var item in alphabet)
            {
                string ch = item.Key == '\n' ? "\\n" :
                    item.Key == '\r' ? "\\r" :
                    item.Key.ToString();

                File.AppendAllText(csvFilePath, ch + ";" + item.Value + '\n');
            }

            var fileEntropy = calculateEntropy(alphabet, stringFileContent.Length);
            var fileBinaryAlphabet = calculateEntropy(binaryAlphabet, sb.Length);

            Console.WriteLine("File enthropy = " + fileEntropy);
            Console.WriteLine("File binary enthropy = " + fileBinaryAlphabet + "\n");
            
            string alphabetFIO = args[0] == "mongol.txt" ? "Воликов Дмитрий Анатольевич" : "Volikov Dmitry Anatolevich";
            string asciiFIO = "Volikov Dmitry Anatolevich";
            Console.WriteLine("FIO on alphabet: " + alphabetFIO + "\tI = " + alphabetFIO.Length * fileEntropy);
            Console.WriteLine("FIO on ASCII(p = 0): " + asciiFIO + "\tI = " + asciiFIO.Length * 8 * fileEntropy);
            Console.WriteLine("FIO on ASCII(p = 0.1): " + asciiFIO + "\tI = " + (1 - calculateConditionalEntropy(0.1)) * asciiFIO.Length);
            Console.WriteLine("FIO on ASCII(p = 0.5): " + asciiFIO + "\tI = " + (1 - calculateConditionalEntropy(0.5)) * asciiFIO.Length);
            Console.WriteLine("FIO on ASCII(p = 1.0): " + asciiFIO + "\tI = " + (1 - calculateConditionalEntropy(1.0)) * asciiFIO.Length);
        }

        public static double calculateEntropy(Dictionary<char, int> dict, double length)
        {
            double result = 0;
            foreach (var item in dict)
            {
                result += Math.Log2(item.Value / length) * (item.Value / length);
            }
            return -result;
        }

        public static double calculateConditionalEntropy(double p)
        {
            double q = 1 - p;
            return -p * Math.Log2(p) - q * Math.Log2(q);
        }
    }
}