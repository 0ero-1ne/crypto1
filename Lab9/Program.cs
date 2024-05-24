// code can read unicode chars

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

static string GetBinaryMessage(string message)
{
    var result = string.Join(
        String.Empty,
        Encoding.UTF8.GetBytes(message).Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0'))
    );

    return result;
}

static string EncodeMessage(string message, int n1, int n2)
{
    int skip = n2;

    string dictionary = new('0', n1);
    string buffer = new(message.Take(skip).ToArray());

    string encodedMessage = "";
    int i = 0; //matched symbols

    while (buffer.Length != 0)
    {
        string str = buffer[i].ToString();

        while (dictionary.IndexOf(str) != -1) {
            i++;
            str += new string(buffer.Skip(i).Take(1).ToArray());

            if (new string(buffer.Skip(i).Take(1).ToArray()) == "") {
                i--;
                break;
            }
        }
        
        str = new(str.Take(i).ToArray());

        var p = !dictionary.Contains(str) ? 
            0.ToString().PadLeft(n1.ToString().Length, '0') : 
            dictionary.IndexOf(str).ToString().PadLeft(n1.ToString().Length, '0'); // match position

        var offset = i.ToString().PadLeft(n2.ToString().Length, '0');

        var s = buffer[i].ToString(); // [offset + 1] char
        encodedMessage += p + offset + s;

        dictionary = new string(dictionary.Skip(i + 1).ToArray()) + new string(buffer.Take(i + 1).ToArray());
        buffer = new string(buffer.Skip(i + 1).ToArray()) + new string(message.Skip(skip).Take(i + 1).ToArray());
        skip += i + 1;
        i = 0;
    }

    return encodedMessage;
}

static string DecodeMessage(string encodedMessage, int n1, int n2)
{
    var decodedMessage = "";
    var buffer = new string('0', n2);
    var skip = 0;

    while (skip != encodedMessage.Length) {
        var part = new string(encodedMessage.Skip(skip).Take(n1.ToString().Length + n2.ToString().Length + 1).ToArray());
        var str = "";

        var p = int.Parse(part[..n1.ToString().Length]);
        var q = int.Parse(part.Substring(n1.ToString().Length, n2.ToString().Length));
        var s = part[^1];

        if (p == 0 && q == 0) {
            str = s.ToString();
        }
        else {
            str = new string(buffer.Skip(p).Take(q).ToArray()) + s;
        }

        decodedMessage += str;
        buffer = new string(buffer.Skip(q + 1).ToArray()) + str;

        skip += n1.ToString().Length + n2.ToString().Length + 1;
    }

    return decodedMessage;
}

static double ClaculateR1(string message, string encodedMessage)
{
    var averageCodeLength = Math.Ceiling(Math.Log2(2)); // binary alphabet has 2 chars
    return encodedMessage.Length / (averageCodeLength * message.Length);
}

static double ClaculateR2(string message, string encodedMessage)
{
    var averageCodeLength = Math.Ceiling(Math.Log2(2)); // binary alphabet has 2 chars
    return (averageCodeLength * message.Length - encodedMessage.Length) / (averageCodeLength * message.Length);
}

static string GetMessageFromBinary(string message)
{
    var result = Encoding.UTF8.GetString(
        Regex.Split(message, "(.{8})")
        .Where(binary => !String.IsNullOrEmpty(binary))
        .Select(binary => Convert.ToByte(binary, 2))
        .ToArray()
    );

    return result;
}

int n1 = 16;
int n2 = 16;

var fileName = @"input.txt";
var message = File.ReadAllText(fileName);
var binaryMessage = GetBinaryMessage(message);

var watch = Stopwatch.StartNew();
var encodedMessage = EncodeMessage(binaryMessage, n1, n2);
var decodedMessage = DecodeMessage(encodedMessage, n1, n2);
var messageFromBinary = GetMessageFromBinary(decodedMessage);
watch.Stop();

Console.WriteLine(encodedMessage);
Console.WriteLine("Are same? " + (message == messageFromBinary));

var R1 = ClaculateR1(binaryMessage, encodedMessage);
var R2 = ClaculateR2(binaryMessage, encodedMessage);

Console.WriteLine($"R1 - {R1}");
Console.WriteLine($"R2 - {R2}");

Console.WriteLine($"Total time - {watch.Elapsed}");