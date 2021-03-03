namespace StreetNameRegistry.Projections.Legacy.StreetNameSyndication
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;

    public static class StreetNameSyndicationExtensions
    {
        public static async Task CreateNewStreetNameSyndicationItem<T>(
            this LegacyContext context,
            Guid streetNameId,
            Envelope<T> message,
            Action<StreetNameSyndicationItem> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance
        {
            var streetNameSyndicationItem = await context.LatestPosition(streetNameId, ct);

            if (streetNameSyndicationItem == null)
                throw DatabaseItemNotFound(streetNameId);

            var provenance = message.Message.Provenance;

            var newStreetNameSyndicationItem = streetNameSyndicationItem.CloneAndApplyEventInfo(
                message.Position,
                message.EventName,
                provenance.Timestamp,
                applyEventInfoOn);

            await context
                .StreetNameSyndication
                .AddAsync(newStreetNameSyndicationItem, ct);
        }

        public static async Task<StreetNameSyndicationItem> LatestPosition(
            this LegacyContext context,
            Guid streetNameId,
            CancellationToken ct)
            => context
                   .StreetNameSyndication
                   .Local
                   .Where(x => x.StreetNameId == streetNameId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .StreetNameSyndication
                   .Where(x => x.StreetNameId == streetNameId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);

        private static ProjectionItemNotFoundException<StreetNameSyndicationProjections> DatabaseItemNotFound(Guid streetNameId)
            => new ProjectionItemNotFoundException<StreetNameSyndicationProjections>(streetNameId.ToString("D"));
    }
}
