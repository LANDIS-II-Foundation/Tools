Widgets and GDAL
================
The Widgets solution depends on the GDAL libraries deployed with LANDIS core.
If the GDAL version is updated for LANDIS core, the Widgets also need to be recompiled
and redeployed.

1. The location of the GDAL folder needs to be pre-pended to the the system PATH.
This occurs in the constructor for the Launcher and Replicator forms. The base path
for the GDAL folder is defined in the app.config file for each project. The C#
code is written to use the most recent GDAL version installed under the GDAL
base path.

2. The gdal_csharp, Landis.RasterIO.Gdal, and Landis.SpatialModeling assemblies are
all dependent on a single version of GDAL. The developer should install the new version
of LANDIS core which may contain a new version of GDAL. Then make sure the Widgets,
Launcher, and Replicator projects are all pointed at the new version(s) of these assemblies.

3. There is a binding redirect element in the Launcher and Replicator app.config files to 
allow older versions of the Landis.RasterIO.Gdal and Landis.SpatialModeling assemblies to 
use later versions of gdal_csharp.dll. The developer should copy this entry from
http://landis-extensions.googlecode.com/svn/model/trunk/console/Landis.Console-X.Y.exe.config
where {GDAL_CSHARP_VERSION} is the version of gdal_csharp.dll installed with LANDIS-II.
You can find the version by looking at the properties of gdal_csharp.dll in Visual Studio.

4. Recompile and redeploy the Widgets. 
