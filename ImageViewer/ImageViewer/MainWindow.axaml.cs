using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform.Storage;
using System.Linq;
using ImageMagick;
using System.IO;
using Avalonia.Interactivity;

namespace ImageViewer {
    public partial class MainWindow : Window {
        public string ImageInfo { get; private set; } = "";
        public MainWindow() {
            InitializeComponent();
        }
        private async void Button_Click(object? sender, RoutedEventArgs e) {
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
                OpenFile(file.Path.LocalPath);
            }
        }
        async void OpenFile(string fileName) {
            using var magickImage = new MagickImage(fileName);
            var profiler = magickImage.GetExifProfile();
            var info = "";
            foreach (var value in profiler.Values) {
                info += $@"
{value.Tag} : {value.GetValue().ToString()}";
            }
//            var model = profiler?.GetValue(ExifTag.Model);
//            var software = profiler?.GetValue(ExifTag.ImageWidth);
//            var dateTime = profiler?.GetValue(ExifTag.DateTime);
//            var info = $@"
//Model : {model}
//Software : {software}
//Width : {magickImage.Width}
//Height : {magickImage.Height}
//DateTime : {dateTime}
//";
            textImageInfo.Text = info;
            var tempFile = Path.GetTempFileName();
            await magickImage.WriteAsync(tempFile, MagickFormat.Jpg);
            imagePicture.Source = new Bitmap(tempFile);
            ToolTip.SetTip(imagePicture, info);
            File.Delete(tempFile);
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
            if (file != null) {
                OpenFile(file.Path.LocalPath);
            }
        }
        async void OnClickLastButton(object? sender, RoutedEventArgs e) {

        }
        async void OnClickNextButton(object? sender, RoutedEventArgs e) {

        }
    }
}