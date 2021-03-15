namespace StreetNameRegistry.Projections.Legacy.StreetNameLinkedDataEventStream
{
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

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

            var provenance = message.Message.Provenance;

            var newItem = streetNameLinkedDataEventStreamItem.CloneAndApplyEventInfo(
                message.Position,
                message.EventName,
                provenance.Timestamp,
                applyEventInfoOn);

            newItem.SetObjectHash();
            newItem.CheckIfRecordCanBePublished();

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

        public static async Task UpdatePersistentLocalIdentifier(
            this LegacyContext context,
            Guid streetNameId,
            int persistentLocalid,
            CancellationToken ct)
        {
            var streetNameItems = context
                    .StreetNameLinkedDataEventStream
                    .Local
                    .Where(x => x.StreetNameId == streetNameId).ToList()
                ?? await context
                    .StreetNameLinkedDataEventStream
                    .Where(x => x.StreetNameId == streetNameId)
                    .ToListAsync(ct);

            foreach (var item in streetNameItems)
                item.PersistentLocalId = persistentLocalid;

            await context.SaveChangesAsync();
        }

        public static async Task StreetNameWasRemoved(
            this LegacyContext context,
            Guid streetNameId,
            CancellationToken ct)
        {
            var streetNameItems = context
                    .StreetNameLinkedDataEventStream
                    .Local
                    .Where(x => x.StreetNameId == streetNameId).ToList()
                ?? await context
                    .StreetNameLinkedDataEventStream
                    .Where(x => x.StreetNameId == streetNameId)
                    .ToListAsync(ct);

            foreach (var item in streetNameItems)
                item.RecordCanBePublished = false;

            await context.SaveChangesAsync();
        }

        public static void CheckIfRecordCanBePublished(this StreetNameLinkedDataEventStreamItem streetNameLinkedDataEventStreamItem)
        {
            var recordCanBePublished = true;

            if (string.IsNullOrEmpty(streetNameLinkedDataEventStreamItem.NameDutch)
                && string.IsNullOrEmpty(streetNameLinkedDataEventStreamItem.NameEnglish)
                && string.IsNullOrEmpty(streetNameLinkedDataEventStreamItem.NameFrench)
                && string.IsNullOrEmpty(streetNameLinkedDataEventStreamItem.NameGerman))
                recordCanBePublished = false;

            if (string.IsNullOrEmpty(streetNameLinkedDataEventStreamItem.NisCode))
                recordCanBePublished = false;

            if (streetNameLinkedDataEventStreamItem.Status == null)
                recordCanBePublished = false;

            streetNameLinkedDataEventStreamItem.RecordCanBePublished = recordCanBePublished;
        }

        public static void SetObjectHash(this StreetNameLinkedDataEventStreamItem linkedDataEventStreamItem)
        {
            var objectString = JsonConvert.SerializeObject(linkedDataEventStreamItem);

            using var md5Hash = MD5.Create();
            var hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(objectString));
            linkedDataEventStreamItem.ObjectHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

        private static ProjectionItemNotFoundException<StreetNameLinkedDataEventStreamItem> DatabaseItemNotFound(Guid streetNameId)
   => new ProjectionItemNotFoundException<StreetNameLinkedDataEventStreamItem>(streetNameId.ToString("D"));
    }
}
