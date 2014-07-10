using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Replicator
{
    public partial class Replicator : Form
    {
        public Replicator()
        {
            InitializeComponent();
            //@ToDo: Don't prepopulate data after development is complete
            SampleData();
        }

        void SampleData()
        {
            string[] row1 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_1\\scenario.txt", "Pending", "False" };
            dataGridView1.Rows.Add(row1);
            string[] row2 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_2\\scenario.txt", "Pending", "True"};
            dataGridView1.Rows.Add(row2);
            string[] row3 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_3\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row3);
            string[] row4 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_1\\scenario.txt", "Pending", "False" };
            dataGridView1.Rows.Add(row4);
            string[] row5 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_2\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row5);
            string[] row6 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_3\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row6);
            string[] row7 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_4\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row7);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Hides the button if we don't want to show it
        private void dataGridView1__CellPainting(object sender, System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
         {
            List<int> indexes = new List<int>();
            indexes.Add(this.dataGridView1.Columns["Edit"].Index);
            indexes.Add(this.dataGridView1.Columns["Delete"].Index);
            
            if (indexes.Contains(e.ColumnIndex) &&
                   e.RowIndex >= 0)
                {

                    Boolean hideButton = Convert.ToBoolean(dataGridView1[dataGridView1.Columns["HideButton"].Index, e.RowIndex].Value);
     
                   //If there are no articles to download, then hide the button
                   if (hideButton == true)
                   {
                       Rectangle newRect = new Rectangle(e.CellBounds.X + 1,
                          e.CellBounds.Y + 1, e.CellBounds.Width - 4,
                          e.CellBounds.Height - 4);
    
                       using (
                           Brush gridBrush = new SolidBrush(this.dataGridView1.GridColor),
                           backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                       {
                          using (Pen gridLinePen = new Pen(gridBrush))
                           {
                              // Erase the cell.
                              e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
   
                             // Draw the grid lines (only the right and bottom lines;
                              // DataGridView takes care of the others).
                              e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                   e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                                   e.CellBounds.Bottom - 1);
                               e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                                   e.CellBounds.Top, e.CellBounds.Right - 1,
                                  e.CellBounds.Bottom);
    
                               e.Handled = true;
                          }
                      }
                   }
              }
           }

       
        // Stop Visual C# from highlighting the first cell
        private void Replicator_Load(object sender, System.EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            ReplicatorStatus statusForm = new ReplicatorStatus();
            statusForm.ShowDialog();
        }

        private void BtnFile_Click(object sender, EventArgs e)
        {
            openFD.Title = "Scenario file";
            // @ToDo: Where to set the initial directory?
            //openFD.InitialDirectory = parentDir.FullName + "\\examples";
            openFD.FileName = "";
            openFD.Filter = "Text|*.txt";
            openFD.ShowDialog();

            string filePath = openFD.FileName;
            txtFilePath.Text = filePath;
        }



    }


}
