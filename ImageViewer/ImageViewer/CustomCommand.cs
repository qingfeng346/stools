using System;
using System.Windows.Input;
public class CustomCommand : ICommand
{
    public event EventHandler CanExecuteChanged;
    public bool CanExecute(object parameter)
    {
        return true; // 在此处可以根据需要设置命令是否可以执行
    }

    public void Execute(object parameter)
    {
        Console.WriteLine("open ");
        // 在这里编写命令执行的逻辑
        // 例如，打开一个新窗口
    }
}