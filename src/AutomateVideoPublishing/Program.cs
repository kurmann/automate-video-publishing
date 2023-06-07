using System;
using TagLib;

class Program
{
    static void Main(string[] args)
    {
        var filePath = "";
        var file = TagLib.File.Create(filePath);

        Console.WriteLine($"Title: {file.Tag.Title}");
        Console.WriteLine($"Description: {file.Tag.Description}");
    }
}
