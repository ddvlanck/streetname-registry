namespace StreetNameRegistry.Api.Legacy.StreetName.Query
{
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Microsoft.EntityFrameworkCore;
    using NodaTime;
    using StreetNameRegistry.Projections.Legacy;
    using StreetNameRegistry.Projections.Legacy.StreetNameLinkedDataEventStream;
    using StreetNameRegistry.Projections.Legacy.StreetNameSyndication;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class StreetNameLinkedDataEventStreamQueryResult
    {
        public string ObjectIdentifier { get; }
        public string ChangeType { get; }
        public int PersistentLocalId { get; }
        public Instant RecordCreatedAt { get; }
        public string NisCode { get; }

        public string? NameDutch { get; }
        public string? NameEnglish { get; }
        public string? NameFrench { get; }
        public string? NameGerman { get; }

        public string? HomonymAdditionDutch { get; }
        public string? HomonymAdditionEnglish { get; }
        public string? HomonymAdditionFrench { get; }
        public string? HomonymAdditionGerman { get; }

        public StreetNameStatus Status { get; set; }

        public StreetNameLinkedDataEventStreamQueryResult(
            string objectIdentifier,
            string changeType,
            int persistentLocalId,
            Instant recordCreatedAt,
            string nisCode,
            StreetNameStatus status,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string? nameEnglish,
            string? homonymAdditionDutch,
            string? homonymAdditionFrench,
            string? homonymAdditionGerman,
            string? homonymAdditionEnglish)
        {
            ObjectIdentifier = objectIdentifier;
            ChangeType = changeType;
            NisCode = nisCode;
            PersistentLocalId = persistentLocalId;
            Status = status;

            RecordCreatedAt = recordCreatedAt;

            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameEnglish = nameEnglish;
            NameGerman = nameGerman;

            HomonymAdditionDutch = homonymAdditionDutch;
            HomonymAdditionEnglish = homonymAdditionEnglish;
            HomonymAdditionFrench = homonymAdditionFrench;
            HomonymAdditionGerman = homonymAdditionGerman;
        }
    }

    public class StreetNameLinkedDataEventStreamQuery : Query<StreetNameLinkedDataEventStreamItem, StreetNameLinkedDataEventStreamFilter, StreetNameLinkedDataEventStreamQueryResult>
    {
        private readonly LegacyContext _context;

        public StreetNameLinkedDataEventStreamQuery(LegacyContext context)
            => _context = context;

        protected override ISorting Sorting => new StreetNameLinkedDataEventStreamSorting();

        protected override Expression<Func<StreetNameLinkedDataEventStreamItem, StreetNameLinkedDataEventStreamQueryResult>> Transformation
        {
            get
            {
                return syndicationItem => new StreetNameLinkedDataEventStreamQueryResult(
                    syndicationItem.ObjectHash,
                    syndicationItem.ChangeType,
                    (int)syndicationItem.PersistentLocalId,
                    syndicationItem.EventGeneratedAtTime,
                    syndicationItem.NisCode,
                    (StreetNameStatus)syndicationItem.Status,
                    syndicationItem.NameDutch,
                    syndicationItem.NameFrench,
                    syndicationItem.NameGerman,
                    syndicationItem.NameEnglish,
                    syndicationItem.HomonymAdditionDutch,
                    syndicationItem.HomonymAdditionFrench,
                    syndicationItem.HomonymAdditionGerman,
                    syndicationItem.HomonymAdditionEnglish);
            }
        }

        protected override IQueryable<StreetNameLinkedDataEventStreamItem> Filter(FilteringHeader<StreetNameLinkedDataEventStreamFilter> filtering)
            => _context
                .StreetNameLinkedDataEventStream
                .Where(x => x.IsComplete == true)
                .OrderBy(x => x.Position)
                .AsNoTracking();
    }

    internal class StreetNameLinkedDataEventStreamSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(StreetNameLinkedDataEventStreamItem.Position)
        };

        public SortingHeader DefaultSortingHeader { get; } = new SortingHeader(nameof(StreetNameLinkedDataEventStreamItem.Position), SortOrder.Ascending);
    }

    public class StreetNameLinkedDataEventStreamFilter
    {
        public int PageNumber { get; set; }
    }
}
