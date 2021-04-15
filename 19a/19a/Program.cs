using System;
using System.IO;
using System.Text.Json;

namespace _19a
{
    class Program
    {
        static void Main(string[] args)
        {
            string data = File.ReadAllText("countries.json");
            JsonDocument doc = JsonDocument.Parse(data);
            Console.WriteLine(doc.RootElement[0].GetProperty("name"));
            Console.WriteLine(doc.RootElement[1].GetProperty("name"));
            Console.WriteLine(doc.RootElement[2].GetProperty("flag"));
        }
    }
}
