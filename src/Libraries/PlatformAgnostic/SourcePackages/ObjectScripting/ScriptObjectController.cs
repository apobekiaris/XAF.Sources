using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XAF.SourcePackages.ObjectScripting
{
    public class ScriptObjectController : ViewController
    {
        public ScriptObjectController()
        {
            ScriptObjectAction = new SimpleAction(this, "ScriptObjectAction", PredefinedCategory.Tools);
            ScriptObjectAction.Caption = "Generate Script";
            ScriptObjectAction.Execute += scriptObjectAction_Execute;
            ScriptObjectAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
        }

        public SimpleAction ScriptObjectAction { get; }

        void scriptObjectAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ObjectScriptGenerator creator = new ObjectScriptGenerator();
            var result = new ScriptingResult() { Script = creator.GenerateScript(e.SelectedObjects.Cast<object>().ToList()) };
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(Application.CreateObjectSpace(result.GetType()), result);
        }
    }
}
