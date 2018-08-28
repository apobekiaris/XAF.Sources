﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using PocketXAF.ActionsPermissions;

namespace PocketXAF.ActionsPermissions
{
    public class ActionsPermissionsModule : ModuleBase
    {

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if (application.Security is SecurityStrategy securityStrategy)
                securityStrategy.CustomizeRequestProcessors += SecurityStrategy_CustomizeRequestProcessors;
        }

        private void SecurityStrategy_CustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e)
        {
            e.Processors.Add(typeof(ExecuteActionPermissionRequest), new ExecuteActionRequestProcessor());
        }
    }
}
