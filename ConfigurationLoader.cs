using revit_aec_dm_ca_demo.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace revit_aec_dm_ca_demo
{
    public static class ConfigurationLoader
    {
        public static async Task<MongoSettings> LoadConfigurationAsync()
        {
            var resourceName = "revit_aec_dm_ca_demo.appsettings.json"; // Use the correct namespace and file name
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new InvalidOperationException("Configuration file not found.");
            }
            // Read the JSON content
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();

                // Deserialize the JSON into a strongly typed object
                var resp= JsonConvert.DeserializeObject<MongoSettings>(json);
                //close StreamReader
                reader.Close();
                return resp;
            }
        }
    }
}
