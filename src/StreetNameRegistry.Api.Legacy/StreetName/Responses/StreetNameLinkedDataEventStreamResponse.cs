using Be.Vlaanderen.Basisregisters.GrAr.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NodaTime;
using StreetNameRegistry.Api.Legacy.Infrastructure;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StreetNameRegistry.Api.Legacy.StreetName.Responses
{
    [DataContract(Name = "StreetNameLinkedDataEventStream", Namespace = "")]
    public class StreetNameLinkedDataEventStreamResponse
    {
        [DataMember(Name = "@context")]
        public readonly object Context = new StreetNameLinkedDataEventStreamContext();

        [DataMember(Name = "@id")]
        public Uri Id { get; set; }

        [DataMember(Name = "@type")]
        public readonly string Type = "tree:Node";

        [DataMember(Name = "viewOf")]
        public Uri CollectionLink { get; set; }

        [DataMember(Name = "tree:shape")]
        public Uri StreetNameShape { get; set; }

        [DataMember(Name = "tree:relation")]
        [JsonProperty(Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<HypermediaControl>? HypermediaControls { get; set; }

        [DataMember(Name = "items")]
        public List<StreetNameVersionObject> StreetNames { get; set; }
    }

    [DataContract(Name = "StraatnaamVersieObject", Namespace = "")]
    public class StreetNameVersionObject
    {
        [DataMember(Name = "@id", Order = 1)]
        public Uri Id { get; set; }

        [DataMember(Name = "@type", Order = 2)]
        public readonly string Type = "Straatnaam";

        [DataMember(Name = "isVersionOf", Order = 3)]
        public Uri IsVersionOf { get; set; }

        [DataMember(Name = "generatedAtTime", Order = 4)]
        public DateTimeOffset GeneratedAtTime { get; set; }

        [DataMember(Name = "eventName", Order = 5)]
        public string ChangeType { get; set; }

        [DataMember(Name = "isToegekendDoor", Order = 6)]
        public Uri ResponsibleMunicipality { get; set; }

        [DataMember(Name = "straatnaam", Order = 7)]
        public List<LanguageString>? StreetNames { get; set; }

        [DataMember(Name = "homoniemToevoeging", Order = 8)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<LanguageString>? Homonyms { get; set; }

        [DataMember(Name = "status", Order = 9)]
        public Uri Status { get; set; }

        [IgnoreDataMember]
        public LinkedDataEventStreamConfiguration Configuration { get; set; }

        public StreetNameVersionObject(
            LinkedDataEventStreamConfiguration configuration,
            long position,
            long persistentLocalId,
            string changeType,
            Instant generatedAtTime,
            StreetNameStatus status,
            string nisCode)
        {
            Configuration = configuration;
            ChangeType = changeType;
            GeneratedAtTime = generatedAtTime.ToBelgianDateTimeOffset();

            Id = CreateVersionUri(position);
            IsVersionOf = GetPersistentUri(persistentLocalId);
            Status = GetStatusUri(status);

            ResponsibleMunicipality = GetResponsibleMunicipality(nisCode);
        }

        public StreetNameVersionObject(
            LinkedDataEventStreamConfiguration configuration,
            long position,
            long persistentLocalId,
            string changeType,
            Instant generatedAtTime,
            StreetNameStatus status,
            string nisCode,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string? nameEnglish,
            string? homonymAdditionDutch,
            string? homonymAdditionFrench,
            string? homonymAdditionGerman,
            string? homonymAdditionEnglish)
            : this (
                  configuration,
                  position,
                  persistentLocalId,
                  changeType,
                  generatedAtTime,
                  status,
                  nisCode)
        {
            StreetNames = CreateListOfLanguageStrings(nameDutch, nameFrench, nameEnglish, nameGerman);
            Homonyms = CreateListOfLanguageStrings(homonymAdditionDutch, homonymAdditionFrench, homonymAdditionEnglish, homonymAdditionGerman);
        }

        private Uri CreateVersionUri(long position) => new Uri($"{Configuration.ApiEndpoint}#{position}");

        private Uri GetPersistentUri(long persistentLocalId) => new Uri($"{Configuration.DataVlaanderenNamespace}/{persistentLocalId}");

        private Uri GetResponsibleMunicipality(string nisCode)
        {
            if (string.IsNullOrEmpty(nisCode))
                throw new Exception("Received NisCode is null and can not be null");

            return new Uri($"https://data.vlaanderen.be/id/gemeente/{nisCode}");
        }

        private Uri GetStatusUri(StreetNameStatus status)
        {
            switch (status)
            {
                case StreetNameStatus.Current:
                    return new Uri("https://data.vlaanderen.be/id/concept/straatnaamstatus/inGebruik");

                case StreetNameStatus.Proposed:
                    return new Uri("https://data.vlaanderen.be/id/concept/straatnaamstatus/voorgesteld");

                case StreetNameStatus.Retired:
                    return new Uri("https://data.vlaanderen.be/id/concept/straatnaamstatus/gehistoreerd");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<LanguageString>? CreateListOfLanguageStrings(string dutch, string french, string english, string german)
        {
            List<LanguageString> languageStrings = new List<LanguageString>();

            if (!string.IsNullOrEmpty(dutch))
            {
                languageStrings.Add(new LanguageString
                {
                    Value = dutch,
                    Language = "nl"
                });
            }

            if (!string.IsNullOrEmpty(french))
            {
                languageStrings.Add(new LanguageString
                {
                    Value = french,
                    Language = "fr"
                });
            }

            if (!string.IsNullOrEmpty(english))
            {
                languageStrings.Add(new LanguageString
                {
                    Value = english,
                    Language = "en"
                });
            }

            if (!string.IsNullOrEmpty(german))
            {
                languageStrings.Add(new LanguageString
                {
                    Value = german,
                    Language = "de"
                });
            }

            if (languageStrings.Count > 0)
            {
                return languageStrings;
            }

            return null;
        }
    }

    public class LanguageString
    {
        [JsonProperty("@value")]
        public string Value { get; set; }

        [JsonProperty("@language")]
        public string Language { get; set; }
    }

    public class StreetNameLinkedDataEventStreamResponseExamples : IExamplesProvider<StreetNameLinkedDataEventStreamResponse>
    {
        private readonly LinkedDataEventStreamConfiguration _configuration;
        public StreetNameLinkedDataEventStreamResponseExamples(LinkedDataEventStreamConfiguration configuration) => _configuration = configuration;
        public StreetNameLinkedDataEventStreamResponse GetExamples()
        {
            var generatedAtTime = Instant.FromDateTimeOffset(DateTimeOffset.Parse("2002-11-21T11:23:45+01:00"));

            var versionObjects = new List<StreetNameVersionObject>()
            {
                new StreetNameVersionObject(
                    _configuration,
                    8,
                    83952,
                    "StreetNameBecameComplete",
                    generatedAtTime,
                    StreetNameStatus.Current,
                    "52043")
            };

            var hypermediaControls = new List<HypermediaControl>()
            {
                new HypermediaControl
                {
                    Type = "tree:GreaterThanOrEqualToRelation",
                    Node = new Uri("https://data.vlaanderen.be/base/straatnamen?page=2"),
                    SelectedProperty = "prov:generatedAtTime",
                    TreeValue = new TreeValue
                    {
                        Value = DateTimeOffset.Parse("2002-11-21T11:23:45+01:00"),
                        Type = "xsd:dateTime"
                    }
                }
            };

            return new StreetNameLinkedDataEventStreamResponse
            {
                Id = new Uri("https://data.vlaanderen.be/base/straatnamen?page=1"),
                HypermediaControls = hypermediaControls,
                CollectionLink = new Uri("https://data.vlaanderen.be/base/straatnamen"),
                StreetNameShape = new Uri("https://data.vlaanderen.be/base/straatnamen/shape"),
                StreetNames = versionObjects
            };
        }
    }
}
