using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;
using Widgets;

namespace Replicator
{
    public partial class Replicator : Form
    {
        SortedDictionary<string, int> m_scenDictionary;
        static string RUN_ = "\\run_";
        
        public Replicator()
        {
            InitializeComponent();
            //@ToDo: Don't prepopulate data after development is complete
            //SampleData();
        }

        void SampleData()
        {
            string[] row0 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\scenario.txt", "", "False" };
            int idxRow = dataGridView1.Rows.Add(row0);
            SetParentCellStyle(idxRow);
            string[] row1 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_1\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row1);
            string[] row2 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_2\\scenario.txt", "Pending", "True"};
            dataGridView1.Rows.Add(row2);
            string[] row3 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\age-only-succession\\run_3\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row3);
            string[] row4 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\scenario.txt", "", "False" };
            idxRow = dataGridView1.Rows.Add(row4);
            SetParentCellStyle(idxRow);
            string[] row5 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_1\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row5);
            string[] row6 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_2\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row6);
            string[] row7 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_3\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row7);
            string[] row8 = new string[] { "C:\\Docs\\Lesley\\Landis\\data\\output-max-spp-age\\run_4\\scenario.txt", "Pending", "True" };
            dataGridView1.Rows.Add(row8);
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

        private void BtnRun_Click(object sender, EventArgs e)
        {
            // Open the status window
            ReplicatorStatus statusForm = new ReplicatorStatus();
            statusForm.Owner = this;
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = statusForm.StatusTextWriter;
            // clear the textbox
            statusForm.TxtBoxStatus_Clear();
            statusForm.Show(this);

            //Disable buttons before starting processing
            enableButtons(false);

            //Reset the textbox color to black
            statusForm.TxtBoxStatus_ForeColor(Color.Black);

            //@ToDo: Is it okay to hard-code this path? If the widget runs from the LANDIS-II bin, shouldn't be needed
            //Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();

            Landis.Core.IExtensionDataset extensions;
            RasterFactory rasterFactory;
            Landis.Landscapes.LandscapeFactory landscapeFactory;
            Landis.Model model;

            try
            {
                string extFolder = Constants.EXTENSIONS_FOLDER + Constants.EXTENSIONS_XML;
                extensions = Landis.Extensions.Dataset.LoadOrCreate(extFolder);
                rasterFactory = new RasterFactory();
                landscapeFactory = new Landis.Landscapes.LandscapeFactory();
                model = new Landis.Model(extensions, rasterFactory, landscapeFactory);
            }
            catch (Exception exc)
            {
                enableButtons(true);
                statusForm.TxtBoxStatus_ForeColor(Color.Red);
                string errorMessage = "An error occurred while trying to start the model.\r\n";
                errorMessage = errorMessage + "Exception: " + exc.Message;
                errorMessage = errorMessage + exc.StackTrace;
                wi.WriteLine(errorMessage);
                return;
            }

            // First we create the replicated directories
            foreach (KeyValuePair<string, int> entry in m_scenDictionary)
            {
                string parentFolder = Path.GetDirectoryName(entry.Key);
                string scenarioFile = Path.GetFileName(entry.Key);
                // Add the child rows
                for (int i = 1; i <= entry.Value; i++)
                {
                    string runFolder = RUN_ + i;
                    string fullPath = parentFolder + runFolder;
                    wi.WriteLine("Creating new subdirectories for " + fullPath);
                    WidgetsUtil.DirectoryCopy(parentFolder, fullPath, RUN_.Trim('\\'));
                    // Delete the landis log and error log files from the children
                    string[] logs = new string[2];
                    logs[0] = WidgetsUtil.GetAppSetting("landis_log");
                    logs[1] = Constants.ERROR_LOG.Trim('\\');
                    WidgetsUtil.DeleteLogFiles(fullPath, logs);

                    // If the scenario path is bad print to the console and exit the sub
                    // Check for the file right before running in case it was moved
                    string scenarioFilePath = fullPath + "\\" + scenarioFile;
                    if (!File.Exists(scenarioFilePath))
                    {
                        statusForm.TxtBoxStatus_ForeColor(Color.Red);
                        string errorMessage = "The scenario file you specified is not valid.\r\n";
                        errorMessage = errorMessage + "Make sure you have write access to the working directory.";
                        wi.WriteLine(errorMessage);
                        break;
                    }

                    try
                    {
                        // The log4net section in the application's configuration file
                        // requires the environment variable WORKING_DIR be set to the
                        // current working directory.
                        // This will be the folder containing the scenario .txt file

                        Environment.SetEnvironmentVariable(Constants.ENV_WORKING_DIR, fullPath);
                        log4net.Config.XmlConfigurator.Configure();

                        // Set the working directory for the Model
                        Directory.SetCurrentDirectory(fullPath);

                        // Get the installed LANDIS version from the console
                        //string version = Landis.App.GetAppSetting("version");
                        string version = WidgetsUtil.GetAssemblySetting("version");
                        if (version == "")
                            throw new Exception("The LANDIS application setting \"version\" is empty or blank");
                        string release = WidgetsUtil.GetAssemblySetting("release");
                        if (release != "")
                            release = string.Format(" ({0})", release);
                        wi.WriteLine("LANDIS-II {0}{1}", version, release);

                        model.Run(fullPath, wi);

                    }
                    catch (Exception exc)
                    {
                        //Enable buttons so user can recover from error
                        enableButtons(true);
                        //Change the text color to red to alert the user
                        statusForm.TxtBoxStatus_ForeColor(Color.Red);
                        Boolean logAvailable = true;
                        // Throw this in a try-catch in case the user doesn't have access to write the log
                        try
                        {
                            using (TextWriter writer = File.CreateText(fullPath + Constants.ERROR_LOG))
                            {
                                writer.WriteLine("Internal error occurred within the program:");
                                writer.WriteLine("  {0}", exc.Message);
                                if (exc.InnerException != null)
                                {
                                    writer.WriteLine("  {0}", exc.InnerException.Message);
                                }
                                writer.WriteLine();
                                writer.WriteLine("Stack trace:");
                                writer.WriteLine(exc.StackTrace);
                            }
                        }
                        catch (Exception exc2)
                        {
                            logAvailable = false;
                            string strError2 = "\r\nAn error occurred while writing the error log.\r\n" +
                                                "The most likely cause is that you do not have permissions to the working directory";
                            wi.WriteLine(strError2);
                            Console.WriteLine("Launcher exception: " + exc2);
                        }
                        //Print an error message
                        if (logAvailable == true)
                        {
                            string strError = "\r\nA program error occurred.\r\nThe error log is available at " + fullPath + Constants.ERROR_LOG;
                            wi.WriteLine(strError);

                        }
                    }
                }
            }
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

            //Make sure user has write access to working directory before proceeding
            if (!String.IsNullOrEmpty(filePath))
            {
                string workingDirectory = Path.GetDirectoryName(filePath);
                if (WidgetsUtil.HasWriteAccess(workingDirectory) == false)
                {
                    MessageBox.Show("The working directory you selected is invalid. You cannot write to this directory.",
                        "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    //@ToDo: Warn if subdirectories already exist. If they do, ask to overwrite
                    txtFilePath.Text = filePath;
                    if (txtFilePath.Text.Length > 0)
                    {
                        BtnSave.Enabled = true;
                    }
                }
            }
        }

        private void SetParentCellStyle(int idxRow) {
            DataGridViewRow row = dataGridView1.Rows[idxRow];
            DataGridViewColumn col = dataGridView1.Columns[0];
            DataGridViewCellStyle parentStyle = col.DefaultCellStyle.Clone();
            parentStyle.Font = new Font(parentStyle.Font, FontStyle.Bold);
            parentStyle.BackColor = SystemColors.InactiveCaption;
            row.DefaultCellStyle = parentStyle;
            DataGridViewCellStyle fileStyle = parentStyle.Clone();
            fileStyle.Padding = new Padding(0, 1, 0, 1);
            row.Cells[0].Style = fileStyle;
            DataGridViewCellStyle buttonStyle = parentStyle.Clone();
            buttonStyle.Padding = new Padding(3, 3, 3, 3);
            row.Cells[3].Style = buttonStyle;
            row.Cells[4].Style = buttonStyle;
            row.Height = 30;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (txtFilePath.Text != null)
            {
                if (m_scenDictionary == null)
                {
                    m_scenDictionary = new SortedDictionary<string, int>();
                }
                // Check to see if the scenario has already been configured. If so ask if the user wants to overwrite
                if (m_scenDictionary.ContainsKey(txtFilePath.Text)) {
                    string msg = "This scenario has already been configured. Do you wish to overwrite the current configuration ?";
                    DialogResult res = MessageBox.Show(msg, "Scenario exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res != DialogResult.Yes)
                    {
                        return;
                    }
                }
                int iRuns = (int) numRuns.Value;
                m_scenDictionary[txtFilePath.Text] = iRuns;
                ReloadGrid();
                // Reset defaults
                txtFilePath.Text = "";
                numRuns.Value = 1;
            }

        }

        void ReloadGrid()
        {
            dataGridView1.Rows.Clear();
            foreach (KeyValuePair<string, int> entry in m_scenDictionary)
            {
                // Add the parent row
                string[] row0 = new string[] { entry.Key, "", "False" };
                int idxRow = dataGridView1.Rows.Add(row0);
                SetParentCellStyle(idxRow);

                string parentFolder = Path.GetDirectoryName(entry.Key);
                string scenarioFile = Path.GetFileName(entry.Key);
                // Add the child rows
                for (int i = 1; i <= entry.Value; i++)
                {
                    string runFolder = RUN_ + i;
                    string fullPath = parentFolder + runFolder + "\\" + scenarioFile;
                    string[] childRow = new string[] { fullPath, "", "True" };
                    int idxChild = dataGridView1.Rows.Add(childRow);
                }

            }
            // Stop Visual C# from highlighting the first cell
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            // Enable Run button if appropriate
            if (m_scenDictionary.Keys.Count > 0)
            {
                BtnRun.Enabled = true;
            }
            else
            {
                BtnRun.Enabled = false;
            }

        }

        // Handle click on edit/delete buttons
        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex ==
                dataGridView1.Columns["Delete"].Index)
            {
                BtnDelete_Click(e.RowIndex);
            }
            else if (e.RowIndex < 0 || e.ColumnIndex ==
                dataGridView1.Columns["Edit"].Index)
            {
                BtnEdit_Click(e.RowIndex);
            }

        }

        void BtnDelete_Click(int idxRow)
        {
            DataGridViewRow row = dataGridView1.Rows[idxRow];
            string key = (string) row.Cells[0].Value;
            m_scenDictionary.Remove(key);
            ReloadGrid();
        }

        void BtnEdit_Click(int idxRow)
        {
            DataGridViewRow row = dataGridView1.Rows[idxRow];
            string key = (string)row.Cells[0].Value;
            txtFilePath.Text = key;
            numRuns.Value = m_scenDictionary[key];
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtFilePath.Text = "";
            BtnSave.Enabled = false;
            m_scenDictionary.Clear();
            ReloadGrid();
        }

        private void enableButtons(Boolean enableButtons)
        {
            BtnFile.Enabled = enableButtons;
            BtnRun.Enabled = enableButtons;
            BtnSave.Enabled = enableButtons;
            BtnClear.Enabled = enableButtons;
            //@ToDo: Enable when validate function is enabled
            //BtnValidate.Enabled = enableButtons;
        }

    }


}
