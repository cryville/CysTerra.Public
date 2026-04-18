using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	/// <summary>
	/// 震度
	/// </summary>
	public class Intensity {
		public IntensityDetail Forecast { get; set; }
		/// <summary>
		/// 震度の観測
		/// </summary>
		/// <remarks>
		/// <para>震度の観測に関する諸要素を記載する。</para>
		/// </remarks>
		public IntensityDetail Observation { get; set; }
	}

	public class IntensityDetail {
		/// <summary>
		/// コード体系の定義
		/// </summary>
		/// <remarks>
		/// <para>「震度の観測」（Body/Intensity/Observation）以下で使用するコード体系を定義する。使用するコードの種類に応じて子要素 Type が出現し、ここにコード種別を記載する。さらに、Typeの@xpath として、定義したコードを使用する要素の相対的な出現位置を記載する。</para>
		/// </remarks>
		public CodeDefine CodeDefine { get; set; }
		/// <summary>
		/// 最大震度
		/// </summary>
		/// <remarks>
		/// <para>本情報で発表する最大の震度を記載する。</para>
		/// </remarks>
		public string MaxInt { get; set; }
		public string MaxLgInt { get; set; }
		public string LgCategory { get; set; }
		public ForecastInt ForecastInt { get; set; }
		public ForecastInt ForecastLgInt { get; set; }
		public IntensityAppendix Appendix { get; set; }
		/// <summary>
		/// 都道府県
		/// </summary>
		/// <remarks>
		/// <para>都道府県毎の震度の観測状況を記載する。震度を観測した都道府県の数に応じて、本要素が複数出現する。</para>
		/// <para>子要素 Name に都道府県名を記載し、対応するコードを子要素 Code に記載する。対応するコードは、「コード体系の定義」（Body/Intensity/Observation/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// </remarks>
		[XmlElement("Pref")]
		public Collection<IntensityPref> Prefs { get; set; }
	}

	public class ForecastInt {
		public string From { get; set; }
		public string To { get; set; }
	}

	public class IntensityAppendix {
		public int MaxIntChange { get; set; }
		public int MaxLgIntChange { get; set; }
		public int MaxIntChangeReason { get; set; }
		public int MaxLgIntChangeReason { get; set; }
	}

	public class IntensityRegion : NameCodePair {
		public Category Category { get; set; }
		/// <summary>
		/// 最大震度
		/// </summary>
		public string MaxInt { get; set; }
		public string MaxLgInt { get; set; }
		public ForecastInt ForecastInt { get; set; }
		public ForecastInt ForecastLgInt { get; set; }
		public XmlSerializedDateTimeOffset ArrivalTime { get; set; }
		public string Condition { get; set; }
		public string Revise { get; set; }

		public override string ToString() => $"{Name}, MaxInt = {MaxInt}, MaxLgInt = {MaxLgInt}";
	}

	public class IntensityPref : IntensityRegion {
		/// <summary>
		/// 地域
		/// </summary>
		/// <remarks>
		/// <para>地域毎の震度の観測状況を記載する。震度を観測した地域の数に応じて、本要素が複数出現する。</para>
		/// <para>子要素 Name に地域名を記載し、対応するコードを子要素 Code に記載する。対応するコードは、「コード体系の定義」（Body/Intensity/Observation/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// </remarks>
		[XmlElement("Area")]
		public Collection<IntensityArea> Areas { get; set; }
	}

	public class IntensityArea : IntensityRegion {
		[XmlElement("City")]
		public Collection<IntensityCity> Cities { get; set; }
		[XmlElement("IntensityStation")]
		public Collection<IntensityStation> IntensityStations { get; set; }
	}

	public class IntensityCity : IntensityRegion {
		[XmlElement("IntensityStation")]
		public Collection<IntensityStation> IntensityStations { get; set; }
	}

	public class IntensityStation : NameCodePair {
		[XmlElement("Int")]
		public string Intensity { get; set; }
		public string K { get; set; }
		public string LgInt { get; set; }
		[XmlElement("LgIntPerPeriod")]
		public Collection<LgIntPerPeriod> LgIntPerPeriods { get; set; }
		public Sva Sva { get; set; }
		[XmlElement("SvaPerPeriod")]
		public Collection<SvaPerPeriod> SvaPerPeriods { get; set; }
		public string Revise { get; set; }

		public override string ToString() => $"{Name}, Int = {Intensity}, LgInt = {LgInt}";
	}

	public class LgIntPerPeriod {
		[XmlText]
		public string Value { get; set; }
		[XmlAttribute]
		public int PeriodicBand { get; set; }
		[XmlAttribute]
		public float Period { get; set; }
		[XmlAttribute]
		public string PeriodUnit { get; set; }
	}

	public class Sva {
		[XmlText]
		public float Value { get; set; }
		[XmlAttribute("unit")]
		public string Unit { get; set; }
	}

	public class SvaPerPeriod : Sva {
		[XmlAttribute]
		public int PeriodicBand { get; set; }
		[XmlAttribute]
		public float Period { get; set; }
		[XmlAttribute]
		public string PeriodUnit { get; set; }
	}
}
