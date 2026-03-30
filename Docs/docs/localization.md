# Localization
You can provide your extension in multiple cultures (languages). Instead of hardcoding strings, you can write message files for each supported culture and later consume them in your code.

## Message Files
Message files are written in a subdirectory `Messages` within your project directory. The messages for the event source culture should be written in the `und.json` file. For each additional supported culture, add a `.json` file named with the corresponding BCP 47 language tag.

> [!TIP]
> It is recommended to minimize ([remove likely subtags](https://unicode.org/reports/tr35/#Likely_Subtags)) the language tag used as the file name. Enter the language tag below to compute a possible minimized tag.
>
> <input id="lang-input" /><input type="button" value="Minimize" onclick="const o = getElementById('lang-output'); o.innerText = ''; o.innerText = 'Result: ' + new Intl.Locale(getElementById('lang-input').value).minimize().baseName;" />
>
> <span id="lang-output"></span>

The content of a message file is structured as follows.

```json
{
	"Culture": "en-US",
	"Strings": {
		"MyString1": "This is an example message.",
		"MyString2": "...",
		"MyString3": "..."
	},
	"StringSets": {
		"MyStringSet1": {
			"Strings": {
				"MyStringInStringSet": "This is an example message in an example string set."
			}
		}
	}
}
```

The `Culture` field specifies the actual culture of this message file (especially useful for identifying the culture of `und.json`).

## Consume Messages in Code
To consume the messages in your code, use the [](xref:Cryville.EEW.LocalizedResource) class.

```cs
using var lres = new LocalizedResource("", ref culture); // Pass the favored culture by reference. When the constructor returns, it will be set to the actual culture of the messages.
var res = lres.RootMessageStringSet;
string myString1 = res.GetStringRequired("MyString1");
string myStringInStringSet = res.GetStringSetRequired("MyStringSet1").GetStringRequired("MyStringInStringSet");
```

If the favored culture is not known or should not be considered when consuming a message, use the [](xref:Cryville.EEW.LocalizableResource) class instead.

```cs
using var lres = new LocalizableResource("");
var res = lres.RootMessageStringSet;
```

## Localize Components and Properties
You can link messages in the localized resources to assemblies, components (classes), and properties to provide a localized name and description for them. To do that, apply [](xref:Cryville.EEW.ComponentModel.LocalizableDisplayNameAttribute) and [](xref:Cryville.EEW.ComponentModel.LocalizableDescriptionAttribute).

To apply the attributes to an assembly, create a file named `AssemblyInfo.cs` and put the following code.

```cs
using Cryville.EEW.ComponentModel;

// Apply localized display name and description to the assembly (extension)
[assembly:LocalizableDisplayName("$ExtensionName")]
[assembly:LocalizableDescription("$ExtensionDesc")]
```
