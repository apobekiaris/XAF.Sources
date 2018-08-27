using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;

namespace PocketXAF.ActionsPermissions.Interfaces
{
	public interface IRoleActionPermission
	{
		string ActionId { get; }
		SecurityPermissionState PermissionState { get; }
	}
}
