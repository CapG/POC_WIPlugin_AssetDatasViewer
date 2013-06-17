using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WIExample.SearchManager;
using WIExample.SessionManager;
using System.Xml;
using System.Windows.Forms;
using WIExample;

namespace AssetDatasViewer
{
    public class AvevaWebService
    {
        private string _dataSource;
        private string _sessionKey = string.Empty;

        #region criteria/projection pairs

            #region searchByAssetId

                #region SEARCH_BY_ASSET_ID_CRITERIA
                private const string SEARCH_BY_ASSET_ID_CRITERIA =
                @"<Criteria xmlns=""http://www.aveva.com/NET/EIA/Search/Criteria\"">
                            <Object>
                                <ClassID>{0}</ClassID>
                            </Object>
                          </Criteria>";
                #endregion

                #region SEARCH_BY_ASSET_ID_PROJECTION
                private const string SEARCH_BY_ASSET_ID_PROJECTION =
                @"<Projection xmlns=""http://www.aveva.com/NET/EIA/Search/Projection\"">
                            <Object>
                                <ID/>
                                <Name/>
                                <ClassID/>  
                            </Object>
                          </Projection>";
                #endregion

            #endregion

            #region searchDocumentsByAssetId

                #region SEARCH_DOCUMENTS_BY_ASSET_ID_CRITERIA
                private const string SEARCH_DOCUMENTS_BY_ASSET_ID_CRITERIA =
                        @"<Criteria xmlns=""http://www.aveva.com/NET/EIA/Search/Criteria"">
                                <Object>
                                    <ClassID>DOCUMENT CONTENT</ClassID>
                                    <Association type=""refers to"">
                                        <Object>
                                            <ID>{0}</ID>
                                        </Object>
                                    </Association>
                                    <Association type=""is fulfilled by"">
                                        <Object>
                                            <ClassID>FILE</ClassID>
                                        </Object>
                                    </Association>
                                </Object>
                            </Criteria>";
                #endregion

                #region SEARCH_DOCUMENTS_BY_ASSET_ID_PROJECTION
                private const string SEARCH_DOCUMENTS_BY_ASSET_ID_PROJECTION =
                @"<Projection xmlns=""http://www.aveva.com/NET/EIA/Search/Projection"">
                        <Object>
                            <ID/>
                            <ClassID/>
                            <Association type=""is fulfilled by"">
                                <Object>
                                    <Characteristic>
                                        <Name>InfoLocator</Name>
                                    </Characteristic>
                                </Object>
                            </Association>
                        </Object>
                     </Projection>";
                #endregion

            #endregion

            #region searchScadaDatasByAssetId

                #region SEARCH_SCADA_DATAS_BY_ASSET_ID_CRITERIA
                private const string SEARCH_SCADA_DATAS_BY_ASSET_ID_CRITERIA =
                @"<Criteria xmlns=""http://www.aveva.com/NET/EIA/Search/Criteria"">
                    <Object>
                        <ID>{0}</ID>
                    </Object>
                </Criteria>";
                #endregion

                #region SEARCH_SCADA_DATAS_BY_ASSET_ID_PROJECTION
                private const string SEARCH_SCADA_DATAS_BY_ASSET_ID_PROJECTION =
                @"<Projection xmlns=""http://www.aveva.com/NET/EIA/Search/Projection"">
                    <Object>
                        <ID/><Name/><ClassID/>
                        <Association type=""has dataset"">
                            <Object>
                                <ID/>
                                <Characteristic>
                                    <Name>measured pressure</Name>
                                </Characteristic>
                                <Characteristic>
                                    <Name>measured temp</Name>
                                </Characteristic>
                            </Object>
                        </Association>
                    </Object>
                 </Projection>";
                #endregion

            #endregion

            #region searchAllScadaDatas

                #region SEARCH_ALL_SCADA_DATAS_CRITERIA
                private const string SEARCH_ALL_SCADA_DATAS_CRITERIA =
                @"<Criteria xmlns=""http://www.aveva.com/NET/EIA/Search/Criteria"">
                    <Object>
                        <ID operator=""like"">%</ID>
                        <Context>
                            <ID operator=""="">IPETEST</ID>
                        </Context>
                        <ClassID>{0}</ClassID>
                        <Association operator=""="" type=""has dataset"">
                            <Object>
                                <ClassID>{0}</ClassID>
                            </Object>
                        </Association>
                    </Object>
                </Criteria>";
                #endregion

                #region SEARCH_ALL_SCADA_DATAS_PROJECTION
                private const string SEARCH_ALL_SCADA_DATAS_PROJECTION =
                @"<Projection xmlns=""http://www.aveva.com/NET/EIA/Search/Projection"">
                    <Object>
                        <ID/><Name/><ClassID/>
                        <Association type=""has dataset"">
                            <Object>
                                <ID/>
                                <Characteristic>
                                    <Name>measured pressure</Name>
                                </Characteristic>
                                <Characteristic>
                                    <Name>measured temp</Name>
                                </Characteristic>
                            </Object>
                        </Association>
                    </Object>
                 </Projection>";
                #endregion

            #endregion

        #endregion

        public AvevaWebService( string dataSource )
        {
            _dataSource = dataSource;
            createSessionkey();
        }

        public AssetDatas GetAssetScadaDatas( AssetDatas assetDatas )
        {
            Dictionary<string, string> ScadaDatas = searchScadaDatasByAssetId( assetDatas.Name );

            if( ScadaDatas.Keys.Count > 0 )
            {
                assetDatas.ScadaDatas.Temperature = ScadaDatas.FirstOrDefault( var => var.Key == "temperature" ).Value;
                assetDatas.ScadaDatas.Pressure = ScadaDatas.FirstOrDefault( var => var.Key == "pressure" ).Value;
            }

            return assetDatas;
        }

        public AssetDatas GetAssetDocuments( AssetDatas assetDatas )
        {
            assetDatas.SetDocuments( searchAssetDocumentsByAssetId( assetDatas.Name ) );
            return assetDatas;
        }

        public AssetDatas GetAssetDocumentsAndScada( AssetDatas assetDatas )
        {
            return GetAssetScadaDatas( GetAssetDocuments( assetDatas ) );            
        }

        private void createSessionkey()
        {
            try
            {
                using( var sessionManagerClient = new SessionManagerClient( "sessionLAN" ) )
                {
                    _sessionKey = sessionManagerClient.CreateSession( new SessionMessage
                    {
                        DataSource = _dataSource,
                        Username = Resource.WebServerUsername,
                        Password = Resource.WebServerPassword,
                        Role = Resource.WebServerRole
                    } );
                }
            }
            catch( Exception e )
            {
                AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message );
            }
        }

        private void destroySessionKey()
        {
            try
            {
                using( var sessionManagerClient = new SessionManagerClient( "sessionLAN" ) )
                {
                    sessionManagerClient.ReleaseSession( _sessionKey );
                }
            }
            catch( Exception e )
            {
                AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message );
            }
        }

        private string searchByMessage( SearchMessage message )
        {
            try
            { 
                createSessionkey(); 
            }
            catch( Exception e )
            {
                AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message );
                return null;
            }

            string result = null;
            try
            {         
                using( var searchManagerClient = new SearchManagerClient( "searchLAN" ) )
                {
                    SearchResult searchResult = searchManagerClient.Search( _sessionKey, message );

                    if( searchResult != null )
                        result = searchResult.XmlResult;
                    else
                        AssetDatasViewer.CurrentViewer.UI.ShowInformation( "No result" ); 
                }
            }
            catch( Exception e )
            { 
                AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message ); 
            }

            try
            { 
                destroySessionKey(); 
            }
            catch( Exception e )
            { 
                AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message ); 
            }

            return result;
        }

        private string searchByCP( string criteria, string projection )
        {
            return searchByMessage( new SearchMessage{
                Criteria = criteria,
                Projection = projection,
                Options = new SearchOptions { Type = SearchType.XML }
            } );
        }

        private string searchByClassId( string classId ) 
        {
            return searchByCP( 
                string.Format( SEARCH_BY_ASSET_ID_CRITERIA, classId ), 
                SEARCH_BY_ASSET_ID_PROJECTION 
            );
        }

        private AssetDocuments searchAssetDocumentsByAssetId( string id )
        {
            AssetDocuments documents = new AssetDocuments();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml( searchByCP( 
                string.Format( SEARCH_DOCUMENTS_BY_ASSET_ID_CRITERIA, id ), 
                SEARCH_DOCUMENTS_BY_ASSET_ID_PROJECTION 
                ) );

            XmlNamespaceManager xmlns = new XmlNamespaceManager( xmlDoc.NameTable );

            XmlNode templateNode = xmlDoc.DocumentElement.FirstChild;

            xmlns.AddNamespace( "default", templateNode.NamespaceURI );

            foreach( XmlNode node in templateNode.ChildNodes )
            {               
                string name = node
                    .SelectSingleNode( "child::default:ID", xmlns )
                    .InnerText;

                string revision = null;
                try
                {
                    revision = node
                        .SelectSingleNode( "child::default:Revision", xmlns )
                        .InnerText;
                }
                catch( Exception e )
                {
                    revision = "0";
                }

                string type = node
                    .SelectSingleNode( "child::default:ClassID", xmlns )
                    .InnerText;

                string url = node
                    .SelectSingleNode( "child::default:Association", xmlns )
                    .FirstChild.FirstChild.LastChild
                    .InnerText;

                if( documents.HasDocument( name ) )
                {
                    documents.GetDocument( name ).AddVersion( new DocumentVersion( url, revision ) );
                }
                else
                {
                    Document document = documents.AddDocument( new Document( type, name ) );
                    document.AddVersion( new DocumentVersion( url, revision ) );
                }
            }
            return documents;
        }

        private Dictionary<string, string> searchScadaDatasByAssetId( string id )
        {
            Dictionary<string, string> scadaDatas = new Dictionary<string, string>();

            XmlDocument xmldoc = new XmlDocument() { InnerXml = searchByCP( 
                string.Format( SEARCH_SCADA_DATAS_BY_ASSET_ID_CRITERIA, id ), 
                SEARCH_SCADA_DATAS_BY_ASSET_ID_PROJECTION
                )};

            foreach( XmlNode node in xmldoc.GetElementsByTagName( "Association" ) )
            {
                if( node.FirstChild.FirstChild.InnerText.Contains( "M Dataset" ) )
                {
                    scadaDatas.Add( "pressure", node.OwnerDocument.GetElementsByTagName( "Value" )[0].InnerText );
                    scadaDatas.Add( "temperature", node.OwnerDocument.GetElementsByTagName( "Value" )[1].InnerText );
                }
            }
            return scadaDatas;
        }

        private Dictionary<string, string> searchAllScadaDatas( string classId )
        {
            Dictionary<string, string> scadaDatas = new Dictionary<string, string>();

            XmlDocument xmldoc = new XmlDocument() { InnerXml = searchByCP( 
                String.Format( SEARCH_ALL_SCADA_DATAS_CRITERIA, classId ),
                SEARCH_ALL_SCADA_DATAS_PROJECTION
                )};

            foreach( XmlNode node in xmldoc.GetElementsByTagName( "Association" ) )
            {
                if( node.FirstChild.FirstChild.InnerText.Contains( "M Dataset" ) )
                {
                    scadaDatas.Add( "pressure", node.OwnerDocument.GetElementsByTagName( "Value" )[0].InnerText );
                    scadaDatas.Add( "temperature", node.OwnerDocument.GetElementsByTagName( "Value" )[1].InnerText );
                }
            }
            return scadaDatas;
        }
    }
}
