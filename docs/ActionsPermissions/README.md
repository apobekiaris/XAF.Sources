# Actions Permissions package


## Introduction

The Actions Permission package adds permissions to the actions:

![Role detail view screenshot](images/RoleDetailView.png)


This extension is working only with [Permission Policies](https://docs.devexpress.com/eXpressAppFramework/116172/concepts/security-system/permission-policies) and is not compatible with the old permission system.

It consists  of two packages: `PocketXAF.ActionsPermissions.SourcePackages` and `PocketXAF.ActionPermissions.BaseImpl.SourcePackages`. 


The `PocketXAF.ActionsPermissions.SourcePackages` contains core functionality and base business objects. 

The `PocketXAF.ActionPermissions.BaseImpl.SourcePackages` contains implementation for the business objects, it also references the `PocketXAF.ActionsPermissions.SourcePackages` package. It should be used for projects without custom role classes. 

## Using with default role class

Install package  `PocketXAF.ActionPermissions.BaseImpl.SourcePackages` in the platform agnostic module:

```Powershell
Install-Package PocketXAF.ActionPermissions.BaseImpl.SourcePackages
```

Add Reference to the ActionPermissionsModule in the module designer or in the source code :

```C#
this.RequiredModuleTypes.Add(typeof(PocketXAF.ActionsPermissions.ActionsPermissionsModule));
```

Change the RoleType property in the application:

```C#
this.securityStrategyComplex1.RoleType = typeof(PocketXAF.ActionsPermissions.BaseImpl.PermissionPolicyRoleWithAction);
```

Since the PermissionPolicyRoleWith action type inherits PermissionPolicyRole, all Your roles will still be available, but they will not contain the ActionsPermission collection in the detail view. Therefore they must be converted to the PermissionPolicyRoleWithAction in the DatabaseUpdater. All new roles will be created as PermissionPolicyRoleWithAction and the ActionPermissions collection will be available in the DetailView



## Using custom role class

Install package  `PocketXAF.ActionPermissions.SourcePackages` in the platform agnostic module:

```Powershell
Install-Package PocketXAF.ActionPermissions.SourcePackages
```


Add Reference to the ActionPermissionsModule in the module designer or in the source code :

```C#
this.RequiredModuleTypes.Add(typeof(PocketXAF.ActionsPermissions.ActionsPermissionsModule));
```

Then add a new class `RoleActionPermission`, the inherts from `PocketXAF.ActionsPermissions.BusinessObjects.RoleActionPermissionBase`:

```C#
[DefaultListViewOptions(true, NewItemRowPosition.Top)]
public class RoleActionPermission : RoleActionPermissionBase
{

    public RoleActionPermission(Session session) : base(session)
    {
    }

    private YourCustomRole role;
    [Association]
    public YourCustomRole Role
    {
        get => role;
        set => SetPropertyValue(nameof(Role), ref role, value);
    }

    protected override IPermissionPolicyRoleWithActions GetRole() => Role;
}

```

Now the custom role class must implement the `IPermissionPolicyRoleWithActions` interface:


```C#
public class YourCustomRole : PermissionPolicyRole, IPermissionPolicyRoleWithActions
{

    private readonly RoleWithActionsHelper helper;
    /// <summary>
    /// Event is fired, when the list of available actions is needed for the drop-down data source
    /// </summary>
    public event EventHandler<RetrieveActionInfosEventArgs> RetrieveActionInfos;
    public YourCustomRole(Session session) : base(session)
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
    public IEnumerable<SecurableActionInfo> ActionInfos => helper.ActionInfos;

    IEnumerable<IRoleActionPermission> IPermissionPolicyRoleWithActions.ActionPermissions => ActionPermissions;

    public void RaiseRetrieveActionInfos(RetrieveActionInfosEventArgs e) => RetrieveActionInfos?.Invoke(this, e);
}
```

## Enabling permissions for actions

By default permissions are disabled for all actions.
Actions can be enabled for permissions in the model editor by setting the `EnablePermissions` property to `true`:

![EnablePermissions property](images/EnablePermissionsProperty.png)
