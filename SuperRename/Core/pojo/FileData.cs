using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Annotations;

namespace SuperRename.Core.pojo
{
    public class FileData : INotifyPropertyChanged
    {



        public FileData(bool enable, string source, string target)
        {
            this.Enable = enable;
            this.Source = source;
            this.Target = target;
        }

        public bool _Enable;
        public bool Enable { get { return _Enable; } set { _Enable = value; OnPropertyChanged(); } }

        public string _Source;
        public string Source { get { return _Source; } set { _Source = value; OnPropertyChanged(); } }

        public string _StatusMessage;
        public string StatusMessage { get { return _StatusMessage; } set { _StatusMessage = value; OnPropertyChanged(); } }
        public string _Target;
        public string Target { get { return _Target; } set { _Target = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
