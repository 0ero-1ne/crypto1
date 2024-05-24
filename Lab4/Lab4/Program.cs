static int[] GenerateXk(int k)
{
    Random random = new((int)DateTime.Now.Ticks);
    int[] word = new int[k];

    for (int i = 0; i < word.Length; i++)
    {
        word[i] = random.Next() % 2;
    }

    return word;
} 

static int[,] Generate2DMatrix(int[] Xk, int rows, int columns)
{
    int[,] matrix = new int[rows, columns];
    
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < columns; j++)
        {
            matrix[i, j] = Xk[i * columns + j];
        }
    }

    return matrix;
}

/*static int[,,] Generate3DMatrix(int[] Xk, int rows, int columns, int layers)
{
    int[,,] matrix = new int[layers, rows, columns];

    for (int i = 0; i < layers; i++)
    {
        for (int j = 0; j < rows; j++)
        {
            for (int k = 0; k < columns; k++)
            {
                matrix[i, j, k] = Xk[i * (rows * columns) + j * columns + k];
            }
        }
    }

    return matrix;
}
*/

static int[] CalculateHorizontalParitets(int[,] matrix)
{
    int[] result = new int[matrix.GetLength(0)];

    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        int sum = 0;

        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            sum += matrix[i, j];
        }

        result[i] = sum % 2;
    }

    return result;
}

static int[] CalculateVerticalParitets(int[,] matrix)
{
    int[] result = new int[matrix.GetLength(1)];

    for (int i = 0; i < matrix.GetLength(1); i++)
    {
        int sum = 0;

        for (int j = 0; j < matrix.GetLength(0); j++)
        {
            sum += matrix[j, i];
        }

        result[i] = sum % 2;
    }

    return result;
}

static int[] CalculateXn(int[] word, int[] Xh, int[] Xv)
{
    return word.Concat(Xh).Concat(Xv).ToArray();
}

static int[] CalculateYn(int[] Xn, int errors = 0)
{
    int[] result = new int[Xn.Length];
    List<int> errorsPositions = new();

    for (int i = 0; i < errors; i++)
    {
        Random random = new();
        int errorPosition = random.Next(0, Xn.Length - 1);

        while (errorsPositions.Contains(errorPosition))
        {
            errorPosition = random.Next(0, Xn.Length - 1);
        }
        errorsPositions.Add(errorPosition);
    }

    for (int i = 0; i < result.Length; i++)
    {
        if (errorsPositions.Contains(i))
        {
            result[i] = Xn[i] ^ 1;
        }
        else
        {
            result[i] = Xn[i];
        }
    }

    return result;
}

static int[] CalculateYk(int[] Yn, int k)
{
    return Yn.ToList().Take(k).ToArray();
}

static int[] CalculateYh(int[] Yn, int k, int k1)
{
    return Yn.ToList().Skip(k).Take(k1).ToArray();
}

static int[] CalculateYv(int[] Yn, int k, int k1, int k2)
{
    return Yn.ToList().Skip(k + k1).Take(k2).ToArray();
}

(int, int) CalculateErrorPosition(int[] Yh, int[] Yhh, int[] Yv, int[] Yvv)
{
    int x = -1;
    int y = -1;

    for (int i = 0; i < Yh.Length; i++)
        if (Yh[i] != Yhh[i])
            x = i;

    for (int i = 0; i < Yv.Length; i++)
        if (Yv[i] != Yvv[i])
            y = i;

    return (x, y);
}

int[] CalculateFixedYn(int[] Yk, int[] Yh, int[] Yv, (int, int) error)
{
    int[] Yn = Yk.Concat(Yh).Concat(Yv).ToArray();

    if (error.Item1 != -1 && error.Item2 == -1)
    {
        int position = Yk.Length + error.Item1;
        Yn[position] = Yn[position] == 1 ? 0 : 1;
    }

    if (error.Item1 == -1 && error.Item2 != -1)
    {
        int position = Yk.Length + Yh.Length + error.Item2;
        Yn[position] = Yn[position] == 1 ? 0 : 1;
    }

    if (error.Item1 != -1 && error.Item2 != -1)
    {
        int position = error.Item1 * Yv.Length + error.Item2;
        Yn[position] = Yn[position] == 1 ? 0 : 1;
    }

    return Yn;
}

Console.WriteLine("ITERATIVE CODING\n");

int k = 20;
int[] k1s = { 5, 2 };
int[] k2s = { 4, 10 };
int k1 = k1s[1];
int k2 = k2s[1];

var Xk = GenerateXk(k);
var XkMatrix = Generate2DMatrix(Xk, k1, k2);
var Xh = CalculateHorizontalParitets(XkMatrix);
var Xv = CalculateVerticalParitets(XkMatrix);
var Xn = CalculateXn(Xk, Xh, Xv);

var Yn = CalculateYn(Xn, 2);
var Yk = CalculateYk(Yn, k);
var YkMatrix = Generate2DMatrix(Yk, k1, k2);
var Yh = CalculateYh(Yn, k, k1);
var Yv = CalculateYv(Yn, k, k1, k2);
var Yhh = CalculateHorizontalParitets(YkMatrix);
var Yvv = CalculateVerticalParitets(YkMatrix);
var point = CalculateErrorPosition(Yh, Yhh, Yv, Yvv);
var fixedYn = CalculateFixedYn(Yk, Yh, Yv, point);

Console.WriteLine("Xk:");
for (int i = 0; i < Xk.Length; i++)
{
    Console.Write(Xk[i] + " ");
}
Console.WriteLine();


Console.WriteLine("XkMatrix:");
for (int i = 0; i < XkMatrix.GetLength(0); i++)
{
    for (int j = 0; j < XkMatrix.GetLength(1); j++)
    {
        Console.Write(XkMatrix[i, j] + " ");
    }
    Console.WriteLine();
}
Console.WriteLine();

Console.WriteLine("Xh:");
for (int i = 0; i < Xh.Length; i++)
{
    Console.Write(Xh[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Xv:");
for (int i = 0; i < Xv.Length; i++)
{
    Console.Write(Xv[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Xn:");
for (int i = 0; i < Xn.Length; i++)
{
    Console.Write(Xn[i] + " ");
}
Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Yk:");
for (int i = 0; i < Yk.Length; i++)
{
    Console.Write(Yk[i] + " ");
}
Console.WriteLine();

Console.WriteLine("YkMatrix:");
for (int i = 0; i < YkMatrix.GetLength(0); i++)
{
    for (int j = 0; j < YkMatrix.GetLength(1); j++)
    {
        Console.Write(YkMatrix[i, j] + " ");
    }
    Console.WriteLine();
}
Console.WriteLine();

Console.WriteLine("Yh:");
for (int i = 0; i < Yh.Length; i++)
{
    Console.Write(Yh[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yv:");
for (int i = 0; i < Yv.Length; i++)
{
    Console.Write(Yv[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yhh:");
for (int i = 0; i < Yhh.Length; i++)
{
    Console.Write(Yhh[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yvv:");
for (int i = 0; i < Yvv.Length; i++)
{
    Console.Write(Yvv[i] + " ");
}
Console.WriteLine();

Console.WriteLine($"Error: ({point.Item1};{point.Item2})");

Console.WriteLine("Xn:");
for (int i = 0; i < Xn.Length; i++)
{
    Console.Write(Xn[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Yn:");
for (int i = 0; i < Yn.Length; i++)
{
    Console.Write(Yn[i] + " ");
}
Console.WriteLine();

Console.WriteLine("Fixed Yn:");
for (int i = 0; i < fixedYn.Length; i++)
{
    Console.Write(fixedYn[i] + " ");
}
Console.WriteLine();