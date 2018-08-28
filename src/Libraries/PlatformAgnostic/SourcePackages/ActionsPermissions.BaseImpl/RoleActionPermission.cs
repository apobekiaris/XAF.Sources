using DevExpress.ExpressApp;
using DevExpress.Xpo;
using PocketXAF.ActionsPermissions.Interfaces;

namespace PocketXAF.ActionsPermissions.BusinessObjects
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
