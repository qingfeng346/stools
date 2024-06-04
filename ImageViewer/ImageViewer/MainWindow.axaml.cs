using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.Linq;
using ImageMagick;
using System.IO;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using Scorpio.Commons;
using System.Threading.Tasks;
namespace ImageViewer {
    public partial class MainWindow : Window {
        private string file = "";
        private List<string> files = new List<string>();
        public string ImageInfo { get; private set; } = "";
        public MainWindow() {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                OpenFile(args[1], true);
            }
        }
        async Task<bool> OpenFile(string fileName, bool loadPathFiles = false) {
            try {
                file = Path.GetFullPath(fileName);
                using var magickImage = new MagickImage(file);
                if (loadPathFiles) {
                    files = FileUtil.GetFiles(Path.GetDirectoryName(file), ["*.jpg", "*.png", "*.livp", "*.bmp", "*.heic"], SearchOption.TopDirectoryOnly);
                }
                var info = $@"文件名 : {Path.GetFileName(file)} ({files.IndexOf(file) + 1}/{files.Count})
大小 : {ScorpioUtil.GetMemory(new FileInfo(file).Length)}
尺寸 : {magickImage.Width}x{magickImage.Height}";
                var profiler = magickImage.GetExifProfile();
                if (profiler != null) {
                    foreach (var value in profiler.Values) {
                        if (value.GetValue() is Number number) {
                            info += $@"
{value.Tag} : {(ushort)number}";
                        } else {
                            info += $@"
{value.Tag} : {value.GetValue().ToString()}";
                        }
                    }
                }
                textImageInfo.Text = info;
                var tempFile = Path.GetTempFileName();
                await magickImage.WriteAsync(tempFile, MagickFormat.Jpg);
                imagePicture.Source = new Bitmap(tempFile);
                //ToolTip.SetTip(imagePicture, info);
                File.Delete(tempFile);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
            return false;
        }
        async void OnClickOpenMenu(object sender, RoutedEventArgs e) {
            var options = new FilePickerOpenOptions {
                Title = "选择文件",
                FileTypeFilter = [
                    new FilePickerFileType("All files") {
                        Patterns = ["*"]
                    }
                ]
            };
            var result = await StorageProvider.OpenFilePickerAsync(options);
            var file = result.FirstOrDefault();
            if (file != null) {
                await OpenFile(file.Path.LocalPath, true);
            }
        }
        void OnClickExitMenu(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }
        async void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Left) {
                var index = files.IndexOf(file);
                if (index > 0) {
                    await OpenFile(files[index - 1]);
                }
            } else if (e.Key == Key.Right) {
                var index = files.IndexOf(file);
                if (index >= 0 && index < files.Count - 1) {
                    await OpenFile(files[index + 1]);
                }
            }
        }
        void OnKeyUp(object sender, KeyEventArgs e) {
            
        }
    }
}