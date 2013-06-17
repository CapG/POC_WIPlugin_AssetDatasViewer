using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vrcontext.walkinside.sdk;

namespace AssetDatasViewer
{
    public class AssetDatas
    {
        private string _name = string.Empty;
        private uint _id = uint.MaxValue;
        private VRVector3D _position = null;
        private AssetScadaDatas _scadaDatas = null;
        private AssetDocuments _documents = null;

        public AssetDatas()
        {
            _scadaDatas = new AssetScadaDatas();
            _documents = new AssetDocuments();
        }

        public AssetDatas( IVRBranch branch )
        {
            _name = branch.Name;
            _id = branch.ID;
            _position = branch.OBB.Position;
            _scadaDatas = new AssetScadaDatas();
            _documents = new AssetDocuments();
        }

        public AssetDatas( string name, uint id, VRVector3D position )
        {
            _name = name;
            _id = id;
            _position = position;
            _scadaDatas = new AssetScadaDatas();
            _documents = new AssetDocuments();
        }

        public AssetDatas( string name, uint id, VRVector3D position, string temperature, string pressure )
        {
            _name = name;
            _id = id;
            _position = position;
            _scadaDatas = new AssetScadaDatas( temperature, pressure );
            _documents = new AssetDocuments();
        }

        public AssetDatas( string name, uint id, VRVector3D position, string temperature, string pressure, AssetDocuments documents )
        {
            _name = name;
            _id = id;
            _position = position;
            _scadaDatas = new AssetScadaDatas( temperature, pressure );
            _documents = documents;
        }

        public AssetDatas( IVRBranch branch, AssetDocuments documents )
        {
            _name = branch.Name;
            _id = branch.ID;
            _position = branch.OBB.Position;
            _scadaDatas = new AssetScadaDatas();
            _documents = documents;
        }

        public AssetDatas( IVRBranch branch, AssetDocuments documents, AssetScadaDatas scadaDatas )
        {
            _name = branch.Name;
            _id = branch.ID;
            _position = branch.OBB.Position;
            _scadaDatas = new AssetScadaDatas();
            _documents = documents;
        }

        public string Name
        {
            get { return _name.Substring( _name.IndexOf( "/" ) ) ; }
        }

        public uint Id
        {
            get { return _id; }
        }

        public VRVector3D Position
        {
            get { return _position; }
        }

        public AssetScadaDatas ScadaDatas
        {
            get { return _scadaDatas; }
            set { _scadaDatas = value; }
        }

        public AssetDocuments Documents
        {
            get { return _documents; }
        }

        public void SetDocuments( AssetDocuments documents )
        {
            _documents = documents;
        }
    }
}
