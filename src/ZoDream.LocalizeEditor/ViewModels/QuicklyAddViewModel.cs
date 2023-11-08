using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class QuicklyAddViewModel: BindableBase
    {
        public QuicklyAddViewModel()
        {
            SaveCommand = new RelayCommand(TapSave);
            ToggleCommand = new RelayCommand(TapToggle);
            TranslateCommand = new RelayCommand(TapTranslate);
        }


        private Action? EventListener;


        public UnitItem Data { get; private set; } = new();

        private bool saveEnabled;

        public bool SaveEnabled {
            get => saveEnabled;
            set => Set(ref saveEnabled, value);
        }

        private bool isFully;

        public bool IsFully {
            get => isFully;
            set {
                Set(ref isFully, value);
                OnPropertyChanged(nameof(MoreIcon));
                OnPropertyChanged(nameof(WindowHeight));
            }
        }

        public double WindowHeight => IsFully ? 300.0 : 130.0;


        public string MoreIcon => IsFully ? "\uE935" : "\uE936";


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


        private string target = string.Empty;

        public string Target {
            get => target;
            set {
                Set(ref target, value);
                SaveEnabled = true;
            }
        }

        private string fileName = string.Empty;

        public string FileName {
            get => fileName;
            set => Set(ref fileName, value);
        }

        private string lineNo = string.Empty;

        public string LineNo {
            get => lineNo;
            set => Set(ref lineNo, value);
        }


        public ICommand SaveCommand { get; private set; }

        public ICommand ToggleCommand { get; private set; }
        public ICommand TranslateCommand { get; private set; }


        private void TapSave(object? _)
        {
            Data.Target = target;
            Data.Source = source;
            Data.Id = id;
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                Data.AddLine(FileName, LineNo);
            }
            EventListener?.Invoke();
            SaveEnabled = false;
        }

        private void TapToggle(object? _)
        {
            IsFully = !IsFully;
        }

        private async void TapTranslate(object? _)
        {
            Target = await App.ViewModel.TranslateAsync(Source);
        }
    
        public void AddListener(Action action)
        {
            EventListener = action;
        }
    }
}
