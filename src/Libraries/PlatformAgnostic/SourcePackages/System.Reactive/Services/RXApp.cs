using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using XAF.SourcePackages.ActiveViewDocumentController;
using XAF.SourcePackages.System.Reactive.Controllers;
using XAF.SourcePackages.System.Reactive.Extensions;

namespace XAF.SourcePackages.System.Reactive.Services{

    public static class RxApp{
        private static XafApplication _xafApplication;
        internal static IObservable<Frame> FrameAssignedToController => RegisterActionsWindowController.WhenFrameAssigned;

        public static IObservable<Frame> Frames(TemplateContext templateContext=default){
            return FrameAssignedToController.Where(frame => templateContext == default || frame.Context == templateContext);
        }

        public static IObservable<TController> GetControllers<TController>(TemplateContext templateContext=default) where TController : Controller{
            return Frames(templateContext).Select(frame => frame.GetController<TController>());
        }

        public static IObservable<Frame> Frames( ActionBase  action) {
            return FrameAssignedToController.WhenFits(action);
        }

        public static IObservable<Frame> Frames( ViewType viewType,
            Type objectType = null, Nesting nesting = Nesting.Any, bool? isPopupLookup = null){
            return FrameAssignedToController.WhenFits(viewType, objectType, nesting, isPopupLookup);
        }

        public static IObservable<View> ActiveViewChanged => FrameAssignedToController.SelectMany(_ => {
            return _.GetController<ActiveDocumentViewController>().AsObservable().ActiveViewChanged().Select(pattern => pattern.EventArgs.View);
        });

        public static XafApplication XafApplication{
            get => _xafApplication;
            set => _xafApplication = value;
        }

        public static void RegisterViewAction(Func<RegisterActionsViewController,ActionBase[]>actions){
            RegisterActionsViewController.RegisterAction(actions);
        }

        public static void RegisterWindowAction(Func<RegisterActionsWindowController,ActionBase[]>actions){
            RegisterActionsWindowController.RegisterAction(actions);
        }

        public static IObservable<EventPattern<LogonEventArgs>> LoggedOn => Observable
            .FromEventPattern<EventHandler<LogonEventArgs>, LogonEventArgs>(h => XafApplication.LoggedOn += h,
                h => XafApplication.LoggedOn -= h).Select(pattern => pattern);


        public static IObservable<View> ViewCreated => Observable
            .FromEventPattern<EventHandler<ViewCreatedEventArgs>, ViewCreatedEventArgs>(h => XafApplication.ViewCreated += h,
                h => XafApplication.ViewCreated -= h)
            .Select(pattern => pattern.EventArgs.View)
            .TakeUntil(XafApplication.WhenDisposed());

        public static IObservable<(Frame masterFrame, NestedFrame detailFrame)> MasterDetailFrames(Type masterType, Type childType){
            var nestedlListViews = Frames(ViewType.ListView, childType, Nesting.Nested)
                .Select(_ => _)
                .Cast<NestedFrame>();
            return Frames(ViewType.DetailView, masterType)
                .CombineLatest(nestedlListViews.WhenIsNotOnLookupPopupTemplate(),
                    (masterFrame, detailFrame) => (masterFrame, detailFrame));
        }

        public static IObservable<(Frame masterFrame, NestedFrame detailFrame)> NestedDetailObjectChanged(Type nestedType, Type childType){
            return MasterDetailFrames(nestedType, childType).SelectMany(_ => {
                return _.masterFrame.View.WhenCurrentObjectChanged().Select(tuple => _);
            });
        }

        public static IObservable<(ObjectsGettingEventArgs e,TSignal signals, Frame masterFrame, NestedFrame detailFrame)>
            AddNestedNonPersistentObjects<TSignal>(Type masterObjectType, Type detailObjectType,
                Func<(Frame masterFrame, NestedFrame detailFrame), IObservable<TSignal>> addSignal){

            return Observable.Create<(ObjectsGettingEventArgs e,TSignal signals, Frame masterFrame, NestedFrame detailFrame)>(
                observer => {
                    return NestedDetailObjectChanged(masterObjectType, detailObjectType)
                        .SelectMany(_ => AddNestedNonPersistentObjectsCore(addSignal, _, observer))
                        .Subscribe(response => {},() => {});
                });
        }

        public static IObservable<(ObjectsGettingEventArgs e, TSignal signal, Frame masterFrame, NestedFrame
                detailFrame)>
            AddNestedNonPersistentObjects<TSignal>(this IObservable<(Frame masterFrame,NestedFrame detailFrame)> source,
                Func<(Frame masterFrame, NestedFrame detailFrame), IObservable<TSignal>> addSignal){

            return source.SelectMany(tuple => {
                return Observable.Create<(ObjectsGettingEventArgs e,TSignal signal, Frame masterFrame, NestedFrame detailFrame)>(
                    observer => AddNestedNonPersistentObjectsCore(addSignal, tuple, observer).Subscribe());

            });
        }

        private static IObservable<TSignal> AddNestedNonPersistentObjectsCore<TSignal>(Func<(Frame masterFrame, NestedFrame detailFrame), IObservable<TSignal>> addSignal,
            (Frame masterFrame, NestedFrame detailFrame) _, IObserver<(ObjectsGettingEventArgs e, TSignal signal, Frame masterFrame, NestedFrame detailFrame)> observer){
            return addSignal(_)
                .When(_.masterFrame, _.detailFrame)
                .ObserveOn(SynchronizationContext.Current)
                .Select(signals => {
                    using (var unused = ((NonPersistentObjectSpace) _.detailFrame.View.ObjectSpace)
                        .WhenObjectsGetting()
                        .Do(tuple => observer.OnNext((tuple.e, signals, _.masterFrame, _.detailFrame)),() => {})
                        .Subscribe()){
                        ((ListView) _.detailFrame.View).CollectionSource.ResetCollection();
                    }

                    return signals;
                });
        }
    }

}