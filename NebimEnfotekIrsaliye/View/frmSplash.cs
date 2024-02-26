using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;

namespace NebimEnfotekIrsaliye.View
{
    public partial class frmSplash : Form
    {
        Controller.clsMain cMain;

        public frmSplash(Controller.clsMain newMain)
        {
            InitializeComponent();
            this.cMain = newMain;
        }
        private void frmSplash_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Thread.Sleep(1000);
            if (!cMain.Init(lblBilgi))
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
            else
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Thread.Sleep(750);
                this.Close();
            }
        }
        private void frmSplash_Load(object sender, EventArgs e)
        {
            lblVersion.Text = cMain.GetVersion();
        }
    }
}
