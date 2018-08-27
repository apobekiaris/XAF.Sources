using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using PocketXAF.ActionsPermissions.Interfaces;

namespace PocketXAF.ActionsPermissions.BusinessObjects
{
	[NavigationItem("Administration")]
	[ModelDefault(nameof(IModelClass.Caption), "Roles")]
	[MapInheritance(MapInheritanceType.ParentTable)]
	public class PermissionPolicyRoleWithAction : PermissionPolicyRole, IPermissionPolicyRoleWithActions
	{

		private readonly RoleWithActionsHelper helper;
        /// <summary>
        /// Event is fired, when the list of available actions is needed for the drop-down data source
        /// </summary>
        public event EventHandler<RetrieveActionInfosEventArgs> RetrieveActionInfos;
        public PermissionPolicyRoleWithAction(Session session) : base(session)
        {
			helper = new RoleWithActionsHelper(this);
        }
	
		[Association, Aggregated]
        public XPCollection<RoleActionPermission> ActionPermissions
        {
            get => GetCollection<RoleActionPermission>(nameof(ActionPermissions));
        }

		[VisibleInDetailView(false)]
		[VisibleInListView(false)]
		[VisibleInLookupListView(false)]
		[ModelDefault(nameof(IModelMember.Caption), "Action Infos")]
		public IReadOnlyCollection<SecurableActionInfo> ActionInfos => helper.ActionInfos;

		IEnumerable<IRoleActionPermission> IPermissionPolicyRoleWithActions.ActionPermissions => ActionPermissions;

		public void RaiseRetrieveActionInfos(RetrieveActionInfosEventArgs e) => RetrieveActionInfos?.Invoke(this, e);
	}
}
