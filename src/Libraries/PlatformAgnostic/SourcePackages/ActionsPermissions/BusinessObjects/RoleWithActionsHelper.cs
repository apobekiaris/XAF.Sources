using System.Collections.Generic;
using System.Linq;
using PocketXAF.ActionsPermissions.Interfaces;

namespace PocketXAF.ActionsPermissions.BusinessObjects
{
	public class RoleWithActionsHelper
	{

        public RoleWithActionsHelper(IPermissionPolicyRoleWithActions role)
		{
            Role = role;
        }

		public IPermissionPolicyRoleWithActions Role { get; }

		private IEnumerable<SecurableActionInfo> actionInfos;
		public IEnumerable<SecurableActionInfo> ActionInfos
		{
			get
			{
				if (actionInfos == null)
				{
					var args = new RetrieveActionInfosEventArgs();
					Role.RaiseRetrieveActionInfos(args);
					if (args.ActionInfos != null)
						actionInfos = args.ActionInfos.ToList().AsReadOnly();
				}

				return actionInfos;
			}
		}

	}
}
