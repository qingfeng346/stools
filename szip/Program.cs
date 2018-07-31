using System;
using System.IO;
using System.IO.Compression;
using LZ4;
namespace szip {
    class Program {
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
        private const int Length = 8192;
        private static byte[] bytes = new byte[Length];
        private static int readed = 0;
        static void Main(string[] args) {
            try {
                CommandLine command = CommandLine.Parse(args);
                var op = (OpType)Enum.Parse(typeof(OpType), command.GetValue("-op") ?? "zip");
                var zip = (ZipType)Enum.Parse(typeof(ZipType), command.GetValue("-zip"));
                var input = Path.Combine(Environment.CurrentDirectory, command.GetValue("-input"));
                var output = Path.Combine(Environment.CurrentDirectory, command.GetValue("-output"));
                if (!File.Exists(input)) {
                    throw new Exception("文件不存在 : " + input);
                }
                var outPath = Path.GetDirectoryName(output);
                if (!Directory.Exists(outPath)) {
                    Directory.CreateDirectory(outPath);
                }
                long inputLength = new FileInfo(input).Length;
                if (zip == ZipType.lz4) {
                    lz4(op, input, output);
                } else if (zip == ZipType.gzip) {
                    gzip(op, input, output);
                }
                long outputLength = new FileInfo(output).Length;
                if (op == OpType.zip) {
                    Console.WriteLine("压缩完成 " + GetLength(inputLength) + " > " + GetLength(outputLength) + "  压缩率 : " + (outputLength * 100 / inputLength) + "%");
                } else if (op == OpType.unzip) {
                    Console.WriteLine("解压完成 " + GetLength(inputLength) + " > " + GetLength(outputLength) + "  压缩率 : " + (inputLength * 100 / outputLength) + "%");
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.WriteLine(HintString);
            }
            Console.WriteLine("执行完成");
#if DEBUG
            Console.ReadKey();
#endif
        }
        static string GetLength(long length) {
            if (length < 1024) {
                return length + " Bit";
            } else if (length < 1024 * 1024) {
                return (length / 1024) + " KB";
            } else if (length < 1024 * 1024 * 1024) {
                return (length / 1024 / 1024) + " MB";
            } else {
                return (length / 1024 / 1024 / 1024) + " GB";
            }
        }
        static void lz4(OpType type, string input, string output) {
            if (type == OpType.zip) {
                using (var inputStream = new FileStream(input, FileMode.Open)) {
                    using (var outputStream = new FileStream(output, FileMode.Create)) {
                        using (var lz4Stream = new LZ4Stream(outputStream, LZ4StreamMode.Compress)) {
                            while ((readed = inputStream.Read(bytes, 0, Length)) > 0) {
                                lz4Stream.Write(bytes, 0, readed);
                            }
                        }
                    }
                }
            } else if (type == OpType.unzip) {
                using (var inputStream = new FileStream(input, FileMode.Open)) {
                    using (var outputStream = new FileStream(output, FileMode.Create)) {
                        using (var lz4Stream = new LZ4Stream(inputStream, LZ4StreamMode.Decompress)) {
                            while ((readed = lz4Stream.Read(bytes, 0, Length)) > 0) {
                                outputStream.Write(bytes, 0, readed);
                            }
                        }
                    }
                }
            }
        }
        static void gzip(OpType type, string input, string output) {
            if (type == OpType.zip) {
                using (var inputStream = new FileStream(input, FileMode.Open)) {
                    using (var outputStream = new FileStream(output, FileMode.Create)) {
                        using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress)) {
                            while ((readed = inputStream.Read(bytes, 0, Length)) > 0) {
                                gzipStream.Write(bytes, 0, readed);
                            }
                        }
                    }
                }
            } else if (type == OpType.unzip) {
                using (var inputStream = new FileStream(input, FileMode.Open)) {
                    using (var outputStream = new FileStream(output, FileMode.Create)) {
                        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                            while ((readed = gzipStream.Read(bytes, 0, Length)) > 0) {
                                outputStream.Write(bytes, 0, readed);
                            }
                        }
                    }
                }
            }
        }
    }
}
