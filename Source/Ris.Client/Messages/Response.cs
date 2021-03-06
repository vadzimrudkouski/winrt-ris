﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 
namespace Ris.Client.Messages.Response {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlRootAttribute("OGDSearchResult", Namespace="", IsNullable=false)]
    public partial class T_OGDSearchResult {
        
        private object itemField;
        
        private T_OGDSearchResultStatus statusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Error", typeof(T_Error), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("SearchDocumentsResult", typeof(T_OGDSearchResultSearchDocumentsResult), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public T_OGDSearchResultStatus status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
    public partial class T_Error {
        
        private T_Applikation applikationField;
        
        private string messageField;
        
        private int errorTypeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public T_Applikation Applikation {
            get {
                return this.applikationField;
            }
            set {
                this.applikationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int errorType {
            get {
                return this.errorTypeField;
            }
            set {
                this.errorTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
    [System.Xml.Serialization.XmlRootAttribute("Applikation", Namespace="http://ris.bka.gv.at/Search/1.3/OGD", IsNullable=false)]
    public enum T_Applikation {
        
        /// <remarks/>
        Bundesrecht,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
    [System.Xml.Serialization.XmlRootAttribute("OGDDocumentReference", Namespace="http://ris.bka.gv.at/Search/1.3/OGD", IsNullable=false)]
    public partial class T_OGDDocumentReference {
        
        private T_Applikation applikationField;
        
        private string dokumentnummerField;
        
        private string artikelParagraphAnlageField;
        
        private string kurzinformationField;
        
        private string dokumentUrlField;
        
        /// <remarks/>
        public T_Applikation Applikation {
            get {
                return this.applikationField;
            }
            set {
                this.applikationField = value;
            }
        }
        
        /// <remarks/>
        public string Dokumentnummer {
            get {
                return this.dokumentnummerField;
            }
            set {
                this.dokumentnummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string ArtikelParagraphAnlage {
            get {
                return this.artikelParagraphAnlageField;
            }
            set {
                this.artikelParagraphAnlageField = value;
            }
        }
        
        /// <remarks/>
        public string Kurzinformation {
            get {
                return this.kurzinformationField;
            }
            set {
                this.kurzinformationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI")]
        public string DokumentUrl {
            get {
                return this.dokumentUrlField;
            }
            set {
                this.dokumentUrlField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class T_OGDSearchResultSearchDocumentsResult {
        
        private Hits hitsField;
        
        private T_OGDDocumentReference[] documentReferencesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
        public Hits Hits {
            get {
                return this.hitsField;
            }
            set {
                this.hitsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
        [System.Xml.Serialization.XmlArrayItemAttribute("OGDDocumentReference", IsNullable=false)]
        public T_OGDDocumentReference[] DocumentReferences {
            get {
                return this.documentReferencesField;
            }
            set {
                this.documentReferencesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD", IsNullable=false)]
    public partial class Hits {
        
        private string pageNumberField;
        
        private string pageSizeField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        public string pageNumber {
            get {
                return this.pageNumberField;
            }
            set {
                this.pageNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        public string pageSize {
            get {
                return this.pageSizeField;
            }
            set {
                this.pageSizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType="integer")]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public enum T_OGDSearchResultStatus {
        
        /// <remarks/>
        ok,
        
        /// <remarks/>
        error,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ris.bka.gv.at/Search/1.3/OGD")]
    [System.Xml.Serialization.XmlRootAttribute("DocumentReferences", Namespace="http://ris.bka.gv.at/Search/1.3/OGD", IsNullable=false)]
    public partial class T_DocumentReferences {
        
        private T_OGDDocumentReference[] oGDDocumentReferenceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OGDDocumentReference")]
        public T_OGDDocumentReference[] OGDDocumentReference {
            get {
                return this.oGDDocumentReferenceField;
            }
            set {
                this.oGDDocumentReferenceField = value;
            }
        }
    }
}
