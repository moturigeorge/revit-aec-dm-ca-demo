#region Namespaces
using revit_aec_dm_ca_demo.Models;
using Autodesk.Revit.UI;
using System;

#endregion

namespace revit_aec_dm_ca_demo
{
    internal class App : IExternalApplication
    {
        private readonly Guid _panelGuid = new Guid("319667F9-E275-4CE3-B673-A83CFF895D9C");
        private AttributesPanelProvider _customPanelProvider;
        private ExternalEvent _externalEvent;
        private SelectionChangedEventHandler _selectionHandler;
        private MongoSettings _mongoSettings;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Load configuration from appsettings.json
                _mongoSettings = ConfigurationLoader.LoadConfigurationAsync().GetAwaiter().GetResult();


                // Instantiate your panel provider with the configuration
                _customPanelProvider = new AttributesPanelProvider(_mongoSettings);

                // Register dockable pane with a valid GUID
                var panelId = new DockablePaneId(_panelGuid);

                application.RegisterDockablePane(panelId, "Cloud Customer Attributes", _customPanelProvider);

                // Create and register the external event
                _selectionHandler = new SelectionChangedEventHandler(_customPanelProvider, _panelGuid);
                _externalEvent = ExternalEvent.Create(_selectionHandler);

                // Subscribe to the ViewActivated event to hide the dockable pane after the UI has been activated
                application.ViewActivated += (sender, e) =>
                {
                    try
                    {
                        // Hide the dockable pane after registration and activation
                        var dockablePane = application.GetDockablePane(panelId);
                        if (dockablePane != null)
                        {
                            dockablePane.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                };

                // Subscribe to the SelectionChanged event
                application.SelectionChanged += OnSelectionChanged;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                // Unsubscribe from SelectionChanged event
                application.SelectionChanged -= OnSelectionChanged;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Result.Failed;
            }
        }

        private void OnSelectionChanged(object sender, Autodesk.Revit.UI.Events.SelectionChangedEventArgs e)
        {
            try
            {
                // Raise the external event
                _externalEvent.Raise();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }

}