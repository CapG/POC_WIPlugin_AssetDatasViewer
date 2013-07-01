using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using vrcontext.walkinside.sdk;
using SlimDX.DirectInput;
using SlimDX;
using System.Threading;
using System.Diagnostics;
using WIExample;
using System.Security;


namespace AssetDatasViewer
{
    public partial class AssetView : VRForm
    {
        private VRBrowserForm _browser = null;
        private Joystick _joystick;
        private JoystickState _state = new JoystickState();
        private AssetDatas _assetDatas = null;
        private bool _hasBeenFocused = false;
        private ProcessStartInfo _ieProcessInfos = null;

        public AssetView()
        {
            InitializeComponent();

            documentsTree.AfterExpand += new TreeViewEventHandler( documentsTree_AfterExpand );
            documentsTree.AfterCollapse += new TreeViewEventHandler( documentsTree_AfterCollapse );
            documentsTree.BeforeExpand += new TreeViewCancelEventHandler( documentsTree_BeforeExpand );

            documentsTree.MouseDoubleClick += new MouseEventHandler( documentsTree_MouseDoubleClick );

            _ieProcessInfos = new ProcessStartInfo
            {
                FileName = Resource.IEPath,
                RedirectStandardInput = true,
                UseShellExecute = false,
            };

            CreateDevice();
        }

        private void assetLinkToPortal_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                Process.Start( _ieProcessInfos );
            }
            catch( Exception ex )
            {
                SDKViewer.UI.ShowError( ex.Message );
            }
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            _assetDatas = null;

            SDKViewer.ProjectManager.CurrentProject.BranchManager.ClearHighlights();

            documentsTree.AfterExpand -= new TreeViewEventHandler( documentsTree_AfterExpand );
            documentsTree.AfterCollapse -= new TreeViewEventHandler( documentsTree_AfterCollapse );
            documentsTree.BeforeExpand -= new TreeViewCancelEventHandler( documentsTree_BeforeExpand );

            documentsTree.MouseDoubleClick -= new MouseEventHandler( documentsTree_MouseDoubleClick );

            if( _browser != null )
            {
                _browser.Close();
            }

            ReleaseDevice();

            base.OnFormClosing( e );
        }

        public void Update( AssetDatas assetDatas )
        {
            _assetDatas = assetDatas;
            _hasBeenFocused = false;
            this.Invoke( ( MethodInvoker )delegate
            {
                string assetName = _assetDatas.Name.Substring( _assetDatas.Name.IndexOf( '/' ) + 1 );

                labelName.Text = "Asset " + assetName;

                assetLinkToPortal.Text = "View " + assetName + " in the portal";
                _ieProcessInfos.Arguments = String.Format( @Resource.PortalAssetLink, assetName );

                updateDocumentsTree();

                if( _assetDatas.ScadaDatas.Exists )
                {
                    labelTemperature.Text = string.Format( Resource.Temperature, _assetDatas.ScadaDatas.Temperature );
                    labelPressure.Text = string.Format( Resource.Pressure, _assetDatas.ScadaDatas.Pressure );
                    panelScada.Visible = true;
                    labelScada.Visible = true;
                    labelTemperature.Visible = true;
                    labelPressure.Visible = true;
                }
                else
                {
                    labelTemperature.Text = "Unable to retrieve temperature";
                    labelPressure.Text = "Unable to retrieve pressure";
                }
            } );
        }

        private void updateDocumentsTree()
        {
            documentsTree.BeginUpdate();
            documentsTree.Nodes.Clear();

            foreach( string type in _assetDatas.Documents.Types )
            {
                TreeNode typeNode = documentsTree.Nodes.Add( type );
                typeNode.Tag = "type";

                foreach( Document doc in _assetDatas.Documents.DocumentsList )
                {
                    if( doc.Type == type )
                    {
                        TreeNode docNode = typeNode.Nodes.Add( doc.Name );
                        docNode.Tag = doc;

                        foreach( DocumentVersion version in doc.Versions )
                        {
                            string rev = !String.IsNullOrEmpty( version.Revision ) ? version.Revision : "0";
                            TreeNode n = new TreeNode( String.Format( "{0} - Revision {1}", doc.Name, rev ) );
                            n.Tag = version;
                            docNode.Nodes.Add( n );
                        }
                    }
                }
            }
            documentsTree.EndUpdate();
        }

        #region AssetView events

        public void documentsTree_AfterExpand( object o, TreeViewEventArgs e )
        {          
            e.Node.ImageIndex = 1 ;
            e.Node.SelectedImageIndex = 1;
        }

        public void documentsTree_AfterCollapse( object o, TreeViewEventArgs e )
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        public void documentsTree_BeforeExpand( object o, TreeViewCancelEventArgs e )
        {
            if( e.Node.LastNode.GetNodeCount(true) == 0 )
            {
                foreach( TreeNode node in e.Node.Nodes )
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;   
                }
            }

        }      

        private void documentsTree_MouseDoubleClick( object o, MouseEventArgs e )
        {
            TreeNode selectedNode = documentsTree.SelectedNode;

            if( selectedNode.Tag.ToString() == "type" )
                return;
             
            if( selectedNode.Tag.GetType().Equals( typeof( DocumentVersion ) ) )
            {
                DocumentVersion version = ( DocumentVersion )selectedNode.Tag;
                try
                {
                    _browser = ( VRBrowserForm )SDKViewer.UI.OpenForm( typeof( VRBrowserForm ) );
                    _browser.TabText = selectedNode.Text;
                    _browser.ShowUrl( version.Url );
                }
                catch( Exception ex )
                {
                    MessageBox.Show( ex.Message );
                }
                return;
            }
            else if( selectedNode.Tag.GetType().Equals( typeof( Document ) ) )
            {
                Document doc = ( Document )selectedNode.Tag;
                try
                {
                    selectedNode.Expand();
                }
                catch( Exception ex )
                {
                    MessageBox.Show( ex.Message );
                }
                return;
            }            
        }

        #endregion

        #region Gamepad management

        private void CreateDevice()
        {
            // make sure that DirectInput has been initialized
            DirectInput dinput = new DirectInput();

            // search for devices
            foreach( DeviceInstance device in dinput.GetDevices( DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly ) )
            {
                // create the device
                try
                {
                    _joystick = new Joystick( dinput, device.InstanceGuid );
                    _joystick.SetCooperativeLevel( this, CooperativeLevel.Exclusive | CooperativeLevel.Foreground );
                    break;
                }
                catch( DirectInputException )
                {
                }
            }

            if( _joystick == null )
            {
                //MessageBox.Show( "There are no joysticks attached to the system." );
                return;
            }

            // acquire the device
            _joystick.Acquire();

            // set the timer to go off 12 times a second to read input
            // NOTE: Normally applications would read this much faster.
            // This rate is for demonstration purposes only.
            timer.Interval = 1000 / 12;
            timer.Start();
        }

        private void ReleaseDevice()
        {
            timer.Stop();

            if( _joystick != null )
            {
                _joystick.Unacquire();
                _joystick.Dispose();
            }
            _joystick = null;
        }

        private void ReadImmediateData()
        {
            if( _joystick.Acquire().IsFailure )
                return;

            if( _joystick.Poll().IsFailure )
                return;

            _state = _joystick.GetCurrentState();
            if( Result.Last.IsFailure )
                return;

            updateUIAfterJoystickEvent();
        }

        private void timer_Tick( object o, EventArgs e )
        {
            if( _joystick.Acquire().IsFailure )
                return;

            if( _joystick.Poll().IsFailure )
                return;

            _state = _joystick.GetCurrentState();
            if( Result.Last.IsFailure )
                return;

            updateUIAfterJoystickEvent();
        }

        private void WaitAfterJoystickEvent( int time )
        {
            Thread.Sleep( time );
        }

        private void updateUIAfterJoystickEvent()
        {
            if( documentsTree.Nodes.Count > 0 )
            {
                //documentsTree.Focus();
                if( !_hasBeenFocused )
                {
                    documentsTree.Focus();
                    _hasBeenFocused = true;
                }
                int timeToWait = 200;
                int currentNodeIndex = 0;
                if( documentsTree.SelectedNode == null )
                {
                    documentsTree.SelectedNode = documentsTree.Nodes[0];
                    return;
                }
                else
                {
                    currentNodeIndex = documentsTree.SelectedNode.Index;

                    //Up
                    if( _state.GetPointOfViewControllers()[0] == 0 )
                    {
                        if( documentsTree.SelectedNode.Parent != null )
                        {
                            if( currentNodeIndex > 0 )
                            {
                                documentsTree.SelectedNode = documentsTree.SelectedNode.Parent.Nodes[currentNodeIndex - 1];
                            }
                            WaitAfterJoystickEvent( timeToWait );
                            return;
                        }

                        if( currentNodeIndex > 0 )
                        {
                            documentsTree.SelectedNode = documentsTree.Nodes[currentNodeIndex - 1];
                        }
                        WaitAfterJoystickEvent( timeToWait );
                        return;
                    }

                    //Down
                    if( _state.GetPointOfViewControllers()[0] == 18000 )
                    {
                        if( documentsTree.SelectedNode.Parent != null )
                        {
                            if( currentNodeIndex < documentsTree.SelectedNode.Parent.Nodes.Count - 1 )
                            {
                                documentsTree.SelectedNode = documentsTree.SelectedNode.Parent.Nodes[currentNodeIndex + 1];
                            }
                            WaitAfterJoystickEvent( timeToWait );
                            return;
                        }

                        if( documentsTree.SelectedNode.Index < documentsTree.Nodes.Count - 1 )
                        {
                            documentsTree.SelectedNode = documentsTree.Nodes[documentsTree.SelectedNode.Index + 1];
                        }
                        WaitAfterJoystickEvent( timeToWait );
                        return;
                    }

                    //Right
                    if( _state.GetPointOfViewControllers()[0] == 9000 )
                    {
                        if( documentsTree.SelectedNode.Nodes.Count > 0 )
                        {
                            documentsTree.SelectedNode.Expand();
                            documentsTree.SelectedNode = documentsTree.SelectedNode.Nodes[0];
                            WaitAfterJoystickEvent( timeToWait );
                            return;
                        }
                        documentsTree_MouseDoubleClick( documentsTree.SelectedNode, null );
                        WaitAfterJoystickEvent( timeToWait * 2 );
                        return;
                    }

                    //Left
                    if( _state.GetPointOfViewControllers()[0] == 27000 )
                    {
                        if( documentsTree.SelectedNode.Parent != null )
                        {
                            documentsTree.SelectedNode = documentsTree.SelectedNode.Parent;
                            documentsTree.SelectedNode.Toggle();
                            WaitAfterJoystickEvent( timeToWait );
                            return;
                        }
                        this.Close();
                    }

                    //Button B
                    if( _state.GetButtons()[1] )
                    {

                        if( _browser != null )
                        {
                            _browser.Close();
                            _browser = null;
                            WaitAfterJoystickEvent( timeToWait );
                            return;
                        }
                    }

                    //Button Y
                    if( _state.GetButtons()[3] )
                    {
                        documentsTree.Focus();
                    }
                }
            }
        }   

        #endregion              
    }
} 

