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
using UnityEngine;

//using Kernel;
//using Global;

namespace UU_LessonTest {
	/// <summary></summary>
	public class TestSceneCtrl : MonoBehaviour {
		/// <summary>创建箱子的区域</summary>
		[SerializeField]
		private Transform transCreateBox;

		/// <summary>箱子的父物体</summary>
		[SerializeField]
		private Transform boxParent;

		/// <summary>箱子的预设</summary>
		private GameObject m_BoxPrefab;

		private int m_CurCount;
		private readonly int m_MaxCount = 10;

		private float m_NextCloneTime;

		private void Start() {
			m_BoxPrefab = Resources.Load("RolePrefab/Item/xianzi") as GameObject;
			if(m_BoxPrefab != null)
				Debug.Log("m_BoxPrefab=" + m_BoxPrefab.name);
		}

		private void Update() {
			if(m_CurCount < m_MaxCount)
				if(Time.time > m_NextCloneTime) {
					m_NextCloneTime = Time.time + 3f;
					//克隆
					var goClone = Instantiate(m_BoxPrefab);
					goClone.transform.parent = boxParent;
					goClone.transform.position = transCreateBox.TransformPoint(
					                                                           new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));

					goClone.GetComponent<BoxCtrl2>().OnHit = BoxHit;


					m_CurCount++;
				}
		}

		private void BoxHit(GameObject go) {
			--m_CurCount;
			Destroy(go);
		}
	}
}