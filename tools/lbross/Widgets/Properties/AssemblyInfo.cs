using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II Widgets")]
[assembly: AssemblyVersion("6.0.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif