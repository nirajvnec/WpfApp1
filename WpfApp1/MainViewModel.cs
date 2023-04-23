using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using OfficeOpenXml;
using WpfApp1;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private ObservableCollection<ExcelDataModel> _dataCollection;
    private double _uploadProgress;
    private bool _isUploading;

    public double UploadProgress
    {
        get { return _uploadProgress; }
        set
        {
            if (_uploadProgress != value)
            {
                _uploadProgress = value;
                OnPropertyChanged("UploadProgress");
            }
        }
    }

    public bool IsUploading
    {
        get { return _isUploading; }
        set
        {
            if (_isUploading != value)
            {
                _isUploading = value;
                OnPropertyChanged("IsUploading");
            }
        }
    }

    public ObservableCollection<ExcelDataModel> DataCollection
    {
        get { return _dataCollection; }
        set
        {
            _dataCollection = value;
            OnPropertyChanged("DataCollection");
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainViewModel()
    {
        DataCollection = new ObservableCollection<ExcelDataModel>();
    }

    public async Task UploadExcelAsync(string filePath)
    {
        DataCollection.Clear();
        IsUploading = true;

        var progress = new CustomProgress(value => UploadProgress = value);
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        await Task.Run(() =>
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int totalRows = worksheet.Dimension.Rows;

                for (int row = 2; row <= totalRows; row++)
                {
                    DateTime.TryParse(worksheet.Cells[row, 3].Text, out var dateOfBirth);
                    var rowData = new ExcelDataModel
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Age = int.Parse(worksheet.Cells[row, 2].Text),
                        DateOfBirth = dateOfBirth,
                        Email = worksheet.Cells[row, 4].Text
                    };

                    double percent = (double)(row - 2) / (totalRows - 2) * 100;
                    progress.Report(percent);

                    Application.Current.Dispatcher.Invoke(() => DataCollection.Add(rowData));
                }
            }
        });

        IsUploading = false;
    }
}

public class CustomProgress
{
    private readonly Action<double> _report;

    public CustomProgress(Action<double> report)
    {
        _report = report;
    }

    public void Report(double value)
    {
        _report(value);
    }
}