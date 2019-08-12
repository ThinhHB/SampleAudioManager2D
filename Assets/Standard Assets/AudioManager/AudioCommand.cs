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
	#endregion

	#region Getting
	///<summary> Call this to play audio </summary>
	public void Execute() {
		if(AudioManager.HasInstance())
			AudioManager.Instance.Play(this);
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
	public class BasicSetting {
		/// Set to false if we use the command for other purpose, not playAudio
		public bool activePlayAudio = true;
		public AudioClip clip;
		public AudioMixerGroup mixerGroup;
		public AudioClipType clipType;
	}

	[System.Serializable]
	public class LoopSetting {
		public bool active = false;
		/// If we only want it to play a limit loop, set it here. 0 = infinite loop
		public int limitLoopCount;
	}

	[System.Serializable]
	public class RandomSetting {
		public bool active = false;
		public AudioClip[] randomClips;
	}

	[System.Serializable]
	public class StartSetting {
		public float delay;
		public float fadeInDuration;
	}

	//-----------------------------
	//------------ AudioManager use

	[System.Serializable]
	public class LimitSetting {
		public int maxInstancePertime = 0;
	}

	[System.Serializable]
	public class LimitPerFrameSetting {
		public bool active = true;
		public int maxInstance = 1;
	}

	[System.Serializable]
	public class StopOtherSetting {
		public bool active = false;
		public AudioCommand[] others;
		public float fadeOutDuration = 0f;
	}

	[System.Serializable]
	public class StopBgmSetting {
		public bool active = false;
		public float fadeOutDuration = 0f;
	}

	[System.Serializable]
	public class StopAllSetting {
		public bool active = false;
		public float fadeOutDuration;
	}
	#endregion//Classes
}