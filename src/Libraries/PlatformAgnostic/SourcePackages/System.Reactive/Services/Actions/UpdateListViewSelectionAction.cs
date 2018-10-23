using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace XAF.SourcePackages.System.Reactive.Services.Actions{
    [ModelAbstractClass]
    public interface IModelColumnUpdateSelectionMember:IModelColumn{
        [Category("eXpand")]
        [Description("Populates a signlechoise action with all columns having this flag and allows to revert all values")]
        [ModelBrowsable(typeof(ColumnUpdateSelectionMemberVisibilityCalculator))]
        bool UpdateSelectionMember{ get; set; }
    }

    public class ColumnUpdateSelectionMemberVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            return ((IModelColumnUpdateSelectionMember) node).ModelMember.MemberInfo.MemberType == typeof(bool);
        }
    }

    internal class UpdateListViewSelectionAction{
        
        private static IEnumerable<ChoiceActionItem> StateItems(){
            yield return new ChoiceActionItem("Enable","Enable");
            yield return new ChoiceActionItem("Disable","Disable");
            yield return new ChoiceActionItem("Revert","Revert");
        }

        public static ActionBase Register(Controller controller){
            
            var singleChoiceAction = new SingleChoiceAction(controller, "UpdateSelectionMember",PredefinedCategory.Menu){TargetViewType = ViewType.ListView,ItemType = SingleChoiceActionItemType.ItemIsOperation};
            singleChoiceAction.Active[singleChoiceAction.Id] = false;
            controller
                .When(singleChoiceAction)
                .SelectMany(_ => {
                    var modelColumnUpdateSelectionMembers = ((ListView) _.Frame.View).Model.Columns
                        .Cast<IModelColumnUpdateSelectionMember>().Where(member => member.UpdateSelectionMember)
                        .ToArray();
                    return PopulateItems(singleChoiceAction, modelColumnUpdateSelectionMembers);
                })
                .Subscribe();

            singleChoiceAction
                .WhenExecuted()
                .SelectMany(tuple => {
                    var memberInfo = ((IModelColumnUpdateSelectionMember) tuple.e.SelectedChoiceActionItem.ParentItem.Data).ModelMember.MemberInfo;
                    if (memberInfo.MemberType==typeof(bool))
                        return tuple.e.SelectedObjects.Cast<object>().ToObservable()
                            .Select(obj => UpdateMemberValue(memberInfo, obj, tuple))
                            .Finally(() => tuple.objectSpace.CommitChanges());
                    return Observable.Empty<SingleChoiceAction>();
                })
                .Subscribe();

            return singleChoiceAction;
        }

        private static SingleChoiceAction UpdateMemberValue(IMemberInfo memberInfo, object obj,
            (SingleChoiceAction simpleAction, SingleChoiceActionExecuteEventArgs e, IObjectSpace objectSpace, Controller
                controller, View view, XafApplication application) tuple){
            var value = !(bool) memberInfo.GetValue(obj);
            if ((string) tuple.e.SelectedChoiceActionItem.Data == "Enable")
                value = true;
            else if ((string) tuple.e.SelectedChoiceActionItem.Data == "Disable")
                value = false;

            memberInfo.SetValue(obj, value);
            return tuple.simpleAction;
        }

        private static IObservable<SingleChoiceAction> PopulateItems(SingleChoiceAction action,
            IModelColumnUpdateSelectionMember[] modelColumnUpdateSelectionMembers){
            action.Active.BeginUpdate();
            action.Active[action.Id] = true;
            action.Active.EndUpdate();
            action.Items.Clear();
            return modelColumnUpdateSelectionMembers.ToObservable().Select(member => {
                var choiceActionItem = new ChoiceActionItem(member.Caption, member);
                choiceActionItem.Items.AddRange(StateItems().ToArray());
                return choiceActionItem;
            }).Do(item => action.Items.Add(item)).Select(item => action);
        }
    }
}