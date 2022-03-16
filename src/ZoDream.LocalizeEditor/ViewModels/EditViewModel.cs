using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class EditViewModel: BindableBase
    {

        private ObservableCollection<SourceLocation> locationItems = new();

        public ObservableCollection<SourceLocation> LocationItems
        {
            get => locationItems;
            set => Set(ref locationItems, value);
        }


    }
}
