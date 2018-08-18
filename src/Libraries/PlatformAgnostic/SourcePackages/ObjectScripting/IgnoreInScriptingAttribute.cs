using System;

namespace PocketXaf.SourcePackages.ObjectScripting
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreInScriptingAttribute : Attribute
    {
    }
}
