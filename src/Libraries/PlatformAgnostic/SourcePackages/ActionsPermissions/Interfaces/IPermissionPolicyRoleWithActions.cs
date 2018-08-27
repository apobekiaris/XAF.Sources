using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using PocketXAF.ActionsPermissions.BusinessObjects;

namespace PocketXAF.ActionsPermissions.Interfaces
{
    public interface IPermissionPolicyRoleWithActions : IPermissionPolicyRole
	{
		event EventHandler<RetrieveActionInfosEventArgs> RetrieveActionInfos;

		void RaiseRetrieveActionInfos(RetrieveActionInfosEventArgs e);
		IEnumerable<SecurableActionInfo> ActionInfos { get; }

		IEnumerable<IRoleActionPermission> ActionPermissions { get; }
	}
}
