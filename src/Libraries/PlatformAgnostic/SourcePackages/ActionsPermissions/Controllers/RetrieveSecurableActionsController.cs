using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using PocketXAF.ActionsPermissions.BusinessObjects;
using PocketXAF.ActionsPermissions.Interfaces;
using PocketXAF.ActionsPermissions.Model;

namespace PocketXAF.ActionsPermissions.Controllers
{
	public class RetrieveSecurableActionsController : ObjectViewController<DetailView, IPermissionPolicyRoleWithActions>
	{
		protected override void OnActivated()
		{
			base.OnActivated();
			View.CurrentObjectChanged += View_CurrentObjectChanged;
			AttachObjectEvents();
		}


		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			View.CurrentObjectChanged -= View_CurrentObjectChanged;
			DetachObjectEvents();
		}

		private void View_CurrentObjectChanged(object sender, EventArgs e)
		{
			DetachObjectEvents();
			AttachObjectEvents();
		}

		private void AttachObjectEvents()
		{
			if (ViewCurrentObject != null)
			{
				DetachObjectEvents();
				ViewCurrentObject.RetrieveActionInfos += ViewCurrentObject_RetrieveActionInfos;
			}
		}

		private void DetachObjectEvents()
		{
			if (ViewCurrentObject != null)
				ViewCurrentObject.RetrieveActionInfos -= ViewCurrentObject_RetrieveActionInfos;

		}

		private void ViewCurrentObject_RetrieveActionInfos(object sender, RetrieveActionInfosEventArgs e)
		{
			e.ActionInfos = View.Model.Application.ActionDesign.Actions
				.OfType<IModelActionExtension>()
				.Where(a => a.EnablePermissions)
				.Cast<IModelAction>()
				.Select(a => CreateActionInfo(a));
		}

		private SecurableActionInfo CreateActionInfo(IModelAction modelAction)
		{
			var result = ObjectSpace.CreateObject<SecurableActionInfo>();
			result.ActionId = modelAction.Id;
			result.ActionName = modelAction.Caption;
			return result;
		}
	}
}
