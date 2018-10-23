using System;
using System.Reactive.Subjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace XAF.SourcePackages.System.Reactive.Controllers{
    public class RegisterActionsViewController:ViewController{
        static readonly ReplaySubject<Func<RegisterActionsViewController, ActionBase[]>> Subject=new ReplaySubject<Func<RegisterActionsViewController, ActionBase[]>>();
        public RegisterActionsViewController(){
            Subject.Subscribe(func => func(this));
        }

        internal static void RegisterAction(Func<RegisterActionsViewController, ActionBase[]> actions){
            Subject.OnNext(actions);
        }
    }
}