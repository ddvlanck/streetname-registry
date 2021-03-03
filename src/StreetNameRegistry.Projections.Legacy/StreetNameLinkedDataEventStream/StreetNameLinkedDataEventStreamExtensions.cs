using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreetNameRegistry.Projections.Legacy.StreetNameLinkedDataEventStream
{
    public static class StreetNameLinkedDataEventStreamExtensions
    {
        public static async Task CreateNewStreetNameLinkedDataEventStreamItem<T>(
            this LegacyContext context,
            Guid streetNameId,
            Envelope<T> message,
            Action<StreetNameLinkedDataEventStreamItem> applyEventInfoOn,
            CancellationToken ct) where T : IHasProvenance
        {
            var streetNameLinkedDataEventStreamItem = await context.LatestPosition(streetNameId, ct);

            if (streetNameLinkedDataEventStreamItem == null)
                throw DatabaseItemNotFound(streetNameId);

            var newItem = streetNameLinkedDataEventStreamItem.CloneAndApplyEventInfo(
                message.Position,
                message.EventName,
                applyEventInfoOn);

            await context
                .StreetNameLinkedDataEventStream
                .AddAsync(newItem, ct);
        }

        public static async Task<StreetNameLinkedDataEventStreamItem> LatestPosition(
            this LegacyContext context,
            Guid streetNameId,
            CancellationToken ct)
            => context
                   .StreetNameLinkedDataEventStream
                   .Local
                   .Where(x => x.StreetNameId == streetNameId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefault()
               ?? await context
                   .StreetNameLinkedDataEventStream
                   .Where(x => x.StreetNameId == streetNameId)
                   .OrderByDescending(x => x.Position)
                   .FirstOrDefaultAsync(ct);


        private static ProjectionItemNotFoundException<StreetNameLinkedDataEventStreamItem> DatabaseItemNotFound(Guid streetNameId)
           => new ProjectionItemNotFoundException<StreetNameLinkedDataEventStreamItem>(streetNameId.ToString("D"));

    }
}
