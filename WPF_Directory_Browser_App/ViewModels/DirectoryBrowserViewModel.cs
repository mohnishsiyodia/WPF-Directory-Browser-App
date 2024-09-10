using Microsoft.Win32;
using MyApp.Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyApp.ViewModels
{
    public class DirectoryBrowserViewModel : INotifyPropertyChanged
    {
        private string currentDirectory;
        public string CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                currentDirectory = value;
                OnPropertyChanged(nameof(CurrentDirectory));
                LoadDirectoryContents();
            }
        }

        public ObservableCollection<string> DirectoryContents { get; set; }
        public ObservableCollection<ImageModel> UploadedImages { get; set; }

        public ICommand BrowseCommand { get; }
        public ICommand UploadCommand { get; }

        public DirectoryBrowserViewModel()
        {
            DirectoryContents = new ObservableCollection<string>();
            UploadedImages = new ObservableCollection<ImageModel>();
            BrowseCommand = new RelayCommand(BrowseDirectory);
            UploadCommand = new RelayCommand(UploadImage);
            LoadUploadedImages();
        }

        private void BrowseDirectory(object parameter)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                CurrentDirectory = dialog.SelectedPath;
            }
        }

        private void LoadDirectoryContents()
        {
            if (Directory.Exists(CurrentDirectory))
            {
                DirectoryContents.Clear();
                foreach (var dir in Directory.GetDirectories(CurrentDirectory))
                {
                    DirectoryContents.Add(dir);
                }
                foreach (var file in Directory.GetFiles(CurrentDirectory))
                {
                    DirectoryContents.Add(file);
                }
            }
        }

        private void UploadImage(object parameter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png)|*.png"
            };
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var imageData = File.ReadAllBytes(fileName);
                DatabaseHelper.SaveImage(Path.GetFileName(fileName), imageData);
                LoadUploadedImages();
            }
        }

        private void LoadUploadedImages()
        {
            UploadedImages.Clear();
            var reader = DatabaseHelper.GetImages();
            while (reader.Read())
            {
                UploadedImages.Add(new ImageModel
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    ImageData = (byte[])reader.GetValue(2)
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}


