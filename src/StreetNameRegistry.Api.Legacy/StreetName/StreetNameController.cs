namespace StreetNameRegistry.Api.Legacy.StreetName
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.Api.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;
    using Projections.Legacy;
    using Projections.Legacy.StreetNameList;
    using Projections.Syndication;
    using Query;
    using Requests;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Infrastructure;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public class StreetNameController : ApiController
    {
        /// <summary>
        /// Vraag een straatnaam op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="persistentLocalId">De persistente lokale identificator van de straatnaam.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de straatnaam gevonden is.</response>
        /// <response code="404">Als de straatnaam niet gevonden kan worden.</response>
        /// <response code="410">Als de straatnaam verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{persistentLocalId}")]
        [ProducesResponseType(typeof(StreetNameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(StreetNameNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(StreetNameGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext legacyContext,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] int persistentLocalId,
            CancellationToken cancellationToken = default)
            => Ok(await new StreetNameDetailQuery(legacyContext, syndicationContext, responseOptions).FilterAsync(persistentLocalId, cancellationToken));

        /// <summary>
        /// Vraag een specifieke versie van een straatnaam op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="persistentLocalId">De persistente lokale identificator van de straatnaam.</param>
        /// <param name="versie">De specifieke versie van de straatnaam.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de straatnaam gevonden is.</response>
        /// <response code="404">Als de straatnaam niet gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{persistentLocalId}/versies/{versie}")]
        [ProducesResponseType(typeof(StreetNameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(StreetNameNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext legacyContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] int persistentLocalId,
            [FromRoute] DateTimeOffset versie,
            CancellationToken cancellationToken = default)
        {
            var streetName = await legacyContext
                .StreetNameVersions
                .AsNoTracking()
                .Where(x => !x.Removed)
                .SingleOrDefaultAsync(item => item.PersistentLocalId == persistentLocalId && item.VersionTimestampAsDateTimeOffset == versie, cancellationToken);

            if (streetName != null && streetName.Removed)
                throw new ApiException("Straatnaam verwijderd.", StatusCodes.Status410Gone);

            if (streetName == null || !streetName.Complete)
                throw new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound);

            var gemeente = new StraatnaamDetailGemeente
            {
                ObjectId = streetName.NisCode,
                Detail = string.Format(responseOptions.Value.GemeenteDetailUrl, streetName.NisCode),
                // TODO: get the name for this nisCode's municipality.
                // Not feasible yet, at least not yet without calling the Municipality Api.
                Gemeentenaam = new Gemeentenaam(new GeografischeNaam(null, Taal.NL))
            };

            return Ok(
                new StreetNameResponse(
                    responseOptions.Value.Naamruimte,
                    persistentLocalId,
                    streetName.Status.ConvertFromStreetNameStatus(),
                    gemeente,
                    streetName.VersionTimestampAsDateTimeOffset.Value,
                    streetName.NameDutch,
                    streetName.NameFrench,
                    streetName.NameGerman,
                    streetName.NameEnglish,
                    streetName.HomonymAdditionDutch,
                    streetName.HomonymAdditionFrench,
                    streetName.HomonymAdditionGerman,
                    streetName.HomonymAdditionEnglish));
        }

        /// <summary>
        /// Vraag een lijst met straatnamen op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="taal">Gewenste taal van de respons.</param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [ProducesResponseType(typeof(StreetNameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> List(
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] LegacyContext legacyContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            Taal? taal,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedStreetNames = new StreetNameListQuery(legacyContext, syndicationContext)
                .Fetch(filtering, sorting, pagination);

            Response.AddPagedQueryResultHeaders(pagedStreetNames);

            return Ok(
                new StreetNameListResponse
                {
                    Straatnamen = await pagedStreetNames
                        .Items
                        .Select(m => new StreetNameListItemResponse(
                            m.PersistentLocalId,
                            responseOptions.Value.Naamruimte,
                            responseOptions.Value.DetailUrl,
                            GetGeografischeNaamByTaal(m, m.PrimaryLanguage),
                            GetHomoniemToevoegingByTaal(m, m.PrimaryLanguage),
                            m.Status.ConvertFromStreetNameStatus(),
                            m.VersionTimestamp.ToBelgianDateTimeOffset()))
                        .ToListAsync(cancellationToken),
                    Volgende = BuildNextUri(pagedStreetNames.PaginationInfo, responseOptions.Value.VolgendeUrl)
                });
        }

        /// <summary>
        /// Vraag het totaal aantal van straatnamen op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("totaal-aantal")]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Count(
            [FromServices] LegacyContext context,
            [FromServices] SyndicationContext syndicationContext,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = new NoPaginationRequest();

            return Ok(
                new TotaalAantalResponse
                {
                    Aantal = filtering.ShouldFilter
                        ? await new StreetNameListQuery(context, syndicationContext)
                            .Fetch(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                        : Convert.ToInt32(context
                            .StreetNameListViewCount
                            .First()
                            .Count)
                });
        }

        /// <summary>
        /// Vraag een lijst met versies van een straatnaam op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="responseOptions"></param>
        /// <param name="persistentLocalId">De persistente lokale identifier van de straatnaam.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met versies van de straatnaam gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{persistentLocalId}/versies")]
        [ProducesResponseType(typeof(List<StreetNameVersionListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameVersionListResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> List(
            [FromServices] LegacyContext legacyContext,
            [FromServices] IHostingEnvironment hostingEnvironment,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] int persistentLocalId,
            CancellationToken cancellationToken = default)
        {
            var streetNameVersions =
                await legacyContext
                    .StreetNameVersions
                    .AsNoTracking()
                    .Where(p => !p.Removed && p.PersistentLocalId == persistentLocalId)
                    .ToListAsync(cancellationToken);

            if (!streetNameVersions.Any())
                throw new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound);

            return Ok(
                new StreetNameVersionListResponse
                {
                    StraatnaamVersies = streetNameVersions
                        .Select(m => new StreetNameVersionResponse(
                            m.VersionTimestampAsDateTimeOffset,
                            responseOptions.Value.DetailUrl,
                            persistentLocalId))
                        .ToList()
                });
        }

        /// <summary>
        /// Vraag een lijst met wijzigingen van straatnamen op.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("sync")]
        [Produces("text/xml")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameSyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Sync(
            [FromServices] IConfiguration configuration,
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameSyndicationFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var lastFeedUpdate = await context
                .StreetNameSyndication
                .AsNoTracking()
                .OrderByDescending(item => item.Position)
                .Select(item => item.SyndicationItemCreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastFeedUpdate == default)
                lastFeedUpdate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var pagedStreetNames =
                new StreetNameSyndicationQuery(
                    context,
                    filtering.Filter?.Embed)
                .Fetch(filtering, sorting, pagination);

            return new ContentResult
            {
                Content = await BuildAtomFeed(lastFeedUpdate, pagedStreetNames, responseOptions, configuration),
                ContentType = MediaTypeNames.Text.Xml,
                StatusCode = StatusCodes.Status200OK
            };
        }

        /// <summary>
        /// Zoek naar straatnamen in het Vlaams Gewest in het BOSA formaat.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="request">De request in BOSA formaat.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("bosa")]
        [ProducesResponseType(typeof(StreetNameBosaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(BosaStreetNameRequest), typeof(StreetNameBosaRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameBosaResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Post(
            [FromServices] LegacyContext legacyContext,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromBody] BosaStreetNameRequest request,
            CancellationToken cancellationToken = default)
        {
            if (Request.ContentLength.HasValue && Request.ContentLength > 0 && request == null)
                return Ok(new StreetNameBosaResponse());

            var filter = new StreetNameNameFilter(request);
            var streetNameBosaResponse = await
                new StreetNameBosaQuery(
                        legacyContext,
                        syndicationContext,
                        responseOptions)
                    .FilterAsync(filter, cancellationToken);

            return Ok(streetNameBosaResponse);
        }

        /// <summary>
        /// Vraag een lijst van wijzigingen van straatnamen op, semantisch geannoteerd (Linked Data Event Stream).
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("linked-data-event-stream")]
        [Produces("application/ld+json")]
        [ProducesResponseType(typeof(StreetNameLinkedDataEventStreamResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameLinkedDataEventStreamResponse))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> LinkedDataEventStream(
           [FromServices] LinkedDataEventStreamConfiguration configuration,
           [FromServices] LegacyContext context,
           [FromServices] IOptions<ResponseOptions> responseOptions,
           CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameLinkedDataEventStreamFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var xPaginationHeader = Request.Headers["x-pagination"].ToString().Split(",");
            var offset = Int32.Parse(xPaginationHeader[0]);
            var pageSize = Int32.Parse(xPaginationHeader[1]);
            var page = (offset / pageSize) + 1;

            var streetNamesPaged =
                new StreetNameLinkedDataEventStreamQuery(context)
                .Fetch(filtering, sorting, pagination);

            Console.WriteLine(streetNamesPaged);

            var streetNameVersionObjects =
                streetNamesPaged
                .Items
                .Select(x => new StreetNameVersionObject(
                    configuration,
                    x.ObjectIdentifier,
                    x.PersistentLocalId,
                    x.ChangeType,
                    x.EventGeneratedAtTime,
                    x.Status,
                    x.NisCode,
                    x.NameDutch,
                    x.NameFrench,
                    x.NameGerman,
                    x.NameEnglish,
                    x.HomonymAdditionDutch,
                    x.HomonymAdditionFrench,
                    x.HomonymAdditionGerman,
                    x.HomonymAdditionEnglish))
                .ToList();

            return Ok(new StreetNameLinkedDataEventStreamResponse
            {
                Id = StreetNameLinkedDataEventStreamMetadata.GetPageIdentifier(configuration, page),
                CollectionLink = StreetNameLinkedDataEventStreamMetadata.GetCollectionLink(configuration),
                StreetNameShape = StreetNameLinkedDataEventStreamMetadata.GetShapeUri(configuration),
                HypermediaControls = StreetNameLinkedDataEventStreamMetadata.GetHypermediaControls(streetNameVersionObjects, configuration, page, pageSize),
                StreetNames = streetNameVersionObjects
            });
        }

        /// <summary>
        /// Vraag de shape van straatnamen op.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("linked-data-event-stream/shape")]
        [Produces("application/ld+json")]
        [ProducesResponseType(typeof(StreetNameShaclShapeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameShaclShapeResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Shape(
            [FromServices] LinkedDataEventStreamConfiguration configuration,
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            return Ok(new StreetNameShaclShapeResponse
            {
                Id = new Uri($"{configuration.ApiEndpoint}/shape")
            });
        }

        private static GeografischeNaam GetGeografischeNaamByTaal(StreetNameListItem item, Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.NameDutch):
                case Language.Dutch when !string.IsNullOrEmpty(item.NameDutch):
                    return new GeografischeNaam(
                        item.NameDutch,
                        Taal.NL);

                case Language.French when !string.IsNullOrEmpty(item.NameFrench):
                    return new GeografischeNaam(
                        item.NameFrench,
                        Taal.FR);

                case Language.German when !string.IsNullOrEmpty(item.NameGerman):
                    return new GeografischeNaam(
                        item.NameGerman,
                        Taal.DE);

                case Language.English when !string.IsNullOrEmpty(item.NameEnglish):
                    return new GeografischeNaam(
                        item.NameEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private static GeografischeNaam GetHomoniemToevoegingByTaal(StreetNameListItem item, Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                case Language.Dutch when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                    return new GeografischeNaam(
                        item.HomonymAdditionDutch,
                        Taal.NL);

                case Language.French when !string.IsNullOrEmpty(item.HomonymAdditionFrench):
                    return new GeografischeNaam(
                        item.HomonymAdditionFrench,
                        Taal.FR);

                case Language.German when !string.IsNullOrEmpty(item.HomonymAdditionGerman):
                    return new GeografischeNaam(
                        item.HomonymAdditionGerman,
                        Taal.DE);

                case Language.English when !string.IsNullOrEmpty(item.HomonymAdditionEnglish):
                    return new GeografischeNaam(
                        item.HomonymAdditionEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private static async Task<string> BuildAtomFeed(
            DateTimeOffset lastFeedUpdate,
            PagedQueryable<StreetNameSyndicationQueryResult> pagedStreetNames,
            IOptions<ResponseOptions> responseOptions,
            IConfiguration configuration)
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Async = true, Indent = true, Encoding = sw.Encoding }))
            {
                var formatter = new AtomFormatter(null, xmlWriter.Settings) { UseCDATA = true };
                var writer = new AtomFeedWriter(xmlWriter, null, formatter);
                var syndicationConfiguration = configuration.GetSection("Syndication");
                var atomFeedConfig = AtomFeedConfigurationBuilder.CreateFrom(syndicationConfiguration, lastFeedUpdate);

                await writer.WriteDefaultMetadata(atomFeedConfig);

                var streetNames = pagedStreetNames.Items.ToList();

                var nextFrom = streetNames.Any()
                    ? streetNames.Max(s => s.Position) + 1
                    : (long?)null;

                var nextUri = BuildNextSyncUri(pagedStreetNames.PaginationInfo.Limit, nextFrom, syndicationConfiguration["NextUri"]);
                if (nextUri != null)
                    await writer.Write(new SyndicationLink(nextUri, GrArAtomLinkTypes.Next));

                foreach (var streetName in streetNames)
                    await writer.WriteStreetName(responseOptions, formatter, syndicationConfiguration["Category"], streetName);

                xmlWriter.Flush();
            }

            return sw.ToString();
        }

        private static Uri BuildNextUri(PaginationInfo paginationInfo, string nextUrlBase)
        {
            var offset = paginationInfo.Offset;
            var limit = paginationInfo.Limit;

            return paginationInfo.HasNextPage
                ? new Uri(string.Format(nextUrlBase, offset + limit, limit))
                : null;
        }

        private static Uri BuildNextSyncUri(int limit, long? from, string nextUrlBase)
        {
            return from.HasValue
                ? new Uri(string.Format(nextUrlBase, from, limit))
                : null;
        }
    }
}
