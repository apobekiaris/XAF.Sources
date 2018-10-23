using System;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Security;

namespace XAF.SourcePackages.ActionsPermissions {
    [Serializable]
    [DataContract]
    public class ExecuteActionPermissionRequest : IPermissionRequest {
        public ExecuteActionPermissionRequest(string actionId) {
            ActionId = actionId;
        }

        [DataMember]
        public string ActionId { get; set; }

        object IPermissionRequest.GetHashObject() {
            return ActionId.GetHashCode();
        }
    }
}