//核心层，XML操作类

//使用方法：
//存储：将字符串序列化，然后写入XML
//读取：读取XML，然后反序列化

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

namespace Kernel {
	public class XmlOperation {
		private static XmlOperation _Instance; //静态类对象

		/// <summary>得到实例</summary>
		/// <returns></returns>
		public static XmlOperation GetInstance() {
			if(_Instance == null)
				_Instance = new XmlOperation();
			return _Instance;
		}


		/// <summary>加密方法 描述： 加密和解密采用相同的key,具体值自己填，但是必须为32位 UTF8到Byte到Base64</summary>
		/// <param name="toE"></param>
		/// <returns></returns>
		public string Encrypt(string toE) {
			var keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");
			var rDel = new RijndaelManaged {
				                               Key = keyArray,
				                               Mode = CipherMode.ECB,
				                               Padding = PaddingMode.PKCS7
			                               };
			var cTransform = rDel.CreateEncryptor();
			var toEncryptArray = UTF8Encoding.UTF8.GetBytes(toE);
			var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}

		/// <summary>解密方法 描述： 加密和解密采用相同的key,具体值自己填，但是必须为32位 Base64到Byte到UTF8</summary>
		/// <param name="toD"></param>
		/// <returns></returns>
		public string Decrypt(string toD) {
			var keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");
			var rDel = new RijndaelManaged {
				                               Key = keyArray,
				                               Mode = CipherMode.ECB,
				                               Padding = PaddingMode.PKCS7
			                               };
			var cTransform = rDel.CreateDecryptor();
			var toEncryptArray = Convert.FromBase64String(toD);
			var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			return UTF8Encoding.UTF8.GetString(resultArray);
		}

		/// <summary> 序列化对象 序列化对象</summary>
		/// <param name="pObject">进行序列化的对象</param>
		/// <param name="ty">序列化对象的类型</param>
		/// <returns></returns>
		public string SerializeObject(object pObject, Type ty) {
			string XmlizedString = null;
			var memoryStream = new MemoryStream();
			var xs = new XmlSerializer(ty);
			var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			xs.Serialize(xmlTextWriter, pObject);
			memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
			XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
			return XmlizedString;
		}

		/// <summary> 反序列化对象 反序列化对象</summary>
		/// <param name="pXmlizedString"></param>
		/// <param name="ty"></param>
		/// <returns></returns>
		public object DeserializeObject(string pXmlizedString, Type ty) {
			var xs = new XmlSerializer(ty);
			var memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
			var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			return xs.Deserialize(memoryStream);
		}

		/// <summary> 创建XML文件 创建XML文件</summary>
		/// <param name="fileName">文件名称</param>
		/// <param name="strFileData">写入的文件数据</param>
		/// <param name="isEncrypting">是否进行加密处理（默认不加密）</param>
		public void CreateXML(string fileName, string strFileData, bool isEncrypting = false) {
			StreamWriter writer; //写文件流
			string strWriteFileData;
			if(isEncrypting)
				strWriteFileData = Encrypt(strFileData); //写入的文件数据（加密）
			else
				strWriteFileData = strFileData; //写入的文件数据
			writer = File.CreateText(fileName);
			writer.Write(strWriteFileData);
			writer.Close(); //关闭文件流
		}

		/// <summary> 读取XML文件 读取XML文件</summary>
		/// <param name="fileName">文件名称</param>
		/// <param name="isDecrypting">是否进行解密处理（默认不解密）</param>
		/// <returns></returns>
		public string LoadXML(string fileName, bool isDecryting = false) {
			StreamReader sReader; //读文件流
			string dataString;    //读出的数据字符串

			sReader = File.OpenText(fileName);
			dataString = sReader.ReadToEnd();
			sReader.Close(); //关闭读文件流
			if(isDecryting)
				return Decrypt(dataString); //解密
			return dataString;
		}

		/// <summary>判断是否存在文件</summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool HasFile(String fileName) {
			return File.Exists(fileName);
		}

		/// <summary>UTF8字节数组转字符串</summary>
		/// <param name="characters"></param>
		/// <returns></returns>
		public string UTF8ByteArrayToString(byte[] characters) {
			var encoding = new UTF8Encoding();
			var constructedString = encoding.GetString(characters);
			return constructedString;
		}

		/// <summary>字符串转UTF8字节数组</summary>
		/// <param name="pXmlString"></param>
		/// <returns></returns>
		public byte[] StringToUTF8ByteArray(String pXmlString) {
			var encoding = new UTF8Encoding();
			var byteArray = encoding.GetBytes(pXmlString);
			return byteArray;
		}
	}
}