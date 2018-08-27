using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace PocketXAF.ActionsPermissions.BusinessObjects
{
    [NonPersistent]
    public class SecurableActionInfo : BaseObject
    {
        public SecurableActionInfo(Session session) : base(session)
        {
        }

        public string ActionId
        {
            get => GetPropertyValue<string>(nameof(ActionId));
            set => SetPropertyValue(nameof(ActionId), value);
        }

        public string ActionName
        {
            get => GetPropertyValue<string>(nameof(ActionName));
            set => SetPropertyValue(nameof(ActionName), value);
        }

    }
}
