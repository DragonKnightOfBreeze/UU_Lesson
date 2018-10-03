/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 协程的使用示例
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
using System.Collections;
using UnityEngine;

//using Kernel;
//using Global;

namespace Sample {
	/// <summary>协程的使用示例</summary>
	public class CoroutineSample : MonoBehaviour {
		private void Start() {
			//开始一个协程
			//StartCoroutine("Test");
			StartCoroutine(Test());



			//停止一个协程
			StopCoroutine(Test());

			//停止脚本中的所有协程
			StopAllCoroutines();

		}


		private IEnumerator Test() {
			//等待界面渲染完毕
			yield return new WaitForEndOfFrame();


			//等待2s
			yield return new WaitForSeconds(2f);

			//等待2s，使用没有被缩放过的时间
			yield return new WaitForSecondsRealtime(2f);

			//等待从某一链接接收好文件
			yield return new WWW("???");

			//中断等待，相当于没有迭代返回一个值
			//yield break;

		}
	}
}