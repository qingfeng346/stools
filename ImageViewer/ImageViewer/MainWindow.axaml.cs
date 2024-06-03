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
        private object sync = new object();
        private int version = 0;
        private string file = "";
        private List<string> files = new List<string>();
        public string ImageInfo { get; private set; } = "";
        public MainWindow() {
            InitializeComponent();
        }
        async Task<bool> OpenFile(string fileName) {
            try {
                using var magickImage = new MagickImage(fileName);
                var info = $@"文件 : {fileName}
大小 : {ScorpioUtil.GetMemory(new FileInfo(fileName).Length)}
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
                ToolTip.SetTip(imagePicture, info);
                File.Delete(tempFile);
                file = Path.GetFullPath(fileName);
                return true;
            } catch (System.Exception e) {
                Console.WriteLine(e.ToString());
            }
            return false;
        }
        void OpenPath(string path) {
            Task.Run(() => {
                version++;
                lock (sync) { this.files.Clear(); }
                var v = version;
                var files = FileUtil.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                foreach (var file in files) {
                    if (v != version) { return; }
                    try {
                        using var magickImage = new MagickImage(file);
                        lock (sync) { this.files.Add(Path.GetFullPath(file)); }
                    } catch (System.Exception e) {

                    }
                }
            });
        }
        async void OpenCommand(object? sender, RoutedEventArgs e) {
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
            if (file != null && await OpenFile(file.Path.LocalPath)) {
                OpenPath(Path.GetDirectoryName(file.Path.LocalPath));
            }
        }
        void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Left) {
                lock (sync) {
                    var index = files.IndexOf(file);
                    if (index > 0) {
                        OpenFile(files[index - 1]);
                    }
                }
            } else if (e.Key == Key.Right) {
                lock (sync) {
                    var index = files.IndexOf(file);
                    if (index >= 0 && index < files.Count - 1) {
                        OpenFile(files[index + 1]);
                    }
                }
            }
        }
        void OnKeyUp(object sender, KeyEventArgs e) {
            
        }
    }
}