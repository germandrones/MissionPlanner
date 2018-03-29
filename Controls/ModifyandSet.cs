using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ModifyandSet : UserControl
    {
        [System.ComponentModel.Browsable(false)]
        public NumericUpDown NumericUpDown {
            get { return numericUpDown1; }
        }

        [System.ComponentModel.Browsable(false)]
        public MyButton Button {
            get { return myButton1; }
        }

        [System.ComponentModel.Browsable(true)]
        public String ButtonText
        {
            get { return Button.Text; }
            set { Button.Text = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Value
        {
            get { return NumericUpDown.Value; }
            set { NumericUpDown.Value = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Minimum
        {
            get { return NumericUpDown.Minimum; }
            set { NumericUpDown.Minimum = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Maximum
        {
            get { return NumericUpDown.Maximum; }
            set { NumericUpDown.Maximum = value; }
        }

        public new event EventHandler Click;
        public event EventHandler ValueChanged;

        public ModifyandSet()
        {
            InitializeComponent();
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        /*
        protected override void OnCreateControl()
        {
            NumericUpDown.Width = (int)(this.Width * 0.25f);
            NumericUpDown.Height = Button.Height;
            Button.Width = (int)(this.Width * 0.7f);
            Button.Dock = DockStyle.Right;
        }
        */
        
        [System.ComponentModel.Browsable(true)]
        public int ButtonWidth
        {
            get { return this.Button.Width; }
            set { this.Button.Width = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public int NumericWidth
        {
            get { return this.NumericUpDown.Width; }
            set { this.NumericUpDown.Width = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public int ButtonHeight
        {
            get { return this.Button.Height; }
            set { this.Button.Height = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public int NumericHeight
        {
            get { return this.NumericUpDown.Height; }
            set { this.NumericUpDown.Height = value; }
        }
    }
}