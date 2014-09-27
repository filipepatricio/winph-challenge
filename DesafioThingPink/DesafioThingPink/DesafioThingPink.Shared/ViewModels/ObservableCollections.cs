using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesafioThingPink.Models;

namespace DesafioThingPink.ViewModels
{
    class ObservableImageItems: ObservableCollection<ImageItemDesign>, INotifyPropertyChanged
    {

        public ObservableImageItems(){ Clear(); }

        public ObservableImageItems(List<ImageData> insta_data)
        {
            Clear();
            foreach (ImageData data in insta_data)
            {
                ImageItemDesign image_item_design = new ImageItemDesign(data);
                Add(image_item_design);
            }
        }
    }

    class ObservableSearchItems: ObservableCollection<SearchItemDesign>, INotifyPropertyChanged
    {

        public ObservableSearchItems() { Clear(); }

        public ObservableSearchItems(List<SearchItem> search_item_list)
        {
            Clear();
            foreach (SearchItem search_item in search_item_list)
            {
                SearchItemDesign search_item_design = new SearchItemDesign(search_item);
                Add(search_item_design);
            }
        }
    }

    class ObservableCommentItems : ObservableCollection<CommentDesign>, INotifyPropertyChanged
    {

        public ObservableCommentItems() { Clear(); }

        public ObservableCommentItems(List<Caption> comment_list)
        {
            Clear();
            foreach (Caption comment_item in comment_list)
            {
                CommentDesign comment_design = new CommentDesign(comment_item);
                Add(comment_design);
            }
        }
    }
}
