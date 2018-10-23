using System;

namespace XAF.SourcePackages.ObjectScripting
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreInScriptingAttribute : Attribute
    {
    }
}
