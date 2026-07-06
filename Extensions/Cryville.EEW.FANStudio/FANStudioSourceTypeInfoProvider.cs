using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Cryville.EEW.FANStudio {
	public class FANStudioSourceTypeInfoProvider(IJsonTypeInfoResolver typeInfoResolver) {
		static FANStudioSourceTypeInfoProvider? s_instance;
		public static FANStudioSourceTypeInfoProvider Instance => s_instance ??= new(SerializerContext.Default);

		readonly JsonSerializerOptions _options = new(SerializerContext.Default.Options) { TypeInfoResolver = typeInfoResolver };
		public IJsonTypeInfoResolver TypeInfoResolver { get; } = typeInfoResolver;

		protected virtual IEnumerable<(string Source, FANStudioSource EnumSource, JsonTypeInfo WrappedJsonTypeInfo)> Enumerate() {
			yield return ("weatheralarm", FANStudioSource.CMAWeatherAlarm, SerializerContext.Default.FANStudioDataCMAWeatherAlarm);
			yield return ("tsunami", FANStudioSource.NMEFCTsunamiWarning, SerializerContext.Default.FANStudioDataNMEFCTsunamiWarning);
			yield return ("cenc", FANStudioSource.CENCEarthquake, SerializerContext.Default.FANStudioDataCENCEarthquake);
			// yield return ("cenc-ir", FANStudioSource.CENCIntensityReport, SerializerContext.Default.FANStudioDataCENCIntensityReport);
			yield return ("cea", FANStudioSource.CEAEEW, SerializerContext.Default.FANStudioDataCEAEEW);
			yield return ("cea-pr", FANStudioSource.CEAProvinceEEW, SerializerContext.Default.FANStudioDataCEAProvinceEEW);
			yield return ("ningxia", FANStudioSource.NingxiaEarthquake, SerializerContext.Default.FANStudioDataNingxiaEarthquake);
			yield return ("guangxi", FANStudioSource.GuangxiEarthquake, SerializerContext.Default.FANStudioDataGuangxiEarthquake);
			yield return ("shanxi", FANStudioSource.ShanxiEarthquake, SerializerContext.Default.FANStudioDataShanxiEarthquake);
			yield return ("beijing", FANStudioSource.BeijingEarthquake, SerializerContext.Default.FANStudioDataBeijingEarthquake);
			// yield return ("yunnan", FANStudioSource.YunnanEarthquake, SerializerContext.Default.FANStudioDataYunnanEarthquake);
			// yield return ("cwa", FANStudioSource.CWAEarthquake, SerializerContext.Default.FANStudioDataCWAEarthquake);
			yield return ("cwa-eew", FANStudioSource.CWAEEW, SerializerContext.Default.FANStudioDataCWAEEW);
			yield return ("hko", FANStudioSource.HKOEarthquake, SerializerContext.Default.FANStudioDataHKOEarthquake);
			yield return ("usgs", FANStudioSource.USGSEarthquake, SerializerContext.Default.FANStudioDataUSGSEarthquake);
			yield return ("sa", FANStudioSource.ShakeAlertEEW, SerializerContext.Default.FANStudioDataShakeAlertEEW);
			yield return ("emsc", FANStudioSource.EMSCEarthquake, SerializerContext.Default.FANStudioDataEMSCEarthquake);
			yield return ("bcsf", FANStudioSource.BCSFEarthquake, SerializerContext.Default.FANStudioDataBCSFEarthquake);
			yield return ("gfz", FANStudioSource.GFZEarthquake, SerializerContext.Default.FANStudioDataGFZEarthquake);
			yield return ("usp", FANStudioSource.USPEarthquake, SerializerContext.Default.FANStudioDataUSPEarthquake);
			yield return ("kma", FANStudioSource.KMAEarthquake, SerializerContext.Default.FANStudioDataKMAEarthquake);
			// yield return ("kma-eew", FANStudioSource.KMAEEW, SerializerContext.Default.FANStudioDataKMAEEW);
			yield return ("fssn", FANStudioSource.FSSNCMT, SerializerContext.Default.FANStudioDataFSSNEarthquake);
			// yield return ("fssn-cmt", FANStudioSource.FSSNCMT, SerializerContext.Default.FANStudioDataFSSNCMT);
		}
		bool _init;
		readonly object _initLock = new();
		void Init() {
			if (_init)
				return;
			lock (_initLock) {
				if (_init)
					return;
				foreach (var (source, enumSource, wrappedJsonTypeInfo) in Enumerate()) {
					_mapEnumSourceToSource.Add(enumSource, source);
					var type = wrappedJsonTypeInfo.Type.GetGenericArguments()[0];
					_mapTypeToSource.Add(type, source);
					_typeInfoMap.Add(source, _options.GetTypeInfo(type) ?? throw new InvalidOperationException($"Missing type info for {type}."));
					_wrappedTypeInfoMap.Add(source, wrappedJsonTypeInfo);
				}
				_init = true;
			}
		}
		readonly Dictionary<FANStudioSource, string> _mapEnumSourceToSource = [];
		public bool TryGetSource(FANStudioSource enumSource, [NotNullWhen(true)] out string? source) {
			Init();
			return _mapEnumSourceToSource.TryGetValue(enumSource, out source);
		}
		readonly Dictionary<Type, string> _mapTypeToSource = [];
		public bool TryGetSource(Type type, [NotNullWhen(true)] out string? source) {
			Init();
			return _mapTypeToSource.TryGetValue(type, out source);
		}
		readonly Dictionary<string, JsonTypeInfo> _typeInfoMap = [];
		public bool TryGetTypeInfo(string source, [NotNullWhen(true)] out JsonTypeInfo? jsonTypeInfo) {
			Init();
			return _typeInfoMap.TryGetValue(source, out jsonTypeInfo);
		}
		readonly Dictionary<string, JsonTypeInfo> _wrappedTypeInfoMap = [];
		public bool TryGetWrappedDataTypeInfo(string source, [NotNullWhen(true)] out JsonTypeInfo? jsonTypeInfo) {
			Init();
			return _wrappedTypeInfoMap.TryGetValue(source, out jsonTypeInfo);
		}
	}
}
