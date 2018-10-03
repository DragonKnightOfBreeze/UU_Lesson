//接口：配置管理器
//作用：读取系统核心XML配置信息

using System.Collections.Generic;

namespace Kernel {
	public interface IConfigManager {
		/// <summary>属性：应用设置</summary>
		Dictionary<string, string> AppSetting { get; }

		/// <summary>得到AppSetting的最大数量</summary>
		/// <returns></returns>
		int GetAppSettingMaxNumber();
	}
}