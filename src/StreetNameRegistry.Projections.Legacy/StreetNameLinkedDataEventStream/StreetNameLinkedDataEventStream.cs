namespace StreetNameRegistry.Projections.Legacy.StreetNameLinkedDataEventStream
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.MigrationExtensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;
    using StreetNameRegistry.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StreetNameLinkedDataEventStreamItem
    {
        public long Position { get; set; }
        public Guid StreetNameId { get; set; }
        public int? PersistentLocalId { get; set; }
        public string? NisCode { get; set; }
        public string? ChangeType { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameEnglish { get; set; }
        public string? NameGerman { get; set; }

        public string? HomonymAdditionDutch { get; set; }
        public string? HomonymAdditionFrench { get; set; }
        public string? HomonymAdditionEnglish{ get; set; }
        public string? HomonymAdditionGerman { get; set; }

        public StreetNameStatus? Status { get; set; }
        public DateTimeOffset EventGeneratedAtTimeAsDateTimeOffset { get; set; }

        public Instant EventGeneratedAtTime
        {
            get => Instant.FromDateTimeOffset(EventGeneratedAtTimeAsDateTimeOffset);
            set => EventGeneratedAtTimeAsDateTimeOffset = value.ToDateTimeOffset();
        }

        public string ObjectHash { get; set; }

        public Boolean RecordCanBePublished { get; set; }

        public StreetNameLinkedDataEventStreamItem CloneAndApplyEventInfo(
            long newPosition,
            string eventName,
            Instant generatedAtTime,
            Action<StreetNameLinkedDataEventStreamItem> editFunc)
        {
            var newItem = new StreetNameLinkedDataEventStreamItem
            {
                Position = newPosition,
                ChangeType = eventName,

                StreetNameId = StreetNameId,
                NisCode = NisCode,

                PersistentLocalId = PersistentLocalId,

                NameDutch = NameDutch,
                NameEnglish = NameEnglish,
                NameFrench = NameFrench,
                NameGerman = NameGerman,

                HomonymAdditionDutch = HomonymAdditionDutch,
                HomonymAdditionEnglish = HomonymAdditionEnglish,
                HomonymAdditionFrench = HomonymAdditionFrench,
                HomonymAdditionGerman = HomonymAdditionGerman,

                Status = Status,
                RecordCanBePublished = RecordCanBePublished,
                EventGeneratedAtTime = generatedAtTime
            };

            editFunc(newItem);

            return newItem;
        }
    }

    public class StreetNameLinkedDataEventStreamConfiguration : IEntityTypeConfiguration<StreetNameLinkedDataEventStreamItem>
    {
        private const string TableName = "StreetNameLinkedDataEventStream";

        public void Configure(EntityTypeBuilder<StreetNameLinkedDataEventStreamItem> builder)
        {
            builder.ToTable(TableName, Schema.Legacy)
                .HasKey(x => x.Position)
                .IsClustered();

            builder.Property(x => x.Position).ValueGeneratedNever();
            builder.HasIndex(x => x.Position).IsColumnStore($"CI_{TableName}_Position");

            builder.Property(x => x.StreetNameId).IsRequired();
            builder.Property(x => x.NisCode);
            builder.Property(x => x.ChangeType);

            builder.Property(x => x.NameDutch);
            builder.Property(x => x.NameFrench);
            builder.Property(x => x.NameGerman);
            builder.Property(x => x.NameEnglish);

            builder.Property(x => x.HomonymAdditionDutch);
            builder.Property(x => x.HomonymAdditionFrench);
            builder.Property(x => x.HomonymAdditionGerman);
            builder.Property(x => x.HomonymAdditionEnglish);

            builder.Property(x => x.Status);
            builder.Property(x => x.RecordCanBePublished);

            builder.Property(x => x.EventGeneratedAtTimeAsDateTimeOffset).HasColumnName("EventGeneratedAtTime");
            builder.Property(x => x.ObjectHash).HasColumnName("ObjectIdentifier");

            builder.Ignore(x => x.EventGeneratedAtTime);

            builder.HasIndex(x => x.StreetNameId);
        }
    }
}
