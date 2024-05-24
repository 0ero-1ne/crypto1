using System.Diagnostics;

static List<string> GenerateW1(string message)
{
    List<string> list = new();

    for (int i = 0; i < message.Length; i++)
    {
        var str = "";
        for (int j = 0; j < message.Length; j++)
        {
            str += message[(i + j) % message.Length].ToString();
        }
        list.Add(str);
    }

    return list;
}

static List<string> GenerateW2(List<string> w1)
{
    var list = new List<string>(w1);
    list.Sort();

    return list;
}

static (string, int) GetCompressedMessage(List<string> w2, string message)
{
    string compressedMessage = "";
    w2.ForEach(item => compressedMessage += item[item.Length - 1]);

    int messagePosition = w2.IndexOf(message);

    return (compressedMessage, messagePosition);
}

static List<string> GetDecompressedW2(string message)
{
    List<string> list = new();

    for (int i = 0; i < message.Length; i++)
    {
        list.Add(message[i].ToString());
    }

    list.Sort();

    for (int i = 0; i < message.Length - 1; i++)
    {
        for (int j = 0; j < message.Length; j++)
        {
            list[j] = message[j].ToString() + list[j].ToString();
        }

        list.Sort();
    }

    return list;
}

static string GetBinaryMessage(string message)
{
    string result = "";

    for (int i = 0; i < message.Length; i++)
    {
        result += Convert.ToString(message[i], 2).PadLeft(8, '0');
    }

    return result;
}

string message = "BANANA".ToUpper();
string binaryMessage = GetBinaryMessage("DMI");

var watch = Stopwatch.StartNew();
var W1 = GenerateW1(message);
var W2 = GenerateW2(W1);
var (CompressedMessage, Z) = GetCompressedMessage(W2, message);
var DecompressedW2 = GetDecompressedW2(CompressedMessage);
watch.Stop();

Console.WriteLine("SIMPLE TEXT BWT\n");

for (int i = 0; i < W1.Count; i++)
{
    Console.WriteLine(W1[i]);
}
Console.WriteLine();

for (int i = 0; i < W2.Count; i++)
{
    Console.WriteLine(W2[i]);
}
Console.WriteLine();

Console.WriteLine(CompressedMessage + " : " + Z);
Console.WriteLine();

for (int i = 0; i < DecompressedW2.Count; i++)
{
    Console.WriteLine(DecompressedW2[i]);
}
Console.WriteLine();

Console.WriteLine($"Total time - {watch.Elapsed}");
Console.WriteLine();

Console.WriteLine("BINARY TEXT BWT\n");

var bWatch = Stopwatch.StartNew();
var bW1 = GenerateW1(binaryMessage);
var bW2 = GenerateW2(bW1);
var (bCompressedMessage, bZ) = GetCompressedMessage(bW2, binaryMessage);
var bDecompressedW2 = GetDecompressedW2(bCompressedMessage);
bWatch.Stop();

for (int i = 0; i < bW1.Count; i++)
{
    Console.WriteLine(bW1[i]);
}
Console.WriteLine();

for (int i = 0; i < bW2.Count; i++)
{
    Console.WriteLine(bW2[i]);
}
Console.WriteLine();

Console.WriteLine(bCompressedMessage + " : " + bZ);
Console.WriteLine();

for (int i = 0; i < bDecompressedW2.Count; i++)
{
    Console.WriteLine(bDecompressedW2[i]);
}
Console.WriteLine();

Console.WriteLine($"Total time - {bWatch.Elapsed}");