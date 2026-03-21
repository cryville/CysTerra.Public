# Builder and Configuration
In order for CysTerra to discover your components, you need to define builder types for them. Builder types must be annotated with the [](xref:System.ComponentModel.Composition.ExportAttribute) attribute with one of the following contract types.

- [IBuilder](xref:Cryville.EEW.IBuilder`1)<[](xref:Cryville.EEW.ISourceWorker)>
- [IBuilder](xref:Cryville.EEW.IBuilder`1)<[IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.Report.ReportModel)>>
- [IBuilder](xref:Cryville.EEW.IBuilder`1)<[IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.Features.Feature)>>
- [IBuilder](xref:Cryville.EEW.IBuilder`1)<[IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.TTS.TTSEntry)>>

A discoverable builder type must derive from its contract type, be concrete (not abstract), and have a public parameterless constructor (either explicit or implicit).

For example:

```cs
using Cryville.EEW;
using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Globalization;

[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
public class MyReportGeneratorBuilder : IBuilder<MyReportGenerator> {
	public override string? GetName([NotNull] ref CultureInfo? culture) {
		using var lres = new LocalizedResource("", ref culture);
		var res = lres.RootMessageStringSet;
		return res.GetStringRequired("SourceName");
	}
	public MyReportGenerator Build(ref CultureInfo? culture) {
		return new MyReportGenerator();
	}
}
```

## Builder Configuration
Public browsable settable properties in the builder are recognized as **builder configurations** configurable by the user. These properties are set by the user before [](xref:Cryville.EEW.IBuilder`1.Build*) is called.

Note that only one component built with the same builder and the same set of builder properties can exist at the same time. If no builder configurations is defined in a builder, only one component built with this builder can exist.

```cs
using Cryville.EEW;
using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Globalization;

[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
public class MyReportGeneratorBuilder : IBuilder<MyReportGenerator> {
	// Define a configuration named MyConfig
	public bool MyConfig { get; set; }

	public override string? GetName([NotNull] ref CultureInfo? culture) {
		using var lres = new LocalizedResource("", ref culture);
		var res = lres.RootMessageStringSet;
		return res.GetStringRequired("SourceName");
	}
	public MyReportGenerator Build(ref CultureInfo? culture) {
		// MyConfig has been set by the user here
		return new MyReportGenerator(MyConfig);
	}
}
```

## Component Configuration
Like builders, components can have **component configurations**, which is defined in the same way as in builders. However, the component has to implement [](xref:Cryville.EEW.IPropertiesHolder) for its properties to be recognized as configurations.

```cs
using Cryville.EEW;
using Cryville.EEW.Report;
using System.Globalization;

// Implement IPropertiesHolder to add configurations
public class MyReportGenerator : IGenerator<MyEvent, ReportModel>, IPropertiesHolder {
	// Define a configuration named AdditionalConfig
	public bool AdditionalConfig { get; set; }
	
	public ReportModel Generate(MyEvent e, ref CultureInfo culture) {
		/* ... */
	}
}
```

> [!NOTE]
> Builders actually implement [](xref:Cryville.EEW.IPropertiesHolder) as well because [](xref:Cryville.EEW.IBuilder) derives from it.

## Configuration Serialization
By default, configurations are saved in an unstable way, which can potentially break across different devices or when the app or the extension is updated. It is recommended to apply [](xref:Cryville.EEW.ComponentModel.JsonSerializedPropertyAttribute) to your properties so that they are saved in a much more stable way.

You can also apply that attribute to the class or the whole assembly to enable stable serialization for all properties within the scope.
