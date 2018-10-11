using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Fasterflect;
using PocketXaf.SourcePackages.Base;
using PocketXaf.SourcePackages.XafApplicationExtensions;

namespace PocketXaf.SourcePackages.ModelExtensions{
    public static class ModelApplicationBaseExtensions {
        public static Platform GetPlatform(this IModelApplication application){
            return ((IModelSources) application).Modules.GetPlatform();
        }

        public static Platform GetPlatform(this ModelApplicationBase applicationBase){
            return ((IModelApplication) applicationBase).GetPlatform();
        }

//        static ModelApplicationBase _strategiesModel;
        public static ModelApplicationBase StrategiesModel(this IModelApplication application, IEnumerable<ModelApplicationBase> modelApplicationBases) {
            throw new NotImplementedException();
//            if (_strategiesModel == null) {
//                var strategies = ((IModelOptionsMergedDifferenceStrategy)application.Application.Options).MergedDifferenceStrategies;
//                var xml = $"<Application><Options>{((ModelNode) strategies).Xml}</Options></Application>";
//                var modelApplicationBase = ((ModelApplicationBase) application).CreatorInstance.CreateModelApplication();
//                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
//                ReadFromOtherLayers(modelApplicationBases, modelApplicationBase);
//                UpdateRemovedNodes(modelApplicationBase);
//                _strategiesModel = modelApplicationBase;
//            }
//            return _strategiesModel;
        }

//        static void ReadFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases,ModelApplicationBase modelApplicationBase) {
//            throw new NotImplementedException();
////            foreach (var applicationBase in modelApplicationBases.Cast<IModelApplication>()){
////                var mergedDifferenceStrategy = ((IModelOptionsMergedDifferenceStrategy) applicationBase.Options);
////                var xml = mergedDifferenceStrategy?.Xml();
////                if (!string.IsNullOrEmpty(xml)){
////                    xml = $"<Application>{xml}</Application>";
////                    new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
////                }
////            }
//        }

//        static void UpdateRemovedNodes(IModelNode modelNode) {
//            for (int i = modelNode.NodeCount - 1; i >= 0; i--) {
//                var node = modelNode.GetNode(i);
//                var modelNode1 = ((ModelNode)node);
//                if (CanBeRemoved(modelNode1))
//                    node.Remove();
//                UpdateRemovedNodes(node);
//            }
//        }

//        static bool CanBeRemoved(ModelNode modelNode1) {
//            if (modelNode1.IsRemovedNode) {
//                for (int i = modelNode1.NodeCount - 1; i >= 0; i--) {
//                    if (!CanBeRemoved(modelNode1.GetNode(i)))
//                        return false;
//                }
//                return !modelNode1.IsNewNode;
//            }
//            return false;
//        }

        public static void RemoveLayer(this ModelApplicationBase application){
            ModelApplicationHelper.RemoveLayer(application);
        }

        public static void RemoveLayer(this ModelApplicationBase application, string id) {
            RefreshLayers(application, @base => @base.Id == id ? null : @base);
        }

        public static void ReplaceLayer(this ModelApplicationBase application, ModelApplicationBase layer) {
            RefreshLayers(application, @base => application.LastLayer.Id == layer.Id ? layer : @base);
        }

        static void RefreshLayers(ModelApplicationBase application, Func<ModelApplicationBase, ModelApplicationBase> func) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            var lastLayer = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(application);
            var afterSetup = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(application);
            while (application.LastLayer.Id != "Unchanged Master Part") {
                ModelApplicationBase modelApplicationBase = application.LastLayer;
                modelApplicationBase = func.Invoke(modelApplicationBase);
                if (modelApplicationBase != null)
                    modelApplicationBases.Add(modelApplicationBase);
                ModelApplicationHelper.RemoveLayer(application);
            }
            modelApplicationBases.Reverse();
            foreach (var modelApplicationBase in modelApplicationBases) {
                ModelApplicationHelper.AddLayer(application, modelApplicationBase);
            }
            ModelApplicationHelper.AddLayer(application, afterSetup);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static void AddLayer(this ModelApplicationBase application, ModelNode layer) {
            ModelApplicationHelper.AddLayer(application, (ModelApplicationBase) layer);
        }

        public static void InsertLayer(this ModelApplicationBase application, int index,ModelApplicationBase layer) {
            application.CallMethod("InsertLayerAtInternal", layer, index);
        }

        public static void AddLayerBeforeLast(this ModelApplicationBase application, ModelApplicationBase layer) {
            ModelApplicationBase lastLayer = application.LastLayer;
            if (lastLayer.Id != "After Setup" && lastLayer.Id != "UserDiff")
                throw new ArgumentException("LastLayer.Id", lastLayer.Id);
            ModelApplicationHelper.RemoveLayer(application);
            ModelApplicationHelper.AddLayer(application, layer);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static ModelApplicationBase GetLayer(this ModelApplicationBase modelApplicationBase, string id){
            var modelNodeWrapper = modelApplicationBase.GetLayers().FirstOrDefault(wrapper => wrapper.ModelNode.Id==id);
            return (ModelApplicationBase) modelNodeWrapper?.ModelNode;
        }

        public static ModelApplicationBase GetLayer(this ModelApplicationBase modelApplicationBase, int index) {
            return (ModelApplicationBase) ((List<ModelNode>)modelApplicationBase.GetPropertyValue("Layers"))[index];
        }

        public static List<ModelNodeWrapper> GetLayers(this ModelApplicationBase modelApplicationBase) {
            return ((List<ModelNode>)modelApplicationBase.GetPropertyValue("Layers")).Select(node => new ModelNodeWrapper(node)).ToList();
        }

        public static void ReInitLayers(this ModelApplicationBase modelApplicationBase) {
            if (modelApplicationBase.Id == "Application") {
                var lastLayer = modelApplicationBase.LastLayer;
                while (lastLayer.Id != "Unchanged Master Part") {
                    ModelApplicationHelper.RemoveLayer(lastLayer);
                    lastLayer = modelApplicationBase.LastLayer;
                }
                var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
                afterSetupLayer.Id = "After Setup";
                ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetupLayer);
            }
        }

        public static bool HasAspect(this ModelApplicationBase modelApplicationBase, string aspectName) {
            for (int i = 0; i < modelApplicationBase.AspectCount; i++) {
                if (modelApplicationBase.GetAspect(i) == aspectName)
                    return true;
            }
            return false;
        }
    }
    public class ModelNodeWrapper {
        readonly ModelNode _modelNode;

        public ModelNodeWrapper(ModelNode modelNode) {
            _modelNode = modelNode;
        }

        public ModelNode ModelNode => _modelNode;

        public override string ToString() {
            return _modelNode.Id;
        }
    }

}