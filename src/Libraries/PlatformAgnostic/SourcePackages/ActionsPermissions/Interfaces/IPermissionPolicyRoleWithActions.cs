using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using XAF.ActionsPermissions.BusinessObjects;

namespace XAF.ActionsPermissions.Interfaces
{
    public interface IPermissionPolicyRoleWithActions : IPermissionPolicyRole
	{
		event EventHandler<RetrieveActionInfosEventArgs> RetrieveActionInfos;

		void RaiseRetrieveActionInfos(RetrieveActionInfosEventArgs e);
		IEnumerable<SecurableActionInfo> ActionInfos { get; }

		IEnumerable<IRoleActionPermission> ActionPermissions { get; }
	}
}
