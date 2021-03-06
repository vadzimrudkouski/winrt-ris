﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ris.Client.PhraseParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ris.Data;
using Ris.Data.Models;
using Risotto.Models;
using Risotto.Services;
using Ris.Client.Models;
using System.Globalization;
using Ris.Client;

namespace Risotto.ViewModels
{
    public class AdvancedSearchViewModel : RisViewModelBase
    {
        public AdvancedSearchViewModel()
        {
            Kundmachungsorgane = Kundmachungsorgan.GenerateList();
            ImRisSeitSource = ImRisSeitListItem.GenerateList();

            SetSearchDefaults();
        }

        public List<ImRisSeitListItem> ImRisSeitSource { get; private set; }
        public List<Kundmachungsorgan> Kundmachungsorgane { get; set; }

        public const string SelectedImRisSeitListItemPropertyName = "SelectedImRisSeitListItem";
        private ImRisSeitListItem _selectedImRisSeitListItem = null;

        public ImRisSeitListItem SelectedImRisSeitListItem
        {
            get
            {
                return _selectedImRisSeitListItem;
            }

            set { Set(SelectedImRisSeitListItemPropertyName, ref _selectedImRisSeitListItem, value); }
        }

        public const string SelectedKundmachungsorganPropertyName = "SelectedKundmachungsorgan";
        private Kundmachungsorgan _selectedKundmachungsorgan = null;

        public Kundmachungsorgan SelectedKundmachungsorgan
        {
            get
            {
                return _selectedKundmachungsorgan;
            }

            set { Set(SelectedKundmachungsorganPropertyName, ref _selectedKundmachungsorgan, value); }
        }

        private void SelectImRisSeitListItem(ChangedWithinEnum c)
        {
            SelectedImRisSeitListItem = ImRisSeitSource.FirstOrDefault(i => i.ImRisSeit == c);
        }

        private void SelectKundmachungsorgan(string k)
        {
            SelectedKundmachungsorgan = Kundmachungsorgane.FirstOrDefault(i => i.Text == k);
        }

        public const string ValidationMessagePropertyName = "ValidationMessage";
        private string _validationMessage = "";

        public string ValidationMessage
        {
            get
            {
                return _validationMessage;
            }

            set { Set(ValidationMessagePropertyName, ref _validationMessage, value); }
        }

        public const string SuchwortePropertyName = "Suchworte";
        private string _suchworte = "";

        public string Suchworte
        {
            get
            {
                return _suchworte;
            }

            set { Set(SuchwortePropertyName, ref _suchworte, value); }
        }

        public const string TitelAbkuerzungPropertyName = "TitelAbkuerzung";
        private string _titelakue = "";

        public string TitelAbkuerzung
        {
            get
            {
                return _titelakue;
            }

            set { Set(TitelAbkuerzungPropertyName, ref _titelakue, value); }
        }

        public const string ParagrafVonPropertyName = "ParagrafVon";
        private string _paragrafvon = "";

        public string ParagrafVon
        {
            get
            {
                return _paragrafvon;
            }

            set { Set(ParagrafVonPropertyName, ref _paragrafvon, value); }
        }

        public const string ParagrafBisPropertyName = "ParagrafBis";
        private string _paragrafbis = "";

        public string ParagrafBis
        {
            get
            {
                return _paragrafbis;
            }

            set { Set(ParagrafBisPropertyName, ref _paragrafbis, value); }
        }

        public const string ArtikelVonPropertyName = "ArtikelVon";
        private string _artikelvon = "";

        public string ArtikelVon
        {
            get
            {
                return _artikelvon;
            }

            set { Set(ArtikelVonPropertyName, ref _artikelvon, value); }
        }

        public const string ArtikelBisPropertyName = "ArtikelBis";
        private string _artikelbis = "";

        public string ArtikelBis
        {
            get
            {
                return _artikelbis;
            }

            set { Set(ArtikelBisPropertyName, ref _artikelbis, value); }
        }

        public const string AnlageVonPropertyName = "AnlageVon";
        private string _anlagevon = "";

        public string AnlageVon
        {
            get
            {
                return _anlagevon;
            }

            set { Set(AnlageVonPropertyName, ref _anlagevon, value); }
        }

        public const string AnlageBisPropertyName = "AnlageBis";
        private string _anlagebis = "";

        public string AnlageBis
        {
            get
            {
                return _anlagebis;
            }

            set { Set(AnlageBisPropertyName, ref _anlagebis, value); }
        }

        public const string KundmachungsorganNummerPropertyName = "KundmachungsorganNummer";
        private string _kundmachungsorganNummer = "";

        public string KundmachungsorganNummer
        {
            get
            {
                return _kundmachungsorganNummer;
            }

            set { Set(KundmachungsorganNummerPropertyName, ref _kundmachungsorganNummer, value); }
        }

        public const string TypPropertyName = "Typ";
        private string _typ = "";

        public string Typ
        {
            get
            {
                return _typ;
            }

            set { Set(TypPropertyName, ref _typ, value); }
        }

        public const string IndexPropertyName = "Index";
        private string _index = "";

        public string Index
        {
            get
            {
                return _index;
            }

            set { Set(IndexPropertyName, ref _index, value); }
        }

        public const string UnterzeichnungsdatumPropertyName = "Unterzeichnungsdatum";
        private string _unterzeichnungsdatum = "";

        public string Unterzeichnungsdatum
        {
            get
            {
                return _unterzeichnungsdatum;
            }

            set { Set(UnterzeichnungsdatumPropertyName, ref _unterzeichnungsdatum, value); }
        }

        public const string FassungVomPropertyName = "FassungVom";
        private string _fassungvom = "";

        public string FassungVom
        {
            get
            {
                return _fassungvom;
            }

            set { Set(FassungVomPropertyName, ref _fassungvom, value); }
        }

        private RisAdvancedQueryParameter Validate()
        {
            var p = new RisAdvancedQueryParameter();

            p.Suchworte = ValidatePhraseExpressionText(Suchworte, "Suchworte enthält keine gültige Abfrage");
            p.TitelAbkuerzung = ValidatePhraseExpressionText(TitelAbkuerzung, "Titel, Abkürzung enthält keine gültige Abfrage");

            ValidateAbschnitt(p);

            p.Kundmachungsorgan = SelectedKundmachungsorgan.Text;
            p.KundmachungsorganNummer = KundmachungsorganNummer;

            p.Typ = ValidatePhraseExpressionText(Typ, "Typ enthält keine gültige Abfrage");
            p.Index = ValidatePhraseExpressionText(Index, "Index enthält keine gültige Abfrage");

            p.Unterzeichnungsdatum = ValidateNonEmptyTextToDate(Unterzeichnungsdatum, "Unterzeichnungsdatum ist kein gültiges Datum");
            p.FassungVom = ValidateNonEmptyTextToDate(FassungVom, "Fassung vom ist kein gültiges Datum");
            p.ImRisSeit = SelectedImRisSeitListItem.ImRisSeit;

            bool anyFailures = !String.IsNullOrWhiteSpace(ValidationMessage);
            return (anyFailures ? null : p);
        }

        private void ValidateAbschnitt(RisAdvancedQueryParameter p)
        {
            var inputmask = new Dictionary<AbschnittTypEnum, bool>();
            inputmask[AbschnittTypEnum.Paragraph] = !String.IsNullOrWhiteSpace(ParagrafVon) || !String.IsNullOrWhiteSpace(ParagrafBis);
            inputmask[AbschnittTypEnum.Artikel] = !String.IsNullOrWhiteSpace(ArtikelVon) || !String.IsNullOrWhiteSpace(ArtikelBis);
            inputmask[AbschnittTypEnum.Anlage] = !String.IsNullOrWhiteSpace(AnlageVon) || !String.IsNullOrWhiteSpace(AnlageBis);

            int countOfAbschnitteSpecified = inputmask.Count(kvp => kvp.Value);

            // If not specified at all, return immediately
            if (0 == countOfAbschnitteSpecified) return;

            if (countOfAbschnitteSpecified > 1)
            {
                ValidationMessage += "Paragraph, Artikel, Anlage dürfen jeweils nur ausschließlich eingegeben werden" + Environment.NewLine;
            }
            else
            {
                string von, bis;
                AbschnittTypEnum typ;
                if (inputmask[AbschnittTypEnum.Paragraph])
                {
                    von = ParagrafVon;
                    bis = ParagrafBis;
                    typ = AbschnittTypEnum.Paragraph;
                }
                else if (inputmask[AbschnittTypEnum.Artikel])
                {
                    von = ArtikelVon;
                    bis = ArtikelBis;
                    typ = AbschnittTypEnum.Artikel;
                }
                else
                {
                    von = AnlageVon;
                    bis = AnlageBis;
                    typ = AbschnittTypEnum.Anlage;
                }

                var abschnitt = AbschnittParser.Parse(von, bis, typ);

                if (null == abschnitt)
                {
                    ValidationMessage += typ.ToString() + " Von-Bis ungültig" + Environment.NewLine;
                }
                else
                {
                    p.Von = von;
                    p.Bis = bis;
                    p.AbschnittTyp = typ;
                }
            }
        }

        private DateTime? ValidateNonEmptyTextToDate(string searchText, string validationMessageToAdd)
        {
            if (String.IsNullOrWhiteSpace(searchText))
                return null;

            DateTime result = DateTime.Now;
            bool pOk = DateTime.TryParse(searchText, out result);

            if (!pOk)
            {
                ValidationMessage += validationMessageToAdd + Environment.NewLine;
                return null;
            }

            return result.Date;
        }

        private string ValidatePhraseExpressionText(string searchText, string validationMessageToAdd)
        {
            if (String.IsNullOrWhiteSpace(searchText))
                return null;

            try
            {
                var expr = QueryParser.Parse(searchText);
                return searchText.Trim();
            }
            catch (ParseException)
            {
                ValidationMessage += validationMessageToAdd + Environment.NewLine;
                return null;
            }
        }

        private RelayCommand _submitCommand;
        public RelayCommand SubmitCommand
        {
            get
            {
                return _submitCommand
                    ?? (_submitCommand = new RelayCommand(Submit));
            }
        }

        public void Submit()
        {
            ValidationMessage = "";
            
            var p = Validate();
            if (null == p) return;

            string navParam = RisQueryParameterSerializeable.Serialize(p);
            NavigationService.Navigate<SearchResultsPage>(navParam);
        }

        private RelayCommand _resetCommand;
        public RelayCommand ResetCommand
        {
            get
            {
                return _resetCommand
                    ?? (_resetCommand = new RelayCommand(Reset));
            }
        }

        public void Reset()
        {
            ValidationMessage = "";

            SetSearchDefaults();

            Suchworte = "";
            TitelAbkuerzung = "";

            ParagrafVon = "";
            ParagrafBis = "";
            ArtikelVon = "";
            ArtikelBis = "";
            AnlageVon = "";
            AnlageBis = "";

            KundmachungsorganNummer = "";

            Typ = "";
            Index = "";

            Unterzeichnungsdatum = "";
        }

        private void SetSearchDefaults()
        {
            // Set the search defaults
            FassungVom = DateTime.Now.ToString("d");
            SelectImRisSeitListItem(ChangedWithinEnum.Undefined);
            SelectKundmachungsorgan("");
        }

        public AdvancedSearchPageState SaveState()
        {
            return new AdvancedSearchPageState()
                       {
                           Suchworte = this.Suchworte,
                           TitelAbkuerzung = this.TitelAbkuerzung,
                           ParagrafVon = this.ParagrafVon,
                           ParagrafBis = this.ParagrafBis,
                           ArtikelVon = this.ArtikelVon,
                           ArtikelBis = this.ArtikelBis,
                           AnlageVon = this.AnlageVon,
                           AnlageBis = this.AnlageBis,
                           Kundmachungsorgan = this.SelectedKundmachungsorgan.Text,
                           KundmachungsorganNummer = this.KundmachungsorganNummer,
                           Typ = this.Typ,
                           Index = this.Index,
                           Unterzeichnungsdatum = this.Unterzeichnungsdatum,
                           FassungVom = this.FassungVom,
                           ImRisSeit = this.SelectedImRisSeitListItem.ImRisSeit,
                       };
        }

        public void LoadState(AdvancedSearchPageState state)
        {
            Suchworte = state.Suchworte;
            TitelAbkuerzung = state.TitelAbkuerzung;
            ParagrafVon = state.ParagrafVon;
            ParagrafBis = state.ParagrafBis;
            ArtikelVon = state.ArtikelVon;
            ArtikelBis = state.ArtikelBis;
            AnlageVon = state.AnlageVon;
            AnlageBis = state.AnlageBis;
            SelectKundmachungsorgan(state.Kundmachungsorgan);
            KundmachungsorganNummer = state.KundmachungsorganNummer;
            Typ = state.Typ;
            Index = state.Index;
            Unterzeichnungsdatum = state.Unterzeichnungsdatum;
            FassungVom = state.FassungVom;
            SelectImRisSeitListItem(state.ImRisSeit);
        }

        public void InitializeFromParameter(RisAdvancedQueryParameter param)
        {
            Suchworte = param.Suchworte;
            TitelAbkuerzung = param.TitelAbkuerzung;

            switch (param.AbschnittTyp)
            {
                case AbschnittTypEnum.Paragraph:
                    ParagrafVon = param.Von;
                    ParagrafBis = param.Bis;
                    break;
                case AbschnittTypEnum.Artikel:
                    ArtikelVon = param.Von;
                    ArtikelBis = param.Bis;
                    break;
                case AbschnittTypEnum.Anlage:
                    AnlageVon = param.Von;
                    AnlageBis = param.Bis;
                    break;
            }

            SelectKundmachungsorgan(param.Kundmachungsorgan);
            KundmachungsorganNummer = param.KundmachungsorganNummer;
            Typ = param.Typ;
            Index = param.Index;
            
            if (param.Unterzeichnungsdatum.HasValue)
                Unterzeichnungsdatum = param.Unterzeichnungsdatum.Value.ToString("d");
            if (param.FassungVom.HasValue)
                FassungVom = param.FassungVom.Value.ToString("d");

            SelectImRisSeitListItem(param.ImRisSeit);
        }
    }
}
