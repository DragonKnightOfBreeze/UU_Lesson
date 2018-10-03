//核心层， 音频管理类

//功能：   
//项目中音频剪辑统一管理。

//注意：
//需要添加3个空的AudioSource组件，分别存放背景音乐、音效1、音效2（播放时才确定赋值）

//音频剪辑数组：用于实现播放方法重载，放入的音频剪辑，可以通过音频剪辑的名字来播放，不需要提供路径
//3个（或更多）空的音频资源（音频源数组）：用于实现音频剪辑的便捷播放，
//背景音乐只能同时播放一个，且重复播放。音效不重复播放

//如果不需要利用音频剪辑的名字访问，完全可以不挂载。


using UnityEngine;
using System.Collections.Generic; //泛型集合命名空间

namespace Kernel {
	public class AudioManager : MonoBehaviour {
		public AudioClip[] AudioClipArray;               //剪辑数组
		public static float AudioBackgroundVolumns = 1F; //背景音量
		public static float AudioEffectVolumns = 1F;     //音效音量

		private static Dictionary<string, AudioClip> _DicAudioClipLib; //音频库
		private static AudioSource[] _AudioSourceArray;                //音频源数组（实现重载）
		private static AudioSource _AudioSource_BackgroundAudio;       //背景音乐
		private static AudioSource _AudioSource_AudioEffect_A;         //音效源A
		private static AudioSource _AudioSource_AudioEffect_B;         //音效源B

		/// <summary>音效库资源加载</summary>
		private void Awake() {
			//音频库加载
			_DicAudioClipLib = new Dictionary<string, AudioClip>();
			foreach(var audioClip in AudioClipArray)
				_DicAudioClipLib.Add(audioClip.name, audioClip);
			//处理音频源
			_AudioSourceArray = GetComponents<AudioSource>();
			_AudioSource_BackgroundAudio = _AudioSourceArray[0];
			_AudioSource_AudioEffect_A = _AudioSourceArray[1];
			_AudioSource_AudioEffect_B = _AudioSourceArray[2];

			//从数据持久化中得到音量数值
			if(PlayerPrefs.GetFloat("AudioBackgroundVolumns") >= 0) {
				AudioBackgroundVolumns = PlayerPrefs.GetFloat("AudioBackgroundVolumns");
				_AudioSource_BackgroundAudio.volume = AudioBackgroundVolumns;
			}
			if(PlayerPrefs.GetFloat("AudioEffectVolumns") >= 0) {
				AudioEffectVolumns = PlayerPrefs.GetFloat("AudioEffectVolumns");
				_AudioSource_AudioEffect_A.volume = AudioEffectVolumns;
				_AudioSource_AudioEffect_B.volume = AudioEffectVolumns;
			}
		} //Start_end


		#region 【背景音乐的控制方法】

		/// <summary>播放背景音乐（可避免重复播放）</summary>
		/// <param name="audioClip">音频剪辑</param>
		public static void PlayBackground(AudioClip audioClip) {
			//防止背景音乐的重复播放。
			if(_AudioSource_BackgroundAudio.clip == audioClip)
				return;
			//处理全局背景音乐音量
			_AudioSource_BackgroundAudio.volume = AudioBackgroundVolumns;
			if(audioClip) {
				_AudioSource_BackgroundAudio.loop = true;
				//将音频剪辑放入到音频源字典中去，在0号位（背景音乐）
				_AudioSource_BackgroundAudio.clip = audioClip;
				//播放音频源
				_AudioSource_BackgroundAudio.Play();
			}
			else {
				Debug.LogWarning("AudioManager.cs：背景音乐剪辑为空，请检查！");
			}
		}

		/// <summary>播放背景音乐</summary>
		/// <param name="strAudioName">背景音乐（名字）</param>
		public static void PlayBackground(string strAudioName) {
			if(!string.IsNullOrEmpty(strAudioName))
				PlayBackground(_DicAudioClipLib[strAudioName]);
			else
				Debug.LogWarning("AudioManager.cs：音频剪辑为空，请检查！");
		}

		/// <summary>改变背景音乐音量</summary>
		/// <param name="floAudioBGVolumns"></param>
		public static void SetAudioBackgroundVolumns(float floAudioBGVolumns) {
			_AudioSource_BackgroundAudio.volume = floAudioBGVolumns;
			AudioBackgroundVolumns = floAudioBGVolumns;
			//数据持久化
			PlayerPrefs.SetFloat("AudioBackgroundVolumns", floAudioBGVolumns);
		}

		#endregion


		#region 【音效A的控制方法】

		/// <summary>播放音效_音频源A</summary>
		/// <param name="audioClip">音频剪辑A</param>
		/// <param name="isReplaced">是否替换正在播放的音频剪辑</param>
		public static void PlayAudioEffect_A(AudioClip audioClip, bool isReplaced = false) {
			//处理全局音效音量
			_AudioSource_AudioEffect_A.volume = AudioEffectVolumns;
			if(audioClip) {
				//如果不需要替换播放，且当前正在播放音频资源A；
				if(!isReplaced && _AudioSource_AudioEffect_A.isPlaying) { }
				else {
					//将音频剪辑A放入到音频源字典中去，在1号位（音效1）
					_AudioSource_AudioEffect_A.clip = audioClip;
					_AudioSource_AudioEffect_A.Play();
				}
			}
			else {
				Debug.LogWarning("AudioManager.cs：音频剪辑为空，请检查！");
			}
		}

		/// <summary>播放音效_音频源A</summary>
		/// <param name="strAudioEffctName">音效剪辑A（名字）</param>
		public static void PlayAudioEffect(string strAudioEffctName, bool isReplaced = false) {
			if(!string.IsNullOrEmpty(strAudioEffctName))
				PlayAudioEffect_A(_DicAudioClipLib[strAudioEffctName], isReplaced);
			else
				Debug.LogWarning("AudioManager.cs：音频为空，请检查！");
		}

		/// <summary>音频资源A，是否正在播放指定音效_音频源</summary>
		/// <param name="audioClip">指定音频剪辑</param>
		public static bool IsPlayingAudioEffect_A(AudioClip audioClip) {
			if(audioClip) {
				if(audioClip == _AudioSource_AudioEffect_A.clip)
					return _AudioSource_AudioEffect_A.isPlaying;
				return false;
			}
			Debug.LogWarning("AudioManager.cs：音频为空，请检查！");
			return false;
		}

		/// <summary>停止播放音效_音频源</summary>
		public static void StopAudioEffect_A() {
			_AudioSource_AudioEffect_A.Stop();
		}

		/// <summary>改变音效音量</summary>
		/// <param name="floAudioEffectVolumns"></param>
		public static void SetAudioEffectVolumns(float floAudioEffectVolumns) {
			_AudioSource_AudioEffect_A.volume = floAudioEffectVolumns;
			_AudioSource_AudioEffect_B.volume = floAudioEffectVolumns;
			AudioEffectVolumns = floAudioEffectVolumns;
			//数据持久化
			PlayerPrefs.SetFloat("AudioEffectVolumns", floAudioEffectVolumns);
		}

		#endregion


		/// <summary>播放音效_音频源B</summary>
		/// <param name="strAudioEffctName">音效名称</param>
		public static void PlayAudioEffectB(string strAudioEffctName) {
			if(!string.IsNullOrEmpty(strAudioEffctName))
				PlayAudioEffectB(_DicAudioClipLib[strAudioEffctName]);
			else
				Debug.LogWarning("AudioManager.cs：音频为空，请检查！");
		}

		/// <summary>播放音效_音频源B</summary>
		/// <param name="audioClip">音频剪辑</param>
		public static void PlayAudioEffectB(AudioClip audioClip) {
			//处理全局音效音量
			_AudioSource_AudioEffect_B.volume = AudioEffectVolumns;

			if(audioClip) {
				_AudioSource_AudioEffect_B.clip = audioClip;
				_AudioSource_AudioEffect_B.Play();
			}
			else {
				Debug.LogWarning("AudioManager.cs：音频为空，请检查！");
			}
		}
	} //Class_end
}