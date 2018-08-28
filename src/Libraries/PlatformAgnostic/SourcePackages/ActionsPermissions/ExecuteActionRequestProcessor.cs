using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using PocketXAF.ActionsPermissions.Interfaces;

namespace PocketXAF.ActionsPermissions
{
	public class ExecuteActionRequestProcessor : PermissionRequestProcessorBase<ExecuteActionPermissionRequest>
	{

		public override bool IsGranted(ExecuteActionPermissionRequest permissionRequest)
		{
			return ((IPermissionPolicyUser)SecuritySystem.CurrentUser).Roles.OfType<IPermissionPolicyRoleWithActions>().Any(role =>
			{
				var action = role.ActionPermissions.FirstOrDefault(r => r.ActionId == permissionRequest.ActionId);
				return (action != null) ? action.PermissionState == SecurityPermissionState.Allow : role.PermissionPolicy == SecurityPermissionPolicy.AllowAllByDefault;
			});
		}
	}
}