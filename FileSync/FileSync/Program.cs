using Scorpio.Commons;
namespace FileSync {
    using static FileUtil;
    internal class Program {
        private const string ParameterSource = "--source|-source|-s";
        private const string ParameterTarget = "--target|-target|-t";
        static void Main(string[] args) {
            var perform = new Perform();
            perform.AddExecute("sync", "文档同步", OnFileSync);
            try {
                perform.Start(args);
                Environment.Exit(0);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                Environment.Exit(1);
            }
        }
        static void OnFileSync([ParamterInfo(Label = "起始目录", Param = ParameterSource, Required = true)] string source,
                               [ParamterInfo(Label = "目标目录", Param = ParameterTarget, Required = true)] string target) {
            source = Path.GetFullPath(source);
            target = Path.GetFullPath(target);
            var sync = new object();
            var changeFiles = new LinkedList<(string, string, WatcherChangeTypes)>();
            var watcher = new FileSystemWatcher(source);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            watcher.Changed += (sender, args) => {
                logger.info($"文件Changed:{args.FullPath}发生变化,变化类型:{args.ChangeType}");
                lock (sync) {
                    changeFiles.AddLast((Path.GetFullPath(args.FullPath), "", args.ChangeType));
                }
            };
            watcher.Created += (sender, args) => {
                logger.info($"文件Created:{args.FullPath}发生变化,变化类型:{args.ChangeType}");
                lock (sync) {
                    changeFiles.AddLast((Path.GetFullPath(args.FullPath), "", args.ChangeType));
                }
            };
            watcher.Deleted += (sender, args) => {
                logger.info($"文件Deleted:{args.FullPath}发生变化,变化类型:{args.ChangeType}");
                lock (sync) {
                    changeFiles.AddLast((Path.GetFullPath(args.FullPath), "", args.ChangeType));
                }
            };
            watcher.Renamed += (sender, args) => {
                logger.info($"文件Rename {args.OldFullPath} -> {args.FullPath} 变化类型:{args.ChangeType}");
                lock (sync) {
                    changeFiles.AddLast((Path.GetFullPath(args.FullPath), Path.GetFullPath(args.OldFullPath), args.ChangeType));
                }
            };
            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            string GetTargetFile(string sourceFile) {
                return Path.Combine(target, sourceFile.Substring(source.Length + 1));
            }
            var sourceLength = PathExist(source) ? GetFiles(source, "*").Count : 0;
            var targetLength = PathExist(target) ? GetFiles(target, "*").Count : 0;
            logger.info($"开始监听目录:{source},当前文件总数量:{sourceLength}");
            logger.info($"目标同步目录:{target},当前文件总数量:{targetLength}");
            var nextCheckTime = DateTime.Now;
            Task.Run(async () => {
                try {
                    SyncFolder(source, target, null, true, CompareType.SizeAndModifyTime, NameType.None);
                } catch (Exception e) {
                    throw new Exception($"同步目录失败 : {e}");
                }
                while (true) {
                    await Task.Delay(500);
                    while (changeFiles.Count > 0) {
                        await Task.Delay(500);
                        lock (sync) {
                            var (filePath, oldFilePath, changeType) = changeFiles.First();
                            try {
                                switch (changeType) {
                                    case WatcherChangeTypes.Created:
                                    case WatcherChangeTypes.Changed:
                                        SyncFile(filePath, GetTargetFile(filePath));
                                        break;
                                    case WatcherChangeTypes.Deleted:
                                        DeleteFile(GetTargetFile(filePath));
                                        break;
                                    case WatcherChangeTypes.Renamed:
                                        MoveFile(GetTargetFile(oldFilePath), GetTargetFile(filePath));
                                        break;
                                }
                            } catch (Exception e) {
                                logger.error($"处理文件出错 FilePath:{filePath} OldPath:{oldFilePath} ChangeType:{changeType} : {e}");
                                changeFiles.AddFirst((filePath, oldFilePath, changeType));
                            }
                        }
                    }
                }
            }).Wait();
        }
    }
}
