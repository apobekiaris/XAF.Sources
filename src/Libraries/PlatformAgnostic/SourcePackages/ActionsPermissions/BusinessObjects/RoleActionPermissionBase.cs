using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using PocketXAF.ActionsPermissions.Interfaces;

namespace PocketXAF.ActionsPermissions.BusinessObjects
{
	[DefaultListViewOptions(true, NewItemRowPosition.Top)]
	public abstract class RoleActionPermissionBase : BaseObject, IRoleActionPermission
	{

		protected RoleActionPermissionBase(Session session) : base(session)
		{
		}

		protected abstract IPermissionPolicyRoleWithActions GetRole();

		private string actionId;
		[VisibleInDetailView(false)]
		[VisibleInListView(false)]
		[VisibleInLookupListView(false)]
		[ModelDefault(nameof(IModelMember.Caption), "Action Id")]
		[Size(SizeAttribute.DefaultStringMappingFieldSize)]
		public string ActionId
		{
			get => actionId;
			set => SetPropertyValue(nameof(ActionId), ref actionId, value);
		}


		private SecurableActionInfo actionInfo;
		[NonPersistent]
		[VisibleInLookupListView(false)]
		[ModelDefault(nameof(IModelMember.Caption), "Aktion")]
		[DataSourceProperty("Role.ActionInfos")]
		public SecurableActionInfo ActionInfo
		{
			get
			{
				if (actionInfo == null && ActionId != null)
					actionInfo = GetRole().ActionInfos.FirstOrDefault(ra => ra.ActionId == ActionId);

				return actionInfo;
			}
			set => SetPropertyValue(nameof(ActionInfo), ref actionInfo, value);
		}

		private SecurityPermissionState permissionState;
		[ModelDefault(nameof(IModelMember.Caption), "Status")]
		public SecurityPermissionState PermissionState
		{
			get => permissionState;
			set => SetPropertyValue(nameof(PermissionState), ref permissionState, value);
		}

		protected override void OnChanged(string propertyName, object oldValue, object newValue)
		{
			base.OnChanged(propertyName, oldValue, newValue);
			if (propertyName == nameof(ActionInfo))
				ActionId = ActionInfo.ActionId;
		}
	}
}
