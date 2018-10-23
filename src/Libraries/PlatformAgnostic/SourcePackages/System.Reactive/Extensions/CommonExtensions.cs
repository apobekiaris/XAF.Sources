using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace XAF.SourcePackages.System.Reactive.Extensions{
    public static class CommonExtensions{
        public static IConnectableObservable<T> BufferUntilSubscribed<T>(this IObservable<T> source) {
            return new BufferUntilSubscribedObservable<T>(source, Scheduler.Immediate);
        }

        public static IObservable<(T sender, TEventArgs e)> TransformPattern<TEventArgs,T>(this IObservable<EventPattern<TEventArgs>> source) where TEventArgs:EventArgs{
            return source.Select(pattern => ((T) pattern.Sender, pattern.EventArgs));
        }

        public static IObservable<(TDisposable frame, EventArgs args)> WhenDisposed<TDisposable>(
            this TDisposable source) where TDisposable : IComponent{
            return Observable.Return(source).Disposed();
        }

        public static IObservable<Unit> ToUnit<T>(
            this IObservable<T> source){
            return source.Select(o => Unit.Default);
        }

        public static IObservable<(TDisposable frame,EventArgs args)> Disposed<TDisposable>(this IObservable<TDisposable> source) where TDisposable:IComponent{
            return source
                .SelectMany(item => {
                    return Observable.FromEventPattern<EventHandler, EventArgs>(h => item.Disposed += h, h => item.Disposed -= h);
                })
                .Select(pattern => pattern)
                .TransformPattern<EventArgs,TDisposable>();
        }

        internal static IObservable<T> AsObservable<T>(this T self, IScheduler scheduler = null){
            scheduler = scheduler ?? Scheduler.Immediate;
            return Observable.Return(self, scheduler);
        }
    }
}