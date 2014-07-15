using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Widgets
{
    public class WidgetsUtil
    {

        public static string GetAppSetting(string settingName)
        {
            string setting = System.Configuration.ConfigurationManager.AppSettings[settingName];
            if (setting == null)
                throw new Exception("The application setting \"" + settingName + "\" is not set");
            return setting.Trim(null);
        }

        public static string GetAssemblySetting(string settingName)
        {
            Type t = Type.GetType("Landis.Model,Landis.Core.Implementation");
            Assembly a = System.Reflection.Assembly.GetAssembly(t);
            string config = string.Empty;
            string version = string.Empty;
            if (a != null) {
                object[] customAttributes = a.GetCustomAttributes(false);
                if (settingName == "release") {
                    foreach (object attribute in customAttributes)
                    {  
                        if (attribute.GetType() == typeof(System.Reflection.AssemblyConfigurationAttribute))
                        {
                            config = ((System.Reflection.AssemblyConfigurationAttribute) attribute).Configuration;
                            return config;
                        }
                    }
                }
                else if (settingName == "version")
                {
                    Version assemblyVersion = a.GetName().Version;
                    string majorVersion = assemblyVersion.Major.ToString();
                    string minorVersion = assemblyVersion.Minor.ToString();
                    version = majorVersion + "." + minorVersion;
                    return version;
                }

             }
            return string.Empty;
        }

        public static Boolean HasWriteAccess(string directory)
        {
            try
            {
                string filename = directory + "\\test.txt";
                using (FileStream fstream = new FileStream(filename, FileMode.Create))
                using (TextWriter writer = new StreamWriter(fstream))
                {
                    writer.WriteLine("sometext");
                }
                File.Delete(filename);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                //No permission. 
                //Either throw an exception so this can be handled by a calling function
                //or inform the user that they do not have permission to write to the folder and return.
                return false;
            }
        }

        public static Boolean LandisLogExists(string directory)
        {
            try
            {
                string logName = GetAppSetting("landis_log");
                string filename = directory + "\\" + logName;
                if (File.Exists(filename))
                {
                    // Make sure file isn't zero length
                    if (new FileInfo(filename).Length > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // An error occured when trying to open the log
                return false;
            }
        }

        // http://msdn.microsoft.com/en-us/library/bb762914(v=vs.90).aspx
        public static void DirectoryCopy(string sourceDirName, string destDirName, string destPrefix)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception. 
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file. 
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                // Don't copy subdirectories containing the destPrefix so we don't copy directories that have just been added
                if (subdir.Name.IndexOf(destPrefix) != 0)
                {
                    // Create the subdirectory. 
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, destPrefix);
                }
            }
        }

        // Delete any log file that we don't want to replicate
        public static void DeleteLogFiles(string scenarioPath, string[] logFiles)
        {
           for (int i = 0; i < logFiles.Length -1; i++)
            {
                string fullPath = scenarioPath + "\\" + logFiles[i];
                File.Delete(fullPath);
            }
        }
    }
}
