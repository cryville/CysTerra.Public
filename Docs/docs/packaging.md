# Packaging
To generate a package for your extension, build the project with the “Release” configuration. You can switch the configuration in Visual Studio by using the configuration dropdown in the toolbar.

By default, the extension package is generated in `bin/ext/Release/` in your project directory. You can change it by adding the following property into your project file.

```xml
<PropertyGroup>
	<ExtensionPackageOutputDirectory>$(MSBuildThisFileDirectory)some/path/in/project/directory/</ExtensionPackageOutputDirectory>
</PropertyGroup>
```

It is recommended to pack the extension package files into a zip file for easy distribution. For a zip file to be recognized as an extension package, the main `.dll` file must be in its root directory (i.e. not in a nested directory).
