using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vrcontext.walkinside.sdk;
using WIExample;

namespace AssetDatasViewer
{
    public class ScadaDatasDisplayer
    {
        private static Dictionary<uint, IVRLabel> m_Labels = new Dictionary<uint,IVRLabel>();
        private static IVRLabelGroup m_LabelGroup = AssetDatasViewer.CurrentViewer.CreateLabelGroup( Resource.ScadaLabelGroup );

        public void AddLabel( AssetDatas assetDatas )
        {
            IVRLabel label = m_LabelGroup.Add( 
                String.Format( "Temperature: {0} °C\nPressure: {1} bar", assetDatas.ScadaDatas.Temperature, assetDatas.ScadaDatas.Pressure ),
                assetDatas.Position
                );
            m_Labels.Add( assetDatas.Id, label );
        }

        public void RemoveLabel( AssetDatas assetDatas )
        {
            IVRLabel label = m_Labels.FirstOrDefault( var => var.Key == assetDatas.Id ).Value;
            m_LabelGroup.Remove( label );
            m_Labels.Remove( assetDatas.Id );
        }

        public void UpdateLabel( AssetDatas assetDatas )
        {
            if( m_Labels != null )
            {
                var e = m_Labels.FirstOrDefault( var => var.Key == assetDatas.Id );
                if( e.Value == null )
                {
                    AddLabel( assetDatas );
                }
                else
                {
                    RemoveLabel( assetDatas );
                    AddLabel( assetDatas );
                }
            }
            else
            {
                AddLabel( assetDatas );
            }
        }

        public void UpdateAllLabels( List<AssetDatas> l_assetDatas )
        {
            if( m_Labels != null && m_LabelGroup != null )
            {
                m_Labels.Clear();
                m_LabelGroup.Clear();
            }
            else
            {
                m_Labels = new Dictionary<uint, IVRLabel>();
                m_LabelGroup = AssetDatasViewer.CurrentViewer.CreateLabelGroup( Resource.ScadaLabelGroup );
            }

            foreach( AssetDatas assetDatas in l_assetDatas )
            {
                IVRLabel label = m_LabelGroup.Add( 
                    string.Format( Resource.ScadaLabel,assetDatas.ScadaDatas.Temperature, assetDatas.ScadaDatas.Pressure ), 
                    assetDatas.Position
                    );
                m_Labels.Add( assetDatas.Id, label );
            }
        }

        public void OnClosing()
        {
            m_LabelGroup = null;
            m_Labels = null;
        }
    }
}