using System.Collections.Generic;

namespace revit_aec_dm_ca_demo.Models
{
    public class MongoResponse
    {
        public List<Documents> Documents { get; set; }
    }

    public class Documents
    {
        public string _id { get; set; }
        public string AecdmElementId { get; set; }
        public int LmvId { get; set; }
        public int Svf2Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ExternalId { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class Property
    {
        public Definition Definition { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Definition
    {
        public string Id { get; set; }
    }
}
