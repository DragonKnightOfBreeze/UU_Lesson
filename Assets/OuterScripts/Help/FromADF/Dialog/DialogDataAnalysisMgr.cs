//核心层，XML对话系统数据解析管理器脚本
//功能：对于对话XML文件，做数据解析

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

namespace Kernel {
	internal class DialogDataAnalysisMgr : MonoBehaviour {
		private static DialogDataAnalysisMgr _Instance;           //本类的实例（单例）
		private readonly List<DialogDataFormat> _DialogDataArray; //对话数据集合
		private string _StrXMLPath;                               //XML文件路径
		private string _StrXMLRootNodeName;                       //XML根节点名称

		private const float DELAY_TIME = 0.1f; //延迟时间

		private const string XML_ATTR_1 = "SectionNum";
		private const string XML_ATTR_2 = "SectionName";
		private const string XML_ATTR_3 = "Index";
		private const string XML_ATTR_4 = "Side";
		private const string XML_ATTR_5 = "Person";
		private const string XML_ATTR_6 = "Content";


		/// <summary>构造函数</summary>
		private DialogDataAnalysisMgr() {
			_DialogDataArray = new List<DialogDataFormat>();
		}


		/// <summary>得到本类实例</summary>
		/// <returns></returns>
		public static DialogDataAnalysisMgr GetInstance() {
			if(_Instance == null)
				_Instance = new GameObject("DialogDataAnalysisMgr").AddComponent<DialogDataAnalysisMgr>();
			return _Instance;
		}


		/// <summary>设置XML路径与根节点名称</summary>
		/// <param name="xmlPath"></param>
		/// <param name="xmlRootNodeName"></param>
		public void SetXMLPathAndRooNodeName(string xmlPath, string xmlRootNodeName) {
			//参数检查
			if(!string.IsNullOrEmpty(xmlPath) && !string.IsNullOrEmpty(xmlRootNodeName)) {
				_StrXMLPath = xmlPath;
				_StrXMLRootNodeName = xmlRootNodeName;
			}
		}


		/// <summary>得到本脚本数据集合</summary>
		/// <returns></returns>
		public List<DialogDataFormat> GetAllXmlDataArray() {
			if(_DialogDataArray != null && !string.IsNullOrEmpty(_StrXMLRootNodeName))
				return _DialogDataArray;
			return null;

		}

		//

		private IEnumerator Start() {
			//需要等待XML路径与XML根节点名称，进行赋值
			yield return new WaitForSeconds(DELAY_TIME);
			if(!string.IsNullOrEmpty(_StrXMLPath) && !string.IsNullOrEmpty(_StrXMLRootNodeName))
				StartCoroutine("ReadXMLConfigByWWW");
			else
				Debug.LogError(GetType() + "/Start()" + "/t空参数错误！");

		}


		/// <summary>读取xml配置文件（使用WWW）</summary>
		/// <returns></returns>
		private IEnumerator ReadXMLConfigByWWW() {
			var www = new WWW(_StrXMLPath);
			while(www.isDone) { //如果下载完毕
				//初始化XML配置
				InitXMLConfig(www, _StrXMLRootNodeName);
				yield return www;
			}
		}


		/// <summary>初始化XML文档配置</summary>
		/// <param name="www"></param>
		/// <param name="rootNodeName"></param>
		private void InitXMLConfig(WWW www, string rootNodeName) {

			//参数检查
			if(_DialogDataArray == null || string.IsNullOrEmpty(www.text)) {
				Debug.LogError(GetType() + "/InitXMLConfig()" + "\t空参数异常");
				return;
			}

			//XML解析程序
			var xmlDoc = new XmlDocument();
			//发现这种方式，发布到Android手机端，不能正确输出中文
			//xmlDoc.LoadXml(www.text);			//读取XML文档

			/* 使用以下四行代码，来代替上面注释掉的内容，解决正确输出中文的问题 */
			var stringReader = new System.IO.StringReader(www.text);
			stringReader.Read();                            //用于跳过首行？
			var xmlReader = XmlReader.Create(stringReader); //这到底有什么用？
			xmlDoc.LoadXml(stringReader.ReadToEnd());

			//选择单个结点
			var nodes = xmlDoc.SelectSingleNode(rootNodeName).ChildNodes;
			foreach(XmlElement xe in nodes) {
				//实例化“XML解析实体类”
				var data = new DialogDataFormat {

					                                /* 得到属性 */
					                                DiaSectionNum = Convert.ToInt32(xe.GetAttribute(XML_ATTR_1)),
					                                DiaSectionName = xe.GetAttribute(XML_ATTR_2),
					                                DiaIndex = Convert.ToInt32(xe.GetAttribute(XML_ATTR_3)),
					                                DiaSide = xe.GetAttribute(XML_ATTR_4),
					                                DiaPerson = xe.GetAttribute(XML_ATTR_5),
					                                DiaContent = xe.GetAttribute(XML_ATTR_6)
				                                };

				//写入缓存数组
				_DialogDataArray.Add(data);
			}
		}
	}
}