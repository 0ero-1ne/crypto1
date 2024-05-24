using System.Text;

static int fact(int n)
{
    if (n == 0)
        return 1;

    if (n == 1)
        return 1;

    return fact(n - 1) * n;
}

static int NewtonBinom(int wt, int r) //wt - min weight of check matrix columns, r - length of check word (binary)
{
    return fact(r) / (fact(wt) * fact(r - wt));
}

static int[,] CalculateH(int k, int r) // H = (P | I), Dmin = 3
{
    int counter = 0; // number of filled columns excluding I-matrix
    int wt = 2; // weight of matrix columns, do not start with 3 (stack overflow)

    int[,] matrix = new int[r,k + r];

    while(counter < k)
    {
        int variationCounter = 0;
        int numberOfVariations = NewtonBinom(wt, r);

        for (byte i = 1; ;i++)
        {
            string binaryByte = Convert.ToString(i, 2); // i in binary
            int sum = binaryByte.Count(ch => ch.Equals('1')); // weight of i
            
            if (sum == wt)
            {
                if (binaryByte.Length < r) //add '0'
                {
                    binaryByte = binaryByte.PadLeft(r, '0');
                }

                for (int j = 0; j < r; j++)
                {
                    matrix[j, counter] = int.Parse(binaryByte[j].ToString());
                }

                counter++;
                variationCounter++;
            }

            if (counter == k)
            {
                break;
            }

            if (variationCounter == numberOfVariations)
            {
                break;
            }
        }

        wt++;
    }

    for (int i = k; i < k + r; i++)
    {
        for (int j = 0; j < r; j++)
        {
            if (j == i - k)
            {
                matrix[j, i] = 1;
            }
            else
            {
                matrix[j, i] = 0;
            }
        }
    }

    return matrix;
}

static int[] CalculateXr(int[,] H, int[] Xk, int r)
{
    int[] matrix = new int[r];

    for (int i = 0; i < r; i++)
    {
        int sum = 0;

        for (int j = 0; j < Xk.Length; j++)
        {
            if (Xk[j] == 1 && Xk[j] == H[i, j])
            {
                sum += 1;
            }

            matrix[i] = sum % 2;
        }
    }

    return matrix;
}

static int[] CalculateXn(int[] Xk, int[] Xr, int n)
{
    int[] vector = new int[n];
    int counter = 0;

    for (int i = 0; i < Xk.Length; i++)
    {
        vector[i] = Xk[i];
        counter++;
    }

    for (int i = 0; i < Xr.Length; i++)
    {
        vector[counter] = Xr[i];
        counter++;
    }

    return vector;
}

static int[] CalculateYn(int[] Xn, int errorsCount = 0)
{
    if (errorsCount == 1)
    {
        int[] vector = new int[Xn.Length];
        Random random = new();
        int index = random.Next(0, Xn.Length - 1);

        for (int i = 0; i < Xn.Length; i++)
        {
            if (i == index)
            {
                vector[i] = Xn[i] == 0 ? 1 : 0;
            }
            else
            {
                vector[i] = Xn[i];
            }
        }

        return vector;
    }

    if (errorsCount == 2)
    {
        int[] vector = new int[Xn.Length];
        Random random = new();
        int indexOne = random.Next(0, Xn.Length - 1);
        int indexTwo = random.Next(0, Xn.Length - 1);

        while (indexOne == indexTwo)
        {
            indexTwo = random.Next(0, Xn.Length - 1);
        }

        for (int i = 0; i < Xn.Length; i++)
        {
            if (i == indexOne || i == indexTwo)
            {
                vector[i] = Xn[i] == 0 ? 1 : 0;
            }
            else
            {
                vector[i] = Xn[i];
            }
        }

        return vector;
    }

    return Xn;
}

static int[] CalculateYk(int[] Yn, int k)
{
    int[] vector = new int[k];

    for(int i = 0; i < k; i++)
    {
        vector[i] = Yn[i];
    }

    return vector;
}

static int[] CalculateYr(int[] Yn, int r, int k)
{
    int[] vector = new int[r];
    int counter = 0;

    for (int i = k; i < Yn.Length; i++)
    {
        vector[counter] = Yn[i];
        counter++;
    }

    return vector;
}

static int[] CalculateYrr(int[,] H, int[] Yk, int r)
{
    int[] matrix = new int[r];

    for (int i = 0; i < r; i++)
    {
        int sum = 0;

        for (int j = 0; j < Yk.Length; j++)
        {
            if (Yk[j] == 1 && Yk[j] == H[i, j])
            {
                sum += 1;
            }

            matrix[i] = sum % 2;
        }
    }

    return matrix;
}

static int[] CalculateS(int[] Yr, int[] Yrr)
{
    int[] vector = new int[Yr.Length];

    for (int i = 0; i < Yr.Length; i++)
    {
        vector[i] = Yr[i] == Yrr[i] ? 0 : 1;
    }

    return vector;
}

static int CalculateErrorPosition(int[,] H, int[] S)
{
    int result = -1;

    for (int j = 0; j < H.GetLength(1); j++)
    {
        int sameBits = 0;
        for (int i = 0; i < S.Length; i++)
        {
            if (H[i, j] == S[i])
            {
                sameBits++;
            }
        }

        if (sameBits == S.Length)
        {
            result = j;
            break;
        }
    }

    return result;
}

static int[] CalculateE(int n, int mist)
{
    int[] vector = new int[n];

    for (int i = 0; i < n; i++)
    {
        if (i == mist)
        {
            vector[i] = 1;
        }
        else
        {
            vector[i] = 0;
        }
    }

    return vector;
}

static int[] GetFixedMessage(int[] Yn, int[] E)
{
    int[] vector = new int[E.Length];

    for (int i = 0; i < Yn.Length; i++)
    {
        vector[i] = Yn[i] ^ E[i];
    }

    return vector;
}

const string inputFilePath = @"./../../../input.txt";
//const string outputFilePath = @"./../../../output.txt";

byte[] binaryFileContent = File.ReadAllBytes(inputFilePath);

byte[] input = new byte[] { (byte)(binaryFileContent[0] ^ binaryFileContent[1]) };


int k = input.Length * 8;
int r = (int)Math.Log2(k) + 1;
int n = k + r;

int[] Xk = new int[k];
StringBuilder sb = new();

input.ToList().ForEach(item => sb.Append(Convert.ToString(item, 2).PadLeft(8, '0')));

for (int i = 0; i < k; i++)
{
    Xk[i] = int.Parse(sb[i].ToString());
}

var H = CalculateH(k, r);
var Xr = CalculateXr(H, Xk, r);
var Xn = CalculateXn(Xk, Xr, n);
var Yn = CalculateYn(Xn, 1);
var Yk = CalculateYk(Yn, k);
var Yr = CalculateYr(Yn, r, k);
var Yrr = CalculateYrr(H, Yk, r);
var S = CalculateS(Yr, Yrr);
var mist = CalculateErrorPosition(H, S);
var E = CalculateE(n, mist);
var Fixed = GetFixedMessage(Yn, E);

Console.WriteLine($"k, r, n: {k}, {r}, {n}\n");

Console.WriteLine("Xk: ");
for (int i = 0; i < Xk.Length; i++)
{
    Console.Write(Xk[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Xr: ");
for (int i = 0; i < Xr.Length; i++)
{
    Console.Write(Xr[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Xn: ");
for (int i = 0; i < Xn.Length; i++)
{
    Console.Write(Xn[i] + " ");
}

Console.WriteLine();

Console.WriteLine("Yn: ");
for (int i = 0; i < Yn.Length; i++)
{
    Console.Write(Yn[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yr: ");
for (int i = 0; i < Yr.Length; i++)
{
    Console.Write(Yr[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yrr: ");
for (int i = 0; i < Yrr.Length; i++)
{
    Console.Write(Yrr[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Fixed Yn: ");

for (int i = 0; i < Fixed.Length; i++)
{
    Console.Write(Fixed[i] + " ");
}

Console.WriteLine();
Console.WriteLine("E: ");

for (int i = 0; i < E.Length; i++)
{
    Console.Write(E[i] + " ");
}

Console.WriteLine();
Console.WriteLine("S: ");

for (int i = 0; i < S.Length; i++)
{
    Console.Write(S[i] + " ");
}

Console.WriteLine();
Console.WriteLine("H: ");

for (int i = 0; i < H.GetLength(0); i++)
{
    for (int j = 0; j < H.GetLength(1); j++)
    {
        Console.Write(H[i, j] + " ");
    }
    Console.WriteLine();
}