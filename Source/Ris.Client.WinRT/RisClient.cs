﻿using Ris.Client.Messages;
using Ris.Data.Models;
using Ris.Client.Models;

using Req = Ris.Client.Messages.Request;
using Resp = Ris.Client.Messages.Response;
using Doc = Ris.Client.Messages.Document;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ris.Client.WinRT
{
    // This wraps all the calls to the Web Service / nice frontend with proper classes
    public class RisClient
    {
        public const string ErrorOnVersionRequest = "Versionsabfrage RIS OGD Service fehlgeschlagen";
        public Func<OGDServiceSoapClient> CreateServiceClient { get; set; }

        public RisClient()
        {
            CreateServiceClient = () => new OGDServiceSoapClient();
        }

        public async Task<string> GetVersionAsync()
        {
            var client = CreateServiceClient();
            string versionToReturn = ErrorOnVersionRequest;

            try
            {
                var version = await client.versionAsync();
                versionToReturn = version.Body.versionResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RisClient::GetVersionAsync Exception: " + ex.ToString());
            }

            return versionToReturn;
        }

        private Req.T_OGDSearchRequest PrepareFulltextSearch(RisFulltextQueryParameter param)
        {
            var request = new Req.T_OGDSearchRequest();
            var volltext = new Req.PhraseSearchExpression();

            volltext.Value = param.SearchText;
            request.Suchworte = volltext;

            return request;
        }

        public async Task<SearchResult> QueryAsync(RisQueryParameter param, int seitenNummer)
        {
            Req.T_OGDSearchRequest request = null;

            if (param is RisFulltextQueryParameter)
            {
                request = PrepareFulltextSearch((RisFulltextQueryParameter)param);
            }
            else if (param is RisAdvancedQueryParameter)
            {
                throw new NotImplementedException();
            }

            if (null == request)
                return new SearchResult("Kein Query Processor gefunden");

            return await QueryAsync(request, seitenNummer);
        }

        private async Task<SearchResult> QueryAsync(Req.T_OGDSearchRequest request, int seitenNummer)
        {
            request.Seitennummer = seitenNummer;
            request.Sortierung = new Req.BundesnormenSortExpression()
            {
                SortDirection = Req.WebSortDirection.Ascending,
                SortedByColumn = Req.BundesnormenSortableColumn.Kurzinformation
            };

            // We do continuous loading in the UI, thus the user cannot specify the page size
            request.DokumenteProSeiteSpecified = true;
            request.DokumenteProSeite = Req.PageSize.Fifty;

            // TODO Read from query parameter (needs to be set in calling method)
            request.ImRisSeitSpecified = true;
            request.ImRisSeit = Req.ChangeSetInterval.Undefined;

            try
            {
                string requestAsString = MessageSerializationHelper.SerializeToString(request);

                var client = CreateServiceClient();
                requestResponse response = await client.requestAsync("Br", requestAsString);

                var searchResult = MessageSerializationHelper.DeserializeFromString<Resp.T_OGDSearchResult>(response.Body.requestResult);
                return Mapper.MapSearchResult(searchResult);
            }
            catch (Exception ex)
            {
                return new SearchResult(ex.ToString());
            }
        }

        public async Task<DocumentResult> GetDocument(string dokumentNummer)
        {
            var client = CreateServiceClient();

            try
            {
                getDocumentResponse doc = await client.getDocumentAsync("Br", dokumentNummer);

                var documentResult = MessageSerializationHelper.DeserializeFromString<Doc.DocumentResult>(doc.Body.getDocumentResult);
                return Mapper.MapDocumentResult(documentResult);
            }
            catch (Exception ex)
            {
                return new DocumentResult(ex.ToString());
            }
        }
    }
}