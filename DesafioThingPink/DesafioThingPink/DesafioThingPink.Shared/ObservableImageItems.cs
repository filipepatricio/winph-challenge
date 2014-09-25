using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioThingPink
{
    class ObservableImageItems: ObservableCollection<ImageItemDesign>, INotifyPropertyChanged
    {

        public ObservableImageItems(){ Clear(); }

        public ObservableImageItems(List<Datum> insta_data)
        {
            Clear();
            foreach (Datum data in insta_data)
            {
                ImageItemDesign tid = new ImageItemDesign(data);
                Add(tid);
            }
        }
    }
}
