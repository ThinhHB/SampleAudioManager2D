using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Work with AudioManager. Attach it to object that have UI Button,
/// it will register Click events with that button, and play audio each click
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonClickAudio : MonoBehaviour {
	#region Init, config
	[SerializeField] AudioCommand audioCommand;
	/// Will be cache in CacheComponent
	Button _attachButton;

	void OnValidate() {
		CacheComponents();
		Log.Warning(audioCommand != null, this, "Missing audioCommand");
		Log.Warning(_attachButton != null, this, "Must attach this to Button gameObject");
	}

	void CacheComponents() {
		_attachButton = GetComponent<Button>();
	}

	bool IsFailedConfig() {
		return _attachButton == null
		|| audioCommand == null;
	}

	void Awake() {
		CacheComponents();
		if(IsFailedConfig()) return;
		_attachButton.onClick.AddListener(() => {
			AudioManager.Instance.Play(audioCommand);
			#if UNITY_EDITOR
			if(AudioManager.Instance.logButtonClick) Log.Info(this, "Button click play [{0}]", name);
			#endif
		});
	}
	#endregion
}