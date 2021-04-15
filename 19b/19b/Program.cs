using System;
using System.IO;
using System.Text.Json;

namespace _19b
{
    class Program
    {
        static void Main(string[] args)
        {
            string data = File.ReadAllText("countries.json");
            JsonDocument doc = JsonDocument.Parse(data);
            EnumerateElement(doc.RootElement);
        }

        static void EnumerateElement(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array)
            {
                var enumerator = el.EnumerateArray();
                EnumerateElement(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    EnumerateElement(enumerator.Current);
                }
            } else if (el.ValueKind == JsonValueKind.Object)
            {
                var enumerator = el.EnumerateObject();
                enumerator.MoveNext();
                Console.WriteLine(enumerator.Current.Name + ": " + enumerator.Current.Value);
                while (enumerator.MoveNext())
                {
                    Console.WriteLine(enumerator.Current.Name + ": " + enumerator.Current.Value);
                }
            } else if (el.ValueKind == JsonValueKind.String)
            {
                Console.WriteLine(el.GetString());
            }
        }
    }
}