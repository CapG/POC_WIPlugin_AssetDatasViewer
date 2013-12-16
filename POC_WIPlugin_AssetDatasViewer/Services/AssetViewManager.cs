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
    public class AssetViewManager
    {
        private IVRViewerSdk _viewer;
        private AvevaWebService _service;
        private AssetView _currentAssetView;
        private ScadaLabelsManager _scadaLabelsManager;

        private delegate AssetDatas Deleg( AssetDatas assetData );
        private Deleg mDeleg_DocumentsAndScada;      

        public AssetViewManager()
        {
            _viewer = AssetDatasViewer.CurrentViewer;
            _service = new AvevaWebService( "ipe" );
            _scadaLabelsManager = new ScadaLabelsManager( _viewer, "assetScadaDatas" );
        }

        public void OnClosing()
        {
        }

        // Get the information about the asset and displays it      
        public void ShowAsset( IVRBranch branch )
        {            
            AssetDatas assetDatas = new AssetDatas( branch );

            if( _currentAssetView == null )
            {
                _currentAssetView = ( AssetView )AssetDatasViewer.CurrentViewer.UI.OpenForm( typeof( AssetView ) );
                _currentAssetView.FormClosing += new FormClosingEventHandler( assetView_Closed );
            }

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

        private void assetView_Closed( object sender, EventArgs e )
        {
            _currentAssetView = null;
            _scadaLabelsManager.Clear();
        }
    }
}
