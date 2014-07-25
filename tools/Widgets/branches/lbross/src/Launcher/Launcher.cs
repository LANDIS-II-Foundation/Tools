using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;
using Widgets;
using Landis.Core;
using Landis;


namespace Launcher
{
    public partial class Launcher : Form
    {
        static Model model;
        static IExtensionDataset extensions;
        static RasterFactory rasterFactory;
        static Landis.Landscapes.LandscapeFactory landscapeFactory;
        // That's our custom to redirect console output to form
        TextWriter _writer = null;
        
        public Launcher()
        {
            InitializeComponent();

            // Prepend system path with LANDIS GDAL folder so app can find the libraries
            string path = Environment.GetEnvironmentVariable(Constants.ENV_PATH);
            string gdalFolder = WidgetsUtil.GetAppSetting("gdal_folder");
            if (gdalFolder != "")
            {
                string newPath = gdalFolder + ";" + path;
                Environment.SetEnvironmentVariable(Constants.ENV_PATH, newPath);
            }
            else
            {
                Console.WriteLine("unable to locate gdalFolder in config file");
            }

            // Instantiate text writer
            _writer = new TextBoxStreamWriter(TxtBoxStatus);

            // Set the BackColor so that we can set the ForeColor to red below if there is an error
            // This is an eccentricity with MS read-only textbox
            TxtBoxStatus.BackColor = SystemColors.Control;

            try
            {
                //@ToDo: Is it okay to hard-code this path? If the widget runs from the LANDIS-II bin, shouldn't be needed
                //Landis.Core.IExtensionDataset extensions = Landis.Extensions.Dataset.LoadOrCreate();
                string extFolder = Constants.EXTENSIONS_FOLDER + Constants.EXTENSIONS_XML;
                extensions = Landis.Extensions.Dataset.LoadOrCreate(extFolder);
                rasterFactory = new RasterFactory();
                landscapeFactory = new Landis.Landscapes.LandscapeFactory();
                model = new Landis.Model(extensions, rasterFactory, landscapeFactory);
            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Unable to create Model:" + Environment.NewLine);
                sb.Append(exc.Message);
                if (exc.InnerException != null)
                {
                    sb.Append( exc.InnerException.Message);
                }
                sb.Append(Environment.NewLine);
                sb.Append("Stack trace:" + Environment.NewLine);
                sb.Append(exc.StackTrace);
                MessageBox.Show(sb.ToString(), "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Close the form immediately if the model couldn't be instantiated
                this.Shown += new EventHandler(Launcher_CloseOnStart);
            }
        }

        private void Launcher_CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = _writer;
            // Clear the textBox
            TxtBoxStatus.Text = "";

            string workingDirectory = Path.GetDirectoryName(txtFilePath.Text);
            // If the scenario path is bad print to the console and exit the sub
            // Check for the file right before running in case it was moved
            if (!File.Exists(txtFilePath.Text))
            {
                TxtBoxStatus.ForeColor = Color.Red;
                string errorMessage = "The scenario file you specified is not valid.\r\n";
                errorMessage = errorMessage + "Make sure you have write access to the working directory.";
                wi.WriteLine(errorMessage);
                return;
            }
 
            try
            {
                //Disable buttons before starting processing
                enableButtons(false);
                
                //Reset the textbox color to black
                TxtBoxStatus.ForeColor = Color.Black;
                
                // The log4net section in the application's configuration file
                // requires the environment variable WORKING_DIR be set to the
                // current working directory.
                // This will be the folder containing the scenario .txt file

                Environment.SetEnvironmentVariable(Constants.ENV_WORKING_DIR, workingDirectory);
                log4net.Config.XmlConfigurator.Configure();


                // Set the working directory for the Model
                Directory.SetCurrentDirectory(workingDirectory);

                // Get the installed LANDIS version from the console
                //string version = Landis.App.GetAppSetting("version");
                string version = WidgetsUtil.GetAssemblySetting("version");
                if (version == "")
                    throw new Exception("The LANDIS application setting \"version\" is empty or blank");
                string release = WidgetsUtil.GetAssemblySetting("release");
                if (release != "")
                    release = string.Format(" ({0})", release);
                wi.WriteLine("LANDIS-II {0}{1}", version, release);
                model.Run(txtFilePath.Text, wi);
                enableButtons(true);
                MessageBox.Show("Model run is complete");
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
                    using (TextWriter writer = File.CreateText(workingDirectory + Constants.ERROR_LOG))
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
                    string strError = "\r\nA program error occurred.\r\nThe error log is available at " + workingDirectory + Constants.ERROR_LOG;
                    wi.WriteLine(strError);

                }
            }

        }

        private void BtnFile_Click(object sender, EventArgs e)
        {

            openFD.Title = "Scenario file";
            openFD.FileName = "";
            openFD.Filter = "Text|*.txt";
            openFD.ShowDialog();

            string filePath = openFD.FileName;

            if (!String.IsNullOrEmpty(filePath))
            {
                //Make sure user has write access to working directory before proceeding
                string workingDirectory = Path.GetDirectoryName(filePath);
                if (WidgetsUtil.HasWriteAccess(workingDirectory) == false)
                {
                    MessageBox.Show("The working directory you selected is invalid. You cannot write to this directory.",
                        "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    txtFilePath.Text = filePath;
                    if (txtFilePath.Text.Length > 0)
                    {
                        BtnRun.Enabled = true;
                        BtnValidate.Enabled = true;
                        enableLogButton();
                    }
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

        private void enableButtons(Boolean enableButtons)
        {
            BtnFile.Enabled = enableButtons;
            BtnRun.Enabled = enableButtons;
            if (enableButtons == false)
            {
                BtnLogFile.Enabled = enableButtons;
            }
            else
            {
                enableLogButton();
            }
            //@ToDo: Enable when validate function is enabled
            BtnValidate.Enabled = enableButtons;
        }

        private void enableLogButton()
        {
            if (WidgetsUtil.LandisLogExists(Path.GetDirectoryName(txtFilePath.Text)) == true)
            {
                BtnLogFile.Enabled = true;
            }
            else {
                BtnLogFile.Enabled = false;
            }
        }

        private void BtnLogFile_Click(object sender, EventArgs e)
        {
            string logName = WidgetsUtil.GetAppSetting("landis_log");
            string fullPath = Path.GetDirectoryName(txtFilePath.Text) + "\\" + logName;
            System.Diagnostics.Process.Start(fullPath);
        }

        private void BtnValidate_Click(object sender, EventArgs e)
        {
            WidgetInterface wi = new WidgetInterface();
            // Set text writer in WidgetInterface
            wi.TextWriter = _writer;
            // Clear the textBox
            TxtBoxStatus.Text = "";

            string workingDirectory = Path.GetDirectoryName(txtFilePath.Text);
            // If the scenario path is bad print to the console and exit the sub
            // Check for the file right before running in case it was moved
            if (!File.Exists(txtFilePath.Text))
            {
                TxtBoxStatus.ForeColor = Color.Red;
                string errorMessage = "The scenario file you specified is not valid.\r\n";
                errorMessage = errorMessage + "Make sure you have write access to the working directory.";
                wi.WriteLine(errorMessage);
                return;
            }

            // The log4net section in the application's configuration file
            // requires the environment variable WORKING_DIR be set to the
            // current working directory.
            // This will be the folder containing the scenario .txt file

            Environment.SetEnvironmentVariable(Constants.ENV_WORKING_DIR, workingDirectory);
            log4net.Config.XmlConfigurator.Configure();
            // Set the working directory for the Model
            Directory.SetCurrentDirectory(workingDirectory);

            try
            {
                //Disable buttons before starting processing
                enableButtons(false);

                //Reset the textbox color to black
                TxtBoxStatus.ForeColor = Color.Black;

                Validator validator = new Validator(model, landscapeFactory, wi);
                validator.ValidateScenario(txtFilePath.Text, extensions, rasterFactory);
                wi.WriteLine("Validation is complete.");
                enableButtons(true);
                MessageBox.Show("Validation is complete");
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
                    using (TextWriter writer = File.CreateText(workingDirectory + Constants.ERROR_LOG))
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
                    Console.WriteLine("Launcher exception: " + exc2);
                }
                //Print an error message
                if (logAvailable == true)
                {
                    wi.WriteLine("\r\nValidation failed:");
                    wi.WriteLine("  {0}", exc.Message);
                    //if (exc.InnerException != null)
                    //{
                    //    wi.WriteLine("  {0}", exc.InnerException.Message);
                    //}
                    string strError = "\r\nAn error log is available at " + workingDirectory + Constants.ERROR_LOG;
                    wi.WriteLine(strError);

                }
            }
        }

    }
}
