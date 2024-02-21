using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using annulus.Core;
using annulus.Services;

namespace annulus.MVVM.ViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        public IItemsService ItemsService { get; set; }
        public RelayCommand AddItemCommand {  get; set; }
        public string Name { get; set; }
        public SettingsViewModel(IItemsService itemsService)
        {
            ItemsService = itemsService;
            AddItemCommand = new RelayCommand(o => { ItemsService.AddItem(Name);  }, o => true);
        }
    }
}
