static Dictionary<string, (int, string)> CalculateAlphabet(string message)
{
    Dictionary<string, (int, string)> alphabet = new();

    foreach (var ch in message)
    {
        if (!alphabet.ContainsKey(ch.ToString())) {
            var newItem = (message.Count(s => s == ch), "");
            alphabet.Add(ch.ToString(), newItem);
        }
    }

    var sortedAlphabet = from entry
        in alphabet
        orderby entry.Value
        descending select entry;

    return sortedAlphabet.ToDictionary(t => t.Key, t => t.Value);
}

static void CalculateCodes(
    Dictionary<string, (int, string)> alphabet,
    Dictionary<string, (int, string)> globalAlphabet
) {
    if (alphabet.Count == 1) {
        return;
    }

    Dictionary<string, (int, string)> buffer = new(alphabet);
    var firstMin = GetMinElementKey(buffer);
    buffer.Remove(firstMin.Key);
    var secondMin = GetMinElementKey(buffer);
    buffer.Remove(secondMin.Key);

    foreach (var ch in firstMin.Key) {
        globalAlphabet[ch.ToString()] = (globalAlphabet[ch.ToString()].Item1, "0" + globalAlphabet[ch.ToString()].Item2);
    }

    foreach (var ch in secondMin.Key) {
        globalAlphabet[ch.ToString()] = (globalAlphabet[ch.ToString()].Item1, "1" + globalAlphabet[ch.ToString()].Item2);
    }

    buffer.Add(secondMin.Key + firstMin.Key, (firstMin.Value.Item1 + secondMin.Value.Item1, ""));

    CalculateCodes(buffer, globalAlphabet);
}

static KeyValuePair<string, (int, string)> GetMinElementKey(Dictionary<string, (int, string)> alphabet)
{
    var minElement = alphabet.First();

    foreach (var item in alphabet) {
        if (item.Value.Item1 <= minElement.Value.Item1) {
            minElement = item;
        }
    }
    
    return minElement;
}

static (int, int) GetMinAndMaxCodesLength(Dictionary<string, (int, string)> alphabet)
{
    var min = alphabet.Min(item => item.Value.Item2.Length);
    var max = alphabet.Max(item => item.Value.Item2.Length);

    return (min, max);
}

static string GetEncodedMessage(
    Dictionary<string, (int, string)> alphabet,
    string message
)
{
    var encodedMessage = "";
    foreach (var ch in message) {
        encodedMessage += alphabet[ch.ToString()].Item2;
    }
    return encodedMessage;
}

static string GetDecodedMessage(
    Dictionary<string, (int, string)> alphabet,
    string encodedMessage,
    int minCodeLength
) {
    var decodedMessage = "";
    var skip = 0;
    var length = minCodeLength;
    
    while (true)
    {
        if (skip == encodedMessage.Length) {
            break;
        }

        var buf = encodedMessage.Substring(skip, length);
        var isCharExists = false;
        
        alphabet.ToList().ForEach(item => {
            if (item.Value.Item2 == buf) {
                isCharExists = true;
            }
        });

        if (!isCharExists) {
            length++;
            continue;
        }

        var ch = alphabet.ToList().Single(item => item.Value.Item2 == buf);
        decodedMessage += ch.Key;

        skip += buf.Length;
        length = minCodeLength;
    }

    return decodedMessage;
}

static double ClaculateR1(string message, string encodedMessage, int alphabetSize)
{
    var averageCodeLength = Math.Ceiling(Math.Log2(alphabetSize));
    return encodedMessage.Length / (averageCodeLength * message.Length);
}

static double ClaculateR2(string message, string encodedMessage, int alphabetSize)
{
    var averageCodeLength = Math.Ceiling(Math.Log2(alphabetSize));
    return (averageCodeLength * message.Length - encodedMessage.Length) / (averageCodeLength * message.Length);
}

static void PrintAlphabet(Dictionary<string, (int, string)> alphabet)
{
    foreach (var item in alphabet)
    {
        Console.WriteLine(item.Key + ": " + item.Value.Item1 + " " + item.Value.Item2);
    }
}

var message = """` === `A1!@#$%^&*()_+-=[]{}|;':",./<>?Б2А3$%^&*()_+-=[]{}|;':",./<>?В4#@%^&*()_+-=[]{}|;':",./<>?Г5@#%^&*()_+-=[]{}|;':",./<>?Д6%@#%^&*()_+-=[]{}|;':",./<>?Е7^@#%^&*()_+-=[]{}|;':",./<>?Ё8&@#%^&*()_+-=[]{}|;':",./<>?Ж9*@#%^&*()_+-=[]{}|;':",./<>?З0(^@#%^&*()_+-=[]{}|;':",./<>?И1)@#%^&*()_+-=[]{}|;':",./<>?Й2_@#%^&*()_+-=[]{}|;':",./<>?К3+@#%^&*()_+-=[]{}|;':",./<>?Л4-@#%^&*()_+-=[]{}|;':",./<>?М5=@#%^&*()_+-=[]{}|;':",./<>?Н6[@#%^&*()_+-=[]{}|;':",./<>?О7]@#%^&*()_+-=[]{}|;':",./<>?П8{@#%^&*()_+-=[]{}|;':",./<>?Р9}@#%^&*()_+-=[]{}|;':",./<>?С0@#%^&*()_+-=[]{}|;':",./<>?Т1#@#%^&*()_+-=[]{}|;':",./<>?У2$@#%^&*()_+-=[]{}|;':",./<>?Ф3%@#%^&*()_+-=[]{}|;':",./<>?Х4^@#%^&*()_+-=[]{}|;':",./<>?Ц5&@#%^&*()_+-=[]{}|;':",./<>?Ч6*@#%^&*()_+-=[]{}|;':",./<>?Ш7(^@#%^&*()_+-=[]{}|;':",./<>?Щ8)@#%^&*()_+-=[]{}|;':",./<>?Ъ9+@#%^&*()_+-=[]{}|;':",./<>?Ы0-@#%^&*()_+-=[]{}|;':",./<>?Ь1=@#%^&*()_+-=[]{}|;':",./<>?Э2_@#%^&*()_+-=[]{}|;':",./<>?Ю3+@#%^&*()_+-=[]{}|;':",./<>?Я4-@#%^&*()_+-=[]{}|;':",./<>?`""";
var alphabet = CalculateAlphabet(message);
CalculateCodes(alphabet, alphabet);
var (minCodeLength, maxCodeLength) = GetMinAndMaxCodesLength(alphabet);
var encodedMessage = GetEncodedMessage(alphabet, message);
var decodedMessage = GetDecodedMessage(alphabet, encodedMessage, minCodeLength);
var R1 = ClaculateR1(message, encodedMessage, alphabet.Count);
var R2 = ClaculateR2(message, encodedMessage, alphabet.Count);

Console.WriteLine($"Message - {message}");
PrintAlphabet(alphabet);
Console.WriteLine($"Min code length - {minCodeLength}");
Console.WriteLine($"Max code length - {maxCodeLength}");
Console.WriteLine($"Encoded message - {encodedMessage}");
Console.WriteLine($"Decoded message - {decodedMessage}");
Console.WriteLine($"Are messages same - {message == decodedMessage}");
Console.WriteLine($"R1 - {R1}");
Console.WriteLine($"R2 - {R2}");