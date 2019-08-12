using UnityEngine;
using System.Collections;

/// <summary>
/// Work with AUdioManager. Have some public functions to invoke, it will call to
/// AUdioManager to play the selected command
/// </summary>
public class PlayAudioRequester : MonoBehaviour {
	[SerializeField] AudioCommand[] commands;

	void OnValidate() {
		Log.Warning(commands != null && (commands != null && commands.Length != 0), this, "Missing config commands");
	}

	bool IsFailedConfig() {
		return commands == null
		|| (commands != null && commands.Length == 0);
	}

	/// Should use UnityEvent to invoke this, if the input index is out of AUdioCommands array, no sound will be play
	public void In_RequestPlay(int audioCommandIndex) {
		var cmd = GetCommand(audioCommandIndex);
		if(cmd == null) {
			Log.Warning(false, this, "REquest play audio index [{0}] out of range or not config yet");
			return;
		}
		else {
			AudioManager.Instance.Play(cmd);
			#if UNITY_EDITOR
			if(AudioManager.Instance.logPlayAudioRequester) Log.Info(this, "PlayAudioRequester [{0}]", name);
			#endif
		}
	}

	AudioCommand GetCommand(int index) {
		if(IsFailedConfig()) return null;
		if(index < 0 || index >= commands.Length) return null;
		return commands[index];
	}
}