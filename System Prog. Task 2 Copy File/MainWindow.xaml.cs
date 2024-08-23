using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace System_Prog._Task_2_Copy_File;


public partial class MainWindow : Window, INotifyPropertyChanged
{
    private int value = 0;

    public string FromText { get; set; }
    public string ToText { get; set; }
    public int Value { get => value; set { this.value = value; OnPropertyChanged(); } }
    public int Maximum { get => maximum; set { maximum = value; OnPropertyChanged(); } }

    public bool IsToButtonClicked { get; set; } = false;
    public bool IsFromButtonClicked { get; set; } = false;
    byte[]? buffer = null;
    int NumBytesToRead = 0;
    int NumByteRead = 0;
    private int maximum = 0;

    public MainWindow()
    {
        InitializeComponent();
        Maximum = 1;
        Value = 0;
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void FileClickButton(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        OpenFileDialog fileDialog = new();
        fileDialog.Filter = "Text files (*.txt)|*.txt";
        fileDialog.ShowDialog();
        if (btn == FromButton)
        {
            FromBox.Text = fileDialog.FileName;
            FromText = fileDialog.FileName;
        }
        else
        {
            IsToButtonClicked = true;
            ToBox.Text = fileDialog.FileName;
            ToText = fileDialog.FileName;
        }

    }

    private void CopyClickButton(object sender, RoutedEventArgs e)
    {

        if (!IsToButtonClicked)
        {
            if (!File.Exists(ToBox.Text))
            {
                using (File.Create(ToBox.Text)) { };
            }
            ToText = ToBox.Text;
        }
        if (!IsFromButtonClicked)
        {
            FromText = FromBox.Text;
        }
        int n = 0;
        using (FileStream fs = new FileStream(FromText, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fs.Length];
            NumBytesToRead = Convert.ToInt32(fs.Length);
            NumByteRead = 0;

            while (NumBytesToRead > 0)
            {
                n = fs.Read(buffer, NumByteRead, NumBytesToRead);
                NumBytesToRead -= n;
                NumByteRead += n;
            }
        }
        Maximum = n;
        new Thread(() =>
        {
            using (FileStream fs = new FileStream(ToText, FileMode.Create, FileAccess.Write))
            {
                int count = 0;

                while (NumByteRead > 0)
                {
                    fs.Write(buffer, count, 1);
                    count++;
                    NumByteRead--;
                    Value += 1;
                    fs.Flush();
                    Thread.Sleep(50);
                }

            }
        }).Start();

    }
}