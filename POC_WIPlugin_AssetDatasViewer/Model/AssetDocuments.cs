using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetDatasViewer
{
    public class AssetDocuments
    {
        List<Document> _documents = null;

        public AssetDocuments()
        {
            _documents = new List<Document>();
        }

        public AssetDocuments( List<Document> documents )
        {
            _documents = documents;
        }

        public List<Document> DocumentsList
        {
            get { return _documents; }
            set { _documents = value; }
        }

        public List<string> Types
        {
            get
            {
                List<string> docTypes = new List<string>();
                foreach( Document data in _documents )
                {
                    if( !docTypes.Contains( data.Type ) )
                        docTypes.Add( data.Type );
                }
                return docTypes;
            }
        }

        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach( Document doc in _documents )
                    names.Add( doc.Name );
                return names;
            }
        }

        public bool HasDocument( string name )
        {
            return Names.Contains( name ) ? true : false;
        }

        public Document GetDocument( string name )
        {
            return _documents.Where( doc => doc.Name == name ).FirstOrDefault();
        }

        public Document AddDocument( Document document )
        {
            _documents.Add( document );
            return document;
        }
    }

    public class Document
    {
        string _type = string.Empty;
        string _name = string.Empty;
        List<DocumentVersion> _versions = null;

        public Document()
        {
            _type = string.Empty;
            _name = string.Empty;
            _versions = new List<DocumentVersion>();
        }

        public Document( string type, string name )
        {
            _type = type;
            _name = name;
            _versions = new List<DocumentVersion>();
        }

        public Document( string type, string name, List<DocumentVersion> versions )
        {
            _type = type;
            _name = name;
            _versions = versions;
        }

        public string Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public List<DocumentVersion> Versions
        {
            get { return _versions; }
        }

        public bool HasManyVersions()
        {
            return _versions.Count > 1 ? true : false;
        }

        public DocumentVersion GetLatestVersion()
        {
            if( !HasManyVersions() )
                return _versions[0];

            Double latestRevision = 0;
            foreach( DocumentVersion version in _versions )
            {
                Double revision = Convert.ToDouble( version.Revision );
                if( revision >= latestRevision )
                    latestRevision = revision;
            }
            return _versions
                .Where( version => Convert.ToDouble( version.Revision ) == latestRevision )
                .FirstOrDefault();
        }

        public void AddVersion( DocumentVersion version )
        {
            version.SetDocument( this );
            _versions.Add( version );
        }
    }

    public class DocumentVersion
    {
        //Parent
        Document _document = null;

        string _url = string.Empty;
        string _revision = string.Empty;

        public DocumentVersion( string url, string revision )
        {
            _url = url;
            _revision = revision;
        }

        public DocumentVersion( Document document, string url, string revision )
        {
            _document = document;
            _url = url;
            _revision = revision;
        }

        public Document Document
        {
            get { return _document; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string Revision
        {
            get { return _revision; }
        }

        public void SetDocument( Document document )
        {
            _document = document;
        }
    }
}
