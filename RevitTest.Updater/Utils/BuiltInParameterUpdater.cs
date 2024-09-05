﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitTest.Updater.Utils
{
    /// <summary>
    /// BuiltInParameterUpdater
    /// </summary>
    /// <remarks>Based: https://github.com/ricaun-io/RevitAddin.UpdaterTester/blob/master/RevitAddin.UpdaterTester/Updaters/BuiltInParameterUpdater.cs</remarks>
    public class BuiltInParameterUpdater : IUpdater
    {
        private UpdaterId updaterId;
        private Guid guid;
        public Dictionary<ElementId, BuiltInParameter[]> ElementIdChangeType { get; set; }
        public BuiltInParameterUpdater(AddInId addInId, Guid guid = default)
        {
            if (guid == default) guid = Guid.NewGuid();
            this.guid = guid;
            this.updaterId = new UpdaterId(addInId, GetId());
        }
        public void Execute(UpdaterData data)
        {
            var document = data.GetDocument();
            var ids = new List<ElementId>();
            ids.AddRange(data.GetAddedElementIds());
            ids.AddRange(data.GetModifiedElementIds());
            ids.AddRange(data.GetDeletedElementIds());

            var idChangeTypes = ids.ToDictionary(e => e,
                e => GetTriggeredBuiltInParameters(data, e).ToArray());

            ElementIdChangeType = idChangeTypes;

            foreach (var idChangeType in idChangeTypes)
            {
                var id = idChangeType.Key;
                var changes = idChangeType.Value;

                System.Console.WriteLine($"{id} [{string.Join(" ", changes)}]");
            }
        }

        private string GetValueParameter(Document document, ElementId elementId, BuiltInParameter builtInParameter)
        {
            var value = "";
            if (document.GetElement(elementId) is Element element)
            {
                value = (builtInParameter == BuiltInParameter.INVALID) ? "ANY" : "INVALID";
                if (element.get_Parameter(builtInParameter) is Parameter parameter)
                {
                    value = parameter.AsValueString();
                    if (parameter.StorageType == StorageType.String)
                        value = parameter.AsString();
                }
            }

            return value;
        }

        public string GetAdditionalInformation() => "All Parameter Updater Tester";
        public ChangePriority GetChangePriority() => ChangePriority.MEPCalculations;
        public UpdaterId GetUpdaterId() => this.updaterId;
        public string GetUpdaterName() => "UpdaterTester";
        public Guid GetId() => guid;
        public ElementFilter GetElementFilter()
        {
            return new ElementCategoryFilter(BuiltInCategory.INVALID, true);
        }
        private void AddTriggerAllBuiltInParameter()
        {
            var elementFilter = GetElementFilter();
            var changeTypes = GetChangeTypes();
            foreach (var changeType in changeTypes)
            {
                UpdaterRegistry.AddTrigger(GetUpdaterId(), elementFilter, changeType);
            }

            UpdaterRegistry.AddTrigger(GetUpdaterId(), elementFilter, Element.GetChangeTypeAny());
        }

        private IEnumerable<ChangeType> GetChangeTypes()
        {
            //return new[] { 
            //    Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)),
            //    Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.HOST_VOLUME_COMPUTED))
            //};
            var values = Enum.GetValues(typeof(BuiltInParameter)).Cast<BuiltInParameter>();
            var changeTypes = values.Select(e => Element.GetChangeTypeParameter(new ElementId(e)));
            return changeTypes;
        }

        private IEnumerable<BuiltInParameter> GetTriggeredBuiltInParameters(UpdaterData data, ElementId elementId)
        {
            var values = Enum.GetValues(typeof(BuiltInParameter)).Cast<BuiltInParameter>();
            var changes = values.Where(e =>
                    data.IsChangeTriggered(elementId, Element.GetChangeTypeParameter(new ElementId(e)))
                );

            if (changes.Any())
                return changes.Distinct();

            return new[] { BuiltInParameter.INVALID };
        }

        public void Enable()
        {
            if (UpdaterRegistry.IsUpdaterRegistered(GetUpdaterId()))
                UpdaterRegistry.EnableUpdater(GetUpdaterId());
        }
        public void Disable()
        {
            if (UpdaterRegistry.IsUpdaterRegistered(GetUpdaterId()))
                UpdaterRegistry.DisableUpdater(GetUpdaterId());
        }

        public void Register()
        {
            if (UpdaterRegistry.IsUpdaterRegistered(GetUpdaterId())) return;

            UpdaterRegistry.RegisterUpdater(this, true);

            AddTriggerAllBuiltInParameter();
        }

        public void Unregister()
        {
            if (!UpdaterRegistry.IsUpdaterRegistered(GetUpdaterId())) return;

            UpdaterRegistry.RemoveAllTriggers(GetUpdaterId());
            UpdaterRegistry.UnregisterUpdater(GetUpdaterId());
        }
    }
}