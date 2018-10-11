using System;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using PocketXaf.SourcePackages.System.Reactive.Extensions;

namespace PocketXaf.SourcePackages.System.Reactive.Services{
    public static class XafApplicationExtensions{
        public static IObservable<(XafApplication, CreateCustomObjectSpaceProviderEventArgs e)> WhenCreateCustomObjectSpaceProvider(this XafApplication application){
            return Observable
                .FromEventPattern<EventHandler<CreateCustomObjectSpaceProviderEventArgs>,
                    CreateCustomObjectSpaceProviderEventArgs>(h => application.CreateCustomObjectSpaceProvider += h,
                    h => application.CreateCustomObjectSpaceProvider -= h)
                .TransformPattern<CreateCustomObjectSpaceProviderEventArgs,XafApplication>();
        }

        public static IObservable<(XafApplication, DatabaseVersionMismatchEventArgs e)> AlwaysUpdateOnDatabaseVersionMismatch(this XafApplication application){
            return application.WhenDatabaseVersionMismatch().Select(tuple => {
                tuple.e.Updater.Update();
                tuple.e.Handled = true;
                return tuple;
            });
        }

        public static IObservable<(XafApplication, DatabaseVersionMismatchEventArgs e)> WhenDatabaseVersionMismatch(this XafApplication application){
            return Observable
                .FromEventPattern<EventHandler<DatabaseVersionMismatchEventArgs>,
                    DatabaseVersionMismatchEventArgs>(h => application.DatabaseVersionMismatch += h,
                    h => application.DatabaseVersionMismatch -= h)
                .TransformPattern<DatabaseVersionMismatchEventArgs,XafApplication>();
        }


    }
}