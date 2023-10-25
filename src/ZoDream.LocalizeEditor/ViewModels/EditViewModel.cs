using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;
using ZoDream.LocalizeEditor.Events;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class EditViewModel: BindableBase
    {

        public EditViewModel()
        {
            SaveCommand = new RelayCommand(TapSave);
            TranslateCommand = new RelayCommand(TapTranslate);
            PreviousCommand = new RelayCommand(TapPrevious);
            NextCommand = new RelayCommand(TapNext);
            AddCommendCommand = new RelayCommand(TapAddComment);
            TogglePluralCommand = new RelayCommand(TapTogglePlural);
        }

        private UnitItem UpdatedData = new();

        private bool saveEnabled;

        public bool SaveEnabled 
        {
            get => saveEnabled;
            set => Set(ref saveEnabled, value);
        }

        private bool nextEnabled;

        public bool NextEnabled {
            get => nextEnabled;
            set => Set(ref nextEnabled, value);
        }

        private bool previousEnabled;

        public bool PreviousEnabled {
            get => previousEnabled;
            set => Set(ref previousEnabled, value);
        }



        private string id = string.Empty;

        public string Id {
            get => id;
            set {
                Set(ref id, value);
                SaveEnabled = true;
            }
        }

        private string source = string.Empty;

        public string Source {
            get => source;
            set {
                Set(ref source, value);
                SaveEnabled = true;
            }
        }

        private string sourcePlural = string.Empty; // 翻译的文字复数形式

        public string SourcePlural {
            get => sourcePlural;
            set {
                Set(ref sourcePlural, value);
                SaveEnabled = true;
            }
        }


        private string target = string.Empty;

        public string Target {
            get => target;
            set {
                Set(ref target, value);
                SaveEnabled = true;
            }
        }


        private ObservableCollection<SourceLocation> locationItems = new();

        public ObservableCollection<SourceLocation> LocationItems
        {
            get => locationItems;
            set => Set(ref locationItems, value);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }
        public ICommand NextCommand { get; private set; }

        public ICommand TranslateCommand { get; private set; }
        public ICommand AddCommendCommand { get; private set; }
        public ICommand TogglePluralCommand { get; private set; }

        public event EmptyEventHandler? OnConfirm;
        public event EmptyEventHandler? OnPrevious;
        public event EmptyEventHandler? OnNext;

        public UnitViewModel Data {
            get {
                return new UnitViewModel(UpdatedData);
            }
            set {
                UpdatedData = value.Clone<UnitItem>();
                Source = value.Source;
                Target = value.Target;
                Id = value.Id;
                LocationItems.Clear();
                foreach (var item in value.Location)
                {
                    LocationItems.Add(item);
                }
                SaveEnabled = false;
            }
        }

        private void TapPrevious(object? _)
        {
            OnPrevious?.Invoke();
        }

        private void TapNext(object? _)
        {
            OnNext?.Invoke();
        }

        private void TapSave(object? _)
        {
            UpdatedData.Target = target;
            UpdatedData.Source = source;
            UpdatedData.Id = id;
            UpdatedData.Location = LocationItems.ToList();

            OnConfirm?.Invoke();
            SaveEnabled = false;
        }
        private async void TapTranslate(object? _)
        {
            Target = await App.ViewModel.TranslateAsync(Source);
        }

        private void TapAddComment(object? _)
        {
        }

        private void TapTogglePlural(object? _)
        {
        }

    }
}
