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
        private Deleg mDeleg_DocumentsAndScada;

        // Initializes the asset fields
        private AssetView _currentAssetView = null;
        private ScadaLabelsManager _scadaLabelsManager;

        // Constructor
        public AvevaManager()
        {
            _viewer = AssetDatasViewer.CurrentViewer;
            _scadaLabelsManager = new ScadaLabelsManager( _viewer, "assetScadaDatas" );
        }

        // Destructor
        public void OnClosing()
        {
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
                    _scadaLabelsManager.Update( asset_result );         
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
