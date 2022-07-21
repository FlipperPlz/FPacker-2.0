using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FPacker.Antlr.Poseidon;
using FPacker.Formats.CPP.Parse;
using FPacker.Formats.RAP.IO;
using FPacker.Formats.RAP.Models;
using FPacker.Formats.RAP.Models.Parse;

namespace FPacker;

public static class Program {

    public static void Main() {
        var x = ReadRap(
            @"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGunstest\RevGunstest\Weapons\config-7.cpp");
        x.BinarizeToFile(
            @"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGunstest\RevGunstest\Weapons\fuckk");
        var y = new RapBinaryParser(
            @"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGunstest\RevGunstest\Weapons\fuckk").ParsedRapFile;

        //new AddonPacker(@"C:\Program Files (x86)\Steam\steamapps\workshop\content\221100\2273590683\Addons\RevGuns\RevGuns", @"C:\Users\dev\Desktop\PlasmaMod.pbo");
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
