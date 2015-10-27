using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II Widgets")]
[assembly: AssemblyVersion("1.1.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif