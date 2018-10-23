using DevExpress.ExpressApp;
using DevExpress.Xpo;
using XAF.ActionsPermissions.BusinessObjects;
using XAF.ActionsPermissions.Interfaces;

namespace XAF.ActionsPermissions.BaseImpl
{
	[DefaultListViewOptions(true, NewItemRowPosition.Top)]
	public class RoleActionPermission : RoleActionPermissionBase
    {

		public RoleActionPermission(Session session) : base(session)
		{
		}

		private PermissionPolicyRoleWithAction role;
        [Association]
        public PermissionPolicyRoleWithAction Role
        {
            get => role;
            set => SetPropertyValue(nameof(Role), ref role, value);
        }

		protected override IPermissionPolicyRoleWithActions GetRole() => Role;
	}
}
