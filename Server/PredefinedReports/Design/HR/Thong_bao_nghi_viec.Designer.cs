namespace D69soft.Server.PredefinedReports.Design.HR
{
    partial class Thong_bao_nghi_viec
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Thong_bao_nghi_viec));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrRichText9 = new DevExpress.XtraReports.UI.XRRichText();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.xrRichText14 = new DevExpress.XtraReports.UI.XRRichText();
            this.xrRichText43 = new DevExpress.XtraReports.UI.XRRichText();
            this.xrRichText45 = new DevExpress.XtraReports.UI.XRRichText();
            this.xrRichText49 = new DevExpress.XtraReports.UI.XRRichText();
            this.xrRichText50 = new DevExpress.XtraReports.UI.XRRichText();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText43)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText45)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText49)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrRichText14,
            this.xrRichText43,
            this.xrRichText45,
            this.xrRichText49,
            this.xrRichText50,
            this.xrRichText9});
            this.Detail.Dpi = 254F;
            this.Detail.HeightF = 1491.831F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.Detail.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Dpi = 254F;
            this.TopMargin.HeightF = 108.3734F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Dpi = 254F;
            this.BottomMargin.HeightF = 98.89803F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Dpi = 254F;
            this.ReportHeader.HeightF = 0F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // PageFooter
            // 
            this.PageFooter.Dpi = 254F;
            this.PageFooter.HeightF = 0F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrRichText9
            // 
            this.xrRichText9.Dpi = 254F;
            this.xrRichText9.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F);
            this.xrRichText9.LocationFloat = new DevExpress.Utils.PointFloat(3.060214F, 402.1716F);
            this.xrRichText9.Name = "xrRichText9";
            this.xrRichText9.SerializableRtfString = resources.GetString("xrRichText9.SerializableRtfString");
            this.xrRichText9.SizeF = new System.Drawing.SizeF(1932.94F, 1089.659F);
            this.xrRichText9.StylePriority.UseFont = false;
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "D69softDB";
            this.sqlDataSource1.Name = "sqlDataSource1";
            storedProcQuery1.Name = "RPT_HR_PrintLaborContract";
            storedProcQuery1.StoredProcName = "RPT.HR_PrintLaborContract";
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // xrRichText14
            // 
            this.xrRichText14.Dpi = 254F;
            this.xrRichText14.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Regular, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.xrRichText14.LocationFloat = new DevExpress.Utils.PointFloat(963.1893F, 182.986F);
            this.xrRichText14.Name = "xrRichText14";
            this.xrRichText14.SerializableRtfString = resources.GetString("xrRichText14.SerializableRtfString");
            this.xrRichText14.SizeF = new System.Drawing.SizeF(969.7506F, 58.42F);
            this.xrRichText14.StylePriority.UseFont = false;
            // 
            // xrRichText43
            // 
            this.xrRichText43.Dpi = 254F;
            this.xrRichText43.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Regular, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.xrRichText43.LocationFloat = new DevExpress.Utils.PointFloat(0F, 121.9201F);
            this.xrRichText43.Name = "xrRichText43";
            this.xrRichText43.SerializableRtfString = resources.GetString("xrRichText43.SerializableRtfString");
            this.xrRichText43.SizeF = new System.Drawing.SizeF(963.1893F, 61.06589F);
            this.xrRichText43.StylePriority.UseFont = false;
            // 
            // xrRichText45
            // 
            this.xrRichText45.Dpi = 254F;
            this.xrRichText45.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Regular, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.xrRichText45.LocationFloat = new DevExpress.Utils.PointFloat(963.1893F, 0F);
            this.xrRichText45.Name = "xrRichText45";
            this.xrRichText45.SerializableRtfString = resources.GetString("xrRichText45.SerializableRtfString");
            this.xrRichText45.SizeF = new System.Drawing.SizeF(969.751F, 121.9201F);
            this.xrRichText45.StylePriority.UseFont = false;
            // 
            // xrRichText49
            // 
            this.xrRichText49.Dpi = 254F;
            this.xrRichText49.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Regular, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.xrRichText49.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrRichText49.Name = "xrRichText49";
            this.xrRichText49.SerializableRtfString = resources.GetString("xrRichText49.SerializableRtfString");
            this.xrRichText49.SizeF = new System.Drawing.SizeF(963.1893F, 121.9201F);
            this.xrRichText49.StylePriority.UseFont = false;
            // 
            // xrRichText50
            // 
            this.xrRichText50.Dpi = 254F;
            this.xrRichText50.Font = new DevExpress.Drawing.DXFont("Times New Roman", 12F, DevExpress.Drawing.DXFontStyle.Regular, DevExpress.Drawing.DXGraphicsUnit.Point, new DevExpress.Drawing.DXFontAdditionalProperty[] {
            new DevExpress.Drawing.DXFontAdditionalProperty("GdiCharSet", ((byte)(0)))});
            this.xrRichText50.LocationFloat = new DevExpress.Utils.PointFloat(963.1893F, 121.9201F);
            this.xrRichText50.Name = "xrRichText50";
            this.xrRichText50.SerializableRtfString = resources.GetString("xrRichText50.SerializableRtfString");
            this.xrRichText50.SizeF = new System.Drawing.SizeF(969.7498F, 61.06588F);
            this.xrRichText50.StylePriority.UseFont = false;
            // 
            // Thong_bao_nghi_viec
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "RPT_HR_PrintLaborContract";
            this.DataSource = this.sqlDataSource1;
            this.Dpi = 254F;
            this.Margins = new DevExpress.Drawing.DXMargins(124F, 99F, 108.3734F, 98.89803F);
            this.PageHeight = 2794;
            this.PageWidth = 2159;
            this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
            this.SnapGridSize = 25F;
            this.Version = "23.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText43)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText45)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText49)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRRichText xrRichText9;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRRichText xrRichText14;
        private DevExpress.XtraReports.UI.XRRichText xrRichText43;
        private DevExpress.XtraReports.UI.XRRichText xrRichText45;
        private DevExpress.XtraReports.UI.XRRichText xrRichText49;
        private DevExpress.XtraReports.UI.XRRichText xrRichText50;
    }
}
