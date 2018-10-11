using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace PocketXaf.SourcePackages.System.Reactive.Services.Actions{
    class ActionsRegistry{
        public static ActionBase[] Register(Controller controller){
            return new[]{UpdateListViewSelectionAction.Register(controller)};
        }

    }

    
}