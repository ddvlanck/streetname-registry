using Be.Vlaanderen.Basisregisters.GrAr.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NodaTime;
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
        public readonly object Context = new StreetNameLdesContext();

        [DataMember(Name = "@id")]
        public Uri Id { get; set; }

        [DataMember(Name = "@type")]
        public readonly string Type = "tree:Node";

        [DataMember(Name = "viewOf")]
        public Uri CollectionLink { get; set; }

        [DataMember(Name = "tree:shape")]
        public Uri StreetNameShape { get; set; }

        [DataMember(Name = "tree:relation")]
        public List<HypermediaControls>? HypermediaControls { get; set; }

        //TODO: add field for context
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
        public IConfigurationSection _configuration { get; set; }

        public StreetNameVersionObject(
            IConfigurationSection configuration,
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
        {
            _configuration = configuration;

            ChangeType = changeType;
            GeneratedAtTime = generatedAtTime.ToBelgianDateTimeOffset();

            Id = CreateVersionUri(position);
            IsVersionOf = GetPersistentUri(persistentLocalId);
            Status = GetStatusUri(status);

            StreetNames = CreateListOfLanguageStrings(nameDutch, nameFrench, nameEnglish, nameGerman);
            Homonyms = CreateListOfLanguageStrings(homonymAdditionDutch, homonymAdditionFrench, homonymAdditionEnglish, homonymAdditionGerman);
            ResponsibleMunicipality = GetResponsibleMunicipality(nisCode);

        }

        private Uri CreateVersionUri(long position)
        {
            return new Uri($"{_configuration["ApiEndpoint"]}#{position}");
        }

        private Uri GetPersistentUri(long persistentLocalId)
        {
            return new Uri($"{_configuration["DataVlaanderenNamespace"]}/{persistentLocalId}");
        }

        private Uri GetResponsibleMunicipality(string nisCode)
        {
            if (string.IsNullOrEmpty(nisCode))
            {
                throw new Exception("Received NisCode is null and can not be null");
            }

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

}
