namespace StreetNameRegistry.Projections.Legacy.StreetNameLinkedDataEventStream
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using StreetNameRegistry.StreetName.Events;
    using StreetNameRegistry.StreetName.Events.Crab;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StreetNameLinkedDataEventStreamProjections : ConnectedProjection<LegacyContext>
    {
        public StreetNameLinkedDataEventStreamProjections()
        {
            When<Envelope<StreetNameWasRegistered>>(async (context, message, ct) =>
            {
                var streetNameLinkedDataEventStreamItem = new StreetNameLinkedDataEventStreamItem
                {
                    Position = message.Position,
                    StreetNameId = message.Message.StreetNameId,
                    NisCode = message.Message.NisCode,
                    EventGeneratedAtTime = message.Message.Provenance.Timestamp,
                    ChangeType = message.EventName
                };

                streetNameLinkedDataEventStreamItem.SetObjectHash();

                await context
                    .StreetNameLinkedDataEventStream
                    .AddAsync(streetNameLinkedDataEventStreamItem, ct);
            });

            When<Envelope<StreetNamePersistentLocalIdWasAssigned>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.PersistentLocalId = message.Message.PersistentLocalId,
                    ct);
            });

            When<Envelope<StreetNameWasNamed>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateNameByLanguage(x, message.Message.Name, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameNameWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateNameByLanguage(x, null, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateNameByLanguage(x, message.Message.Name, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateNameByLanguage(x, null, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasDefined>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateHomonymAdditionByLanguage(x, message.Message.HomonymAddition, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateHomonymAdditionByLanguage(x, null, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrected>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateHomonymAdditionByLanguage(x, message.Message.HomonymAddition, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => UpdateHomonymAdditionByLanguage(x, null, message.Message.Language),
                    ct);
            });

            When<Envelope<StreetNameBecameCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Current,
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Current,
                    ct);
            });

            When<Envelope<StreetNameWasProposed>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Proposed,
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToProposed>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Proposed,
                    ct);
            });

            When<Envelope<StreetNameWasRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Retired,
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = StreetNameStatus.Retired,
                    ct);
            });

            When<Envelope<StreetNameStatusWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = null,
                    ct);
            });

            When<Envelope<StreetNameStatusWasCorrectedToRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.Status = null,
                    ct);
            });

            When<Envelope<StreetNameBecameComplete>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.IsComplete = true,
                    ct);
            });

            When<Envelope<StreetNameBecameIncomplete>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => x.IsComplete = false,
                    ct);
            });

            When<Envelope<StreetNameWasRemoved>>(async (context, message, ct) =>
            {
                await context.CreateNewStreetNameLinkedDataEventStreamItem(
                    message.Message.StreetNameId,
                    message,
                    x => { },
                    ct);
            });
        }

        private static void UpdateNameByLanguage(StreetNameLinkedDataEventStreamItem streetNameSyndicationItem, string name, Language? language)
        {
            switch (language)
            {
                case Language.Dutch:
                    streetNameSyndicationItem.NameDutch = name;
                    break;
                case Language.French:
                    streetNameSyndicationItem.NameFrench = name;
                    break;
                case Language.German:
                    streetNameSyndicationItem.NameGerman = name;
                    break;
                case Language.English:
                    streetNameSyndicationItem.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateHomonymAdditionByLanguage(StreetNameLinkedDataEventStreamItem streetNameSyndicationItem, string homonymAddition, Language? language)
        {
            switch (language)
            {
                case Language.Dutch:
                    streetNameSyndicationItem.HomonymAdditionDutch = homonymAddition;
                    break;
                case Language.French:
                    streetNameSyndicationItem.HomonymAdditionFrench = homonymAddition;
                    break;
                case Language.German:
                    streetNameSyndicationItem.HomonymAdditionGerman = homonymAddition;
                    break;
                case Language.English:
                    streetNameSyndicationItem.HomonymAdditionEnglish = homonymAddition;
                    break;
            }
        }
    }
}
