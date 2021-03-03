using Be.Vlaanderen.Basisregisters.Api.Search;
using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using StreetNameRegistry.Projections.Legacy;
using StreetNameRegistry.Projections.Legacy.StreetNameSyndication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StreetNameRegistry.Api.Legacy.StreetName.Query
{
    public class StreetNameLinkedDataEventStreamQueryResult
    {
        public long Position { get; }
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
            long position,
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
            Position = position;
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

    public class StreetNameLinkedDataEventStreamQuery : Query<StreetNameSyndicationItem, StreetNameLDESFilter, StreetNameLinkedDataEventStreamQueryResult>
    {
        private readonly LegacyContext _context;

        public StreetNameLinkedDataEventStreamQuery(LegacyContext context)
        {
            _context = context;
        }

        protected override ISorting Sorting => new StreetNameLDESSorting();

        protected override Expression<Func<StreetNameSyndicationItem, StreetNameLinkedDataEventStreamQueryResult>> Transformation
        {
            get
            {
                return syndicationItem => new StreetNameLinkedDataEventStreamQueryResult(
                    syndicationItem.Position,
                    syndicationItem.ChangeType,
                    (int)syndicationItem.PersistentLocalId,
                    syndicationItem.RecordCreatedAt,
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

        protected override IQueryable<StreetNameSyndicationItem> Filter(FilteringHeader<StreetNameLDESFilter> filtering)
        {
            var streetNameSet = _context
                .StreetNameSyndication
                .Where(x => x.IsComplete == true)
                .OrderBy(x => x.Position)
                .AsNoTracking();

            return streetNameSet;
        }
    }

    internal class StreetNameLDESSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(StreetNameSyndicationItem.Position)
        };

        public SortingHeader DefaultSortingHeader { get; } = new SortingHeader(nameof(StreetNameSyndicationItem.Position), SortOrder.Ascending);
    }

    public class StreetNameLDESFilter
    {
        public int PageNumber { get; set; }
    }
}
