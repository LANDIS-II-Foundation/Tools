using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Widgets
{
    public static class Constants
    {
        //Commonly used file paths
        public static string LANDIS_BIN = "C:\\Program Files\\LANDIS-II\\v6\\bin";
        public static string EXTENSIONS_FOLDER = LANDIS_BIN + "\\extensions";
        public static string EXTENSIONS_XML = "\\extensions.xml";
        public static string ERROR_LOG = "\\errorLog.txt";

        //Environment variables
        public static string ENV_PATH = "PATH";
        public static string ENV_WORKING_DIR = "WORKING_DIR";
    }
}
