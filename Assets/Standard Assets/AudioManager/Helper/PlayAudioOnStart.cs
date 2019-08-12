using UnityEngine;
using System.Collections;
using UniRx;

/// <summary>
/// Simple request AudioManager to play a command on Start()
/// </summary>
public class PlayAudioOnStart : MonoBehaviour {
	[SerializeField] PlayType playType = PlayType.Start;
	[SerializeField] AudioCommand command;
	[SerializeField] float delay;

	void OnValidate() {
		Log.Warning(command != null, this, "Missing audioCommand");
	}

	void Start() {
		if(playType == PlayType.Start) PlayCommand();
	}

	void OnEnable() {
		if(playType == PlayType.OnEnable) PlayCommand();
	}

	void OnDisable() {
		if(playType == PlayType.OnDisable) PlayCommand();
	}

	void PlayCommand() {
		if(command == null) return;
		if(delay > 0) {
			Observable.Timer(System.TimeSpan.FromSeconds(delay))
				.Subscribe(_ => SendCommandToAudioManager())
				.AddTo(_destroyDispose);
		}
		else {
			SendCommandToAudioManager();
		}
	}

	/// Use a separate function so we can use debug or other functions
	void SendCommandToAudioManager() {
		AudioManager.Instance.Play(command);
		#if UNITY_EDITOR
		if(AudioManager.Instance.logPlayAudioOnStart) Log.Info(this, "PlayAudioOnStart [{0}]", name);
		#endif
	}

	CompositeDisposable _destroyDispose = new CompositeDisposable();
	void OnDestroy() {
		_destroyDispose.Clear();
	}

	enum PlayType {
		Start,
		OnEnable,
		OnDisable,
	}
}