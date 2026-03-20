namespace GTYApp.Forms
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabs;
        private TabPage tabHome;
        private TabPage tabUsers;
        private TextBox txtHero;
        private TextBox txtFeatures;
        private TextBox txtCta;
        private Button btnSaveHome;
        private DataGridView gridUsers;
        private Button btnSaveUsers;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabHome = new System.Windows.Forms.TabPage();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.txtHero = new System.Windows.Forms.TextBox();
            this.txtFeatures = new System.Windows.Forms.TextBox();
            this.txtCta = new System.Windows.Forms.TextBox();
            this.btnSaveHome = new System.Windows.Forms.Button();
            this.gridUsers = new System.Windows.Forms.DataGridView();
            this.btnSaveUsers = new System.Windows.Forms.Button();

            this.tabs.SuspendLayout();
            this.tabHome.SuspendLayout();
            this.tabUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            this.SuspendLayout();

            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabHome);
            this.tabs.Controls.Add(this.tabUsers);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(800, 420);
            this.tabs.TabIndex = 0;

            // 
            // tabHome
            // 
            this.tabHome.Controls.Add(this.btnSaveHome);
            this.tabHome.Controls.Add(this.txtCta);
            this.tabHome.Controls.Add(this.txtFeatures);
            this.tabHome.Controls.Add(this.txtHero);
            this.tabHome.Location = new System.Drawing.Point(4, 24);
            this.tabHome.Name = "tabHome";
            this.tabHome.Padding = new System.Windows.Forms.Padding(10);
            this.tabHome.Size = new System.Drawing.Size(792, 392);
            this.tabHome.TabIndex = 0;
            this.tabHome.Text = "Homepage";
            this.tabHome.UseVisualStyleBackColor = true;

            // 
            // tabUsers
            // 
            this.tabUsers.Controls.Add(this.btnSaveUsers);
            this.tabUsers.Controls.Add(this.gridUsers);
            this.tabUsers.Location = new System.Drawing.Point(4, 24);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(10);
            this.tabUsers.Size = new System.Drawing.Size(792, 392);
            this.tabUsers.TabIndex = 1;
            this.tabUsers.Text = "Users";
            this.tabUsers.UseVisualStyleBackColor = true;

            // 
            // txtHero
            // 
            this.txtHero.Location = new System.Drawing.Point(20, 20);
            this.txtHero.Multiline = true;
            this.txtHero.Name = "txtHero";
            this.txtHero.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHero.Size = new System.Drawing.Size(740, 80);
            this.txtHero.TabIndex = 0;

            // 
            // txtFeatures
            // 
            this.txtFeatures.Location = new System.Drawing.Point(20, 120);
            this.txtFeatures.Multiline = true;
            this.txtFeatures.Name = "txtFeatures";
            this.txtFeatures.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFeatures.Size = new System.Drawing.Size(740, 80);
            this.txtFeatures.TabIndex = 1;

            // 
            // txtCta
            // 
            this.txtCta.Location = new System.Drawing.Point(20, 220);
            this.txtCta.Multiline = true;
            this.txtCta.Name = "txtCta";
            this.txtCta.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCta.Size = new System.Drawing.Size(740, 60);
            this.txtCta.TabIndex = 2;

            // 
            // btnSaveHome
            // 
            this.btnSaveHome.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnSaveHome.Location = new System.Drawing.Point(660, 320);
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.Size = new System.Drawing.Size(100, 30);
            this.btnSaveHome.TabIndex = 3;
            this.btnSaveHome.Text = "Save";
            this.btnSaveHome.UseVisualStyleBackColor = true;
            this.btnSaveHome.Click += new System.EventHandler(this.BtnSaveHome_Click);

            // 
            // gridUsers
            // 
            this.gridUsers.Anchor = System.Windows.Forms.AnchorStyles.Top
                                   | System.Windows.Forms.AnchorStyles.Bottom
                                   | System.Windows.Forms.AnchorStyles.Left
                                   | System.Windows.Forms.AnchorStyles.Right;
            this.gridUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUsers.Location = new System.Drawing.Point(20, 20);
            this.gridUsers.Name = "gridUsers";
            this.gridUsers.RowTemplate.Height = 25;
            this.gridUsers.Size = new System.Drawing.Size(740, 280);
            this.gridUsers.TabIndex = 0;

            // 
            // btnSaveUsers
            // 
            this.btnSaveUsers.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnSaveUsers.Location = new System.Drawing.Point(660, 320);
            this.btnSaveUsers.Name = "btnSaveUsers";
            this.btnSaveUsers.Size = new System.Drawing.Size(100, 30);
            this.btnSaveUsers.TabIndex = 1;
            this.btnSaveUsers.Text = "Save Users";
            this.btnSaveUsers.UseVisualStyleBackColor = true;
            this.btnSaveUsers.Click += new System.EventHandler(this.BtnSaveUsers_Click);

            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 420);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin • TechStyle";
            this.Load += new System.EventHandler(this.AdminForm_Load);

            this.tabs.ResumeLayout(false);
            this.tabHome.ResumeLayout(false);
            this.tabHome.PerformLayout();
            this.tabUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            this.ResumeLayout(false);
        }
    }
}