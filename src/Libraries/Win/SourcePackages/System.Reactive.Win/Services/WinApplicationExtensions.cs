using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using PocketXaf.SourcePackages.System.Reactive.Extensions;

namespace PocketXaf.Win.SourcePackages.System.Reactive.Win.Services{
    public static class WinApplicationExtensions{

        public static IObservable<(T view, StatusUpdatingEventArgs e)> WhenStatusUpdating<T>(this T view) where T : WinApplication{
            return Observable.Return(view).StatusUpdating();
        }

        public static IObservable<(T view, StatusUpdatingEventArgs e)> StatusUpdating<T>(this IObservable<T> source)
            where T : WinApplication{
            return source.SelectMany(item => {
                return Observable.FromEventPattern<EventHandler<StatusUpdatingEventArgs>, StatusUpdatingEventArgs>(
                        handler => item.StatusUpdating += handler,
                        handler => item.StatusUpdating -= handler)
                    .TakeUntil(item.WhenDisposed());
            }).TransformPattern<StatusUpdatingEventArgs, T>();
        }

        public static IObservable<(Form form, EventArgs e)> WhenActivated(this Form form){
            return Observable.Return(form).Activated();
        }

        public static IObservable<(Form form, EventArgs e)> Activated(this IObservable<Form> source){
            
            return source.SelectMany(item => {
                    return Observable.FromEventPattern<EventHandler, EventArgs>(
                            handler => item.Activated += handler,
                            handler => item.Activated -= handler)
                        .TakeUntil(item.WhenDisposed());
                })

                .TransformPattern<EventArgs, Form>();
        }
    }

}