using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StreetNameRegistry.Api.Legacy.StreetName.Responses
{
    [DataContract(Name = "StreetNameShaclShape", Namespace = "")]
    public class StreetNameShaclShapeResponse
    {
        [DataMember(Name = "@id", Order = 2)]
        public Uri Id { get; set; }

        [DataMember(Name = "@type", Order = 3)]
        public readonly string Type = "sh:NodeShape";

        [DataMember(Name = "@context", Order = 1)]
        public readonly StreetNameShaclContext Context = new StreetNameShaclContext();

        [DataMember(Name = "sh:property", Order = 4)]
        public readonly List<StreetNameShaclProperty> Shape = new StreetNameShaclShape().Properties;
    }

    public class StreetNameShaclShape
    {
        public readonly List<StreetNameShaclProperty> Properties = new List<StreetNameShaclProperty>
        {
            new StreetNameShaclProperty
            {
                PropertyPath = "dct:isVersionOf",
                DataType = "sh:IRI",
                MinimumCount = 1,
                MaximumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "prov:generatedAtTime",
                DataType = "xsd:dateTime",
                MinimumCount = 1,
                MaximumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "adms:versionNotes",
                DataType = "xsd:string",
                MinimumCount = 1,
                MaximumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "adres:straatnaam",
                DataType = "rdf:langString",
                MinimumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "adres:Straatnaam.status",
                DataType = "skos:Concept",
                MinimumCount = 1,
                MaximumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "prov:wasAttributedTo",
                DataType = "sh:IRI",
                MinimumCount = 1,
                MaximumCount = 1
            },
            new StreetNameShaclProperty
            {
                PropertyPath = "adres:homoniemToevoeging",
                DataType = "xsd:string",
                MaximumCount = 1
            },
        };

    }

    public class StreetNameShaclProperty
    {
        [JsonProperty("sh:path")]
        public string PropertyPath { get; set; }

        [JsonProperty("sh:datatype")]
        public string DataType { get; set; }

        [JsonProperty(PropertyName = "sh:minCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinimumCount { get; set; }

        [JsonProperty(PropertyName = "sh:maxCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaximumCount { get; set; }
    }
}
