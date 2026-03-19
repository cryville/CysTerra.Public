# TTS Message Generator
CysTerra makes use of TTS (text-to-speech) engines to announce events. TTS message generators are the components responsible for generating TTS messages from events.

A TTS message generator is a class implementing the [IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.TTS.TTSEntry)> interface or the [IContextedGenerator](xref:Cryville.EEW.IContextedGenerator`2)<[](xref:Cryville.EEW.TTS.ITTSMessageGeneratorContext), [](xref:Cryville.EEW.TTS.TTSEntry)> interface. The following is an example implementation.

```cs
using Cryville.Common.Compat;
using Cryville.EEW;
using Cryville.EEW.TTS;
using System;
using System.Globalization;

public class MyTTSMessageGenerator : IContextedGenerator<MyEvent, ITTSMessageGeneratorContext, TTSEntry?> {
	public TTSEntry? Generate(MyEvent e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
		ThrowHelper.ThrowIfNull(e);
		context ??= EmptyTTSMessageGeneratorContext.Instance;

		using var lres = new LocalizedResource("", ref culture);
		var res = lres.RootMessageStringSet;
		return new(culture, res.GetStringRequired("Title"), /* ... */, 0, "eq");
	}
}
```

For more information, see the API documentation of the [](xref:Cryville.EEW.TTS.TTSEntry) class.

A TTS message generator is built with a [builder](builder.md) exported with `[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]`.

## TTS Message Priority
TTS messages are queued based on their [](xref:Cryville.EEW.TTS.TTSEntry.Priority) property values. A lower priority value indicates a higher priority and puts the message more to the front within the queue. Generally, messages for normal events have a priority value `0`, while messages for emergency events have a priority value `-100`.

While a TTS message is being spoken, a new message with higher priority will interrupt the current message and puts it back into the queue, to make way for that new message which will be spoken instead. When an interrupted message is spoken again, an informative text “Repeating the last <*Title*>” will be prepended to the message.

A message will also be interrupted when another message whose corresponding report has its unit keys covering the ones of the report of the current message is enqueued.

## Sounds
An additional sound can be attached to a TTS message ([](xref:Cryville.EEW.TTS.TTSEntry.Sound)). The following is the list of all available sounds.

| Name | Usage |
| ---- | ----- |
| `eew_1` | Earthquake early warning with weak intensity maximum |
| `eew_2` | Earthquake early warning with light intensity maximum |
| `eew_3` | Earthquake early warning with moderate intensity maximum |
| `eew_4` | Earthquake early warning with strong intensity maximum |
| `eew_5` | Earthquake early warning with violent intensity maximum |
| `eew_update` | Update of earthquake early warning |
| `eew_update_final` | Final update of earthquake early warning |
| `eew_update_cancel` | Cancellation of earthquake early warning |
| `eq_a` | Earthquake information automatically produced, with hypocenter but no per-area intensity |
| `eq_i` | Earthquake information with per-area intensity but no hypocenter |
| `eq` | Earthquake information with hypocenter but no per-area intensity |
| `eq_d` | Earthquake information with both hypocenter and per-area intensity |
| `eq_c` | Deletion of earthquake information |
| `ev` | Other information |
