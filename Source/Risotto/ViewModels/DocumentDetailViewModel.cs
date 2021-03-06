﻿using System.Diagnostics;
using System.IO;
using System.Net.Http;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Ris.Client;
using Ris.Client.Messages;
using Ris.Client.Models;
using Ris.Client.WinRT;
using Ris.Data;
using Ris.Data.Models;
using Risotto.Models;
using Risotto.Services;
using Windows.Storage.Pickers;

namespace Risotto.ViewModels
{
    public class DocumentDetailViewModel : RisViewModelBase
    {
        public bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
            }
        }
        public const string UpdateInProgressPropertyName = "UpdateInProgress";
        private bool _updateInProgress = false;

        public bool UpdateInProgress
        {
            get
            {
                return _updateInProgress;
            }
            set
            {
                Set(UpdateInProgressPropertyName, ref _updateInProgress, value);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                Set(PageTitlePropertyName, ref _pageTitle, value);
            }
        }

        public const string SourceHtmlPropertyName = "SourceHtml";
        private string _sourceHtml = null;

        public string SourceHtml
        {
            get
            {
                return _sourceHtml;
            }
            set
            {
                Set(SourceHtmlPropertyName, ref _sourceHtml, value);
            }
        }

        public const string NavigationParameterPropertyName = "NavigationParameter";
        private DocumentDetailNavigationParameter _navigationParameter = null;

        public DocumentDetailNavigationParameter NavigationParameter
        {
            get
            {
                return _navigationParameter;
            }
            set
            {
                Set(NavigationParameterPropertyName, ref _navigationParameter, value);
                RaisePropertyChanged(CanAddDownloadPropertyName);
            }
        }

        public const string CurrentDocumentPropertyName = "CurrentDocument";
        private Ris.Client.Models.DocumentResult _currentDocument = null;

        public Ris.Client.Models.DocumentResult CurrentDocument
        {
            get
            {
                return _currentDocument;
            }
            set
            {
                Set(CurrentDocumentPropertyName, ref _currentDocument, value);
            }
        }

        public const string AttachmentsPropertyName = "Attachments";
        private List<Ris.Client.Models.DocumentContent> _attachments = null;

        public List<Ris.Client.Models.DocumentContent> Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                Set(AttachmentsPropertyName, ref _attachments, value);
            }
        }

        public async Task Load()
        {
            UpdateInProgress = true;
            PageTitle = "Lade " + NavigationParameter.DokumentTitel;

            bool loadingSucceeded = false;

            if (NavigationParameter.Action == NavigationAction.LoadFromService)
            {
                string dokumentNummer = NavigationParameter.Command;
                var result = await Task.Run(() => ParallelLoadSynced(dokumentNummer));

                if (null != result)
                {
                    CurrentDocument = result.Item1;
                    SourceHtml = result.Item2;
                    loadingSucceeded = true;
                }
            }
            else if (NavigationParameter.Action == NavigationAction.LoadCachedDownload)
            {
                loadingSucceeded = await LoadFromCacheAsync();
            }

            if (!loadingSucceeded)
            {
                PageTitle = "Fehler: Laden fehlgeschlagen";

                CurrentDocument = null;
                Attachments = null;
                SourceHtml = null;
            }
            else
            {
                PageTitle = CreateTitleFromDocument();
                Attachments = CurrentDocument.GetAttachments();
            }

            RaisePropertyChanged(CanAddDownloadPropertyName);
            UpdateInProgress = false;
        }

        private static Tuple<DocumentResult, string> ParallelLoadSynced(string dokumentNummer)
        {
            var svcTask = new Task<DocumentResult>(() => LoadFromServiceAsync(dokumentNummer).Result);
            var htmlTask = new Task<string>(() => DownloadHtmlFromRisServer(dokumentNummer).Result);

            svcTask.Start();
            htmlTask.Start();

            Task.WaitAll(svcTask, htmlTask);

            DocumentResult docResult = svcTask.Result;
            string html = htmlTask.Result;

            if (null != docResult && html != null)
            {
                return new Tuple<DocumentResult, string>(docResult, html);
            }

            return null;
        }

        private static async Task<string> DownloadHtmlFromRisServer(string dokumentNummer)
        {
            try
            {
                string url = RisUrlHelper.UrlForHtmlFromDokumentNummer(dokumentNummer);

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    client.Dispose();

                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string CreateTitleFromDocument()
        {
            var item = CurrentDocument.Document;
            return String.Format("{0} {1}", item.ArtikelParagraphAnlage, item.Kurztitel);
        }

        private async Task<bool> LoadFromCacheAsync()
        {
            int id = 0;
            if (!Int32.TryParse(NavigationParameter.Command, out id)) return false;

            var ctx = new RisDbContext();
            var doc = await ctx.GetDownload(id);

            if (null == doc) return false;

            try
            {
                // Rehydrate document from storage
                var documentResult = MessageSerializationHelper.DeserializeFromString<Ris.Client.Messages.Document.DocumentResult>(doc.OriginalDocumentResultXml);
                CurrentDocument = Mapper.MapDocumentResult(documentResult);

                CurrentDocument.OriginalDocumentResultXml = doc.OriginalDocumentResultXml;
                SourceHtml = doc.HtmlFromRisServer;
                CachedDocumentDatabaseId = doc.Id;

                return CurrentDocument.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error("LoadFromCacheAsync", ex);
            }

            return false;
        }

        private static async Task<DocumentResult> LoadFromServiceAsync(string dokumentNummer)
        {
            var client = new RisClient();
            var result = await client.GetDocumentAsync(dokumentNummer);

            if (result.Succeeded)
            {
                return result;
            }

            return null;
        }

        private RelayCommand _addDownloadCommand;
        public RelayCommand AddDownloadCommand
        {
            get
            {
                return _addDownloadCommand
                    ?? (_addDownloadCommand = new RelayCommand(
                        async () => await AddDownloadAsync(), () => CanAddDownload));
            }
        }

        private async Task AddDownloadAsync()
        {
            try
            {
                var ctx = new RisDbContext();
                var dl = DbDownloadedDocumentFromViewModel(NavigationParameter.Command);
                await ctx.InsertDownload(dl);

                _addOperationHasBeenExecuted = true;
                CachedDocumentDatabaseId = dl.Id;

                RaisePropertyChanged(CanAddDownloadPropertyName);
            }
            catch (Exception ex)
            {
                Log.Error("AddDownloadAsync", ex);
            }
        }

        private DbDownloadedDocument DbDownloadedDocumentFromViewModel(string dokumentNummer)
        {
            return new DbDownloadedDocument(dokumentNummer,
                                            NavigationParameter.DokumentTitel,
                                            CurrentDocument.OriginalDocumentResultXml,
                                            SourceHtml);
        }

        public const string CanAddDownloadPropertyName = "CanAddDownload";
        public bool CanAddDownload
        {
            get
            {
                if (null == NavigationParameter) return false;
                if (NavigationParameter.Action == NavigationAction.LoadFromUrl) return false;
                if (NavigationParameter.Action == NavigationAction.LoadCachedDownload) return false;

                if (CurrentDocument != null && CurrentDocument.Succeeded && !_addOperationHasBeenExecuted)
                    return true;

                return false;
            }
        }

        private bool _addOperationHasBeenExecuted = false;

        public const string CachedDocumentDatabaseIdPropertyName = "CachedDocumentDatabaseId";
        private int? _cachedDocumentDbId = null;

        public int? CachedDocumentDatabaseId
        {
            get
            {
                return _cachedDocumentDbId;
            }
            set
            {
                Set(CachedDocumentDatabaseIdPropertyName, ref _cachedDocumentDbId, value);
            }
        }

        private RelayCommand _refreshCachedDocumentCommand;
        public RelayCommand RefreshCachedDocumentCommand
        {
            get
            {
                return _refreshCachedDocumentCommand
                    ?? (_refreshCachedDocumentCommand = new RelayCommand(
                        async () => await RefreshCachedDocumentAsync()));
            }
        }

        private async Task RefreshCachedDocumentAsync()
        {
            try
            {
                UpdateInProgress = true;

                // #1: Refresh the data
                string dokumentNummer = CurrentDocument.Document.Dokumentnummer;
                var result = await Task.Run(() => ParallelLoadSynced(dokumentNummer));

                if (null != result)
                {
                    CurrentDocument = result.Item1;
                    SourceHtml = result.Item2;

                    // #2: Delete old row and Insert new one
                    var ctx = new RisDbContext();
                    var dl = DbDownloadedDocumentFromViewModel(dokumentNummer);
                    await ctx.RefreshDownload(dl, CachedDocumentDatabaseId.Value);

                    CachedDocumentDatabaseId = dl.Id;

                    ToastService.Display("Aktualisierung erfolgreich", CurrentDocument.Document.Kurztitel);

                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("RefreshCachedDocumentAsync", ex);
            }
            finally
            {
                UpdateInProgress = false;
            }

            ToastService.Display("Fehler",
                String.Format("Aktualisierung von \"{0}\" konnte nicht durchgeführt werden", CurrentDocument.Document.Kurztitel));
        }

        public void LoadState(DocumentDetailPageState state)
        {
            _addOperationHasBeenExecuted = state.AddOperationHasBeenExecuted;
            CachedDocumentDatabaseId = state.CachedDocumentDatabaseId;
        }

        public DocumentDetailPageState SaveState()
        {
            return new DocumentDetailPageState()
                            {
                                AddOperationHasBeenExecuted = _addOperationHasBeenExecuted,
                                CachedDocumentDatabaseId = this.CachedDocumentDatabaseId
                            };
        }

        private RelayCommand _saveHtmlCommand;
        public RelayCommand SaveHtmlCommand
        {
            get
            {
                return _saveHtmlCommand
                    ?? (_saveHtmlCommand = new RelayCommand(
                        async () => await SaveHtmlAsync()));
            }
        }

        public async Task SaveHtmlAsync()
        {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Html Datei", new List<string> { ".html" });
            fileSavePicker.DefaultFileExtension = ".html";

            string dokumentNummer = CurrentDocument.Document.Dokumentnummer;

            fileSavePicker.SuggestedFileName = String.Format("Ris{0}.html", dokumentNummer);

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            try
            {
                using (var stream = await fileToSave.OpenStreamForWriteAsync())
                {
                    var writer = new StreamWriter(stream);
                    await writer.WriteAsync(SourceHtml);
                    await writer.FlushAsync();
                    writer.Dispose();
                }

                ToastService.Display("Dokument gespeichert", 
                    String.Format("\"{0}\" in \"{1}\" abgelegt", CurrentDocument.Document.Kurztitel, fileToSave.Name));
            }
            catch (Exception ex)
            {
                Log.Error("SaveHtmlAsync", ex);
                ToastService.Display("Fehler", "Dokument konnte nicht gespeichert werden");
            }
        }

        public async Task SaveAttachmentAsync(DocumentContent attachment)
        {
            if (attachment == null || attachment.Content == null) return;

            var extension = Mapper.MapDocumentContentDataTypeEnumToExtension(attachment.DataType);

            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add(extension + " Datei", new List<string> { "." + extension });
            fileSavePicker.DefaultFileExtension = "." + extension;

            fileSavePicker.SuggestedFileName = attachment.ProposedFilename;

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            try
            {
                using (var stream = await fileToSave.OpenStreamForWriteAsync())
                {
                    await stream.WriteAsync(attachment.Content, 0, attachment.Content.Length);
                    await stream.FlushAsync();
                    stream.Dispose();

                    Windows.System.Launcher.LaunchFileAsync(fileToSave);
                }
            }
            catch (Exception ex)
            {
                Log.Error("SaveAttachmentAsync", ex);
                ToastService.Display("Fehler", "Attachment konnte nicht gespeichert werden");
            }
        }

        private RelayCommand _saveServiceXmlCommand;
        public RelayCommand SaveServiceXmlCommand
        {
            get
            {
                return _saveServiceXmlCommand
                    ?? (_saveServiceXmlCommand = new RelayCommand(
                        async () => await SaveServiceXmlAsync()));
            }
        }

        public async Task SaveServiceXmlAsync()
        {
            if (null == CurrentDocument) return;

            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Service XML", new List<string> { ".xml" });
            fileSavePicker.DefaultFileExtension = ".xml";

            string dokumentNummer = CurrentDocument.Document.Dokumentnummer;

            fileSavePicker.SuggestedFileName = String.Format("Ris{0}.xml", dokumentNummer);

            var fileToSave = await fileSavePicker.PickSaveFileAsync();
            if (null == fileToSave) return;

            using (var stream = await fileToSave.OpenStreamForWriteAsync())
            {
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(CurrentDocument.OriginalDocumentResultXml);
                await writer.FlushAsync();
                writer.Dispose();
            }

            // bool launched = await Windows.System.Launcher.LaunchFileAsync(fileToSave);
        }

        private RelayCommand _viewpdfCommand;
        public RelayCommand ViewPdfCommand
        {
            get
            {
                return _viewpdfCommand
                    ?? (_viewpdfCommand = new RelayCommand(
                        () => Windows.System.Launcher.LaunchUriAsync(new Uri(CurrentDocument.Document.PdfUrl))));
            }
        }

        private RelayCommand _viewonwebCommand;
        public RelayCommand ViewOnWebCommand
        {
            get
            {
                return _viewonwebCommand
                    ?? (_viewonwebCommand = new RelayCommand(
                        () => Windows.System.Launcher.LaunchUriAsync(new Uri(CurrentDocument.Document.HtmlUrl))));
            }
        }
    }
}
