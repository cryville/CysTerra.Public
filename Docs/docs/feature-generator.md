# Feature Generator
CysTerra can display complex details of an event both as texts and map elements, provided a feature is generated for the event. Feature generators are the components responsible for generating features from events.

A feature generator is a class implementing the [IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.Features.Feature)> interface. The following is an example implementation.

```cs
using Cryville.Common.Compat;
using Cryville.EEW;
using Cryville.EEW.Features;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

public class MyFeatureGenerator : IGenerator<MyEvent, Feature> {
	public Feature Generate(MyEvent e, ref CultureInfo culture) {
		ThrowHelper.ThrowIfNull(e);

		var f = new Feature() {
			{ Is, Earthquake },
			{ Time, /* ... */ },
			{ At, new Feature(new Point(/* ... */)) {
				{ Is, Hypocenter },
				{ Name, /* ... */ },
				{ HypocenterDepth, /* ... */ },
			} },
			{ Magnitude, /* ... */ },
			{ Source, /* ... */ },
		};
		return f;
	}
}
```

A feature is essentially a dictionary with [](xref:Cryville.EEW.TagTypeKey) keys and any values. The keys defined in [](xref:Cryville.EEW.TagTypeKeys) are usually used, and therefore it is recommended to use `using static Cryville.EEW.TagTypeKeys;`. Different keys have different recommended value types. See the API documentation of [](xref:Cryville.EEW.TagTypeKeys) for more details.

For more information, see the API documentation of the [](xref:Cryville.EEW.Features.Feature) class.

A TTS message generator is built with a [builder](builder.md) exported with `[Export(typeof(IBuilder<IGenerator<Feature>>))]`.

## Quantity
It is very common for physical quantities to appear in a feature, such as length, acceleration, etc. The package `Cryville.Measure` provides several types to represent these quantities.

| Type | Usage |
| ---- | ----- |
| [](xref:Cryville.Measure.Quantity) | Represents a value with a unit, uncertainty unspecified |
| [](xref:Cryville.Measure.QuantityInc) | Represents a value with a unit, uncertainty implied |
| [](xref:Cryville.Measure.QuantityUnc) | Represents a value with a unit, uncertainty explicitly specified |
| [Interval](xref:Cryville.Measure.Interval`1)<[](xref:Cryville.Measure.Quantity)> | Represents an interval of quantity |
| [Interval](xref:Cryville.Measure.Interval`1)<[](xref:Cryville.Measure.QuantityInc)> | Represents an interval of quantity with implied uncertainty |

Examples:
- `new Quantity(10.0, Units.Metre)`: 10m
- `new QuantityInc(1.2, 0.05, Units.Metre)`: 1.2m
- `new QuantityInc(1.2, 0.005, Units.Metre)`: 1.20m
- `new QuantityUnc(12.0, 3.4, Units.Metre)`: 12.0±3.4m
- `new QuantityUnc(12.0, 3.4, 5.6, Units.Metre)`: 12.0(-3.4/+5.6)m

For quantities without a unit, use [](xref:Cryville.Measure.Units.Dimensionless) as the unit.
