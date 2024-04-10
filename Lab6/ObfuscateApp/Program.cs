using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ObfuscateApp;
using System.Text;
using System.Text.RegularExpressions;


string inputDir = "F:\\BSUIR\\6\\ISOB\\Practice\\Labs\\Lab4-5\\SecureApp";
string outputDir = "F:\\BSUIR\\6\\ISOB\\Practice\\Labs\\Lab6\\Output";

Dictionary<string, string> dirs = new Dictionary<string, string>()
{
    { $"{inputDir}\\Converters", $"{outputDir}\\Converters" },
    { $"{inputDir}\\Models", $"{outputDir}\\Models" },
    { $"{inputDir}\\Protection", $"{outputDir}\\Protection" },
    { $"{inputDir}\\Repositories", $"{outputDir}\\Repositories" },
    { $"{inputDir}\\ViewModels", $"{outputDir}\\ViewModels" }
};

await StringDictionary.LoadWords();


foreach (var dir in dirs)
{
    if (!Directory.Exists(dir.Value))
    {
        Directory.CreateDirectory(dir.Value);
    }
}


foreach (var dir in dirs)
{
    if (Directory.Exists(dir.Key))
    {
        var files = Directory.GetFiles(dir.Key);
        foreach (var input in files)
        {
            var output = dir.Value + $"\\{Path.GetFileName(input)}";

            var newCode = AddUnreachableCode(input);
            File.WriteAllText(output, newCode);
            newCode = AddDeadCode(output);
            File.WriteAllText(output, newCode);
            string code = File.ReadAllText(output);
            newCode = await RenameVariables(code);
            newCode = AddRandomSpaceAndTabs(newCode);

            File.WriteAllText(output, newCode);
        }
    }
}


static async Task<string> RenameVariables(string code)
{
    SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
    var root = tree.GetRoot();

    var rewriter = new VariableRenamer();
    var newRoot = rewriter.Visit(root);

    return newRoot.ToFullString();
}

static string AddUnreachableCode(string path)
{
    StringBuilder sb = new StringBuilder();
    using StreamReader sr = new StreamReader(path);
    while (sr.Peek() >= 0)
    {
        string line = sr.ReadLine();

        sb.Append(line);
        sb.Append('\n');

        if (line.Trim() == "break;" || line.Trim() == "return;")
        {
            sb.Append(GenerateRandomInitialization());
        }
    }

    return sb.ToString();
}


static string AddDeadCode(string path) // создает избыточные переменные разных типов данных
{
    StringBuilder sb = new StringBuilder();
    using StreamReader sr = new StreamReader(path);
    while (sr.Peek() >= 0 )
    {
        string line = sr.ReadLine();

        if (Regex.IsMatch(line.Trim(), @"^if(.+)$"))
        {
            Random random = new Random();
            var randomValue = random.Next() % 10;
            if (randomValue < 3)
                sb.AppendLine($"string {StringDictionary.GetRandom()} = \"{StringDictionary.GetRandom()}\";");
            else if (randomValue < 6)
                sb.AppendLine($"int {StringDictionary.GetRandom()} = {random.Next(123)} + {random.Next(321)} / {random.Next(111)};");
            else
                sb.AppendLine($"double {StringDictionary.GetRandom()} = 123 / 888 * 555;");
            sr.Peek();
        }
        sb.AppendLine(line);
    }

    return sb.ToString();
}

static List<string> GenerateRandomInitializations(int count)
{
    List<string> initializations = new List<string>();

    for (int i = 0; i < count; i++)
    {
        string initialization = GenerateRandomInitialization();
        initializations.Add(initialization);
    }

    return initializations;
}

static string GenerateRandomInitialization()
{
    Random random = new Random();
    Type[] types = { typeof(int), typeof(double), typeof(float), typeof(string), typeof(bool) };

    Type type = types[random.Next(types.Length)];
    string name = StringDictionary.GetRandom();
    string value = GenerateRandomValue(type);

    string initialization = $"{type.Name} {name} = {value};";

    return initialization;
}

static string GenerateRandomValue(Type type)
{
    Random random = new Random();

    if (type == typeof(int))
        return random.Next().ToString();
    else if (type == typeof(double))
        return random.NextDouble().ToString();
    else if (type == typeof(float))
        return ((float)random.NextDouble()).ToString();
    else if (type == typeof(string))
        return $"\"{StringDictionary.GetRandom()}\"";
    else if (type == typeof(bool))
        return (random.Next() % 2 == 0).ToString();
    else
        throw new ArgumentOutOfRangeException(nameof(type));
}

static string AddRandomSpaceAndTabs(string code)
{
    Regex regex = new Regex(@"[ \t]+");
    Random random = new Random();

    return regex.Replace(code, match =>
    {
        int numSpaces = random.Next(1, 50);
        int numTabs = random.Next(1, 10);
        return new string(' ', numSpaces) + new string('\t', numTabs);
    });
}