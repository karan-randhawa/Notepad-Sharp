using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NotePad_Sharp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            App.FileReceived += async (s, e) => await _FileService.LoadAsync(e);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private Models.FileInfo _File;
        public Models.FileInfo File
        {
            get { return _File; }
            set
            {
                _File = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(File)));
            }
        }

        Services.FileService _FileService = new Services.FileService();
        public async void Save()
        { await _FileService.SaveAsync(File); }

        public async void Open()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".txt");
            var file = await picker.PickSingleFileAsync();
            if (file == null)
                await new Windows.UI.Popups.MessageDialog("No file selected.").ShowAsync();
            else
                File = await _FileService.LoadAsync(file);
        }
    }
}
