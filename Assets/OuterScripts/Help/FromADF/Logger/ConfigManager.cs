//核心层，配置管理器
//作用：读取系统核心XML配置信息

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq; //XDocument的命名空间
using System.IO;

namespace Kernel {
	public class ConfigManager : IConfigManager {
		//定义应用设置集合
		private static Dictionary<string, string> _AppSetting;

		/// <summary>属性：应用设置</summary>
		public Dictionary<string, string> AppSetting => _AppSetting;

		/// <summary>配置管理器构造函数</summary>
		/// <param name="logPath">日志的路径</param>
		/// <param name="rootNodeName">XML根节点名称</param>
		public ConfigManager(string logPath, string rootNodeName) {
			_AppSetting = new Dictionary<string, string>();
			//初始化解析XML数据，到集合_AppSetting中
			InitAndAnalysisXML(logPath, rootNodeName);
		}


		/// <summary>得到AppSetting的最大数量</summary>
		/// <returns></returns>
		public int GetAppSettingMaxNumber() {
			if(_AppSetting != null && _AppSetting.Count >= 0)
				return AppSetting.Count;
			return 0;
		}


		/// <summary>初始化解析XML数据，到集合_AppSetting中</summary>
		/// <param name="logPath">日志的路径</param>
		/// <param name="rootNodeName">XML根节点名称</param>
		private void InitAndAnalysisXML(string logPath, string rootNodeName) {
			//参数检查
			if(string.IsNullOrEmpty(logPath) || string.IsNullOrEmpty(rootNodeName))
				return;
			XDocument xmlDoc;    //代表XML文档
			XmlReader xmlReader; //XML读写器

			try {
				xmlDoc = XDocument.Load(logPath);                                  //加载日志路径
				xmlReader = XmlReader.Create(new StringReader(xmlDoc.ToString())); //创建XML读写器
			}
			catch {
				//应该自定义一个专有异常
				//throw new System.Exception(GetType() + "/InitAndAnalysisXML() " + "XML Analysis Exception!");
				//抛出自定义的异常
				throw new XMLAnalysisException(GetType() + "/InitAndAnalysisXML() " + "XML Analysis Exception!");
			}

			//循环解析XML
			while(xmlReader.Read()) //XML读写器从指定根节点开始读写
				if(xmlReader.IsStartElement() && xmlReader.LocalName == rootNodeName)
					using(var xmlReaderItem = xmlReader.ReadSubtree()) {
						while(xmlReaderItem.Read()) //如果是节点的元素
							if(xmlReaderItem.NodeType == XmlNodeType.Element) {
								var strNode = xmlReaderItem.Name;
								//读XML当前行的下一个内容
								xmlReaderItem.Read();

								//如果是节点内容
								if(xmlReaderItem.NodeType == XmlNodeType.Text)
									_AppSetting[strNode] = xmlReaderItem.Value;
							}
					}

		}
	}
}