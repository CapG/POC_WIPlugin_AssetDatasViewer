using System.Windows.Forms;
using System.Drawing;
using WIExample;
namespace AssetDatasViewer
{
    partial class AssetView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing &&( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetView));
            this.panelDocument = new System.Windows.Forms.Panel();
            this.labelDocument = new System.Windows.Forms.Label();
            this.panelScada = new System.Windows.Forms.Panel();
            this.labelScada = new System.Windows.Forms.Label();
            this.documentsTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.labelTemperature = new System.Windows.Forms.Label();
            this.labelPressure = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.labelName = new System.Windows.Forms.Label();
            this.panelName = new System.Windows.Forms.Panel();
            this.assetLinkToPortal = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelDocument.SuspendLayout();
            this.panelScada.SuspendLayout();
            this.panelName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDocument
            // 
            this.panelDocument.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.panelDocument.Controls.Add(this.labelDocument);
            resources.ApplyResources(this.panelDocument, "panelDocument");
            this.panelDocument.Name = "panelDocument";
            // 
            // labelDocument
            // 
            resources.ApplyResources(this.labelDocument, "labelDocument");
            this.labelDocument.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(123)))), ((int)(((byte)(61)))));
            this.labelDocument.Name = "labelDocument";
            // 
            // panelScada
            // 
            this.panelScada.BackColor = System.Drawing.Color.LightBlue;
            this.panelScada.Controls.Add(this.labelScada);
            resources.ApplyResources(this.panelScada, "panelScada");
            this.panelScada.Name = "panelScada";
            // 
            // labelScada
            // 
            resources.ApplyResources(this.labelScada, "labelScada");
            this.labelScada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(96)))), ((int)(((byte)(125)))));
            this.labelScada.Name = "labelScada";
            // 
            // documentsTree
            // 
            this.documentsTree.BackColor = System.Drawing.Color.White;
            this.documentsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.documentsTree.Cursor = System.Windows.Forms.Cursors.Arrow;
            resources.ApplyResources(this.documentsTree, "documentsTree");
            this.documentsTree.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.documentsTree.ImageList = this.imageList1;
            this.documentsTree.LineColor = System.Drawing.Color.DimGray;
            this.documentsTree.Name = "documentsTree";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Close Folder.png");
            this.imageList1.Images.SetKeyName(1, "Open Folder.png");
            this.imageList1.Images.SetKeyName(2, "document.jpg");
            // 
            // labelTemperature
            // 
            resources.ApplyResources(this.labelTemperature, "labelTemperature");
            this.labelTemperature.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.labelTemperature.Name = "labelTemperature";
            // 
            // labelPressure
            // 
            resources.ApplyResources(this.labelPressure, "labelPressure");
            this.labelPressure.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.labelPressure.Name = "labelPressure";
            // 
            // timer
            // 
            this.timer.Interval = 350;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(98)))), ((int)(((byte)(17)))));
            this.labelName.Name = "labelName";
            // 
            // panelName
            // 
            this.panelName.BackColor = System.Drawing.Color.Moccasin;
            this.panelName.Controls.Add(this.labelName);
            resources.ApplyResources(this.panelName, "panelName");
            this.panelName.Name = "panelName";
            // 
            // assetLinkToPortal
            // 
            resources.ApplyResources(this.assetLinkToPortal, "assetLinkToPortal");
            this.assetLinkToPortal.Name = "assetLinkToPortal";
            this.assetLinkToPortal.TabStop = true;
            this.assetLinkToPortal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.assetLinkToPortal_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // AssetView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(254)))));
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.assetLinkToPortal);
            this.Controls.Add(this.documentsTree);
            this.Controls.Add(this.labelPressure);
            this.Controls.Add(this.labelTemperature);
            this.Controls.Add(this.panelScada);
            this.Controls.Add(this.panelDocument);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelName);
            this.Name = "AssetView";
            this.panelDocument.ResumeLayout(false);
            this.panelDocument.PerformLayout();
            this.panelScada.ResumeLayout(false);
            this.panelScada.PerformLayout();
            this.panelName.ResumeLayout(false);
            this.panelName.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panelDocument;
        private Label labelDocument;
        private Panel panelScada;
        private Label labelScada;
        private TreeView documentsTree;
        private Label labelTemperature;
        private Label labelPressure;
        private Timer timer;
        private Label labelName;
        private Panel panelName;
        private ImageList imageList1;
        private LinkLabel assetLinkToPortal;
        private Panel panel1;
        private PictureBox pictureBox1;
    }
}