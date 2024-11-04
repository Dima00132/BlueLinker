using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLinker.ViewModels.Base
{
    public class ViewModelBase : ObservableObject, IDisposable
    {
        public virtual void Dispose()
        {
            
        }
    }
}
