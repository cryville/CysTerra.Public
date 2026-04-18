using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public abstract class CoordinateBase : TypedValue<string> {
		[XmlAttribute("datum")]
		public string Datum { get; set; }
		[XmlAttribute("condition")]
		public string Condition { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }
	}

	public partial class Coordinate : CoordinateBase {
		bool _parsed;

		double m_latitude;
		public double Latitude {
			get {
				Parse();
				return m_latitude;
			}
		}

		double m_longitude;
		public double Longitude {
			get {
				Parse();
				return m_longitude;
			}
		}

		double? m_height;
		public double? Height {
			get {
				Parse();
				return m_height;
			}
		}

		string m_reference;
		public string Reference {
			get {
				Parse();
				return m_reference;
			}
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^([N\+\-S])(\d+)(\.\d+)?([W\-\+E])(\d+)(\.\d+)?(([\-\+])(\d+)(\.\d+)?)?(CRS(.+))?/?$", RegexOptions.IgnoreCase)]
		private static partial Regex ISO6709();
#else
		static readonly Regex r_ISO6709 = new(@"^([N\+\-S])(\d+)(\.\d+)?([W\-\+E])(\d+)(\.\d+)?(([\-\+])(\d+)(\.\d+)?)?(CRS(.+))?/?$", RegexOptions.IgnoreCase);
		static Regex ISO6709() => r_ISO6709;
#endif
		void Parse() {
			if (_parsed) return;
			var m = ISO6709().Match(Value);
			if (!m.Success) throw new FormatException("Invalid ISO 6709 coordinate.");
			ParseComponent(m.Groups[1].Value is "N" or "n" or "+", m.Groups[2].Value, m.Groups[3].Value, 2, out m_latitude);
			ParseComponent(m.Groups[4].Value is "E" or "e" or "+", m.Groups[5].Value, m.Groups[6].Value, 3, out m_longitude);
			if (m.Groups[7].Success) m_height = double.Parse(m.Groups[7].Value, CultureInfo.InvariantCulture);
			if (m.Groups[12].Success) m_reference = m.Groups[12].Value;
			_parsed = true;
		}
		static void ParseComponent(bool isPositive, string ip, string dp, int bp, out double result) {
			result = (ip.Length - bp) switch {
				0 => double.Parse(ip + dp, CultureInfo.InvariantCulture),
				2 => int.Parse(ip[..^2], CultureInfo.InvariantCulture)
					+ double.Parse(ip[^2..] + dp, CultureInfo.InvariantCulture) / 60d,
				4 => int.Parse(ip[..^2], CultureInfo.InvariantCulture)
					+ int.Parse(ip[^2..^4], CultureInfo.InvariantCulture) / 60d
					+ double.Parse(ip[^4..] + dp, CultureInfo.InvariantCulture) / 3600d,
				_ => throw new FormatException("Invalid ISO 6709 coordinate."),
			};
			if (!isPositive) result = -result;
		}

		public override string ToString() => $"{Latitude}, {Longitude}, {Height}, {Reference}";
	}

	public class Polygon : CoordinateBase {
		Coordinate[] m_coordinates;
		public IReadOnlyList<Coordinate> Coordinates => m_coordinates ??=
			[.. Value.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(c => new Coordinate { Value = c })];

		public override string ToString() => string.Join("; ", Coordinates);
	}
}
