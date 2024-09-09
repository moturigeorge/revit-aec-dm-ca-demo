using revit_aec_dm_ca_demo.Models;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace revit_aec_dm_ca_demo
{
    public class AttributesPanelProvider : UserControl, IDockablePaneProvider
    {
        private readonly MongoSettings _mongoSettings;
        private StackPanel _panel;
        private Dictionary<string, System.Windows.Controls.TextBox> _propertyInputs;
        private string _documentId;
        private List<string> _propertyNames = new List<string>(); // Store property names globally
        private TextBlock _loaderMessage;

        public AttributesPanelProvider(MongoSettings mongoSettings)
        {
            _mongoSettings = mongoSettings;
            _panel = new StackPanel
            {
                Background = SystemColors.WindowBrush // Set background to default system color
            };
            _propertyInputs = new Dictionary<string, System.Windows.Controls.TextBox>();

            _loaderMessage = new TextBlock
            {
                Text = "Loading...",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(10),
                Visibility = Visibility.Collapsed
            };

            _panel.Children.Add(_loaderMessage);
            this.Content = _panel;

        }

        public void DisplayLoader()
        {
            _panel.Children.Clear();
            _loaderMessage.Visibility = Visibility.Visible;
            _panel.Children.Add(_loaderMessage);
        }

        public void HideLoader()
        {
            _loaderMessage.Visibility = Visibility.Collapsed;
        }



        public void SetupDockablePane(DockablePaneProviderData data)
        {
            try
            {
                data.FrameworkElement = this;
                data.InitialState = new DockablePaneState
                {
                    DockPosition = DockPosition.Tabbed,
                    TabBehind = DockablePanes.BuiltInDockablePanes.PropertiesPalette,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async void FilterAndShowResults(string externalId)
        {
            DisplayLoader(); // Show loader

            var filterBody = new
            {
                dataSource = _mongoSettings.DataSource,
                database = _mongoSettings.Database,
                collection = _mongoSettings.Collection,
                filter = new { externalId = externalId },
                sort = new { completedAt = 1 },
                limit = 10
            };

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(filterBody), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("apiKey", _mongoSettings.ApiKey);
                    var response = await httpClient.PostAsync("https://ap-southeast-1.aws.data.mongodb-api.com/app/data-isssnzm/endpoint/data/v1/action/find", jsonContent);
                    var responseData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response data
                        var apiResponse = JsonConvert.DeserializeObject<MongoResponse>(responseData);

                        // Extract property names from the first document and store them
                        if (apiResponse?.Documents != null && apiResponse.Documents.Any())
                        {
                            _propertyNames = apiResponse.Documents.First().Properties.Select(p => p.Name).ToList();
                        }
                        else
                        {
                            _propertyNames.Clear();
                        }

                        // Update the properties in the UI
                        UpdatePropertiesUI(apiResponse);
                    }
                    else
                    {
                        // Handle error status codes
                        HandleError(response.StatusCode, responseData);
                        HideLoader(); // Hide loader in case of error
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }



        private void HandleError(HttpStatusCode statusCode, string responseData)
        {
            try
            {
                _panel.Dispatcher.Invoke(() =>
                {
                    _panel.Children.Clear();

                    var errorMessage = $"Error: {statusCode} - {responseData}";
                    var errorTextBlock = new TextBlock
                    {
                        Text = errorMessage,
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Colors.Red),
                        Margin = new Thickness(10)
                    };

                    _panel.Children.Add(errorTextBlock);
                });
                // Log the error (optional)
                Console.WriteLine($"HTTP Error: {statusCode} - Response: {responseData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void UpdatePropertiesUI(MongoResponse apiResponse)
        {
            try
            {
                HideLoader(); // Hide loader when data is ready

                // Clear any previous content in the panel
                _panel.Children.Clear();
                _propertyInputs.Clear(); // Clear previous property inputs

                if (apiResponse?.Documents == null || !apiResponse.Documents.Any())
                {
                    // If no documents are found, display a message in the panel
                    var noResultsMessage = new TextBlock
                    {
                        Text = "No data found for the selected family.",
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        Margin = new Thickness(10)
                    };

                    _panel.Children.Add(noResultsMessage);
                    return;
                }

                var document = apiResponse.Documents.First();
                _documentId = document.ExternalId; // Store the document ID for update actions

                _panel.Dispatcher.Invoke(() =>
                {
                    foreach (var property in document.Properties)
                    {
                        // Create a new row panel for each property
                        var rowPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(5)
                        };

                        var textBlock = new TextBlock
                        {
                            Text = property.Name,
                            FontSize = 10,
                            Foreground = new SolidColorBrush(Colors.Black),
                            Width = 100,
                            Margin = new Thickness(5)
                        };

                        var textBox = new System.Windows.Controls.TextBox
                        {
                            Text = property.Value,
                            FontSize = 10,
                            Width = 100,
                            Margin = new Thickness(5)
                        };


                        // Create an Image control
                        var image = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/revit-aec-dm-ca-demo;component/Resources/cloud-upload.png")),
                            Width = 24,
                            Height = 20
                        };

                        // Create the Button
                        var updateButton = new Button
                        {
                            Margin = new Thickness(5),
                            BorderThickness = new Thickness(0),
                            Foreground = new SolidColorBrush(Colors.Transparent),
                            Content = image // Set the Image as the Button's content
                        };

                        // Pass the current text box's value to the OnUpdateButtonClick method
                        updateButton.Click += (sender, e) => OnUpdateButtonClick(sender, e, property.Name, textBox);

                        rowPanel.Children.Add(textBlock);
                        rowPanel.Children.Add(textBox);
                        rowPanel.Children.Add(updateButton);

                        _panel.Children.Add(rowPanel);

                        // Store the reference to the new text box
                        _propertyInputs[property.Name] = textBox;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        // Updated method signature to pass the TextBox itself, so we can capture the latest value
        private async void OnUpdateButtonClick(object sender, RoutedEventArgs e, string propertyName, System.Windows.Controls.TextBox textBox)
        {
            if (string.IsNullOrEmpty(_documentId)) return;

            // Capture the latest value from the TextBox
            string updatedValue = textBox.Text;

            var updatedProperties = new Dictionary<string, string>
            {
                { propertyName, updatedValue }
            };
            // Disable buttons on selection
            DisableButtons(_panel);
            try
            {
                // Send the updated property to the API
                bool updateSuccess = await UpdatePropertyInDatabase(_documentId, updatedProperties);

                if (updateSuccess)
                {
                    MessageBox.Show("Property updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EnableButtons(_panel);
            }
        }

        private async Task<bool> UpdatePropertyInDatabase(string externalId, Dictionary<string, string> updatedProperties)
        {
            try
            {
                // Use the stored property names list to find the correct index for each property
                var updateFields = updatedProperties.ToDictionary(
                    kvp => $"properties.{_propertyNames.IndexOf(kvp.Key)}.value",
                    kvp => (object)kvp.Value
                );

                var updateObject = new Dictionary<string, object>
            {
                { "$set", updateFields }
            };

                var updateBody = new
                {
                    dataSource = _mongoSettings.DataSource,
                    database = _mongoSettings.Database,
                    collection = _mongoSettings.Collection,
                    filter = new { externalId = externalId },
                    update = updateObject
                };

                using (var httpClient = new HttpClient())
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(updateBody), Encoding.UTF8, "application/json");
                    jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    httpClient.DefaultRequestHeaders.Add("apiKey", _mongoSettings.ApiKey);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    var response = await httpClient.PostAsync("https://ap-southeast-1.aws.data.mongodb-api.com/app/data-isssnzm/endpoint/data/v1/action/updateOne", jsonContent);
                    // Log the response for debugging
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        // Ensure the update was successful
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed response: {responseBody}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }



        public void ClearPanelContent()
        {
            _panel.Children.Clear();
        }
        private void DisableButtons(Panel parentPanel)
        {
            foreach (var child in parentPanel.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
                else if (child is Panel panel)
                {
                    // Recursively check nested panels
                    DisableButtons(panel);
                }
            }
        }

        private void EnableButtons(Panel parentPanel)
        {
            foreach (var child in parentPanel.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = true;
                }
                else if (child is Panel panel)
                {
                    // Recursively check nested panels
                    DisableButtons(panel);
                }
            }
        }


    }

}