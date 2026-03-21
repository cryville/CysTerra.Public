# Packaging
After implementing your [components](getting-started.md#components) and [builders](builder.md), you are ready to package your extension for using and distribution.

## Versioning
Before actually building the package, it is essential to properly specify the version of your extension. A proper version gives CysTerra a hint to the compatibility of your extension with other components.

You can specify the version in your project file.

```xml
<PropertyGroup>
	<Version>1.0.0</Version>
</PropertyGroup>
```

Two versions are compared by looking at their first different numbers, separated by dots, from left to right. A greater number indicates a greater (later) version. For example, `1.0.0` < `2.0.0` < `2.1.0` < `2.1.1` < `2.2.0` < `3.0.0`. When you build an updated version of your extension, you must increment the version so that it is greater than the old version.

Assume another extension `B` has a reference to the version `r` of your extension `A`, and the version `l` of `A` is loaded in CysTerra. When CysTerra is trying to load `B`, if `l` < `r`, the loading fails.

On the other hand, if `l` > `r`, by default CysTerra loads `B` but with a warning message, as CysTerra cannot ensure there is no breaking change within `A` from version `r` to `l`. However if you use [semantic versioning](https://semver.org/), you can apply [](xref:Cryville.EEW.ComponentModel.UsesSemanticVersioningAttribute) to your assembly, so that CysTerra can tell what versions of your extension are interchangeable and removes the warning message accordingly.

> [!WARNING]
> DO NOT apply [](xref:Cryville.EEW.ComponentModel.UsesSemanticVersioningAttribute) unless you fully understand and actually follow semantic versioning.

## Building the Package
To generate a package for your extension, build the project with the “Release” configuration. You can switch the configuration in Visual Studio by using the configuration dropdown in the toolbar (the default configuration is “Debug”).

By default, the extension package is generated in `bin/ext/Release/` in your project directory. You can change it by adding the following property into your project file.

```xml
<PropertyGroup>
	<ExtensionPackageOutputDirectory>$(MSBuildThisFileDirectory)some/path/in/project/directory/</ExtensionPackageOutputDirectory>
</PropertyGroup>
```

It is recommended to pack the extension package files into a zip file for easy distribution. For a zip file to be recognized as an extension package, the main `.dll` file must be in its root directory (i.e. not in a nested directory).
