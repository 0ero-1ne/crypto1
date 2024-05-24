static Dictionary<char, decimal> CalculateAlphabet(string message)
{
    Dictionary<char, decimal> alphabet = new();

    foreach (var ch in message)
    {
        if (!alphabet.ContainsKey(ch)) {
            alphabet.Add(ch, (decimal)message.Count(c => c == ch) / message.Length);
        }
    }

    var sortedAlphabet = from entry
        in alphabet
        orderby entry.Value
        ascending select entry;

    return sortedAlphabet.ToDictionary(t => t.Key, t => t.Value);
}

static void PrintAlphabet(Dictionary<char, decimal> alphabet)
{
    foreach (var item in alphabet)
    {
        Console.WriteLine(item.Key + ": " + item.Value);
    }
}

static void PrintInterval(Dictionary<char, (decimal, decimal)> interval)
{
    Console.WriteLine("Interval");
    foreach (var item in interval)
    {
        Console.WriteLine($"{item.Key}: [ {item.Value.Item1} ; {item.Value.Item2} ]");
    }
}

static (decimal, decimal, char) GetCharInInterval(
    decimal value,
    Dictionary<char, (decimal, decimal)> interval
)
{
    decimal lowBoundary = 0m;
    decimal highBoundary = 0m;
    char ch = new();

    foreach (var item in interval)
    {
        if (item.Value.Item1 <= value && value <= item.Value.Item2)
        {
            lowBoundary = item.Value.Item1;
            highBoundary = item.Value.Item2;
            ch = item.Key;
        }
    }

    return (lowBoundary, highBoundary, ch);
}

static Dictionary<char, (decimal, decimal)> InitStartInterval(Dictionary<char, decimal> alphabet)
{
    Dictionary<char, (decimal, decimal)> interval = new();

    for (int i = 0; i < alphabet.Count; i++)
    {
        decimal lowBoundary = i == 0 ? 0 : interval.ElementAt(i - 1).Value.Item2;
        decimal highBoundary = i == 0 ? alphabet.ElementAt(i).Value : lowBoundary + alphabet.ElementAt(i).Value;
        interval.Add(alphabet.ElementAt(i).Key, (lowBoundary, highBoundary));
    }

    return interval;
}

static Dictionary<char, (decimal, decimal)> CalculateInterval(
    Dictionary<char, (decimal, decimal)> startInterval,
    decimal newLowBoundary, // left value of interval
    decimal newHighBoundary // right value of interval
)
{
    Dictionary<char, (decimal, decimal)> interval = new();

    for (int i = 0; i < startInterval.Count; i++)
    {
        decimal lowBoundary = newLowBoundary + (newHighBoundary - newLowBoundary) * startInterval.ElementAt(i).Value.Item1;
        decimal highBoundary = newLowBoundary + (newHighBoundary - newLowBoundary) * startInterval.ElementAt(i).Value.Item2;
        
        interval.Add(startInterval.ElementAt(i).Key, (lowBoundary, highBoundary));
    }

    return interval;
}

static decimal EncodeMessage(Dictionary<char, decimal> alphabet, string message)
{
    Dictionary<char, (decimal, decimal)> startInterval = InitStartInterval(alphabet);
    Dictionary<char, (decimal, decimal)> stepInterval = InitStartInterval(alphabet); // variable interval for every step of encoding

    for (int i = 0; i < message.Length - 1; i++) // i - is a char position to encode
    {
        decimal newLowBoundary = stepInterval[message[i]].Item1;
        decimal newHighBoundary = stepInterval[message[i]].Item2;
        stepInterval = CalculateInterval(startInterval, newLowBoundary, newHighBoundary);
    }

    return stepInterval[message[^1]].Item1;
}

static string DecodeMessage(decimal encodedMessage, Dictionary<char, decimal> alphabet, int messageLength)
{
    var startInterval = InitStartInterval(alphabet);
    decimal stepValue = encodedMessage;

    string decodedMessage = "";

    for (int i = 0; i < messageLength; i++)
    {
        (decimal newLowBoundary, decimal newHighBoundary, char ch) = GetCharInInterval(stepValue, startInterval);
        stepValue = (stepValue - newLowBoundary) / (newHighBoundary - newLowBoundary);
        decodedMessage += ch;
    }

    return decodedMessage;
}

string firstMessage = "молоко";

var alphabet = CalculateAlphabet(firstMessage);
var interval = InitStartInterval(alphabet);
PrintAlphabet(alphabet);
PrintInterval(interval);

var encodedMessage = EncodeMessage(alphabet, firstMessage);
var decodedMessage = DecodeMessage(encodedMessage, alphabet, firstMessage.Length);

Console.WriteLine(encodedMessage);
Console.WriteLine(decodedMessage);