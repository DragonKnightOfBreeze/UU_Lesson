//核心层，日志调试系统（Log日志）（静态类）
//作用：更方便于软件（游戏）开发人员，调试系统程序
//作用：快速定位可能出错的源代码，利于调试
//具体功能：
//		输出目录定制化
//		输出信息定制化（按照等级）
//基本实现原理：
//1：把开发人员在代码中定义的调试语句，写入本日志的缓存
//2：当缓存中的数量超过定义的最大写入文件数值，则把缓存内容调试语句一次性写入文本文件

using System.Collections.Generic;
using System;    //C#的核心命名空间
using System.IO; //文件读写命名空间

//多线程命名空间


namespace Kernel {
	public static class Log {
		#region 【核心字段】

		/// <summary>Log日志缓存数据</summary>
		private static readonly List<string> _LogArray;
		/// <summary>Log日志路径</summary>
		private static readonly string _LogPath;
		/// <summary>Log日志部署模式</summary>
		private static readonly State _LogState;
		/// <summary>Log日志的最大容量</summary>
		private static readonly int _LogMaxCapacity;
		/// <summary>Log日志的最大缓存数量</summary>
		private static readonly int _LogMaxCacheNumber;

		#endregion


		#region 【临时字段定义】

		private static readonly string strLogState;       //日志状态（部署模式）
		private static readonly string strLogMaxCapacity; //日志最大容量
		private static readonly string strLogCacheNumber; //日志缓存最大容量

		#endregion


		#region 【日志文件的常量定义】

		//XML标签
		private const string LOG_LogPath = "LogPath";
		private const string LOG_LogState = "LogState";
		private const string LOG_LogMaxCapacity = "LogMaxCapacity";
		private const string LOG_LogMaxCacheNumber = "LogMaxCacheNumber";

		//日志状态部署模式
		private const string LOG_LogState_Develop = "Develop";
		private const string LOG_LogState_Special = "Special";
		private const string LOG_LogState_Deploy = "Deploy";
		private const string LOG_LogState_Stop = "Stop";

		//其他常量
		private const int LOG_DEFAULT_MaxCapacity = 2000;
		/// 日志默认最大容量
		private const int LOG_DEFAULT_NumMaxCacheNumber = 1; // 日志缓存默认最大容量

		//日志提示信息
		private const string LOG_TIPS = "*** Important !!! *** \n";

		#endregion


		/// <summary>静态构造函数</summary>
		static Log() {

			//日志缓存数据
			_LogArray = new List<string>();

			////日志文件路径
			//IConfigManager configMgr = new ConfigManager(KernelParameter.SystemConfigInfo_LogPath, KernelParameter.SystemConfigInfo_RootNodeName);
			//_LogPath = configMgr.AppSetting[LOG_LogPath];

#if UNITY_STANDALONE_WIN || UNITY_EDITOR

			//日志文件路径
			IConfigManager configMgr = new ConfigManager(KernelParameter.GetLogPath(), KernelParameter.GetLogRootNodeName());
			_LogPath = configMgr.AppSetting[LOG_LogPath];
			//日志状态
			strLogState = configMgr.AppSetting[LOG_LogState];
			//日志的最大容量
			strLogMaxCapacity = configMgr.AppSetting[LOG_LogMaxCapacity];
			//日志缓存的最大容量
			strLogCacheNumber = configMgr.AppSetting[LOG_LogMaxCacheNumber];

#endif

			if(string.IsNullOrEmpty(_LogPath))
				_LogPath = UnityEngine.Application.persistentDataPath + @"\\ADFLog.txt"; //到底是哪种斜线？

			//日志状态
			//string strLogState = configMgr.AppSetting[LOG_LogState];
			if(!string.IsNullOrEmpty(strLogState))
				switch(strLogState) {
					case LOG_LogState_Develop:
						_LogState = State.Develop;
						break;
					case LOG_LogState_Special:
						_LogState = State.Special;
						break;
					case LOG_LogState_Deploy:
						_LogState = State.Deploy;
						break;
					case LOG_LogState_Stop:
						_LogState = State.Stop;
						break;
				}
			else
				_LogState = State.Stop;

			//日志的最大容量
			//string strLogMaxCapacity = configMgr.AppSetting[LOG_LogMaxCapacity];
			if(!string.IsNullOrEmpty(strLogMaxCapacity))
				_LogMaxCapacity = Convert.ToInt32(strLogMaxCapacity);
			else
				_LogMaxCapacity = LOG_DEFAULT_MaxCapacity; //

			//日志缓存的最大容量
			//string strLogCacheNumber = configMgr.AppSetting[LOG_LogMaxCacheNumber];
			if(!string.IsNullOrEmpty(strLogCacheNumber))
				_LogMaxCacheNumber = Convert.ToInt32(strLogCacheNumber);
			else
				_LogMaxCacheNumber = LOG_DEFAULT_NumMaxCacheNumber; //

#if UNITY_STANDALONE_WIN || UNITY_EDITOR

			//创建文件
			if(!File.Exists(_LogPath))
				File.Create(_LogPath);

			//把日志文件中的数据同步到日志缓存中
			SyncFileDataToLogArray();

#endif
		}


		#region 【基本方法】

		/// <summary>把日志文件中的数据同步到日志缓存汇总</summary>
		private static void SyncFileDataToLogArray() {
			if(!string.IsNullOrEmpty(_LogPath)) {
				var sr = new StreamReader(_LogPath); //创建流读取器
				while(sr.Peek() >= 0)
					_LogArray.Add(sr.ReadLine());
				sr.Close();
			}
		}


		/// <summary>写数据到文件中 （调试中需要主动调用）</summary>
		/// <param name="writeFileData">写入的调试信息</param>
		/// <param name="level">重要等级级别（默认为Low）</param>
		public static void Write(string writeFileData, Level level = Level.Low) {
			//参数检查
			if(_LogState == State.Stop)
				return;

			//如果日志缓存数量超过指定容量，则清空
			if(_LogArray.Count >= _LogMaxCapacity)
				_LogArray.Clear(); //清空缓存中的数据

			//参数检查
			if(!string.IsNullOrEmpty(writeFileData)) {
				//增加日期和时间
				writeFileData = "Log State:" + "\t" +
				                _LogState + "/" + "\t" +
				                DateTime.Now + "/" + "\t" +
				                writeFileData;
				//对于不同的日志状态，分特定清新写入文件
				if(level == Level.High)
					writeFileData = LOG_TIPS + writeFileData;

				//对于各种日志模式的处理
				switch(_LogState) {
					case State.Develop: //开发状态
						//追加调试信息，写入文件
						AppendDataToFile(writeFileData);
						break;
					case State.Special: //指定状态
						if(level == Level.High || level == Level.Special)
							AppendDataToFile(writeFileData);
						break;
					case State.Deploy: //部署状态
						if(level == Level.High)
							AppendDataToFile(writeFileData);
						break;
				}
			}
		}

		///// <summary>
		///// Write方法的重载，默认的重要等级级别
		///// </summary>
		///// <param name="writeFileData"></param>
		//public static void Write(string writeFileData) {
		//	Write(writeFileData, Level.Low);
		//}


		/// <summary>追加数据到文件</summary>
		/// <param name="writeFileData"></param>
		private static void AppendDataToFile(string writeFileData) {
			//参数检查，写入缓存集合
			if(!string.IsNullOrEmpty(writeFileData))
				_LogArray.Add(writeFileData);

			//每次缓存集合数量超过一定数量_LogMaxCacheNumber，进行同步
			if(_LogArray.Count % _LogMaxCacheNumber == 0)
				SyncLogArrayToFile();
		}

		#endregion


		#region  【重要管理方法】

		/// <summary>查询日志缓存中的所有数据</summary>
		/// <returns></returns>
		public static List<string> SearchCacheData() {
			if(_LogArray != null)
				return _LogArray;
			return null;
		}


		/// <summary>清楚实体日志文件与日志缓存中的所有数据</summary>
		public static void ClearLogFileAndCacheData() {
			if(_LogArray != null)
				_LogArray.Clear(); //清空数据
			SyncLogArrayToFile();  //缓存中没有数据，同步后相当于清空
		}


		/// <summary>同步缓存集合到实体文件中</summary>
		public static void SyncLogArrayToFile() {
			if(!string.IsNullOrEmpty(_LogPath)) {
				var sw = new StreamWriter(_LogPath);
				//遍历缓存集合，写入集合元素到实体文件
				foreach(var item in _LogArray)
					sw.WriteLine(item);
				sw.Close();
			}
		}

		#endregion


		#region 【本类的枚举类型】

		/// <summary>日志状态（部署模式）</summary>
		public enum State {
			/// <summary>开发模式（输出所有日志内容）</summary>
			Develop,
			/// <summary>指定输出模式</summary>
			Special,
			/// <summary>部署模式（只输出最核心的日志信息）</summary>
			Deploy,
			/// <summary>停止输出模式（不输出任何日志信息）</summary>
			Stop
		}

		/// <summary>调试信息的等级（表示调试信息本身的重要程度）</summary>
		public enum Level {
			High,
			Special,
			Low
		}

		#endregion
	}
}