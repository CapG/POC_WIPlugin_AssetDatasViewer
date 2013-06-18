using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using vrcontext.walkinside.sdk;
using WIExample;

namespace AssetDatasViewer
{
    /// <summary>
    /// The most basic example of a working plugin for walkinside.
    /// </summary>
    /// <remarks>
    /// This module does not do anything realy. The module just shows a message dialog
    /// when the plugin is created and destroyed.
    /// </remarks>
    public class AssetDatasViewer : IVRPlugin
    {
        internal static VRPluginDescriptor pDescriptor = new VRPluginDescriptor( VRPluginType.Unknown, 1, "1.0", "04/02/2013", "POC_WIPlugin_AssetDatasViewer", "Search on Web server and display Asset data", "Vrcontext_SDK" );
        /// <summary>
        /// Get the plugin descriptor of this walkinside module without creating a plugin instance of type WIPlugin.
        /// </summary>
        static public VRPluginDescriptor GetDescriptor
        {
            get
            {
                return pDescriptor;
            }
        }

        /// <summary>
        /// Get the plugin descriptor of this walkinside module object of type WIPlugin.
        /// </summary>
        public VRPluginDescriptor Description
        {
            get
            {
                return pDescriptor;
            }
        }

        public static IVRViewerSdk CurrentViewer = null;
        public static IVRBranch Root = null;

        private IVRSelection _selection = null;

        AssetViewManager _avevaManager = null;
        ToolStripMenuItem m_ToolStripItem1 = null;

        /// <summary>
        /// The method called by walkinside, immediatly after the plugin assembly is loaded in walkinside.
        /// </summary>
        /// <param name="viewer">The context of the viewer when plugin is created.</param>
        /// <returns>True if creation of plugin succeeded.</returns>
        public bool CreatePlugin( IVRViewerSdk viewer )
        {
            CurrentViewer = viewer;

            m_ToolStripItem1 = viewer.UI.PluginMenu.DropDownItems.Add( Resource.AssetViewButton ) as ToolStripMenuItem;
            viewer.UI.RegisterVRFormWithMenu( Keys.NoName, m_ToolStripItem1, typeof( AssetView ) );

            viewer.ProjectManager.OnProjectOpen += new VRProjectEventHandler( ProjectManager_OnProjectOpen );
            viewer.ProjectManager.OnProjectClose += new VRProjectEventHandler( ProjectManager_OnProjectClose );
            viewer.ProjectManager.OnBranchSelect += new VROnBranchSelectedEventHandler( ProjectManager_OnBranchSelect );

            return true;
        }

        /// <summary>
        /// The method called by walkinside, just before the plugin is removed from walkinside environment.
        /// </summary>
        /// <param name="viewer">The context of the viewer when plugin is created.</param>
        /// <returns>True if destruction of plugin succeeded.</returns>
        public bool DestroyPlugin( IVRViewerSdk viewer )
        {
            _avevaManager.OnClosing();

            viewer.ProjectManager.OnProjectOpen -= new VRProjectEventHandler( ProjectManager_OnProjectOpen );
            viewer.ProjectManager.OnProjectClose -= new VRProjectEventHandler( ProjectManager_OnProjectClose );
            viewer.ProjectManager.OnBranchSelect -= new VROnBranchSelectedEventHandler( ProjectManager_OnBranchSelect );
            
            viewer.UI.UnregisterVRFORM( m_ToolStripItem1, typeof( AssetView ) );
            viewer.UI.PluginMenu.DropDownItems.Remove( m_ToolStripItem1 );
            m_ToolStripItem1 = null;

            _avevaManager = null;
            Root = null;

            return true;
        }

        void ProjectManager_OnProjectOpen( object o, VRProjectEventArgs e )
        {
            IVRBranch[] rootarray = e.Project.ProjectManager.CurrentProject.BranchManager.GetBranchesByType( 0 );
            if( rootarray.Length > 0 )
            {
                Root = rootarray[0];                
                _avevaManager = new AssetViewManager();

                IVRSelection first = CurrentViewer.CreateSelection();
                first.Add( Root.Manager.GetBranch( 35626 ) );
                first.GoTo();
                first.Dispose();
            }
        }

        void ProjectManager_OnProjectClose( object o, VRProjectEventArgs e )
        {
            if( _avevaManager != null )
            {
                _avevaManager.OnClosing();
            }
        }

        void ProjectManager_OnBranchSelect( object o, VRBranchSelectedEventArgs e )
        {
            //Get clicked branch
            IVRBranch branch = e.Branch[0].Branch;

            //Get clicked branch parent( asset )
            if( !branch.HasChildren )
            {
                branch = branch.Parent;
            }

            if( _selection == null )
            {
                _selection = CurrentViewer.CreateSelection();
            }
            else
            {
                _selection.Clear();
            }

            IVRBranch[] branches = CurrentViewer.ProjectManager.CurrentProject.BranchManager.BranchGetChildren( branch );
            foreach( IVRBranch b in branches )
            {
                _selection.Add( b );
            }
            _selection.HighLight( VRHighlightMode.Cad );

            _avevaManager.ShowAsset( branch );
        }
    }
}
