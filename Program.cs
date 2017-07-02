using System;
using System.Collections.Generic;

class Program {
    private static void Main(string[] args) {
        Console.WriteLine("object serialization:");
        var json = Json.ToJson(new { Name = "james", Age = 35, Married = true });
        Console.WriteLine("\t{0}", json);
        Console.WriteLine("json parse:");
        var jobj = (Dictionary<string, object>)Json.FromJson(json);
        foreach (var key in jobj.Keys) {
            Console.WriteLine("\t{0}:{1}", key, jobj[key]);
        }
        Console.ReadLine();
    }
}
