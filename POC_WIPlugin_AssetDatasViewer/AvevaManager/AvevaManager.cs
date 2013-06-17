using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vrcontext.walkinside.sdk;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Data.Sql;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using System.Drawing;

namespace AssetDatasViewer
{
    public class AvevaManager
    {
        private IVRViewerSdk _viewer;

        private AvevaWebService _service;

        // Defines the delegates for asynchronous calls
        private delegate AssetDatas Deleg( AssetDatas assetData );
        private Deleg mDeleg_Scada;
        private Deleg mDeleg_Document;
        private Deleg mDeleg_DocumentsAndScada;

        // Initializes the asset fields
        private List<AssetDatas> l_AssetDatas = null;
        private AssetView _currentAssetView = null;

        private IVRLabelGroup _labels = AssetDatasViewer.CurrentViewer.CreateLabelGroup( "AssetIds" );
        private ScadaDatasDisplayer _scadaDatasDisplayer = null;

        // Constructor
        public AvevaManager()
        {
            _viewer = AssetDatasViewer.CurrentViewer;
            l_AssetDatas = new List<AssetDatas>();
            _scadaDatasDisplayer = new ScadaDatasDisplayer();
        }

        // Destructor
        public void OnClosing()
        {
            _scadaDatasDisplayer.OnClosing();
            l_AssetDatas = null;
        }

        // Get the information about the asset and displays it
        
        public void ShowAsset( IVRBranch branch )
        {            
            //AssetDatas assetDatas = l_AssetDatas.FirstOrDefault( var => var.Id == branch.ID );
            //if( assetDatas == null )
            //{
            //    assetDatas = new AssetDatas( branch );
            //}

            AssetDatas assetDatas = new AssetDatas( branch );

            if( _currentAssetView == null )
            {
                _currentAssetView = ( AssetView )AssetDatasViewer.CurrentViewer.UI.OpenForm( typeof( AssetView ) );
                _currentAssetView.FormClosing += new FormClosingEventHandler( assetView_FormClosed );
            }

            _service = new AvevaWebService( "ipetest" );

            mDeleg_DocumentsAndScada = new Deleg( _service.GetAssetDocumentsAndScada );
            AsyncCallback callback_DocumentsAndScada = new AsyncCallback( buildAssetView );
            mDeleg_DocumentsAndScada.BeginInvoke( assetDatas, callback_DocumentsAndScada, null );
        }     

        private void processScadaInformation( IAsyncResult result )
        {
            if( result != null )
            {
                AsyncResult test = ( AsyncResult )result;
                Deleg test_delegate = ( Deleg )test.AsyncDelegate;
                AssetDatas asset_result = test_delegate.EndInvoke( test );
                try
                {
                    // Check if the label is already displayed
                    AssetDatas assetData = l_AssetDatas.FirstOrDefault( var => var.Id == asset_result.Id );
                    if( asset_result.ScadaDatas.Exists )
                    {
                        if( assetData == null )
                        {
                            l_AssetDatas.Add( asset_result );
                            _scadaDatasDisplayer.AddLabel( asset_result );
                        }
                        else
                        {
                            _scadaDatasDisplayer.UpdateLabel( asset_result );
                        }
                        _currentAssetView.Update( asset_result );
                    }
                }
                catch( InvalidOperationException e )
                { 
                    AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message ); 
                }
            }
        }

        private void processDocumentInformation( IAsyncResult result )
        {
            if( result != null )
            {
                AsyncResult test = ( AsyncResult )result;
                Deleg test_delegate = ( Deleg )test.AsyncDelegate;
                AssetDatas asset_result = test_delegate.EndInvoke( test );
                try
                {
                    AssetDatas assetData = l_AssetDatas.FirstOrDefault( var => var.Id == asset_result.Id );

                    if( assetData == null )
                        l_AssetDatas.Add( asset_result );

                    _currentAssetView.Update( asset_result );
                }
                catch( InvalidOperationException e )
                { 
                    AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message ); 
                }
            }
        }

        private void buildAssetView( IAsyncResult result )
        {
            if( result != null )
            {
                AsyncResult test = ( AsyncResult )result;
                Deleg test_delegate = ( Deleg )test.AsyncDelegate;
                AssetDatas asset_result = test_delegate.EndInvoke( test );
                try
                {
                    _currentAssetView.Update( asset_result );

                    _labels.Clear();
                    IVRLabel l = _viewer.CreateLabel(
                            String.Format("{0}\n\nTemperature: {1}°C\nPressure: {2} bar", 
                                asset_result.Name.Substring( asset_result.Name.IndexOf( '/' ) + 1 ),
                                asset_result.ScadaDatas.Temperature,
                                asset_result.ScadaDatas.Pressure),
                            asset_result.Position,
                            _labels
                            );
                    _labels.MaximumDistance = (float)12;
                    _labels.Color = Color.LightSalmon;
                    _labels.Transparency = ( float )0.4;
                    //_labels.Add( l );
                }
                catch( InvalidOperationException e )
                {
                   AssetDatasViewer.CurrentViewer.UI.ShowError( e.Message );
                }
            }
        }

        private void assetView_FormClosed( object sender, EventArgs e )
        {
            _currentAssetView = null;
        }
    }
}
