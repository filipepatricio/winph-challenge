using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DesafioThingPink.Models;

namespace DesafioThingPink.ViewModels
{
    class CommentDesign : INotifyPropertyChanged
    {
        private string _text;
        private string _user;
        public Caption comment_data { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("text");
            }
        }

        public string user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                RaisePropertyChanged("user");
            }
        }

        public CommentDesign(Caption comment)
        {
            comment_data = comment;
            text = comment.text;
            user = "@"+comment.from.username;

        }
    }
}
