using UnityEngine;

namespace EscapeGame.Building.Config {
	public enum ArchivePkgType {
		[InspectorName("不处理")]
		None = 0,

		[InspectorName("仅压缩Debug和Burst的调试内容")]
		PkgDoNotShip = 1,

		[InspectorName("全压缩")]
		Total = 99,
	}

	[InsConfig("./Assets/Editor/MenuTools/Packing/Config/BuildingCmdEditorConfig.json")]
	internal class BuildingCmdEditorConfig : CmdInsConfigBase<BuildingCmdEditorConfig> {
		/// <summary>
		/// 出包路径目录
		/// </summary>
		public string OutputPkgPath = "../Build/";

		/// <summary>
		/// AB包资源版本
		/// 一般为打包时间(自动),精确到时分,例如:2025-07-30-1350 为2025年7月30日13点50分出的资源包
		/// 除非特殊情况需要完全新的命名方式,此方式手动命名即可
		/// </summary>
		public string ABResVersion = string.Empty;

		/// <summary>
		/// 自动AB包版本
		/// </summary>
		public bool AutoABResVersion = false;

		/// <summary>
		/// App打包完打开对应目录
		/// </summary>
		public bool PackingEndDirectFolder = false;

		/// <summary>
		/// 自动整包压缩(仅PC平台)
		/// </summary>
		public ArchivePkgType AutoArchive = ArchivePkgType.None;

		#region Steam

		// #if STEAM
		/// <summary>
		/// Steam后台配置的AppId
		/// </summary>
		public string SteamAppId = "3639650";

		/// <summary>
		/// Steam后台配置的DepotId
		/// </summary>
		public string SteamDepotId = "3639651";

		/// <summary>
		/// App描述
		/// </summary>
		public string SteamDesc = "";

		/// <summary>
		/// 冗余信息->就当作数字版版本号即可.填写数字
		/// </summary>
		public string SteamVerbose = "1";

		/// <summary>
		/// 内部分支描述,填写本次上传目的即可
		/// </summary>
		public string SteamSetLive = "Internal Test";

		/// <summary>
		/// 上传账号
		/// </summary>
		public string UploadSteamAccount = "atomstringyutouren";

		/// <summary>
		/// 上传账号的密码
		/// </summary>
		public string UploadSteamPwd = "Nicefish2023";
		// #endif

		#endregion
	}
}
