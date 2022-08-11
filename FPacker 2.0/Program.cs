using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPackerLibrary.Antlr.Poseidon;
using FPackerLibrary.Formats.CPP.Parse;
using FPackerLibrary.Formats.RAP.IO;
using FPackerLibrary.Formats.RAP.Models;
//using FPackerLibrary.Formats.RAP.Models.Parse;

namespace FPackerLibrary;

public static class Program {

    public static void Main(string[] args) {
        if (args.Length == 0)
        {
            args = new[]
            {
                @"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGunstest\RevGunstest\Weapons\config-7.cpp",
                @"TestFiles\TestMod\config.cpp",
                @"TestFiles\TestMod\"
            };
        }

        for (int i = 0; i < args.Length; i++)
        {            
            if (!File.Exists(args[i]) && !Directory.Exists(args[i]))
            {
                Console.WriteLine($"{DateTime.Now} File/Directory does not exist: {args[i]}");
                continue;
            }
            switch (args[i].Split(".").Last().ToLower())
            {
                case "cpp":
                    var x = ReadRap(args[i]);
                    x.BinarizeToFile(args[i].Replace("cpp", "bin"));
                    break;
                case "bin":
                    //var y = new RapBinaryParser(args[i]).ParsedRapFile;
                    break;
                case "pbo":                    
                    break;
                default:
                    new AddonPacker(args[i], $"{args[i]}.pbo");
                    break;
            }
        }
    }

    public static RapFile ReadRap(string path) {
        var lexer = new PoseidonLexer( CharStreams.fromPath(path));
        var tokens = new CommonTokenStream(lexer);

        var parser = new PoseidonParser(tokens);

        var listener = new ConfigPreParser();
        new ParseTreeWalker().Walk(listener, parser.computationalUnit());
        return listener.ConfigFile;
    }
}
