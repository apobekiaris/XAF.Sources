using System;
using System.Security;
using DevExpress.ExpressApp;
using XAF.SourcePackages.FrameExtensions;

namespace XAF.SourcePackages.ViewExtensions{
    [SecuritySafeCritical]
    public static class ViewExtensions {
        public static bool Fits(this View view,ViewType viewType=ViewType.Any,Nesting nesting=Nesting.Any,Type objectType=null) {
            objectType = objectType ?? typeof(object);
            return FitsCore(view, viewType)&&FitsCore(view,nesting)&&objectType.IsAssignableFrom(view.ObjectTypeInfo.Type);
        }

        private static bool FitsCore(View view, Nesting nesting) {
            return nesting == Nesting.Nested ? !view.IsRoot : nesting != Nesting.Root || view.IsRoot;
        }

        private static bool FitsCore(View view, ViewType viewType){
            if (view == null)
                return false;
            if (viewType == ViewType.ListView)
                return view is ListView;
            if (viewType == ViewType.DetailView)
                return view is DetailView;
            if (viewType == ViewType.DashboardView)
                return view is DashboardView;
            return true;
        }

        public static void Clean(this DetailView detailView,Frame frame) {
            frame.CleanDetailView();
        }

        
    }
}