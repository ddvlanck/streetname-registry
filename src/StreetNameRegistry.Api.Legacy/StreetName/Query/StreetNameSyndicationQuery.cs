namespace StreetNameRegistry.Api.Legacy.StreetName.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Microsoft.EntityFrameworkCore;
    using NodaTime;
    using Projections.Legacy;
    using Projections.Legacy.StreetNameSyndication;

    public class StreetNameSyndicationQueryResult
    {
        public bool ContainsEvent { get; }
        public bool ContainsObject { get; }

        public Guid? StreetNameId { get; }
        public long Position { get; }
        public string ChangeType { get; }
        public int? PersistentLocalId { get; }
        public string NisCode { get; }
        public Instant RecordCreatedAt { get; }
        public StreetNameStatus? Status { get; }
        public string NameDutch { get; }
        public string NameFrench { get; }
        public string NameGerman { get; }
        public string NameEnglish { get; }
        public string HomonymAdditionDutch { get; }
        public string HomonymAdditionFrench { get; }
        public string HomonymAdditionGerman { get; }
        public string HomonymAdditionEnglish { get; }
        public bool IsComplete { get; }

        public StreetNameSyndicationQueryResult(
            Guid? streetNameId,
            long position,
            int? persistentLocalId,
            string nisCode,
            string changeType,
            Instant recordCreatedAt,
            bool isComplete)
        {
            ContainsEvent = false;
            ContainsObject = false;

            StreetNameId = streetNameId;
            Position = position;
            PersistentLocalId = persistentLocalId;
            NisCode = nisCode;
            ChangeType = changeType;
            RecordCreatedAt = recordCreatedAt;
            IsComplete = isComplete;
        }

        /*public StreetNameSyndicationQueryResult(
            Guid? streetNameId,
            long position,
            int? persistentLocalId,
            string nisCode,
            string changeType,
            Instant recordCreatedAt,
            bool isComplete)
            : this(streetNameId,
                position,
                persistentLocalId,
                nisCode,
                changeType,
                recordCreatedAt,
                isComplete)
        {
            ContainsEvent = true;
        }*/

        public StreetNameSyndicationQueryResult(
            Guid? streetNameId,
            long position,
            int? persistentLocalId,
            string nisCode,
            string changeType,
            Instant recordCreatedAt,
            StreetNameStatus? status,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            string homonymAdditionDutch,
            string homonymAdditionFrench,
            string homonymAdditionGerman,
            string homonymAdditionEnglish,
            bool isComplete)
            : this(
                streetNameId,
                position,
                persistentLocalId,
                nisCode,
                changeType,
                recordCreatedAt,
                isComplete)
        {
            ContainsObject = true;

            Status = status;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NameEnglish = nameEnglish;
            HomonymAdditionDutch = homonymAdditionDutch;
            HomonymAdditionFrench = homonymAdditionFrench;
            HomonymAdditionGerman = homonymAdditionGerman;
            HomonymAdditionEnglish = homonymAdditionEnglish;
        }

        /*public StreetNameSyndicationQueryResult(
            Guid? streetNameId,
            long position,
            int? persistentLocalId,
            string nisCode,
            string changeType,
            Instant recordCreatedAt,
            StreetNameStatus? status,
            string nameDutch,
            string nameFrench,
            string nameGerman,
            string nameEnglish,
            string homonymAdditionDutch,
            string homonymAdditionFrench,
            string homonymAdditionGerman,
            string homonymAdditionEnglish,
            bool isComplete)
            : this(
                streetNameId,
                position,
                persistentLocalId,
                nisCode,
                changeType,
                recordCreatedAt,
                status,
                nameDutch,
                nameFrench,
                nameGerman,
                nameEnglish,
                homonymAdditionDutch,
                homonymAdditionFrench,
                homonymAdditionGerman,
                homonymAdditionEnglish,
                isComplete)
        {
            ContainsEvent = true;
        }*/
    }

    public class StreetNameSyndicationQuery : Query<StreetNameSyndicationItem, StreetNameSyndicationFilter, StreetNameSyndicationQueryResult>
    {
        private readonly LegacyContext _context;
        private readonly bool _embedEvent;
        private readonly bool _embedObject;

        public StreetNameSyndicationQuery(
            LegacyContext context,
            EmbedValue embed)
        {
            _context = context;
            _embedEvent = embed?.Event ?? false;
            _embedObject = embed?.Object ?? false;
        }

        protected override ISorting Sorting => new StreetNameSyndicationSorting();

        protected override Expression<Func<StreetNameSyndicationItem, StreetNameSyndicationQueryResult>> Transformation
        {
            get
            {
                if (_embedEvent && _embedObject)
                    return x => new StreetNameSyndicationQueryResult(
                        x.StreetNameId,
                        x.Position,
                        x.PersistentLocalId,
                        x.NisCode,
                        x.ChangeType,
                        x.RecordCreatedAt,
                        x.Status,
                        x.NameDutch,
                        x.NameFrench,
                        x.NameGerman,
                        x.NameEnglish,
                        x.HomonymAdditionDutch,
                        x.HomonymAdditionFrench,
                        x.HomonymAdditionGerman,
                        x.HomonymAdditionEnglish,
                        x.IsComplete);

                if (_embedEvent)
                    return x => new StreetNameSyndicationQueryResult(
                        x.StreetNameId,
                        x.Position,
                        x.PersistentLocalId,
                        x.NisCode,
                        x.ChangeType,
                        x.RecordCreatedAt,
                        x.IsComplete);

                if (_embedObject)
                    return x => new StreetNameSyndicationQueryResult(
                        x.StreetNameId,
                        x.Position,
                        x.PersistentLocalId,
                        x.NisCode,
                        x.ChangeType,
                        x.RecordCreatedAt,
                        x.Status,
                        x.NameDutch,
                        x.NameFrench,
                        x.NameGerman,
                        x.NameEnglish,
                        x.HomonymAdditionDutch,
                        x.HomonymAdditionFrench,
                        x.HomonymAdditionGerman,
                        x.HomonymAdditionEnglish,
                        x.IsComplete);

                return x => new StreetNameSyndicationQueryResult(
                    x.StreetNameId,
                    x.Position,
                    x.PersistentLocalId,
                    x.NisCode,
                    x.ChangeType,
                    x.RecordCreatedAt,
                    x.IsComplete);
            }
        }

        protected override IQueryable<StreetNameSyndicationItem> Filter(FilteringHeader<StreetNameSyndicationFilter> filtering)
        {
            var streetNames = _context
                .StreetNameSyndication
                .OrderBy(x => x.Position)
                .AsNoTracking();

            if (!filtering.ShouldFilter)
                return streetNames;

            if (filtering.Filter.Position.HasValue)
                streetNames = streetNames.Where(m => m.Position >= filtering.Filter.Position);

            return streetNames;
        }
    }

    public class StreetNameSyndicationSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(StreetNameSyndicationItem.Position)
        };

        public SortingHeader DefaultSortingHeader { get; } = new SortingHeader(nameof(StreetNameSyndicationItem.Position), SortOrder.Ascending);
    }

    public class StreetNameSyndicationFilter
    {
        public long? Position { get; set; }
        public EmbedValue Embed { get; set; }
    }
}
