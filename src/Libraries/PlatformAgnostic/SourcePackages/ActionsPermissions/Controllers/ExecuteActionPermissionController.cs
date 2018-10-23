using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using XAF.ActionsPermissions.Model;
using XAF.SourcePackages.ActionsPermissions;

namespace XAF.ActionsPermissions.Controllers
{
	public class ExecuteActionPermissionController : ViewController, IModelExtender
    {
		public void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
		{
			extenders.Add<IModelAction, IModelActionExtension>();
		}

		protected override void OnActivated()
        {
            base.OnActivated();

            var actions = View.Model.Application.ActionDesign.Actions
                            .OfType<IModelActionExtension>()
                            .Where(a => a.EnablePermissions)
                            .Cast<IModelAction>().ToList();

            foreach (Controller controller in Frame.Controllers)
                foreach (ActionBase action in controller.Actions)
                    if (actions.Any(a => a.Id == action.Id))
                        action.Active[GetType().Name] = SecuritySystem.IsGranted(new ExecuteActionPermissionRequest(action.Id));
        }
    }
}
