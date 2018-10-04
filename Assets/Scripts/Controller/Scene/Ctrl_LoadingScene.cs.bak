/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 
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
using UU_Lesson.Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using UU_Lesson.UI;

namespace UU_Lesson.Common {
	/// <summary></summary>
	public class Ctrl_LoadingScene : MonoBehaviour {
		/// <summary>UI场景控制器</summary>
		public GameObject goUIRoot;
		private const float waitTime = 1f;

		private AsyncOperation _Async;
		//当前的进度
		private int _CurProgress;

		private IEnumerator Start() {
			goUIRoot = GameObject.FindWithTag("UIRoot");
			
			//等待一段时间
			yield return new WaitForSeconds(waitTime);
//			//加载UI场景
//			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoadingScene);

			//开始异步加载
			goUIRoot.GetComponent<UIScene_Loading>().SetProgressValue(0);
			StartCoroutine(Load());

			
		}

		
		private void Update() {

			int toProgress = 0;
			if(_Async.progress < 0.9f)
				toProgress = (int) _Async.progress * 100;
			else
				toProgress = 100;
			if(_CurProgress < toProgress)
				++_CurProgress;
			else
				_Async.allowSceneActivation = true;
			goUIRoot.GetComponent<UIScene_Loading>().SetProgressValue(_CurProgress * 0.01f);
		}
		

		/// <summary>协程：异步加载下一个场景</summary>
		/// <returns></returns>
		private IEnumerator Load() {
			//开始异步加载下一个场景
			_Async = SceneManager.LoadSceneAsync(ScenesMgr.GetInstance().NextSceneType.ToString());
			_Async.allowSceneActivation = false;
			yield return _Async;
		}
	}
}