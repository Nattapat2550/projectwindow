using System.Windows.Forms;
using System.Drawing;
namespace GTYApp.Forms
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lbl;

        protected override void Dispose(bool disposing)
        { if (disposing && (components is not null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.lbl = new Label();
            this.SuspendLayout();
            this.lbl.AutoSize = true;
            this.lbl.Text = "TechStyle — modern, fast, secure.";
            this.lbl.Location = new System.Drawing.Point(16, 16);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(360, 100);
            this.Controls.Add(this.lbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += AboutForm_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}