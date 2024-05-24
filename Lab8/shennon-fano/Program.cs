static (Dictionary<string, (int, string)>, Dictionary<string, (int, string)>) DivideAlphabet(Dictionary<string, (int, string)> alphabet)
{
    var divider = 1;
    var realDivider = 1;
    int minSum = int.MaxValue;

    while (true)
    {
        var leftSum = alphabet.Take(divider).Sum(item => item.Value.Item1);
        var rightSum = alphabet.Skip(divider).Sum(item => item.Value.Item1);

        // Why ABS?
        // The formula is:
        // |Sum(P(Ai)) - Sum(P(Bi))| must be min
        // A - left part of alphabet, B - right part of alphabet
        var difference = Math.Abs(leftSum - rightSum);

        if (difference < minSum)
        {
            minSum = difference;
            realDivider = divider;
        }

        if (alphabet.Count - divider == 1) {
            break;
        }

        divider++;
    }

    var leftAlphabet = alphabet.Take(realDivider).ToDictionary(item => item.Key, item => item.Value);
    var rightAlphabet = alphabet.Skip(realDivider).ToDictionary(item => item.Key, item => item.Value);

    return (leftAlphabet, rightAlphabet);
}

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
    Dictionary<string, (int, string)> partOfAlphabet,
    Dictionary<string, (int, string)> alphabet
)
{
    var (leftAlphabet, rightAlphabet) = DivideAlphabet(partOfAlphabet);

    foreach (var item in leftAlphabet) {
        alphabet[item.Key] = (alphabet[item.Key].Item1, alphabet[item.Key].Item2 + "1");
    }

    foreach (var item in rightAlphabet) {
        alphabet[item.Key] = (alphabet[item.Key].Item1, alphabet[item.Key].Item2 + "0");
    }

    if (leftAlphabet.Count > 1) CalculateCodes(leftAlphabet, alphabet);
    if (rightAlphabet.Count > 1) CalculateCodes(rightAlphabet, alphabet);
}

static void PrintAlphabet(Dictionary<string, (int, string)> alphabet)
{
    foreach (var item in alphabet)
    {
        Console.WriteLine(item.Key + ": " + item.Value.Item1 + " " + item.Value.Item2);
    }
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
)
{
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

var message = """A1!@#$%^&*()_+-=[]{}|;':",./<>?Б2А3$%^&*()_+-=[]{}|;':",./<>?В4#@%^&*()_+-=[]{}|;':",./<>?Г5@#%^&*()_+-=[]{}|;':",./<>?Д6%@#%^&*()_+-=[]{}|;':",./<>?Е7^@#%^&*()_+-=[]{}|;':",./<>?Ё8&@#%^&*()_+-=[]{}|;':",./<>?Ж9*@#%^&*()_+-=[]{}|;':",./<>?З0(^@#%^&*()_+-=[]{}|;':",./<>?И1)@#%^&*()_+-=[]{}|;':",./<>?Й2_@#%^&*()_+-=[]{}|;':",./<>?К3+@#%^&*()_+-=[]{}|;':",./<>?Л4-@#%^&*()_+-=[]{}|;':",./<>?М5=@#%^&*()_+-=[]{}|;':",./<>?Н6[@#%^&*()_+-=[]{}|;':",./<>?О7]@#%^&*()_+-=[]{}|;':",./<>?П8{@#%^&*()_+-=[]{}|;':",./<>?Р9}@#%^&*()_+-=[]{}|;':",./<>?С0@#%^&*()_+-=[]{}|;':",./<>?Т1#@#%^&*()_+-=[]{}|;':",./<>?У2$@#%^&*()_+-=[]{}|;':",./<>?Ф3%@#%^&*()_+-=[]{}|;':",./<>?Х4^@#%^&*()_+-=[]{}|;':",./<>?Ц5&@#%^&*()_+-=[]{}|;':",./<>?Ч6*@#%^&*()_+-=[]{}|;':",./<>?Ш7(^@#%^&*()_+-=[]{}|;':",./<>?Щ8)@#%^&*()_+-=[]{}|;':",./<>?Ъ9+@#%^&*()_+-=[]{}|;':",./<>?Ы0-@#%^&*()_+-=[]{}|;':",./<>?Ь1=@#%^&*()_+-=[]{}|;':",./<>?Э2_@#%^&*()_+-=[]{}|;':",./<>?Ю3+@#%^&*()_+-=[]{}|;':",./<>?Я4-@#%^&*()_+-=[]{}|;':",./<>?""";
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