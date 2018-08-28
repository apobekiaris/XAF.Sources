using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketXAF.ActionsPermissions.BusinessObjects
{
    public class RetrieveActionInfosEventArgs : EventArgs
    {
        public IEnumerable<SecurableActionInfo> ActionInfos { get; set; }
    }
}
