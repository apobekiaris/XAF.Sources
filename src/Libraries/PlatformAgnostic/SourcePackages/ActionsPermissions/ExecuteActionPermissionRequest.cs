using System;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Security;

namespace PocketXAF.ActionsPermissions
{
	[Serializable]
	[DataContract]
    public class ExecuteActionPermissionRequest : IPermissionRequest
    {

		[DataMember]
        public string ActionId { get; set; }

        public ExecuteActionPermissionRequest(string actionId) => ActionId = actionId;

        object IPermissionRequest.GetHashObject() => ActionId.GetHashCode();
    }
}
	