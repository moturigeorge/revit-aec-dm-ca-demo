using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

namespace revit_aec_dm_ca_demo
{
    public class SelectionChangedEventHandler : IExternalEventHandler
    {
        private readonly AttributesPanelProvider _customPanelProvider;
        private readonly Guid _panelGuid;

        public SelectionChangedEventHandler(AttributesPanelProvider customPanelProvider, Guid panelGuid)
        {
            _customPanelProvider = customPanelProvider;
            _panelGuid = panelGuid;
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                var uidoc = uiapp.ActiveUIDocument;

                if (uidoc != null)
                {
                    // Get the selected elements
                    var selection = uidoc.Selection.GetElementIds();

                    if (selection.Count == 1)
                    {
                        var element = uidoc.Document.GetElement(selection.First());
                        if (element is FamilyInstance familyInstance &&
                            (familyInstance.Symbol.Family.FamilyCategory.Name == "Doors" || familyInstance.Symbol.Family.FamilyCategory.Name == "Windows"))
                        {
                            // Show the dockable pane
                            var panelId = new DockablePaneId(_panelGuid);
                            var pane = uiapp.GetDockablePane(panelId);
                            pane.Show();

                            // Fetch the externalId of the door or window
                            var externalId = familyInstance.UniqueId;

                            // Always force the panel to update by clearing the previous content
                            _customPanelProvider.ClearPanelContent();

                            // Update the panel by filtering and showing the results based on the externalId
                            _customPanelProvider.FilterAndShowResults(externalId);
                        }
                        else
                        {
                            // Hide the custom panel and switch to the Properties palette
                            var panelId = new DockablePaneId(_panelGuid);
                            var pane = uiapp.GetDockablePane(panelId);
                            pane.Hide();

                            // Switch to the Properties palette
                            var propertiesPaneId = DockablePanes.BuiltInDockablePanes.PropertiesPalette;
                            var propertiesPane = uiapp.GetDockablePane(propertiesPaneId);
                            propertiesPane.Show(); // Show the Properties palette
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public string GetName()
        {
            return "Selection Handler";
        }
    }


}
