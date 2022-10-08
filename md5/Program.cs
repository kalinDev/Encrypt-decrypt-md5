using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

WriteMenu();
static void WriteMenu()
{
    Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");
    Console.WriteLine("1 - Para gerar as senhas encryptadas");
    Console.WriteLine("2 - Para desencryptar uma senha");
    Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");

    var option = Console.ReadLine();

    if (option == "1")
    {
        var before0 = GC.CollectionCount(0);
        var before1 = GC.CollectionCount(1);
        var before2 = GC.CollectionCount(2);

        var sw = new Stopwatch();
        
        sw.Start();
        Encrypt();
        sw.Stop();
        
        Console.WriteLine($"Tempo: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Gen0: {GC.CollectionCount(0) - before0}");
        Console.WriteLine($"Gen1: {GC.CollectionCount(1) - before1}");
        Console.WriteLine($"Gen2: {GC.CollectionCount(2) - before2}");
        Console.WriteLine($"Memoria: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} mb");
    }
    else if (option == "2")
    {
        Console.WriteLine("Digiter a senha encryptada:");
        var passwordEncypted = Console.ReadLine();
        
        if (string.IsNullOrEmpty(passwordEncypted)) return;
        Decrypt(passwordEncypted);
    }
    else WriteMenu();
    
}
static void Encrypt()
{
    using var fs = File.OpenRead(@"..\..\..\senhas.txt");
    using var reader = new StreamReader(fs);
    using var writer = new StreamWriter(@"..\..\..\senhasEncriptadas.txt");
    using var md5 = MD5.Create();

    string line;
    while ((line = reader.ReadLine()) != null)
    {
        writer.WriteLine(GetMD5Hash(line, md5));
    }
}

static void Decrypt(string passwordEncrypted)
{
    int count = 0;
    string line;
    using var fs = File.OpenRead(@"..\..\..\senhasEncriptadas.txt");
    using var reader = new StreamReader(fs);
    while ((line = reader.ReadLine()) != null)
    {
        if (line == passwordEncrypted)
        {
            Console.WriteLine($"\nA senha desencriptada é: {GetPasswordByLine(count)}");
            break;
        };
        count++;
    }

}

static string GetPasswordByLine(int linePosition)
{
    return File.ReadLines(@"..\..\..\senhas.txt").Skip(linePosition).Take(1).First();
}

static string GetMD5Hash(string input, MD5 md5)
{
    var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
    var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    return hash;
}