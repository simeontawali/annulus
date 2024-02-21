using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace annulus.Services
{
    public interface IItemsService
    {
        public ObservableCollection<string> Items { get; set; }
        void AddItem(string paramater);
    }
    internal class ItemsService : IItemsService
    {
        public ObservableCollection<string> Items { get; set; } = new();
        // TODO: update AddItem to accept a parameter
        public void AddItem(string paramater)
        {
            Items.Add(paramater);
        }
    }
}
