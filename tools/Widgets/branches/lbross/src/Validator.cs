using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Landis;
using Landis.Core;
using Landis.Species;
using Landis.RasterIO.Gdal;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using Loader = Edu.Wisc.Forest.Flel.Util.PlugIns.Loader;
using Troschuetz.Random;
using log4net;


namespace Widgets
{
    public class Validator
         : ICore
    {

        private SiteVarRegistry siteVarRegistry;
        private IExtensionDataset extensionDataset;
        private ISpeciesDataset species;
        private IEcoregionDataset ecoregions;
        private IConfigurableRasterFactory rasterFactory;
        private ILandscapeFactory landscapeFactory;
        private ILandscape landscape;
        //private float cellLength;  // meters
        //private float cellArea;    // hectares
        private ISiteVar<IEcoregion> ecoregionSiteVar;
        //private int startTime;
        //private int endTime;
        //private int currentTime;
        //private int timeSinceStart;
        private List<ExtensionMain> disturbAndOtherExtensions;
        private SuccessionMain succession;
        // Resolve warning about RandomNumberGenerator being never assigned to
        private static Generator RandomNumberGenerator;
        private static BetaDistribution betaDist;
        private static BetaPrimeDistribution betaPrimeDist;
        private static CauchyDistribution cauchyDist;
        private static ChiDistribution chiDist;
        private static ChiSquareDistribution chiSquareDist;
        private static ContinuousUniformDistribution continuousUniformDist;
        private static ErlangDistribution erlangDist;
        private static ExponentialDistribution exponentialDist;
        private static FisherSnedecorDistribution fisherSnedecorDist;
        private static FisherTippettDistribution fisherTippettDist;
        private static GammaDistribution gammaDist;
        private static LaplaceDistribution laplaceDist;
        private static LognormalDistribution lognormalDist;
        private static NormalDistribution normalDist;
        private static ParetoDistribution paretoDist;
        private static PowerDistribution powerDist;
        private static RayleighDistribution rayleighDist;
        private static StudentsTDistribution studentsTDist;
        private static TriangularDistribution triangularDist;
        private static WeibullDistribution weibullDist;
        private static PoissonDistribution poissonDist;

        private Model model;
        private IUserInterface ui;

        //---------------------------------------------------------------------

        private static ILog logger = LogManager.GetLogger("Landis");


        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Validator(IExtensionDataset extensionDataset,
                         IConfigurableRasterFactory rasterFactory,
                         ILandscapeFactory landscapeFactory, 
                         Model aModel, 
                         IUserInterface UI)
        {
            this.extensionDataset = extensionDataset;
            this.rasterFactory = rasterFactory;
            this.landscapeFactory = landscapeFactory;
            this.model = aModel;
            this.ui = UI;
            siteVarRegistry = new SiteVarRegistry();
        }

        public void ValidateScenario(string scenarioTextFilePath)
        {
            siteVarRegistry.Clear();

            ScenarioParser parser = new ScenarioParser(extensionDataset);
            Scenario scenario = Landis.Data.Load<Scenario>(scenarioTextFilePath, parser);
            InitializeRandomNumGenerator(scenario.RandomNumberSeed);
            LoadSpecies(scenario.Species, ui);
            LoadEcoregions(scenario.Ecoregions, ui);

            //ui.WriteLine("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
            Landis.Ecoregions.Map ecoregionsMap = new Landis.Ecoregions.Map(scenario.EcoregionsMap,
                                                              ecoregions,
                                                              rasterFactory);

            using (IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid())
            {
                //ui.WriteLine("Map dimensions: {0} = {1:#,##0} cell{2}", grid.Dimensions,
                             //grid.Count, (grid.Count == 1 ? "" : "s"));
                //landscape = new Landscape(grid);
                landscape = landscapeFactory.CreateLandscape(grid);
            }

            //ui.WriteLine("Sites: {0:#,##0} active ({1:p1}), {2:#,##0} inactive ({3:p1})",
            //             landscape.ActiveSiteCount, (landscape.Count > 0 ? ((double)landscape.ActiveSiteCount) / landscape.Count : 0),
            //             landscape.InactiveSiteCount, (landscape.Count > 0 ? ((double)landscape.InactiveSiteCount) / landscape.Count : 0));

            ecoregionSiteVar = ecoregionsMap.CreateSiteVar(landscape);

            disturbAndOtherExtensions = new List<ExtensionMain>();
            ui.WriteLine("Loading {0} extension ...", scenario.Succession.Info.Name);
            succession = Edu.Wisc.Forest.Flel.Util.PlugIns.Loader.Load<SuccessionMain>(scenario.Succession.Info);
            //@ToDo:Is the state of the Validator object acceptable for passing in?
            succession.LoadParameters(scenario.Succession.InitFile, this);
            succession.Initialize();
            ExtensionMain[] disturbanceExtensions = LoadExtensions(scenario.Disturbances, model, ui);
            //InitExtensions(disturbanceExtensions);
            ExtensionMain[] otherExtensions = LoadExtensions(scenario.OtherExtensions, model, ui);
            //InitExtensions(otherExtensions);
        }

        private void InitializeRandomNumGenerator(uint? seed)
        {
            if (seed.HasValue)
                Initialize(seed.Value);
            else
            {
                uint generatedSeed = GenerateSeed();
                Initialize(generatedSeed);
                ui.WriteLine("Initialized random number generator with seed = {0:#,##0}", generatedSeed);
            }
        }

        private void LoadSpecies(string path, IUserInterface ui)
        {
            //ui.WriteLine("Loading species data from file \"{0}\" ...", path);
            Landis.Species.DatasetParser parser = new Landis.Species.DatasetParser();
            species = Landis.Data.Load<ISpeciesDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private void LoadEcoregions(string path, IUserInterface ui)
        {
            //ui.WriteLine("Loading ecoregions from file \"{0}\" ...", path);
            Landis.Ecoregions.DatasetParser parser = new Landis.Ecoregions.DatasetParser();
            ecoregions = Landis.Data.Load<IEcoregionDataset>(path, parser);
        }

        private ExtensionMain[] LoadExtensions(ExtensionAndInitFile[] extensions,
                                       Model model, IUserInterface ui)
        {
            ExtensionMain[] loadedExtensions = new ExtensionMain[extensions.Length];
            foreach (int i in Indexes.Of(extensions))
            {
                ExtensionAndInitFile extensionAndInitFile = extensions[i];
                ui.WriteLine("Loading {0} extension ...", extensionAndInitFile.Info.Name);
                ExtensionMain loadedExtension = Loader.Load<ExtensionMain>(extensionAndInitFile.Info);
                loadedExtension.LoadParameters(extensionAndInitFile.InitFile, this);

                //loadedExtensions[i] = loadedExtension;

                //disturbAndOtherExtensions.Add(loadedExtension);
            }
            return loadedExtensions;
        }

        //-----------------------------------------------------------------------

        private void InitExtensions(ExtensionMain[] extensions)
        {
            foreach (ExtensionMain extension in extensions)
            {
                extension.Initialize();
            }
        }

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public FileLineReader OpenTextFile(string path)
        {
            return model.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public T Load<T>(string path,
                                ITextParser<T> parser)
        {
            return model.Load<T>(path, parser);
        }

        //---------------------------------------------------------------------

        // Flagged in ICore as deprecated; client code instructed to use Landis.Data directly
        public StreamWriter CreateTextFile(string path)
        {
            return model.CreateTextFile(path);
        }

        //------------------------------------------------------------------------

        public BetaDistribution BetaDistribution
        {
            get
            {
                return betaDist;
            }
        }

        //------------------------------------------------------------------------

        public BetaPrimeDistribution BetaPrimeDistribution
        {
            get
            {
                return betaPrimeDist;
            }
        }

        //------------------------------------------------------------------------

        public CauchyDistribution CauchyDistribution
        {
            get
            {
                return cauchyDist;
            }
        }

        //------------------------------------------------------------------------

        public ChiDistribution ChiDistribution
        {
            get
            {
                return chiDist;
            }
        }

        //------------------------------------------------------------------------

        public ChiSquareDistribution ChiSquareDistribution
        {
            get
            {
                return chiSquareDist;
            }
        }

        //------------------------------------------------------------------------

        public ContinuousUniformDistribution ContinuousUniformDistribution
        {
            get
            {
                return continuousUniformDist;
            }
        }

        //------------------------------------------------------------------------

        public ErlangDistribution ErlangDistribution
        {
            get
            {
                return erlangDist;
            }
        }

        //------------------------------------------------------------------------

        public ExponentialDistribution ExponentialDistribution
        {
            get
            {
                return exponentialDist;
            }
        }

        //------------------------------------------------------------------------

        public FisherSnedecorDistribution FisherSnedecorDistribution
        {
            get
            {
                return fisherSnedecorDist;
            }
        }

        //------------------------------------------------------------------------

        public FisherTippettDistribution FisherTippettDistribution
        {
            get
            {
                return fisherTippettDist;
            }
        }

        //------------------------------------------------------------------------

        public GammaDistribution GammaDistribution
        {
            get
            {
                return gammaDist;
            }
        }

        //------------------------------------------------------------------------

        public LaplaceDistribution LaplaceDistribution
        {
            get
            {
                return laplaceDist;
            }
        }

        //------------------------------------------------------------------------

        public LognormalDistribution LognormalDistribution
        {
            get
            {
                return lognormalDist;
            }
        }

        //------------------------------------------------------------------------

        public NormalDistribution NormalDistribution
        {
            get
            {
                return normalDist;
            }
        }

        //------------------------------------------------------------------------

        public ParetoDistribution ParetoDistribution
        {
            get
            {
                return paretoDist;
            }
        }

        //------------------------------------------------------------------------

        public PowerDistribution PowerDistribution
        {
            get
            {
                return powerDist;
            }
        }

        //------------------------------------------------------------------------

        public RayleighDistribution RayleighDistribution
        {
            get
            {
                return rayleighDist;
            }
        }

        //------------------------------------------------------------------------

        public StudentsTDistribution StudentsTDistribution
        {
            get
            {
                return studentsTDist;
            }
        }

        //------------------------------------------------------------------------

        public TriangularDistribution TriangularDistribution
        {
            get
            {
                return triangularDist;
            }
        }

        //------------------------------------------------------------------------

        public WeibullDistribution WeibullDistribution
        {
            get
            {
                return weibullDist;
            }
        }

        //------------------------------------------------------------------------

        public PoissonDistribution PoissonDistribution
        {
            get
            {
                return poissonDist;
            }
        }

        //------------------------------------------------------------------------

        float ICore.CellLength
        {
            get
            {
                ICore core = (ICore)model;
                return core.CellLength;
            }
        }

        //---------------------------------------------------------------------

        float ICore.CellArea
        {
            get
            {
                ICore core = (ICore)model;
                return core.CellArea;
            }
        }

        //---------------------------------------------------------------------

        int ICore.StartTime
        {
            get
            {
                ICore core = (ICore)model;
                return core.StartTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.EndTime
        {
            get
            {
                ICore core = (ICore)model;
                return core.EndTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.CurrentTime
        {
            get
            {
                ICore core = (ICore)model;
                return core.CurrentTime;
            }
        }

        //---------------------------------------------------------------------
        int ICore.TimeSinceStart
        {
            get
            {
                ICore core = (ICore)model;
                return core.TimeSinceStart;
            }
        }

        //---------------------------------------------------------------------

        Generator ICore.Generator
        {
            get
            {
                return RandomNumberGenerator;
            }
        }

        //---------------------------------------------------------------------

        ISpeciesDataset ICore.Species
        {
            get
            {
                // Use local variable; Initialized in ValidateScenario
                return species;
            }
        }

        //---------------------------------------------------------------------

        IEcoregionDataset ICore.Ecoregions
        {
            get
            {
                // Use local variable; Initialized in ValidateScenario
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        ISiteVar<IEcoregion> ICore.Ecoregion
        {
            get
            {
                // using local variable here so we can manage the site variables in the validator
                return ecoregionSiteVar;
            }
        }

        //---------------------------------------------------------------------

        ILandscape ICore.Landscape
        {
            get
            {
                // using local variable here because we need to instantiate separate of running the model
                return landscape;
            }
        }

        //---------------------------------------------------------------------

        IUserInterface ICore.UI
        {
            get
            {
                // using local variable here; passed into constructor
                return ui;
            }
        }

        //[System.Obsolete("Use the UI property instead.")]
        IUserInterface ICore.Log
        {
            get
            {
                // using local variable here; passed into constructor
                return ui;
            }
        }

        //---------------------------------------------------------------------

        public uint GenerateSeed()
        {
            return model.GenerateSeed();
        }

         //<summary>
         //Generates a random number with a uniform distribution between
         //0.0 and 1.0.
         //</summary>
        double ICore.GenerateUniform()
        {
            return RandomNumberGenerator.NextDouble();
        }

        //ISiteVar<T> ICore.GetSiteVar<T>(string name)
        //{
        //    return siteVarRegistry.GetVar<T>(name);
        //}

        public void Initialize(uint seed)
        {
            RandomNumberGenerator = new MT19937Generator(seed);
            betaDist = new BetaDistribution(RandomNumberGenerator);
            betaPrimeDist = new BetaPrimeDistribution(RandomNumberGenerator);
            cauchyDist = new CauchyDistribution(RandomNumberGenerator);
            chiDist = new ChiDistribution(RandomNumberGenerator);
            chiSquareDist = new ChiSquareDistribution(RandomNumberGenerator);
            continuousUniformDist = new ContinuousUniformDistribution(RandomNumberGenerator);
            erlangDist = new ErlangDistribution(RandomNumberGenerator);
            exponentialDist = new ExponentialDistribution(RandomNumberGenerator);
            fisherSnedecorDist = new FisherSnedecorDistribution(RandomNumberGenerator);
            fisherTippettDist = new FisherTippettDistribution(RandomNumberGenerator);
            gammaDist = new GammaDistribution(RandomNumberGenerator);
            laplaceDist = new LaplaceDistribution(RandomNumberGenerator);
            lognormalDist = new LognormalDistribution(RandomNumberGenerator);
            normalDist = new NormalDistribution(RandomNumberGenerator);
            paretoDist = new ParetoDistribution(RandomNumberGenerator);
            powerDist = new PowerDistribution(RandomNumberGenerator);
            rayleighDist = new RayleighDistribution(RandomNumberGenerator);
            studentsTDist = new StudentsTDistribution(RandomNumberGenerator);
            triangularDist = new TriangularDistribution(RandomNumberGenerator);
            weibullDist = new WeibullDistribution(RandomNumberGenerator);
            poissonDist = new PoissonDistribution(RandomNumberGenerator);
        }

        //---------------------------------------------------------------------

        public double NextDouble()
        {
            return RandomNumberGenerator.NextDouble();
        }

        /// <summary>
        /// Writes an informational message into the ui.
        /// </summary>
        /// <param name="message">
        /// Message to write into the ui.  It may contain placeholders for
        /// optional arguments using the "{n}" notation used by the
        /// System.String.Format method.
        /// </param>
        /// <param name="mesgArgs">
        /// Optional arguments for the message.
        /// </param>
        public void Info(string message,
                                params object[] mesgArgs)
        {
            logger.Info(string.Format(message, mesgArgs));
        }

        //---------------------------------------------------------------------

        void ICore.RegisterSiteVar(ISiteVariable siteVar,
                                   string name)
        {
            ui.WriteLine("   Registering Data:  {0}.", name);
            siteVarRegistry.RegisterVar(siteVar, name);
        }

        //---------------------------------------------------------------------

        ISiteVar<T> ICore.GetSiteVar<T>(string name)
        {
            //System.Windows.Forms.MessageBox.Show(name);
            return siteVarRegistry.GetVar<T>(name);
        }

        //---------------------------------------------------------------------

        public List<T> shuffle<T>(List<T> list)
        {
            List<T> shuffledList = new List<T>();

            int randomIndex = 0;
            while (list.Count > 0)
            {
                randomIndex = RandomNumberGenerator.Next(list.Count); //Choose a random object in the list
                shuffledList.Add(list[randomIndex]); //add it to the new, random list
                list.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return shuffledList;
        }

        IOutputRaster<TPixel> IRasterFactory.CreateRaster<TPixel>(string path,
                                                          Dimensions dimensions)
        {
            ICore core = (ICore)model;
            return core.CreateRaster<TPixel>(path, dimensions);
        }

        //---------------------------------------------------------------------

        IInputRaster<TPixel> IRasterFactory.OpenRaster<TPixel>(string path)
        {
            ICore core = (ICore)model;
            return core.OpenRaster<TPixel>(path);
        }

    }
}
