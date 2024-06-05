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
using System.Text;
using ImageMagick.Formats;
namespace ImageViewer {
    public partial class MainWindow : Window {
        private string file = "";
        private List<string> files = new List<string>();
        public string ImageInfo { get; private set; } = "";
        public MainWindow() {
            InitializeComponent();
            SizeChanged += (_, _) => OnSizeChanged();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                OpenFile(args[1], true);
            }
        }
        async Task<bool> OpenFile(string fileName, bool loadPathFiles = false) {
            try {
                file = Path.GetFullPath(fileName);
                using var magickImage = new MagickImage(file);
                var builder = new StringBuilder();
                builder.Append($@"大小 : {ScorpioUtil.GetMemory(new FileInfo(file).Length)}
宽高 : {magickImage.Width}x{magickImage.Height}");
                var profiler = magickImage.GetExifProfile();
                if (profiler != null) {
                    var tags = new HashSet<ExifTag>();
                    void AddInfo<TValueType>(ExifTag<TValueType> tag, string name) {
                        tags.Add(tag);
                        var value = profiler.GetValue(tag);
                        if (value != null) {
                            builder.Append(@$"
{name} : {value.Value}({value.DataType})");
                        }
                    }
                    AddInfo(ExifTag.Make, "相机制造商");
                    AddInfo(ExifTag.Model, "相机设备");
                    AddInfo(ExifTag.DateTimeOriginal, "拍摄时间");
                    AddInfo(ExifTag.DateTimeDigitized, "数字化时间");
                    AddInfo(ExifTag.DateTime, "最后修改时间");
                    AddInfo(ExifTag.GPSDateStamp, "GPS时间");
                    foreach (var value in profiler.Values) {
                        if (!tags.Contains(value.Tag)) {
                            builder.Append($@"
{value.Tag} : {value.GetValue().ToString()}");
                        }
                    }
                }
                textImageInfo.Text = builder.ToString();
                var tempFile = Path.GetTempFileName();
                await magickImage.WriteAsync(tempFile, MagickFormat.Jpg);
                imagePicture.Source = new Bitmap(tempFile);
                File.Delete(tempFile);
                if (loadPathFiles) {
                    files = FileUtil.GetFiles(Path.GetDirectoryName(file), ["*.jpg", "*.png", "*.livp", "*.bmp", "*.heic"], SearchOption.TopDirectoryOnly);
                }
                this.textImageTitle.Text = $"{Path.GetFileName(file)} ({files.IndexOf(file) + 1}/{files.Count})";
                OnSizeChanged();
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
        void OnSizeChanged() {
            scrollViewer.Height = Height - 100;
        }
    }
}