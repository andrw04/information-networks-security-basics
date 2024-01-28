using FileEncryptor;

string path = @"F:\test_input.txt";

var fr = new FileReader(path);

await foreach (var line in fr.GetDataAsync())
{
    Console.WriteLine(line);
}