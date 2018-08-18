using DevExpress.ExpressApp.DC;

namespace PocketXaf.SourcePackages.ObjectScripting
{
    [DomainComponent]
    public class ScriptingResult
    {
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public string Script { get; set; }
    }
}
