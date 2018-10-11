using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using MoreLinq.Extensions;
using PocketXaf.SourcePackages.ConvertExtensions;
using PocketXaf.SourcePackages.FrameExtensions;
using PocketXaf.SourcePackages.TypeInfoExtensions;

namespace PocketXaf.SourcePackages.ModelExtensions{
    public static class ModelNodeExtensions {
        public static IEnumerable<IModelPropertyEditor> GetModelPropertyEditors(this IModelDetailView modelDetailView){
            var modelPropertyEditors = modelDetailView.Items.OfType<IModelPropertyEditor>();
            foreach (var modelPropertyEditor in modelPropertyEditors){
                if (modelPropertyEditor.ModelMember.MemberInfo.IsList){
                    if (modelPropertyEditor.View is IModelListView modelListView){
                        if (modelListView.MasterDetailMode==MasterDetailMode.ListViewAndDetailView){
                            foreach (var propertyEditor in modelListView.MasterDetailView.GetModelPropertyEditors()){
                                yield return propertyEditor;
                            }
                        }
                        else{
                            yield return modelPropertyEditor;
                        }
                    }
                }
                else{
                    yield return modelPropertyEditor;
                }
            }
        }


        public static ITypeInfo GetGenericListArgument(this IModelNode nodeByPath) {
            var type = nodeByPath.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(type)) {
                var genericModelList = type.GetInterfaces().First(type1 => typeof(IEnumerable).IsAssignableFrom(type1) && type1.IsGenericType);
                return XafTypesInfo.Instance.FindTypeInfo(genericModelList.GetGenericArguments()[0]);
            }
            return null;
        }

        public static IEnumerable<IModelChoiceActionItem> ActionChoiceItems(this IModelNode modelnode, Frame frame) {
            return modelnode.Application.ActionDesign.Actions.Where(action => action.ChoiceActionItems != null && action.ChoiceActionItems.Any()).SelectMany(action => action.ChoiceActionItems);
        }

        public static ActionBase ToAction(this IModelAction modelAction) {
            return modelAction.GetValue<ActionBase>(ModelActionsNodesGenerator.ActionPropertyName);
        }

        public static ActionBase ToAction(this IModelAction modelAction, Frame frame){
            return frame.Actions().First(@base => @base.Model.Id == modelAction.Id);
        }

        private static ExpressionEvaluator GetExpressionEvaluator(IModelNode dataSourceNode, CriteriaOperator criteriaOperator) {
            var typeInfo = dataSourceNode.GetGenericListArgument();
            var descendants = ReflectionHelper.FindTypeDescendants(typeInfo);
            var propertyDescriptors = descendants.SelectMany(info => info.Members).DistinctBy(info => info.Name).Select(info => new XafPropertyDescriptor(info,info.Name)).Cast<PropertyDescriptor>().ToArray();
            var evaluatorContextDescriptor = new EvaluatorContextDescriptorDefault(new PropertyDescriptorCollection(propertyDescriptors));
            return new ExpressionEvaluator(evaluatorContextDescriptor, criteriaOperator, false,
                XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.CustomFunctionOperators);    
        }

        public static IEnumerable<T> GetNodes<T>(this IEnumerable<T> modelNodes, string criteria) where T:IModelNode{
            var expressionEvaluator = GetExpressionEvaluator((IModelNode) modelNodes, CriteriaOperator.Parse(criteria));
            return expressionEvaluator!=null ? modelNodes.Where(arg => (bool) expressionEvaluator.Evaluate(arg)) : Enumerable.Empty<T>();
        }

        public static XPClassInfo QueryXPClassInfo(this IModelClass modelClass){
            return modelClass.TypeInfo.QueryXPClassInfo();
        }

        public static XPMemberInfo GetXpmemberInfo(this IModelMember modelMember){
            return ((XpoTypeInfoSource) ((TypeInfo) modelMember.ModelClass.TypeInfo).Source).XPDictionary.GetClassInfo(
                modelMember.ModelClass.TypeInfo.Type).FindMember(modelMember.Name);
        }

        public static TNode GetParent<TNode>(this IModelNode modelNode) where TNode : class, IModelNode{
            if (modelNode is TNode node)
                return node;
            var parent = modelNode.Parent;
            while (!(parent is TNode)) {
                parent = parent.Parent;
                if (parent == null)
                    break;
            }
            return (TNode) parent;
        }

        public static void Undo(this IModelNode modelNode){
            ((ModelNode) modelNode).Undo();
        }

        public static IModelNode FindNodeByPath(this IModelNode modelNode,string nodePath){
            return ModelEditorHelper.FindNodeByPath(nodePath, (ModelNode) modelNode);
        }

        public static string Path(this IModelNode modelNode){
            return ModelEditorHelper.GetModelNodePath((ModelNode) modelNode);
        }

        public static string Xml(this IModelNode modelNode) {
            return ((ModelNode) modelNode).Xml;
        }

        public static object GetValue<T>(this IModelNode modelNode, Expression<Func<T, object>> property){
            throw new NotImplementedException();
//            var name = typeof(T).GetMemberInfo(property).Name;
//            return modelNode.GetValue(name);
        }

        public static object GetValue(this IModelNode modelNode, string propertyName){
            var modelValueInfo = GetModelValueInfo(modelNode, propertyName);
            return GetValue(modelValueInfo.Item2, propertyName.Split('.').Last(), modelValueInfo.Item1.PropertyType);
        }

        public static Tuple<ModelValueInfo,IModelNode> GetModelValueInfo(this IModelNode modelNode, string propertyName) {
            if (propertyName.Contains(".")){
                var split = propertyName.Split('.');
                var strings = string.Join(".", split.Skip(1));
                var node = ((IModelNode) modelNode.GetValue(split.First()));
                return node.GetModelValueInfo(strings);
            }
            var modelValueInfo = ((ModelNode) modelNode).GetValueInfo(propertyName);
            return new Tuple<ModelValueInfo, IModelNode>(modelValueInfo, modelNode);
        }

        public static object GetValue(this IModelNode modelNode,string propertyName,Type propertyType) {
            return modelNode.CallMethod(new[]{propertyType}, "GetValue", propertyName);
        }

        public static void SetChangedValue(this IModelNode modelNode, string propertyName,  string value){
            modelNode.SetValue(propertyName, null,value);
        }

        public static void SetValue(this IModelNode modelNode,string propertyName,Type propertyType,object value){
            if (propertyType==null){
                var modelValueInfo = modelNode.GetModelValueInfo(propertyName).Item1;
                var changedValue = modelValueInfo.ChangedValue(value, modelValueInfo.PropertyType);
                modelNode.CallMethod(new[] { modelValueInfo.PropertyType }, "SetValue", propertyName, changedValue);
            }
            else
                modelNode.CallMethod(new[] { propertyType }, "SetValue", propertyName, value);
        }

        public static object ChangedValue(this ModelValueInfo modelValueInfo,object value, Type destinationType){
            var typeConverter = modelValueInfo.TypeConverter;
            return typeConverter != null ? typeConverter.ConvertFrom(value) : value.Change(destinationType);
        }

        public static bool IsRemovedNode(this IModelNode modelNode) {
            return ((ModelNode) modelNode).IsRemovedNode;
        }

        public static bool IsNewNode(this IModelNode modelNode) {
            return ((ModelNode) modelNode).IsNewNode;
        }

        public static bool HasValue(this IModelNode modelNode,params Type[] interfacesToSearch  ){
            var valueInfos = ((ModelNode) modelNode).NodeInfo.ValuesInfo.ToArray();
            var namesToSearch=valueInfos.Select(info => info.Name);
            if (interfacesToSearch != null){
                namesToSearch = interfacesToSearch.SelectMany(type => type.Properties()).Select(info => info.Name).ToArray();
            }
            return valueInfos.Where(info => namesToSearch.Contains(info.Name)).Select(info => modelNode.HasValue(info.Name)).Any();
        }

        public static string Id(this IModelNode modelNode) {
            return ((ModelNode) modelNode).Id;
        }

        public static IEnumerable<IModelLayoutViewItem> ViewItems(this IEnumerable<IModelViewLayoutElement> modelViewLayout,IModelMemberViewItem memberViewItem) {
            throw new NotImplementedException();
//            var layoutViewItems = modelViewLayout.GetItems<ModelNode>(ModelEditorHelper.GetChildNodes).OfType<IModelLayoutViewItem>();
//            return layoutViewItems.Where(item => item.ViewItem == memberViewItem);
        }
    }
}