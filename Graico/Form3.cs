using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graico
{
    public partial class Form3 : Form
    {
        public void setNumericUpDownMaxValue(int max)
        {
            numericUpDown1.Maximum = max;
            label1.Text = "/" + max;
        }

        public int getNumericUpDownValue()
        {
            return Convert.ToInt32(numericUpDown1.Value);
        }

        public void setNumericUpDownValue(int val)
        {
            numericUpDown1.Value = val;
        }

        public Form3()
        {
            InitializeComponent();
            numericUpDown1.Accelerations.Add(new NumericUpDownAcceleration(5, 10));
        }
    }
}
