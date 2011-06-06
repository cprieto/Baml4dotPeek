using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Baml4DotPeek")]
[assembly: AssemblyProduct("Baml4DotPeek")]
[assembly: AssemblyCopyright("Copyright ©  2011")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("823eb947-4e96-4331-b925-98d1609b4b1b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: PluginDescription("Show a list of Baml resources")]
[assembly: PluginTitle("Cprieto.DotPeek.BamlResources")]
[assembly: PluginVendor("@cprieto")]

[assembly: ActionsXml("Cprieto.DotPeek.Actions.xml")]
