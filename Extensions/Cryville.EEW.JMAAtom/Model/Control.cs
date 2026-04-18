#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	/// <summary>
	/// 管理部
	/// </summary>
	public class Control {
		/// <summary>
		/// 情報名称
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 発表時刻
		/// </summary>
		public XmlSerializedDateTimeOffset DateTime { get; set; }
		/// <summary>
		/// 運用種別（「通常」、「訓練」、「試験」など）
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 編集官署名
		/// </summary>
		public string EditorialOffice { get; set; }
		/// <summary>
		/// 発表官署名
		/// </summary>
		public string PublishingOffice { get; set; }
	}
}
