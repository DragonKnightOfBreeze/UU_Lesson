//核心层，核心层的参数列表

using UnityEngine;

namespace Kernel {
	public class KernelParameter {
		#region 旧的方式（使用预编译指令）

		/* 

		#if UNITY_STANDALONE_WIN

			//系统配置信息中的日志路径（只读）
			internal static readonly string SystemConfigInfo_LogPath = "file:///" + Application.dataPath + "/StreamingAssets/SystemConfigInfo.xml";
			//系统配置信息中的日志根节点名称
			internal static readonly string SystemConfigInfo_RootNodeName = "SystemConfigInfo";

			//对话系统XML的路径（只读）
			internal static readonly string DialogConfig_Path = "file:///" + Application.dataPath + "/StreamingAssets/SystemDialogsInfo.xml";
			//对话系统XML的根节点名称
			internal static readonly string DialogConfig_RootNodeName = "Dialogs_CN";

		#elif UNITY_ANDROID

			//系统配置信息中的日志路径（只读）
			internal static readonly string SystemConfigInfo_LogPath = Application.dataPath + "!/Assets/SystemConfigInfo.xml";
			//系统配置信息中的日志根节点名称
			internal static readonly string SystemConfigInfo_RootNodeName= "SystemConfigInfo";

			//对话系统XML的路径（只读）
			internal static readonly string DialogConfig_Path =  Application.dataPath + "!/Assets/SystemDialogsInfo.xml";
			//对话系统XML的根节点名称
			internal static readonly string DialogConfig_RootNodeName = "Dialogs_CN";

		#elif UNITY_IPHONE

			//系统配置信息中的日志路径（只读）
			internal static readonly string SystemConfigInfo_LogPath = Application.dataPath + "/Raw/SystemConfigInfo.xml";
			//系统配置信息中的日志根节点名称
			internal static readonly string SystemConfigInfo_RootNodeName= "SystemConfigInfo";

			//对话系统XML的路径（只读）
			internal static readonly string DialogConfig_Path = Application.dataPath + "/Raw/SystemDialogsInfo.xml";
			//对话系统XML的根节点名称
			internal static readonly string DialogConfig_RootNodeName = "Dialogs_CN";

		#endif

		*/

		#endregion


		#region 有待重构的方式（使用动态持久化资源地址）

		/// <summary>得到系统日志路径 动态设置，不使用预编译指令</summary>
		/// <returns></returns>
		public static string GetLogPath() {
			string logPath = null;

			//如果是安卓或者Iphone环境
			if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				logPath = Application.streamingAssetsPath + "/SystemConfigInfo.xml";
			//如果是Win环境
			else
				logPath = "file://" + Application.streamingAssetsPath + "/SystemConfigInfo.xml";
			return logPath;
		}

		/// <summary>得到系统日志的根节点名称</summary>
		/// <returns></returns>
		public static string GetLogRootNodeName() {
			string rootNodeName = null;
			rootNodeName = "SystemConfigInfo";
			return rootNodeName;
		}

		/// <summary>得到对话配置路径</summary>
		/// <returns></returns>
		public static string GetDialogConfigPath() {
			string logPath = null;

			//如果是安卓或者Iphone环境
			if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				logPath = Application.streamingAssetsPath + "/SystemDialogsInfo.xml";
			//如果是Win环境
			else
				logPath = "file://" + Application.streamingAssetsPath + "/SystemDialogsInfo.xml";
			return logPath;
		}

		/// <summary>得到对话配置的根节点名称</summary>
		/// <returns></returns>
		public static string GetDialogConfigRootNodeName() {
			string rootNodeName = null;
			rootNodeName = "Dialogs_CN";
			return rootNodeName;
		}

		#endregion
	}
}