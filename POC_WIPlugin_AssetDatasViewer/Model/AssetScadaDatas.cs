using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetDatasViewer
{
    public class AssetScadaDatas
    {
        private AssetDatas _asset = null;
        private string _temperature = string.Empty;
        private string _pressure = string.Empty;

        public AssetScadaDatas() 
        {

        }

        public AssetScadaDatas( string temperature, string pressure )
        {
            _temperature = temperature;
            _pressure = pressure;
        }

        public AssetScadaDatas( AssetDatas asset, string temperature, string pressure )
        {
            _asset = asset;
            _temperature = temperature;
            _pressure = pressure;
        }

        public string Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public string Pressure
        {
            get { return _pressure; }
            set { _pressure = value; }
        }

        public bool Exists
        {
            get
            {
                return _pressure != string.Empty && _temperature != string.Empty ? true : false;
            }
        }
    }
}
