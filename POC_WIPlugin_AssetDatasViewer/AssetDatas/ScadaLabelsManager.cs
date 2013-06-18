using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vrcontext.walkinside.sdk;

namespace AssetDatasViewer
{
    public class ScadaLabelsManager
    {
        public IVRLabelGroup _labels;

        public ScadaLabelsManager( IVRViewerSdk viewer, string name )
        {
            _labels = viewer.CreateLabelGroup( name );
            _labels.Scale = ( float )0.8;
            _labels.MaximumDistance = ( float )12;
            _labels.Transparency = ( float )0.4;
        }

        public void Update( AssetDatas asset )
        {
            _labels.Clear();
            IVRLabel l = _labels.Add(
            String.Format("{0}\n\nTemperature: {1}°C\nPressure: {2} bar", 
                asset.Name.Substring( asset.Name.IndexOf( '/' ) + 1 ),
                asset.ScadaDatas.Temperature,
                asset.ScadaDatas.Pressure),
                asset.Position
            );
        }




    }
}
