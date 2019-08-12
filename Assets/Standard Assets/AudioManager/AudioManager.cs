#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class AudioManager : MonoSingleton<AudioManager> {
	#region Init
	///<summary> Will use it to adjust global sound mute/unmute/volumn. Notes: all sound must use the same mixer</summary>
	public VolumeAdjustor volumeAdjustor;
	///<summary> Some warnings like reach limit instance .. will be block if turn this to false</summary>
	[SerializeField] bool logWarnings = false;
	public bool logAudioNameOnPlay = false;

#if UNITY_EDITOR
	///<summary> use in Editor only, set it to true in Inspector to enable logging SourceSetting
	/// each time PlayAudio() was called</summary>
	[Header("Editor only")]
	///<summary> This will let ButtonClickAudio component to be highlighted when it play</summary>
	public bool logButtonClick = false;
	///<summary> This will let ToggleClickAudio component to be highlighted when it play</summary>
	public bool logToggleClick = false;
	///<summary> This will let PlayAudioOnStart component to be highlighted when it play</summary>
	public bool logPlayAudioOnStart = false;
	///<summary> This will let PlayAudioRequestor component to be highlighted when it play</summary>
	public bool logPlayAudioRequester = false;
#endif//UNITY_EDITOR

	// fields
	List<AudioPlayer> _players = new List<AudioPlayer>();
	const int CACHE_SOURCE_CONTROLL = 5;

	void OnValidate() {
		volumeAdjustor.Validate();
		Log.Warning(!volumeAdjustor.IsFaildedConfig(), this, "Missing config for VolumeAdjuster");
	}

	protected override void DoOnAwake() {
		base.DoOnAwake();
		if(_players == null || _players.Count == 0)
			GeneratePlayers(CACHE_SOURCE_CONTROLL);
	}

	void GeneratePlayers(int amount) {
		// make sure all child are cleared
		while(transform.childCount > 0) {
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
		// generate new
		_players.Clear();
		ExpandPlayerList(amount);
	}
	#endregion// Init


	#region Publics - Play
	/// Execute the input AudioCommand
	public void Play(AudioCommand cmd) {
		if(cmd == null) {
			Log.Warning(false, "Play null SourceAudio, no sound will be played !!!");
			return;
		}
		// --------- Checking setting
		// checking max Instance
		if(cmd.limitCfg.maxInstancePertime > 0) {
			var currentPlayerForCmd = GetNumberOfPlayerArePlayingCommand(cmd);
			if(currentPlayerForCmd >= cmd.limitCfg.maxInstancePertime) {
				if(logWarnings) {
					Log.Info(cmd, "Audio, reach instance [{0}] for this [{1}], wont play this time", currentPlayerForCmd, cmd.name);
				}
				return;
			}
		}
		// check limit frame player
		if(cmd.limitPerFrameCfg.active) {
			var currentPlayerForCmd = GetNumberOfPlayerArePlayingCommandInTheDefineTime(cmd, Time.time);
			if(currentPlayerForCmd >= cmd.limitPerFrameCfg.maxInstance) {
				if(logWarnings) {
					Log.Info(cmd, "Audio, reach frame limit [{0}] for this [{1}], wont play this time", currentPlayerForCmd, cmd.name);
				}
				return;
			}
		}
		// stop other source
		if(cmd.stopOtherCfg.active) {
			for(int n = 0, amount = _players.Count; n < amount; n++) {
				_players[n].StopIfPlayAnyOfCommands(cmd.stopOtherCfg.others, cmd.stopOtherCfg.fadeOutDuration);
			}
		}
		// fadeOut BGM
		if(cmd.stopBgmCfg.active) {
			for(int n = 0, amount = _players.Count; n < amount; n++) {
				_players[n].StopIfPlayingType(AudioClipType.BGM, cmd.stopBgmCfg.fadeOutDuration);
			}
		}
		// stop all
		if(cmd.stopAllCfg.active) {
			for(int n = 0, amount = _players.Count; n < amount; n++) {
				_players[n].Stop(cmd.stopAllCfg.fadeOutDuration);
			}
		}
		//------------ Play
		if(cmd.basicCfg.activePlayAudio) {
			var readyPlayer = GetReadyPlayer();
			readyPlayer.Play(cmd);
			if(logAudioNameOnPlay) {
				Log.Info(cmd, "Audio [{0}], time {1}, player[{2}]", cmd.name, Time.time, readyPlayer.name);
			}
		}
	}
	#endregion//Public


	#region Public - manual play Audio, checking, getting
	/// Play the audio and also get reference to the Player that execute the command
	public AudioPlayer PlayAndGetThePlayer(AudioCommand command) {
		// sure the ready player we got here is exactly the player will be assigned to play Command in Play() func
		var readyPlayer = GetReadyPlayer();
		Play(command);
		return readyPlayer;
	}

	public bool IsReachLimitInstanceForCommand(AudioCommand cmd) {
		// no limit
		if(cmd.limitCfg.maxInstancePertime == 0)
			return false;
		// has limit instance
		var currentPlayerForCmd = GetNumberOfPlayerArePlayingCommand(cmd);
		return currentPlayerForCmd >= cmd.limitCfg.maxInstancePertime;
	}
	#endregion//Public - manual play Audio, checking, getting


	#region Public - Mute
	public void ActiveVolumeAll(bool active) {
		volumeAdjustor.ActiveVolumeAll(active);
	}

	public void ActiveVolumeBGM(bool active) {
		volumeAdjustor.ActiveVolumeBGM(active);
	}

	public void ActiveVolumeSFX(bool active) {
		volumeAdjustor.ActiveVolumeSFX(active);
	}
	#endregion// Mute


	#region Private
	AudioPlayer GetReadyPlayer() {
		// find the Idle source, if no source available, then expand sourceList
		var result = FindIdlePlayer();
		if(result == null) {
			ExpandPlayerList();
			// find again, sure we'll have one this time
			result = FindIdlePlayer();
		}
		return result;
	}

	void ExpandPlayerList(int amount = 1) {
		var currentPlayerCount = _players.Count;
		for(int i = 0; i < amount; i++) {
			var playerObject = new GameObject("Player_" + (i + currentPlayerCount));
			playerObject.AddComponent<AudioSource>();
			_players.Add(playerObject.AddComponent<AudioPlayer>());
			playerObject.transform.SetParent(transform);
		}
	}

	AudioPlayer FindIdlePlayer() {
		return _players.Find(x => !x.IsWorking());
	}

	int GetNumberOfPlayerArePlayingCommand(AudioCommand command) {
		var sum = 0;
		for(int i = 0, amount = _players.Count; i < amount; i++) {
			if(_players[i].IsPlayingCommand(command))
				sum++;
		}
		return sum;
	}

	int GetNumberOfPlayerArePlayingCommandInTheDefineTime(AudioCommand command, float defineTime) {
		var sum = 0;
		for(int i = 0, amount = _players.Count; i < amount; i++) {
			var player = _players[i];
			if(player.IsPlayingCommand(command) && player.IsSamePlayTime(defineTime))
				sum++;
		}
		return sum;
	}
	#endregion// Private


	#region Classes
	[System.Serializable]
	public class VolumeAdjustor {
		#region Field, validate
		public AudioMixer mixer;
		/// Use this to adjust master volume of this mixer. NOTES : you must expose this parameter in Editor.
		/// Follow this : https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
		public string masterVolumeParameterName;
		public string bgmVolumeParameterName;
		public string sfxVolumeParameterName;
		[Header("Default value in Unity is [0, -80]")]
		[SerializeField] float maxVolumeDB = 0;
		[SerializeField] float minVolumeDB = -80;

		public void Validate() {
			Log.Warning(mixer != null, "Missing AudioMixer");
			Log.Warning(!string.IsNullOrEmpty(masterVolumeParameterName), "Missing masterVolumeParameterName");
			Log.Warning(!string.IsNullOrEmpty(bgmVolumeParameterName), "Missing bgmVolumeParameterName");
			Log.Warning(!string.IsNullOrEmpty(sfxVolumeParameterName), "Missing sfxVolumeParameterName");
		}

		public bool IsFaildedConfig() {
			return mixer == null
			|| string.IsNullOrEmpty(masterVolumeParameterName)
			|| string.IsNullOrEmpty(bgmVolumeParameterName)
			|| string.IsNullOrEmpty(sfxVolumeParameterName);
		}
		#endregion//Field, validate


		#region Public - mute
		public void ActiveVolumeAll(bool active) {
			if(IsFaildedConfig())
				return;
			mixer.SetFloat(masterVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
		}

		public void ActiveVolumeBGM(bool active) {
			if(IsFaildedConfig())
				return;
			mixer.SetFloat(bgmVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
		}

		public void ActiveVolumeSFX(bool active) {
			if(IsFaildedConfig())
				return;
			mixer.SetFloat(sfxVolumeParameterName, active ? maxVolumeDB : minVolumeDB);
		}
		#endregion//Public - mute


		#region Public - GetInfo
		public bool IsMuteAll() {
			float masterVol;
			mixer.GetFloat(masterVolumeParameterName, out masterVol);
			return masterVol.IsEqual(0, 0.01f);
		}

		public bool IsMuteBGM() {
			float bgmVol;
			mixer.GetFloat(bgmVolumeParameterName, out bgmVol);
			return bgmVol.IsEqual(0, 0.01f);
		}

		public bool IsMuteSFX() {
			float sfxVol;
			mixer.GetFloat(sfxVolumeParameterName, out sfxVol);
			return sfxVol.IsEqual(0, 0.01f);
		}
		#endregion//Public - GetInfo
	}
	#endregion//Classes
}


#region Custom Classes
public enum AudioClipType {
	SFX = 0,
	BGM,
}
#endregion//Classes