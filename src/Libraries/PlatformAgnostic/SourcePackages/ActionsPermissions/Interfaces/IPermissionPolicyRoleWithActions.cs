using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;
using PocketXAF.ActionsPermissions.BusinessObjects;

namespace PocketXAF.ActionsPermissions.Interfaces
{
	public interface IPermissionPolicyRoleWithActions : IPermissionPolicyRole
	{
		event EventHandler<RetrieveActionInfosEventArgs> RetrieveActionInfos;

		void RaiseRetrieveActionInfos(RetrieveActionInfosEventArgs e);
		IReadOnlyCollection<SecurableActionInfo> ActionInfos { get; }

		IEnumerable<IRoleActionPermission> ActionPermissions { get; }
	}
}
