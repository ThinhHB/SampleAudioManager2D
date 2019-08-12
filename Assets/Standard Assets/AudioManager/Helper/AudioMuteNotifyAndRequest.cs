using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Notifier : it check the mute status of Master, BGM, SFX then output some events, we can use those events
/// to config UI.
/// REquest : it have some public functions to toggle mute Master/BGM/SFX. Invoke them using Unityevents
/// </summary>
public class AudioMuteNotifyAndRequest : MonoBehaviour {
	#region Init
	void Start() {
		// first invoke at Start
		CheckInvokeMuteEvents();
	}
	#endregion

	#region Events
	///<summary> Invoke at start, base on mute status of Master Group of Mixer</summary>
	[Header("Master volume")]
	public UnityEventBool OnMasterMuteStateChangedHandler;
	/// <summary> Same with OnMasterMute, but invert the value (true-> false, false ->true),
	/// some cases we need this to config UI (eg: turn on the slash image)</summary>
	public UnityEventBool OnMasterMuteStateChangedInvertHandler;
	///<summary> Invoke at start, base on mute status of BGM Group of Mixer</summary>
	[Header("BGM volume")]
	public UnityEventBool OnBGMMuteStateChangedHandler;
	/// <summary> Same with OnBGMMute, but invert the value (true-> false, false ->true),
	/// some cases we need this to config UI (eg: turn on the slash image)</summary>
	public UnityEventBool OnBGMMuteStateChangedInvertHandler;
	///<summary> Invoke at start, base on mute status of SFX Group of Mixer</summary>
	[Header("SFX volume")]
	public UnityEventBool OnSFXMuteStateChangedHandler;
	/// <summary> Same with OnSFXMute, but invert the value (true-> false, false ->true),
	/// some cases we need this to config UI (eg: turn on the slash image)</summary>
	public UnityEventBool OnSFXMuteStateChangedInvertHandler;
	#endregion

	#region Public - receive commands
	public void In_ActiveVolumeAll(bool active) {
		AudioManager.Instance.ActiveVolumeAll(active);
		CheckInvokeMuteEvents();
	}

	public void In_ActiveVolumeBGM(bool active) {
		AudioManager.Instance.ActiveVolumeBGM(active);
		CheckInvokeMuteEvents();
	}

	public void In_ActiveVolumesSFX(bool active) {
		AudioManager.Instance.ActiveVolumeSFX(active);
		CheckInvokeMuteEvents();
	}
	#endregion//Public - receive commands

	#region Private
	void CheckInvokeMuteEvents() {
		var adjustor = AudioManager.Instance.volumeAdjustor;
		if(adjustor.IsFaildedConfig()) return;

		var muteAll = adjustor.IsMuteAll();
		OnMasterMuteStateChangedHandler.Invoke(muteAll);
		OnMasterMuteStateChangedInvertHandler.Invoke(!muteAll);

		var muteBGM = adjustor.IsMuteBGM();
		OnBGMMuteStateChangedHandler.Invoke(muteBGM);
		OnBGMMuteStateChangedInvertHandler.Invoke(!muteBGM);

		var muteSFX = adjustor.IsMuteSFX();
		OnSFXMuteStateChangedHandler.Invoke(muteSFX);
		OnSFXMuteStateChangedInvertHandler.Invoke(!muteSFX);
	}
	#endregion//Private

	#region Classes
	[System.Serializable]
	public class UnityEventBool : UnityEvent<bool> {
	}
	#endregion//Classes
}