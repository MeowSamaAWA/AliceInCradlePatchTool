using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AliceInCradle
{
    public partial class tips : Form
    {
        public tips()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
