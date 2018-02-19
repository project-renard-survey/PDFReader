using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFReader
{
    public partial class FormProgress : Form
    {
        public FormProgress()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Program.Cancel = true;
            this.Close();
        }

        private void tmrProgressUpdater_Tick(object sender, EventArgs e)
        {
            this.lblOpening.Text = Program.ProgressMessage;
        }
    }
}
