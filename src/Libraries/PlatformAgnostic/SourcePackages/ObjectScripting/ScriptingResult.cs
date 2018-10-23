using DevExpress.ExpressApp.DC;

namespace XAF.SourcePackages.ObjectScripting
{
    [DomainComponent]
    public class ScriptingResult
    {
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public string Script { get; set; }
    }
}
