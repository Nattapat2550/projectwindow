using GTYApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GTYApp.Forms
{
    public partial class ContactForm : Form
    {
        public ContactForm()
        {
            InitializeComponent();
        }
        private void ContactForm_Load(object? sender, EventArgs e)
        {
            ThemeService.Apply(this);
        }
    }
}
