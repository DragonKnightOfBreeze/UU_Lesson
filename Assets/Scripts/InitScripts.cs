//标题：
//替换代码注释

//注意：这个脚本没有必要执行！！！


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class InitScripts : UnityEditor.AssetModificationProcessor {

//	private const string NAME_Author = "微风的龙骑士";

//	private static void OnWillCreateAsset(string path){
//		path = path.Replace(".meta", "");
//		if (path.EndsWith(".cs")) {
//			string strContent = File.ReadAllText(path);
//			strContent = strContent.Replace("#AuthorName#", "ss")
//				.Replace("#CreateTime#", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//			File.WriteAllText(path,strContent);
//			AssetDatabase.Refresh();
//		}
//	}

//}
