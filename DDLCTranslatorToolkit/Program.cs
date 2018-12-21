using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDLCTranslatorToolkit
{
    class Program
    {
        static void Main(string[] args)
        {
            PC.Export ExportPC = new PC.Export();
            PC.Import ImportPC = new PC.Import();
            Console.WriteLine("Doki Doki Literature Club Translation Toolkit — A easy toolkit for Doki Doki Literature Club fantranslations by Darkmet98.\nThanks to Pleonex for Yarhl libraries.\nVersion: 1.0");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("\nUsage: MetalMax3 TXT2PO.exe <-Corpse1> <-PC/-3DS/-PSP> <-export/-exportwithtrans/-import>");
                Console.WriteLine("Export TXT to PO: MetalMax3 TXT2PO.exe -export C01.MSG.txt");
                Console.WriteLine("Export PO to TXT: MetalMax3 TXT2PO.exe -import C01.MSG.po");
                Console.WriteLine("Export PO to TXT and import the translations: MetalMax3 TXT2PO.exe -exportwithtrans 'jp\\C01.MSG.txt' 'en\\C01.MSG.txt'");
                return;
            }
            switch (args[0])
            {
                case "-export":
                    ExportPC.Exports(args[1]);
                    break;
                    case "-import":
                    ImportPC.Imports(args[1]);
                    break;
            }
        }
    }
}
