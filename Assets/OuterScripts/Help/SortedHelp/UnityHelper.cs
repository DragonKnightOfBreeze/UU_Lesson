/*******
 * ［前置］
 * 帮助脚本
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * Unity帮助类
 * 
 * ［功能］
 * 提供程序用户一些常用的功能方法的实现，方便程序员快速开发
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//using Kernel;
//using Global;

namespace SortedHelp {
	/// <summary>Unity帮助类</summary>
	public class UnityHelper : MonoBehaviour {
		private static float floDeltaTime;

		/// <summary>递归查找子节点对象</summary>
		/// <param name="goParent">父对象</param>
		/// <param name="childName">指定的子对象名字</param>
		/// <returns>要查找的子对象的方位</returns>
		public static Transform FindChildNode(GameObject goParent, string childName) {
			//TODO 算法可能还有问题
			//查找直接的子节点
			var searchTra = goParent.transform.Find(childName);

			//如果还没有查找到
			if(searchTra == null)
				foreach(Transform tra in goParent.transform)
					if(goParent.transform.childCount != 0) {
						searchTra = FindChildNode(tra.gameObject, childName);
						if(searchTra && searchTra.name == childName)
							return searchTra;

					}
			return searchTra;
		}

		/// <summary>递归查找多个子节点对象（同一目录下，同名）</summary>
		/// <param name="goParent">父对象</param>
		/// <param name="childName">指定的子对象名字</param>
		/// <returns>要查找的子对象的方位</returns>
		public static Transform[] FindChildNodes(GameObject goParent, string childName) {
			var tfsList = new List<Transform>();

			var tf = FindChildNode(goParent, childName);
			var tfs = tf.parent.GetComponentsInChildren<Transform>();
			foreach(var tran in tfs)
				if(tran.name == childName)
					tfsList.Add(tran);

			var tfsResult = new Transform[tfsList.Count];
			foreach(var tran in tfsList)
				tfsResult.Append(tran);
			return tfsResult;
			//TODO
		}


		/// <summary>获取子节点的脚本</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="goParent">父对象</param>
		/// <param name="childName">指定的子对象名称</param>
		/// <returns></returns>
		public static T GetComponentInChildNode<T>(GameObject goParent, string childName)
			where T : Component {
			//查找指定的子节点
			var searchTra = FindChildNode(goParent, childName);
			if(searchTra != null)
				return searchTra.gameObject.GetComponent<T>();
			return null;
		}


		/// <summary>向子节点添加脚本</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="goParent"></param>
		/// <param name="childName"></param>
		/// <returns></returns>
		public static T AddComponentToChildNode<T>(GameObject goParent, string childName)
			where T : Component {
			//查找指定的子节点
			var searchTra = FindChildNode(goParent, childName);
			//如果查找成功，则考虑是否已经有相同的脚本，否则直接添加
			if(searchTra != null) {
				//如果已经有相同的脚本，则删除
				var componentArray = searchTra.GetComponents<T>();
				foreach(var script in componentArray)
					Destroy(script);
				return searchTra.gameObject.AddComponent<T>();
			}
			//如果查找不成功，则返回null
			return null;
		}

		/// <summary>向多个子节点添加脚本（同一目录下，同名）</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="goParent"></param>
		/// <param name="childName"></param>
		public static void AddComponentToChildNodes<T>(GameObject goParent, string childName)
			where T : Component {
			var tfs = FindChildNodes(goParent, childName);
			if(tfs != null && tfs.Length > 0)
				foreach(var tran in tfs) {
					//如果已经有相同的脚本，则删除
					var componentArray = tran.GetComponents<T>();
					foreach(var script in componentArray)
						Destroy(script);
					tran.gameObject.AddComponent<T>();
				}

		}

		/// <summary>为子节点设置父节点</summary>
		/// <param name="parent">父对象的方位</param>
		/// <param name="child">子对象的方位</param>
		public static void AddParentToChildNode(Transform parent, Transform child) {
			child.SetParent(parent, false); //按照局部坐标
			child.localPosition = Vector3.zero;
			child.localScale = Vector3.one;
			child.localEulerAngles = Vector3.zero;
		}


		/// <summary>间隔指定时间段，返回bool值</summary>
		/// <param name="smallIntervalTime">指定的时间段间隔（0.1-3f秒之间）</param>
		/// <returns>true：表明指定的时间段到了</returns>
		public static bool GetSmallTime(float smallIntervalTime) {
			floDeltaTime += Time.deltaTime;
			//到了指定时间就归零，并且返回true 
			if(floDeltaTime >= smallIntervalTime) {
				floDeltaTime = 0;
				return true;
			}
			return false;
		}


		/// <summary>（角色）面向指定目标旋转 适用于所有人形角色</summary>
		/// <param name="self">本身</param>
		/// <param name="goal">目标</param>
		/// <param name="rotationSpeed">旋转速度</param>
		public void FaceToGoal(Transform self, Transform goal, float rotationSpeed) {
			self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(new Vector3(goal.position.x, 0, goal.position.z) - new Vector3(self.position.x, 0, self.position.z)), rotationSpeed);
		}


		/// <summary>得到指定范围的随机整数</summary>
		/// <param name="minNum"></param>
		/// <param name="maxNum"></param>
		/// <returns></returns>
		public int GetRandomNum(int minNum, int maxNum) {
			if(minNum == maxNum)
				return minNum;
			var num = Random.Range(minNum, maxNum + 1);
			return num;
		}

		/// <summary>得到波动范围内的随机数</summary>
		/// <returns></returns>
		public int GetFloatNum(int num, int downNum = 0, int upNum = 0) {
			num = Random.Range(num - upNum, num + upNum + 1);
			return num;
		}

		/// <summary>根据标签，确定一个游戏对象的基游戏对象</summary>
		/// <param name="tra"></param>
		/// <param name="baseTag"></param>
		/// <returns></returns>
		public GameObject SelectGOWhithTag(Transform tra, string baseTag) {
			var baseTra = tra.parent;
			while(baseTra.gameObject.tag != baseTag)
				baseTra = baseTra.parent;
			return baseTra.gameObject;
		}
	}
}