# Getting Started
Extensions can be written in any [.NET programming languages](https://dotnet.microsoft.com/en-us/languages) (C#, F#, Visual Basic). It is recommended to use C# for development, and this documentation also uses C# to demonstrate, but using other languages is also possible. Anyways, having basic knowledges about the language is essential to get started.

To build an extension, [.NET SDK](https://dotnet.microsoft.com/en-us/) is required. You can set it up by simply installing [Visual Studio](https://visualstudio.microsoft.com/), which itself is an IDE as well.

An extension is essentially a class library. To create one in Visual Studio, select the “Class Library” template when creating a project.

Next, choose a name for your extension. Consider naming your extension according to the pattern `[<Author>.]<SourceOrFeature>.CysTerra[.<Subcomponent>]`. For example:

- `MyApi.CysTerra`
- `MyApi.CysTerra.Tts`
- `Lime.LimesApi.CysTerra`

> [!TIP]
> All the interfaces and first-party extensions of CysTerra have their names started with `Cryville.EEW`. This is not mandatory, and you should pick the name of your extension by yourself instead.

> [!IMPORTANT]
> However, if you wish to publish your extension to the [official extension repository](https://github.com/cryville/CysTerra.ExtensionRepo), your extension name MUST NOT start with `Cryville` to distinguish it from first-party components.

When prompted to choose a framework, select .NET Standard 2.1.

> [!WARNING]
> .NET Standard 2.1 is the framework CysTerra extensions should be targeting currently. Typically .NET 6 is also supported but the compatibility may vary depending on the hosting app and the environment. .NET 7+ is not supported.

To instruct the compiler to generate the extension package, you need to open the project file (`.csproj`, `.fsproj`, or `.vbproj` depending on the programming language), and then append the following property.

```xml
<PropertyGroup>
	<IsExtensionProject>true</IsExtensionProject>
</PropertyGroup>
```

## Dependencies
After creating the project, decide which kind of extension you want to make. According to the components you want to provide in the extension, you need to install different packages as dependencies.

Package | Components
------- | ----------
`Cryville.EEW` | Source workers that **fetches events** from the event source; Report generators that generates **event summaries** (reports) from events
`Cryville.EEW.Features` | Feature generators that generates **event details** from events
`Cryville.EEW.TTS` | TTS message generators that generates **TTS messages** from events

Additional packages may be installed per requirement.

Package | Contents
------- | --------
`Cryville.EEW.Analyzer` | Provides code analyzers that report potential issues within your code
`Cryville.EEW.Measure` | Provides helper types and methods related to quantities
`Cryville.EEW.TagTypeKeys` | Provides a set of stable tag type keys that are well defined
`Cryville.Measure` | Provides quantity types

You can install the packages from NuGet. In Visual Studio, select “Manage NuGet Packages” in the Project menu, browse and install the packages you need.

> [!TIP]
> It is recommended to always install `Cryville.EEW.Analyzer`.

> [!TIP]
> It is recommended to split your extension into multiple extensions if you are making multiple types of components. (Except for source workers and report generators which should be placed into a single extension)
>
> For example, if you are implementing the source worker with TTS support for an event source, put the source worker and the report generator into one extension, and the TTS message generator into another extension.
>
> Since the event model is generally defined with the source worker, you should add a project reference to the source worker extension project in the TTS extension project, for the TTS extension to use the event model.
>
> In Visual Studio, you can add project references with “Add Project Reference” in the Project menu.
>
> The project reference should be non-private as described below.

### Dependency Runtime Trimming
References to other extensions and public packages should have their runtime files excluded from the build to reduce the build size. To do this, open the project file and append the following attributes.

# [Since 0.8.14](#tab/0-8-14--)
Starting from `Cryville.EEW` 0.8.14, you should use the unified `IsExtension` attribute to specify whether a reference is an extension. By default, all package references are not extensions, and all project references are extensions.

CysTerra interface packages are now automatically processed. Specifying the `IsExtension` attribute for them does nothing.

```xml
<ItemGroup>
	<!-- Reference to another extension package should have
		IsExtension="true" -->
	<PackageReference Include="SomeOtherExtension" Version="0.8.14"
		IsExtension="true" />
</ItemGroup>

<ItemGroup>
	<!-- Reference to another non-extension project should have
		IsExtension="false" -->
	<ProjectReference Include="..\Cryville.EEW.JMAAtom\Cryville.EEW.JMAAtom.csproj"
		IsExtension="false" />
</ItemGroup>
```

# [Before 0.8.14](#tab/--0-8-13)
```xml
<ItemGroup>
	<!-- Reference to a public package (listed above) or another extension package should have
		PrivateAssets="contentfiles;analyzers;build;runtime" ExcludeAssets="runtime" -->
	<PackageReference Include="Cryville.EEW.Features" Version="0.8.0"
		PrivateAssets="contentfiles;analyzers;build;runtime" ExcludeAssets="runtime" />

	<!-- Reference to an external package should have
		PrivateAssets="contentfiles;analyzers;build;runtime" -->
	<PackageReference Include="SomeExternalLibrary" Version="0.8.0"
		PrivateAssets="contentfiles;analyzers;build;runtime" />
</ItemGroup>

<ItemGroup>
	<!-- Reference to another extension project should have
		Private="false" -->
	<ProjectReference Include="..\Cryville.EEW.JMAAtom\Cryville.EEW.JMAAtom.csproj"
		Private="false" />
</ItemGroup>
```

***

## Components
The following are documentations about different types of components. Read the one about your targeted component for the next steps.

- [Source worker and report generator](source-worker.md)
- [Feature generator](feature-generator.md)
- [TTS message generator](tts-message-generator.md)

In order for CysTerra to discover your components, you also need to define [builders](builder.md) for them.

## Additional Guides
- [Localization](localization.md): Localize the texts in your extension.
- [Packaging](packaging.md): Package your extension for publishing.
- ~~[Error Reporting](error-reporting.md): Allow users to report errors occurred in your extension to you.~~
- ~~[Update Checking](update-checking.md): Allow CysTerra to check for updates for your extension.~~
