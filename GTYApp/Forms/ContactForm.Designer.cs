namespace GTYApp.Forms
{
    partial class ContactForm
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
            this.lbl.Text = "Contact: support@example.com";
            this.lbl.Location = new Point(16, 16);

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(360, 100);
            this.Controls.Add(this.lbl);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Contact";
            this.Load += ContactForm_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}