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
using UU_Lesson.Common;
using UU_Lesson.UI;

namespace UU_Lesson.Controller.Scene {
	/// <summary></summary>
	public class Ctrl_LoadingScene : MonoBehaviour {
		/// <summary>UI场景控制器</summary>
		private UIScene_Loading uiLoadingCtrl;
		private const float waitTime = 1f;

		private AsyncOperation _Async;
		/// <summary>当前的进度</summary>
		private int _CurProgress;


		private void Start() {
//			//加载UI场景
//			UISceneMgr.GetInstance().LoadUIScene(UISceneType.UIRoot_LoadingScene);
//			//等待一段时间
//			yield return new WaitForSeconds(waitTime);
			
			uiLoadingCtrl = GameObject.FindWithTag("UIRoot").GetComponent<UIScene_Loading>();
			
			//开始异步加载
			StartCoroutine(Load());
		}


		private void Update() {
			UpdateProcessValue();
		}


		/// <summary>协程：异步加载下一个场景</summary>
		/// <returns></returns>
		private IEnumerator Load() {
			//开始异步加载下一个场景
			_Async = SceneManager.LoadSceneAsync(ScenesMgr.GetInstance().NextSceneType.ToString());
			//只有进度条走到100，才能进入下一个场景
			_Async.allowSceneActivation = false;
			yield return _Async;
			//销毁没有的资源
			Resources.UnloadUnusedAssets();
		}

		/// <summary>
		/// 更新进度
		/// </summary>
		private void UpdateProcessValue() {
			int toProgress = 0;
			if(_Async.progress < 0.9f)
				//Clamp：限制最小值和最大值（也适用于生命值、魔法值等）
				toProgress =Mathf.Clamp((int) _Async.progress * 100,1,100) ;
			else
				toProgress = 100;
			if(_CurProgress < toProgress)
				_CurProgress++;
			else
				_Async.allowSceneActivation = true;
			uiLoadingCtrl.SetProgressValue(_CurProgress * 0.01f);
		}
	}
}