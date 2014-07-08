#include GetEnv("LANDIS_SDK") + '\packaging\initialize.iss'

#define ExtInfoFile "Widgets 1.0.txt"
#define CoreMajorVersion "6"

#include LandisSDK + '\packaging\read-ext-info.iss'
#include LandisSDK + '\packaging\Landis-vars.iss'

[Setup]
; Directives for the [Setup] section of the Inno Setup script for the setup
; program (installer) for a LANDIS-II extension.
; This is pulled from the Landis SDK Setup-directives.iss with modifications for Widgets

AppName=LANDIS-II {#ExtensionName} v{#Version}{#ReleaseForAppName}
AppVerName=LANDIS-II {#ExtensionName} v{#Version}{#ReleaseForAppVerName}
AppPublisher=Portland State University

DefaultDirName={#LandisInstallDir}\v{#CoreMajorVersion}
UsePreviousAppDir=no
DisableDirPage=yes
AlwaysShowDirOnReadyPage=yes
DisableProgramGroupPage=yes
AlwaysShowGroupOnReadyPage=yes

OutputDir={#SourcePath}
OutputBaseFilename=LANDIS-II {#ExtensionName} {#VersionRelease}-setup

#if PatchLevel == ""
VersionInfoVersion={#MajorMinor}.0.{#ReleaseAsInt}
#else
VersionInfoVersion={#MajorMinor}.{#PatchLevel}.{#ReleaseAsInt}
#endif

UninstallFilesDir={#LandisMajorVerDir}\uninstall

LicenseFile={#LandisSDK}\licenses\LANDIS-II_Binary_license.rtf
DefaultGroupName=LANDIS-II\v{#CoreMajorVersion}
SetupIconFile=.\icons\Tree.ico
UninstallDisplayIcon=.\icons\Tree.ico

[Files]
; Install Widgets .dll
Source: ..\build\{#ExtensionAssembly}; DestDir: {app}\bin
; Install Launcher executable
Source: ..\build\Launcher.exe; DestDir: {app}\bin
; Install configuration file
Source: ..\build\Launcher.exe.config; DestDir: {app}\bin
; Install Replicator executable
Source: ..\build\Replicator.exe; DestDir: {app}\bin
; Install configuration file
Source: ..\build\Replicator.exe.config; DestDir: {app}\bin


; Install Files and Necessary Libraries
;Source: ..\build\gdal_csharp.dll; DestDir: {app}\bin Flags: uninsneveruninstall replacesameversion

; Copy icons
Source: .\icons\rocket2.ico; DestDir: {app}\icons
Source: .\icons\replicator.ico; DestDir: {app}\icons

[Icons]
Name: {group}\Widgets\Launcher;     Filename: {app}\bin\Launcher.exe;  IconFilename: "{app}\icons\rocket2.ico"
Name: {group}\Widgets\Replicator;   Filename: {app}\bin\Replicator.exe;  IconFilename: "{app}\icons\replicator.ico"
Name: {group}\Uninstall\Widgets {#VersionRelease};   Filename: {uninstallexe}; Parameters: "/log";

[Code]
#include LandisSDK + '\packaging\Pascal-code.iss'

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;


