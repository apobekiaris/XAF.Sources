using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF.ActionsPermissions.BusinessObjects
{
    public class RetrieveActionInfosEventArgs : EventArgs
    {
        public IEnumerable<SecurableActionInfo> ActionInfos { get; set; }
    }
}
