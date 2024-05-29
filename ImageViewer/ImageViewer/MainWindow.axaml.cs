using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform.Storage;
using System.Linq;
using ImageMagick;
using System.IO;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace ImageViewer {
    public partial class MainWindow : Window {
        // public ICommand OpenCommand { get; } = new CustomCommand();
        public string ImageInfo {get;private set;} ="fwafwef";
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
            textImageInfo.Text = @"fwafweffwe
            fwafweffawe
            fewafwaef
            fawefwef
            fewfwaefewf
            fewafwaefwaef
            fweafwaefawef
            fweafwaefwef";
            using var magickImage = new MagickImage(fileName);
            var tempFile = Path.GetTempFileName();
            await magickImage.WriteAsync(tempFile, MagickFormat.Jpg);
            imagePicture.Source = new Bitmap(tempFile);
            File.Delete(tempFile);
            ToolTip.SetTip(imagePicture, @"fweafwaefawef
            fwea
            fewafwaeffew
            fweafwaef
            fweafwaefawefwaef
            waef");
        }
        public async void OpenCommand(object? sender, RoutedEventArgs e) {
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
    }
}