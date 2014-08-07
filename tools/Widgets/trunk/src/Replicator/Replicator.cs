using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Landis.Core;
using Landis;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;
using Widgets;

namespace Replicator
{
    public partial class Replicator : Form
    {        
        SortedDictionary<string, int> m_scenDictionary;
        static string RUN_ = "\\run_";
        static string VALIDATE_ = "\\validate_";
        static string STATUS_PENDING = "Pending";
        static string STATUS_RUNNING = "Running";
        static string STATUS_FAILED = "Failed";
        static string STATUS_COMPLETE = "Complete";
        static string STATUS_VALIDATED = "Validated";
        // That's our custom to redirect console output to form
        TextWriter _writer = null;
        
        public Replicator()
        {
            InitializeComponent();

            // Prepend system path with LANDIS GDAL folder so app can find the libraries
            string path = Environment.GetEnvironmentVariable(Constants.ENV_PATH);
            string gdalFolder = WidgetsUtil.GetAppSetting("gdal_folder");
            string verGdalFolder = "";
            if (gdalFolder != "")
            {
                verGdalFolder = WidgetsUtil.CurrentGdalFolder(gdalFolder);
            }

            if (verGdalFolder != "")
            {
                string newPath = verGdalFolder + ";" + path;
                Environment.SetEnvironmentVariable(Constants.ENV_PATH, newPath);
            }
            else
            {
                Console.WriteLine("unable to configure gdal folder correctly");
            }

            // Instantiate text writer
            _writer = new TextBoxStreamWriter(TxtBoxStatus);

            // Set the BackColor so that we can set the ForeColor to red below if there is an error
            // This is an eccentricity with MS read-only textbox
            TxtBoxStatus.BackColor = SystemColors.Control;
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
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = _writer;
            TxtBoxStatus.Text = "";

            // Get the installed LANDIS version from the console
            //string version = Landis.App.GetAppSetting("version");
            string version = WidgetsUtil.GetAssemblySetting("version");
            if (version == "")
                throw new Exception("The LANDIS application setting \"version\" is empty or blank");
            string release = WidgetsUtil.GetAssemblySetting("release");
            if (release != "")
                release = string.Format(" ({0})", release);
            wi.WriteLine("LANDIS-II {0}{1}", version, release);

            // First we create the replicated directories
            foreach (KeyValuePair<string, int> entry in m_scenDictionary)
            {
                string parentFolder = Path.GetDirectoryName(entry.Key);
                string scenarioFile = Path.GetFileName(entry.Key);

                int success = DeleteOldDirectories(parentFolder, wi, false);
                if (success != 0)
                {
                    break;
                }

                // Add the child rows
                for (int i = 1; i <= entry.Value; i++)
                {
                    //Disable buttons before starting processing
                    enableButtons(false);
                    string runFolder = RUN_ + i;
                    string fullPath = parentFolder + runFolder;
                    string scenarioFilePath = fullPath + "\\" + scenarioFile;
                    UpdateStatus(scenarioFilePath, STATUS_RUNNING);

                    // Create run_ subdirectories
                    wi.WriteLine("Creating new subdirectories for " + fullPath);
                    WidgetsUtil.DirectoryCopy(parentFolder, fullPath, RUN_.Trim('\\'));
                    // Delete the landis log and error log files from the children
                    string[] logs = new string[2];
                    logs[0] = WidgetsUtil.GetAppSetting("landis_log");
                    logs[1] = Constants.ERROR_LOG.Trim('\\');
                    WidgetsUtil.DeleteLogFiles(fullPath, logs);

                    // If the scenario path is bad print to the console and exit the sub
                    // Check for the file right before running in case it was moved
                    if (!File.Exists(scenarioFilePath))
                    {
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

                        Model model = InitializeModel();
                        model.Run(scenarioFilePath, wi);
                        UpdateStatus(scenarioFilePath, STATUS_COMPLETE);
                    }
                    catch (IOException IOexc)
                    {
                        //Enable buttons so user can recover from error
                        enableButtons(true);
                        //Change the text color to red to alert the user
                        TxtBoxStatus.ForeColor = Color.Red;
                        String search = "used by another process";
                        if (IOexc.Message.IndexOf(search) > -1)
                        {
                            wi.WriteLine("\r\nOne or more files is locked due to the previous scenario run.");
                            wi.WriteLine("\rPlease close the Replicator and try again.");
                        }
                        else
                        {
                            wi.WriteLine("\r\nA file access error occurred.");
                        }
                        using (TextWriter writer = File.CreateText(fullPath + Constants.ERROR_LOG))
                        {
                            writer.WriteLine("A file access error occurred:");
                            writer.WriteLine("  {0}", IOexc.Message);
                            if (IOexc.InnerException != null)
                            {
                                writer.WriteLine("  {0}", IOexc.InnerException.Message);
                            }
                            writer.WriteLine();
                            writer.WriteLine("Stack trace:");
                            writer.WriteLine(IOexc.StackTrace);
                        }
                    }
                    catch (Exception exc)
                    {
                        //Enable buttons so user can recover from error
                        enableButtons(true);
                        Boolean logAvailable = true;
                        UpdateStatus(scenarioFilePath, STATUS_FAILED);
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
            enableButtons(true);
        }

        private void BtnFile_Click(object sender, EventArgs e)
        {

            openFD.Title = "Scenario file";
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
                    //Warn if subdirectories already exist. If they do, ask to overwrite
                    int iCount = WidgetsUtil.LocateDirectoriesByName(workingDirectory, RUN_.Trim('\\'));
                    if (iCount > 0)
                    {
                        DialogResult res = MessageBox.Show("The working directory you selected already contains LANDIS-II subdirectories. Do you want to overwrite them?", 
                            "Overwrite subfolders", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res != DialogResult.Yes) 
                        {
                            return;
                        }
                    }
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
                    string[] childRow = new string[] { fullPath, STATUS_PENDING, "True" };
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
                BtnValidate.Enabled = true;
            }
            else
            {
                BtnRun.Enabled = false;
                BtnValidate.Enabled= false;
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
            TxtBoxStatus.Text = "Ready ...";
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
            BtnValidate.Enabled = enableButtons;
            dataGridView1.Enabled = enableButtons;
        }

        private void UpdateStatus(string scenarioPath, string newStatus)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                string nextPath = (string) row.Cells[0].Value;
                if (nextPath == scenarioPath)
                {
                    row.Cells[1].Value = newStatus;
                    dataGridView1.Refresh();
                }
            }
        }

        private void lblWww_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            this.lblWww.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start(lblWww.Text);
        }

        private void BtnValidate_Click(object sender, EventArgs e)
        {
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = _writer;
            TxtBoxStatus.Text = "";

            // Get the installed LANDIS version from the console
            //string version = Landis.App.GetAppSetting("version");
            string version = WidgetsUtil.GetAssemblySetting("version");
            if (version == "")
                throw new Exception("The LANDIS application setting \"version\" is empty or blank");
            string release = WidgetsUtil.GetAssemblySetting("release");
            if (release != "")
                release = string.Format(" ({0})", release);
            wi.WriteLine("LANDIS-II {0}{1}", version, release);

            //Disable buttons before starting processing
            enableButtons(false);

            // First we create the replicated directories
            foreach (KeyValuePair<string, int> entry in m_scenDictionary)
            {
                string parentFolder = Path.GetDirectoryName(entry.Key);
                string scenarioFile = Path.GetFileName(entry.Key);

                // Delete any existing run_, validate_ subdirectories
                int success = DeleteOldDirectories(parentFolder, wi, true);
                if (success != 0)
                {
                    break;
                }

                // Add the child folders
                for (int i = 1; i <= entry.Value; i++)
                {
                    string validateFolder = VALIDATE_ + i;
                    string validatePath = parentFolder + validateFolder;
                    string validateScenarioFilePath = validatePath + "\\" + scenarioFile;
                    string runFolder = RUN_ + i;
                    string runPath = parentFolder + runFolder;
                    string scenarioFilePath = runPath + "\\" + scenarioFile;
                    UpdateStatus(scenarioFilePath, STATUS_RUNNING);

                    // Create validate_ subdirectories
                    wi.WriteLine("Creating new subdirectories for " + validatePath);
                    WidgetsUtil.DirectoryCopy(parentFolder, validatePath, VALIDATE_.Trim('\\'));
                    // Delete the landis log and error log files from the children
                    string[] logs = new string[2];
                    logs[0] = WidgetsUtil.GetAppSetting("landis_log");
                    logs[1] = Constants.ERROR_LOG.Trim('\\');
                    WidgetsUtil.DeleteLogFiles(validatePath, logs);

                    // If the scenario path is bad print to the console and exit the sub
                    // Check for the file right before running in case it was moved
                    if (!File.Exists(validateScenarioFilePath))
                    {
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

                        Environment.SetEnvironmentVariable(Constants.ENV_WORKING_DIR, validatePath);
                        log4net.Config.XmlConfigurator.Configure();

                        // Set the working directory for the Model
                        Directory.SetCurrentDirectory(validatePath);

                        Model model = InitializeModel();
                        Validator validator = InitializeValidator(model, wi);
                        validator.ValidateScenario(validateScenarioFilePath);
                        UpdateStatus(scenarioFilePath, STATUS_VALIDATED);
                        wi.WriteLine("\nValidation is complete");
                    }
                    catch (Exception exc)
                    {
                        //Enable buttons so user can recover from error
                        enableButtons(true);
                        Boolean logAvailable = true;
                        UpdateStatus(scenarioFilePath, STATUS_FAILED);
                        // Throw this in a try-catch in case the user doesn't have access to write the log
                        try
                        {
                            using (TextWriter writer = File.CreateText(validatePath + Constants.ERROR_LOG))
                            {
                                writer.WriteLine("Validation failed:");
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
                            Console.WriteLine("Replicator exception: " + exc2);
                        }
                        //Print an error message
                        if (logAvailable == true)
                        {

                            wi.WriteLine("\r\nValidation failed:");
                            wi.WriteLine("  {0}", exc.Message);
                            string strError = "\r\nAn error log is available at " + validatePath + Constants.ERROR_LOG;
                            wi.WriteLine(strError);
                        }
                    }
                }
            }
            enableButtons(true);
        }

        private Model InitializeModel()
        {

            Model model;
            //@ToDo: Is it okay to hard-code this path? If the widget runs from the LANDIS-II bin, shouldn't be needed
            //Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
            string extFolder = Constants.EXTENSIONS_FOLDER + Constants.EXTENSIONS_XML;
            IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate(extFolder);
            //IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
            RasterFactory rasterFactory = new RasterFactory();
            Landis.Landscapes.LandscapeFactory landscapeFactory = new Landis.Landscapes.LandscapeFactory();
            model = new Landis.Model(extensions, rasterFactory, landscapeFactory);
            return model;
        }

        private Validator InitializeValidator(Model _model, IUserInterface _ui)
        {

            Validator _validator;
            string extFolder = Constants.EXTENSIONS_FOLDER + Constants.EXTENSIONS_XML;
            IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate(extFolder);
            RasterFactory rasterFactory = new RasterFactory();
            Landis.Landscapes.LandscapeFactory landscapeFactory = new Landis.Landscapes.LandscapeFactory();
            _validator = new Validator(extensions, rasterFactory, landscapeFactory, _model, _ui);
            return _validator;
        }

        private int DeleteOldDirectories(string scenarioFolder, IUserInterface wi, Boolean deleteValidate)
        {

            try
            {
                // Delete any existing run_, validate_ subdirectories
                DirectoryInfo dir = new DirectoryInfo(scenarioFolder);
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    // I also wanted to delete the validation directories here but couldn't
                    // due to a file lock on ecoregions.gis
                    if (subdir.Name.IndexOf(RUN_.Trim('\\')) > -1)
                    {
                        subdir.Delete(true);
                    }
                    if (deleteValidate == true)
                    {
                        if (subdir.Name.IndexOf(VALIDATE_.Trim('\\')) > -1)
                        {
                            subdir.Delete(true);
                        }

                    }
                }
                return 0;
            }
            catch (Exception exc)
            {
                //Enable buttons so user can recover from error
                enableButtons(true);
                //Change the text color to red to alert the user
                TxtBoxStatus.ForeColor = Color.Red;
                Boolean logAvailable = true;
                // Throw this in a try-catch in case the user doesn't have access to write the log
                try
                {
                    using (TextWriter writer = File.CreateText(scenarioFolder + Constants.ERROR_LOG))
                    {
                        writer.WriteLine("Delete failed:");
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
                    Console.WriteLine("Replicator exception: " + exc2);
                    return -1;
                }
                //Print an error message
                if (logAvailable == true)
                {
                    wi.WriteLine("\r\nOne or more files is locked due to the previous scenario validation or run.");
                    wi.WriteLine("\rPlease close the Replicator and try again.");
                    string strError = "\r\nAn error log is available at " + scenarioFolder + Constants.ERROR_LOG;
                    wi.WriteLine(strError);
                }
                return -1;
            }
        }

    }


}
