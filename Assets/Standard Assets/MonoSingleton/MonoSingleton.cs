using UnityEngine;
using System.Collections;

/// <summary>
/// The template for Singleton in Unity.
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
	#region Static
	static T s_instance = null;
	public static T Instance {
		get {
			if (!Application.isPlaying) {
				Log.Warning(false, "Call Instance, but Application is not running, are you call to any Singleton at OnDestroy?");
				return null;
			}
			// dont exist, then create new one
			if(s_instance == null) {
				//create new Gameobject
				GameObject singleton = new GameObject();
				s_instance = singleton.AddComponent<T>();
				singleton.name = "Singleton - " + typeof(T).ToString();
				Log.Info("Create singleton : {0}", singleton.name);
			}
			return s_instance;
		}
		private set { }
	}

	public static bool HasInstance() {
		return s_instance != null;
	}
	#endregion//Statics


	#region Init, Destroy, Checking
	/// Custom this field in Inspector, default is true
	[SerializeField] bool dontDestroyThisObject = true;
	/// <summary> By default, all subclass of MonoSingleton is DontDestroy, if you want to custom it, 
	/// you can modify it in Inspector. If you want to force subclass DontDestroy=false, then override
	/// this Property, set it return false </summary>
	protected virtual bool IsDontDestroyThisObject { get { return dontDestroyThisObject; } }

	/// <summary> The extend classes MUST NOT override this function, let the MonoSingleton do the checking stuff here. The 
	/// extend classes should override the InitOnAwake function to do the task that they want to do on Awake().
	/// Mark this function as protected, so the compiler will thrown a warning if any extend class write Awake function </summary>
	protected void Awake() {
		// check if there another instance already exist in scene
		if(s_instance != null && s_instance.GetInstanceID() != this.GetInstanceID()) {
			// Destroy other instances if it not the same
			Log.Info("An instance of [{0}] already exist : [{1}], So destroy this instance : [{2}]!!", typeof(T).ToString(), s_instance.gameObject.name, name);
			Destroy(this);
		}
		// !!! IMPORTANT : only set Instance when there no duplicate object
		else {
			// set instance
			s_instance = this as T;
			// call InitOnAwake only for the static instance. Wont call on other redundancies instances
			DoOnAwake();
		}
	}

	/// <summary> The extend classes MUST NOT override this function, let the MonoSingleton do the checking stuff here. The 
	/// extend classes should override the InitOnStart function to do the task that they want to do on Start().
	/// Mark this function as protected, so the compiler will thrown a warning if any extend classes write Start function </summary>
	protected void Start() {
		/// An issue with Unity 5.4, 5.5 that make the DontDestroyOnLoad wont work
		/// on Android devices. Put it on Start will solve this issue
		if(IsDontDestroyThisObject) DontDestroyOnLoad(gameObject);
		// call InitOnStart for the static instance only. Wont call on other redundancies instances
		if(s_instance == this) DoOnStart();
	}

	/// <summary> The extend classes MUST NOT override this function, let the MonoSingleton do the checking stuff here. The 
	/// extend classes should override the DoOnDestroy function to do the task that they want to do on OnDestroy().
	/// Mark this function as protected, so the compiler will thrown a warning if any extend classes write OnDestroy function </summary>
	protected void OnDestroy() {
		// reset this static var to null ONLY if the current instance is the singleton
		if(s_instance == this) {
			Log.Info("Destroy instance of [{0}]", typeof(T));
			DoOnDestroy();
			s_instance = null;
		}
	}

	/// <summary> Derivered class should override this func to do stuffs that need to
	/// do in Awake. The Awake is now use for checking Singleton only </summary>
	protected virtual void DoOnAwake() {
	}

	/// <summary> Override to do Start() stuffs. Read InitOnAwake for more details </summary>
	protected virtual void DoOnStart() {
	}

	/// <summary> Override to do OnDestroy stuff. Read InitOnAwake for more details </summary>
	protected virtual void DoOnDestroy() {
	}
	#endregion//Init, Destroy, Checking
}