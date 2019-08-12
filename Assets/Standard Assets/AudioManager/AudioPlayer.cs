using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;

/// <summary>
/// Receive the Command from AudioManager then play the audio, follow configs in the Command
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {
	#region Init
	/// The main source, will be config clip, mixer ... and call Play, Stop ...
	AudioSource _source;
	/// Store current playing PlayAudioSetting 
	AudioCommand _command;
	AudioClipType _clipType;

	void Awake() {
		_source = GetComponent<AudioSource>();
		// first configs
		ResetAudioSource();
	}
	#endregion

	#region Public
	/// Save Time.time each time Play() was call. Will use for AudioManager when limit audio perframe
	float _playTime;
	public bool IsSamePlayTime(float playTime) {
		/// A frame is 0.016 second, so use the accuracy 0.001 is better than the default epsilon
		return _playTime.IsEqual(playTime, 0.001f);
	}

	public void Play(AudioCommand command) {
		if(command == null) return;
		// save to current
		_command = command;
		_playTime = Time.time;
		// config AudioSource. Reset all setting first
		ResetAudioSource();
		_source.outputAudioMixerGroup = command.basicCfg.mixerGroup;
		_clipType = command.basicCfg.clipType;
		_source.clip = command.GetClipForPlay();
		// start the playing process
		StartPlayingProcess(command);
	}

	public void Stop(float fadeOutDuration = 0) {
		if(!IsWorking()) return;
		if(fadeOutDuration > 0) {
			StartFadeOutProcess(fadeOutDuration);
		}
		else {
			_source.Stop();
			ResetProcessIfNotPlayingAnything();
			ResetAudioSource();
		}
	}

	public void StopIfPlayAnyOfCommands(AudioCommand[] commands, float fadeOutDuration) {
		if(!IsWorking() || commands == null || (commands != null && commands.Length == 0)) return;
		for(int n = 0; n < commands.Length; n++) {
			if(_command == commands[n]) {
				Stop(fadeOutDuration);
				break;
			}
		}
	}

	public void StopIfPlayingType(AudioClipType clipType, float fadeOutDuration) {
		if(!IsWorking()) return;
		if(_clipType == clipType) Stop(fadeOutDuration);
	}

	//-------------------------
	//--------------- Checking
	//-------------------------

	public bool IsWorking() {
		return _source.isPlaying || _playProcess != null || _fadeOutProcess != null;
	}

	public bool IsPlayingCommand(AudioCommand command) {
		if(_command == null) return false;
		else return (IsWorking() && _command == command);
	}
	#endregion

	#region Private - Playing, Stoping
	IDisposable _playProcess = null;
	IDisposable _fadeOutProcess = null;

	void StartPlayingProcess(AudioCommand cmd) {
		_playProcess = Observable.FromMicroCoroutine(_ => IE_PlayingProcess(cmd)).Subscribe().AddTo(_destroyDispose);
	}

	IEnumerator IE_PlayingProcess(AudioCommand cmd) {
		var ticker = new TickHelper();
		// check delay first
		if(cmd.startCfg.delay > 0) {
			ticker.Reset(cmd.startCfg.delay);
			yield return null;
			while(ticker.Tick()) yield return null;
		}
		// play here
		_source.Play();
		// the fadeIn effect
		if(cmd.startCfg.fadeInDuration > 0) {
			// first volumne
			_source.volume = 0f;
			// fadein
			ticker.Reset(cmd.startCfg.fadeInDuration);
			yield return null;//NOTES : must have this, if not, some short audio maybe stop before it play
			while(ticker.Tick()) {
				_source.volume = ticker.GetWaitedTimeInPercent();
				yield return null;
			}
			// final volumn
			_source.volume = 1f;
		}
		// check loop
		if(cmd.loopCfg.active) {
			_source.loop = cmd.loopCfg.active;
			if(cmd.loopCfg.limitLoopCount > 0) {
				var playTime = _source.clip.length * cmd.loopCfg.limitLoopCount;
				ticker.Reset(playTime);
				yield return null;//NOTES : must have this, if not, some short audio maybe stop before it play
				while(ticker.Tick()) yield return null;
				// stop after finish waiting
				Stop();
			}
		}
		// not loop, then wait for finised sound, then call Stop, also let is "ready"
		else {
			// if clip shorter than this, the player maybe stop before the sound start.
			const float TOO_SHORT_THRESHOLD = 0.08f;
			ticker.Reset((_source.clip.length < TOO_SHORT_THRESHOLD) ? TOO_SHORT_THRESHOLD : _source.clip.length);
			yield return null;//NOTES : must have this, if not, some short audio maybe stop before it play
			while(ticker.Tick()) yield return null;
			Stop();
		}
	}


	void StartFadeOutProcess(float duration) {
		_fadeOutProcess = Observable.FromMicroCoroutine(_ => IE_FadeOurProcess(duration)).Subscribe().AddTo(_destroyDispose);
	}

	IEnumerator IE_FadeOurProcess(float duration) {
		var ticker = new TickHelper();
		ticker.Reset(duration);
		yield return null;
		while(ticker.Tick()) {
			_source.volume = ticker.GetRemainTimeInPercent();
			yield return null;
		}
		// finish fadeOut duration, call stop
		Stop();
	}

	CompositeDisposable _destroyDispose = new CompositeDisposable();
	void OnDestroy() {
		_destroyDispose.Clear();
	}
	#endregion//Private - Playing, Stoping

	#region Private - Reset
	/// Some default config for source
	void ResetAudioSource() {
		_source.loop = false;
		_source.playOnAwake = false;
		_source.volume = 1f;
		_source.clip = null;
		_source.outputAudioMixerGroup = null;
	}

	void ResetProcessIfNotPlayingAnything() {
		if(!_source.isPlaying) {
			if(_playProcess != null) {
				_playProcess.Dispose();
				_playProcess = null;
			}
			if(_fadeOutProcess != null) {
				_fadeOutProcess.Dispose();
				_fadeOutProcess = null;
			}
		}
	}
	#endregion//Private - Reset
}