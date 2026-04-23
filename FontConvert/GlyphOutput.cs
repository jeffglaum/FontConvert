using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontConvert
{
    public partial class GlyphOutput : Form
    {
        public GlyphOutput()
        {
            InitializeComponent();
        }

        private void glyphOutput_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
