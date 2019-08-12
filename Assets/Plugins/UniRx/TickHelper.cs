using UnityEngine;

/// <summary>
/// Simple Utils class, use with UniRX's MicroCorountine (MC). Cause currently, the MC only support yield return null,
/// so we can't use WaitForSecond(). So use this to do the WaitForSecond, like this :
/// - Create a ticker, Reset it to a delay time.
/// - yield return null : must do this, to make sure we alway have 1 frame skip (in case Reset was call with 0 para), there will be no delay
/// - do this : while (ticker.Tick()) yiel return null.
/// That all, after the ticker finished, the while loop will be break.
/// </summary>
public struct TickHelper {
	/// <summary> The duration for this ticker, will be set on Reset(), and stay untouch, the _counter var
	/// will be use to reduced each Tick() called </summary>
	float _duration;
	/// <summary> Will use to reduce by frameTime each Tick() called. </summary>
	float _counter;

	/// <summary> Reset the duration time </summary>
	public void Reset(float time) {
		_duration = time;
		_counter = _duration;
	}

	/// <summary> Reduce the duration time by Time.deltaTime.
	/// Return true if remain duration > 0, return false otherwise </summary>
	public bool Tick() {
		_counter -= Time.deltaTime;
		return _counter > 0;
	}

	///<summary> This will return float value from 1-0. cause the _counter is down from duration to zero</summary>>
	public float GetRemainTimeInPercent() {
		return _counter / _duration;
	}

	///<summary> This will return float value from 0-1. cause the _counter is down from duration to zero</summary>
	public float GetWaitedTimeInPercent() {
		return (_duration - _counter) / _duration;
	}
}