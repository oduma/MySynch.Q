using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Sciendo.Common.WPF.MVVM
{
    public abstract class ViewModelWithTrackChangesBase: ViewModelBase
    {
        public ICommand ViewChangeRequest { get; private set; }

        public event EventHandler ViewModelChanged;

        protected ViewModelWithTrackChangesBase()
        {
            ViewChangeRequest= new RelayCommand(ViewStateChanged);
        }

        private void ViewStateChanged()
        {
            ViewModelChanged?.Invoke(this, new EventArgs());
        }

        protected void TrackAllChildren(IEnumerable<ViewModelWithTrackChangesBase> children)
        {
            foreach (var child in children)
            {
                child.ViewModelChanged += Child_ViewModelChanged;
            }
        }

        private void Child_ViewModelChanged(object sender, EventArgs e)
        {
            ViewModelChanged?.Invoke(this, new EventArgs());
        }

        protected void RaiseChangeEvent()
        {
            ViewModelChanged?.Invoke(this, new EventArgs());
        }
    }
}
