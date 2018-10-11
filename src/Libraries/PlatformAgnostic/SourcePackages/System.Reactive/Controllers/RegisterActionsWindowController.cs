using System;
using System.Reactive.Subjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace PocketXaf.SourcePackages.System.Reactive.Controllers{
    public class RegisterActionsWindowController:WindowController{
        public static IObservable<Frame> WhenFrameAssigned => FrameAssignedSubject;
        static readonly ReplaySubject<Func<RegisterActionsWindowController, ActionBase[]>> Subject=new ReplaySubject<Func<RegisterActionsWindowController, ActionBase[]>>();
        private static readonly Subject<Frame> FrameAssignedSubject=new Subject<Frame>();

        public RegisterActionsWindowController(){
            Subject.Subscribe(func => func(this));
            
        }

        internal static void RegisterAction(Func<RegisterActionsWindowController, ActionBase[]> actions){
            Subject.OnNext(actions);
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            FrameAssignedSubject.OnNext(Frame);
        }
    }
}