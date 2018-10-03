/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 单例的实现实例
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 
 */
using UnityEngine;

//using Kernel;
//using Global;

namespace Sample {
	/// <summary>单例的实现示例</summary>
	public class SingletonSample : MonoBehaviour {
		#region ［1.对于唯一脚本，直接获得单例，只能存在于唯一物体上］

		public static SingletonSample Instance1;

		private void Awake() {
			Instance1 = this;
		}

		#endregion


		#region ［2.对于普通类/脚本，间接获得单例］

		private static SingletonSample _Instance2;

		public static SingletonSample GetInstance2() {
			if(_Instance2 == null)
				_Instance2 = new SingletonSample();
			return _Instance2;
		}

		#endregion


		#region ［3.对于特定脚本，动态创建对象并挂载］

		private static SingletonSample _Instance3;

		public static SingletonSample GetInstance3() {
			if(_Instance3 == null)
				_Instance3 = new GameObject("_" + nameof(SingletonSample)).AddComponent<SingletonSample>();
			return _Instance3;
		}

		#endregion


		#region ［4.对于特殊类/脚本，保证线程安全，脚本延迟执行］

		//禁止创建内存cache
		private static volatile SingletonSample _Instance4;
		//静态对象可以作为一个锁
		protected static readonly object _StaticLock = new object();

		public static SingletonSample GetInstance4 {
			get {
				if(_Instance4 == null)
					lock(_StaticLock) {
						if(_Instance4 == null)
							_Instance4 = new SingletonSample();
					}
				return _Instance4;
			}
		}

		#endregion
	}
}