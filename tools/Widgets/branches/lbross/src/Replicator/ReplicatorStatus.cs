using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Widgets;

namespace Replicator
{
    public partial class ReplicatorStatus : Form
    {
        // That's our custom to redirect console output to form
        TextWriter _writer = null;

        public ReplicatorStatus()
        {
            InitializeComponent();

            // Instantiate text writer
            _writer = new TextBoxStreamWriter(TxtBoxStatus);

            // Set the BackColor so that we can set the ForeColor to red below if there is an error
            // This is an eccentricity with MS read-only textbox
            TxtBoxStatus.BackColor = SystemColors.Control;
        }

        // Public method so the parent form can clear the status
        public void TxtBoxStatus_Clear()
        {
            TxtBoxStatus.Clear();
        }

        public TextWriter StatusTextWriter
        {
            get
            {
                return _writer;
            }
        }

        public void TxtBoxStatus_ForeColor(Color value)
        {
            TxtBoxStatus.ForeColor = value;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
