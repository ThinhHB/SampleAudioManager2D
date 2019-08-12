using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// <summary>
/// Work with AudioManager. Attach it to object that have UI Toggle,
/// it will register toggle events with that button, and play audio each click
/// </summary>
public class ToggleButtonAudio : MonoBehaviour {
	#region Init, config
	[SerializeField] AudioCommand audioCommand;
	/// Will be cache in CacheComponent
	Toggle _attachToggleBtn;

	void OnValidate() {
		CacheComponents();
		Log.Warning(audioCommand != null, this, "Missing audioCommand");
		Log.Warning(_attachToggleBtn != null, this, "Must attach this to Toggle Button gameObject");
	}

	void CacheComponents() {
		_attachToggleBtn = GetComponent<Toggle>();
	}

	bool IsFailedConfig() {
		return _attachToggleBtn == null
		|| audioCommand == null;
	}

	void Awake() {
		CacheComponents();
		if(IsFailedConfig()) return;
		_attachToggleBtn.onValueChanged.AddListener((isOn) => {
			AudioManager.Instance.Play(audioCommand);
			#if UNITY_EDITOR
			if(AudioManager.Instance.logToggleClick) Log.Info(this, "ToggleButtonAudio [{0}]", name);
			#endif
		});
	}
	#endregion
}