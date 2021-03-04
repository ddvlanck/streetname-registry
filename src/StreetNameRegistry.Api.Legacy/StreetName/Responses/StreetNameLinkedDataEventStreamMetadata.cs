using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StreetNameRegistry.Api.Legacy.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreetNameRegistry.Api.Legacy.StreetName.Responses
{
    public class StreetNameLinkedDataEventStreamMetadata
    {
        public static Uri GetPageIdentifier(LinkedDataEventStreamConfiguration configuration, int page) => new Uri($"{configuration.ApiEndpoint}?page={page}");

        public static Uri GetCollectionLink(LinkedDataEventStreamConfiguration configuration) => new Uri($"{configuration.ApiEndpoint}");

        public static List<HypermediaControl>? GetHypermediaControls(List<StreetNameVersionObject> items, LinkedDataEventStreamConfiguration configuration, int page, int pageSize)
        {
            List<HypermediaControl> controls = new List<HypermediaControl>();

            var previous = AddPrevious(items, configuration, page);
            if (previous != null)
                controls.Add(previous);

            var next = AddNext(items, configuration, page, pageSize);
            if (next != null)
                controls.Add(next);

            return controls.Count > 0 ? controls : null;
        }

        private static HypermediaControl? AddPrevious(List<StreetNameVersionObject> items, LinkedDataEventStreamConfiguration configuration, int page)
        {
            if (page <= 1)
                return null;

            var previousUrl = new Uri($"{configuration.ApiEndpoint}?page={page - 1}");

            return new HypermediaControl
            {
                Type = "tree:LessThanOrEqualToRelation",
                Node = previousUrl,
                SelectedProperty = "prov:generatedAtTime",
                TreeValue = new TreeValue
                {
                    Value = items.FirstOrDefault().GeneratedAtTime,
                    Type = "xsd:dateTime"
                }
            };
        }

        private static HypermediaControl? AddNext(List<StreetNameVersionObject> items, LinkedDataEventStreamConfiguration configuration, int page, int pageSize)
        {
            if (items.Count != pageSize)
                return null;

            var nextUrl = new Uri($"{configuration.ApiEndpoint}?page={page + 1}");

            return new HypermediaControl
            {
                Type = "tree:GreaterThanOrEqualToRelation",
                Node = nextUrl,
                SelectedProperty = "prov:generatedAtTime",
                TreeValue = new TreeValue
                {
                    Value = items[items.Count - 1].GeneratedAtTime,
                    Type = "xsd:dateTime"
                }
            };
        }
    }

    public class HypermediaControl
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("tree:node")]
        public Uri Node { get; set; }

        [JsonProperty("tree:path")]
        public string SelectedProperty { get; set; }

        [JsonProperty("tree:value")]
        public TreeValue TreeValue { get; set; }
    }

    public class TreeValue
    {
        [JsonProperty("@value")]
        public DateTimeOffset Value { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }
    }
}
