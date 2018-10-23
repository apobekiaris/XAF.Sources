using System;
using DevExpress.ExpressApp;

namespace XAF.SourcePackages.ActiveViewDocumentController{
    public class ActiveViewChangedEventArgs : EventArgs {
        public View View{ get; }

        public ActiveViewChangedEventArgs(View view){
            View = view;
        }
        
    }

    public abstract class ActiveDocumentViewController:WindowController{
        public event EventHandler<ActiveViewChangedEventArgs> ActiveViewChanged;

        protected ActiveDocumentViewController(){
            TargetWindowType=WindowType.Main;
        }

        protected virtual void OnActiveViewChanged(ActiveViewChangedEventArgs e){
            ActiveViewChanged?.Invoke(this, e);
        }
    }
}
