using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars.Docking2010.Views;
using XAF.SourcePackages.ActiveViewDocumentController;

namespace XAF.Win.SourcePackages.ActiveViewDocumentController.Win{
    public class ActiveDocumentViewController:XAF.SourcePackages.ActiveViewDocumentController.ActiveDocumentViewController{

        protected override void OnActivated(){
            base.OnActivated();
            Frame.TemplateChanged += Frame_TemplateChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.TemplateChanged -= Frame_TemplateChanged;
            if (Frame.Template is IXafDocumentsHostWindow hostWindow) {
                hostWindow.DocumentManager.ViewChanged -= DocumentManager_ViewChanged;
                hostWindow.DocumentManager.ViewChanging -= DocumentManager_ViewChanging;
            }
        }

        void Frame_TemplateChanged(object sender, EventArgs e) {
            if (((IModelOptionsWin)Application.Model.Options).UIType == UIType.TabbedMDI && Frame.Template is IXafDocumentsHostWindow hostWindow) {
                hostWindow.DocumentManager.ViewChanged += DocumentManager_ViewChanged;
                hostWindow.DocumentManager.ViewChanging += DocumentManager_ViewChanging;
            }
        }
        void DocumentManager_ViewChanging(object sender, DevExpress.XtraBars.Docking2010.ViewEventArgs args) {
            args.View.DocumentActivated -= View_DocumentActivated;
        }
        void DocumentManager_ViewChanged(object sender, DevExpress.XtraBars.Docking2010.ViewEventArgs args) {
            args.View.DocumentActivated += View_DocumentActivated;
        }
        void View_DocumentActivated(object sender, DocumentEventArgs e){
            if (e.Document.Form is IViewHolder documentForm) {
                OnActiveViewChanged(new ActiveViewChangedEventArgs(documentForm.View));
            }
        }
    }
}
