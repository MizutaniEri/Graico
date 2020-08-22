namespace Graico
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.beforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volButtonEnableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wideFitZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideProgressbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator2,
            this.beforeToolStripMenuItem,
            this.jumpToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.volButtonEnableToolStripMenuItem,
            this.toolStripSeparator3,
            this.zoomToolStripMenuItem,
            this.wideFitZoomToolStripMenuItem,
            this.fullscreenToolStripMenuItem,
            this.hideProgressbarToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(165, 286);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(161, 6);
            // 
            // beforeToolStripMenuItem
            // 
            this.beforeToolStripMenuItem.Name = "beforeToolStripMenuItem";
            this.beforeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.beforeToolStripMenuItem.Text = "Before";
            this.beforeToolStripMenuItem.Click += new System.EventHandler(this.beforeToolStripMenuItem_Click);
            // 
            // jumpToolStripMenuItem
            // 
            this.jumpToolStripMenuItem.Name = "jumpToolStripMenuItem";
            this.jumpToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.jumpToolStripMenuItem.Text = "Jump";
            this.jumpToolStripMenuItem.Click += new System.EventHandler(this.jumpToolStripMenuItem_Click);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.nextToolStripMenuItem.Text = "Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // volButtonEnableToolStripMenuItem
            // 
            this.volButtonEnableToolStripMenuItem.CheckOnClick = true;
            this.volButtonEnableToolStripMenuItem.Name = "volButtonEnableToolStripMenuItem";
            this.volButtonEnableToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.volButtonEnableToolStripMenuItem.Text = "VolButton enable";
            this.volButtonEnableToolStripMenuItem.Click += new System.EventHandler(this.volButtonEnableToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(161, 6);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.CheckOnClick = true;
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
            // 
            // wideFitZoomToolStripMenuItem
            // 
            this.wideFitZoomToolStripMenuItem.CheckOnClick = true;
            this.wideFitZoomToolStripMenuItem.Name = "wideFitZoomToolStripMenuItem";
            this.wideFitZoomToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.wideFitZoomToolStripMenuItem.Text = "Wide fit zoom";
            this.wideFitZoomToolStripMenuItem.Click += new System.EventHandler(this.wideFitZoomToolStripMenuItem_Click);
            // 
            // fullscreenToolStripMenuItem
            // 
            this.fullscreenToolStripMenuItem.CheckOnClick = true;
            this.fullscreenToolStripMenuItem.Name = "fullscreenToolStripMenuItem";
            this.fullscreenToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.fullscreenToolStripMenuItem.Text = "Fullscreen";
            this.fullscreenToolStripMenuItem.Click += new System.EventHandler(this.fullscreenToolStripMenuItem_Click);
            // 
            // hideProgressbarToolStripMenuItem
            // 
            this.hideProgressbarToolStripMenuItem.CheckOnClick = true;
            this.hideProgressbarToolStripMenuItem.Name = "hideProgressbarToolStripMenuItem";
            this.hideProgressbarToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.hideProgressbarToolStripMenuItem.Text = "Hide progressbar";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Graico";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Form1_Scroll);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem jumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem beforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullscreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem volButtonEnableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideProgressbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wideFitZoomToolStripMenuItem;
    }
}

