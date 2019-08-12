using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// <summary>
/// Use this to create configuration asset for playing audio :
/// which clips ? whichs audioMixerGroup (channel)? maxInstance ...
/// </summary>
[CreateAssetMenu(fileName = "AudioCommand", menuName = "GameConfiguration/Audio/AudioCommand")]
public class AudioCommand : ScriptableObject {
	#region Config
	[Header("For AudioPlayer")]
	public BasicSetting basicCfg;
	public LoopSetting loopCfg;
	public RandomSetting randomCfg;
	public StartSetting startCfg;

	[Header("For AudioManager")]
	public LimitSetting limitCfg;
	public LimitPerFrameSetting limitPerFrameCfg;
	public StopOtherSetting stopOtherCfg;
	public StopBgmSetting stopBgmCfg;
	public StopAllSetting stopAllCfg;

	[Header("Mute/Unmute")]
	public MuteSetting muteCfg;
	#endregion


	#region Getting
	///<summary> Call this to play audio </summary>
	public void Execute() {
		if(AudioManager.HasInstance()) {
			AudioManager.Instance.Play(this);
		}
		else {
			Log.Warning(false, this, "No AudioManager instance on scene. Drag one to the scene");
		}
	}

	/// Base on random setting ... it will return the clip to set to AudioSource
	public AudioClip GetClipForPlay() {
		// active random clip
		if(randomCfg.active) {
			var randomIndex = Random.Range(0, randomCfg.randomClips.Length);
			return randomCfg.randomClips[randomIndex];
		}
		// normal
		return basicCfg.clip;
	}
	#endregion


	#region Classes
	//-----------------------------
	//------------ AudioPlayer use
	[System.Serializable]
	public struct BasicSetting {
		/// Set to false if we use the command for other purpose, not playAudio
		public bool activePlayAudio;
		public AudioClip clip;
		public AudioMixerGroup mixerGroup;
		public AudioClipType clipType;
	}

	[System.Serializable]
	public struct LoopSetting {
		public bool active;
		/// If we only want it to play a limit loop, set it here. 0 = infinite loop
		public int limitLoopCount;
	}

	[System.Serializable]
	public struct RandomSetting {
		public bool active;
		public AudioClip[] randomClips;
	}

	[System.Serializable]
	public struct StartSetting {
		public float delay;
		public float fadeInDuration;
	}

	//-----------------------------
	//------------ AudioManager use

	[System.Serializable]
	public struct LimitSetting {
		public int maxInstancePertime;
	}

	[System.Serializable]
	public struct LimitPerFrameSetting {
		public bool active;
		public int maxInstance;
	}

	[System.Serializable]
	public struct StopOtherSetting {
		public bool active;
		public AudioCommand[] others;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct StopBgmSetting {
		public bool active;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct StopAllSetting {
		public bool active;
		public float fadeOutDuration;
	}

	[System.Serializable]
	public struct MuteSetting {
		public bool active;
		public bool muteAll;
	}
	#endregion//Classes
}