using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class EditViewModel: BindableBase
    {

        private bool saveEnabled;

        public bool SaveEnabled 
        {
            get => saveEnabled;
            set => Set(ref saveEnabled, value);
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


    }
}
