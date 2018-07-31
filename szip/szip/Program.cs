using System;
using System.IO;
using System.IO.Compression;

namespace szip
{
    class Program
    {
        enum OpType {
            zip,
            unzip,
        }
        enum ZipType {
            lz4,
            zip,
            gzip,
        }
        private const string HintString = @"-optype [zip压缩(默认) unzip解压]
-zip 类型 [lz4 zip gzip] (必须)
-input 文件路径 (必须)
-output 导出文件路径 (必须)
";
        static void Main(string[] args) {
            try {
                CommandLine command = CommandLine.Parse(args);
                Console.WriteLine(Environment.CurrentDirectory);
                var op = (OpType)Enum.Parse(typeof(OpType), command.GetValue("-optype") ?? "zip");
                var zip = (ZipType)Enum.Parse(typeof(ZipType), command.GetValue("-zip"));
                var input = command.GetValue("-input");
                var output = command.GetValue("-output");
                if (!File.Exists(input)) {
                    throw new Exception("文件不存在 : " + input);
                }
                if (zip == ZipType.lz4) {
                    lz4(op, input, output);
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.WriteLine(HintString);
            }
            Console.ReadKey();
        }
        static void lz4(OpType type, string input, string output) {

        }
    }
}
