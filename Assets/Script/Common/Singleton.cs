using UnityEngine;

/// <summary>
/// Helper base class to create singleton classes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {

	/// <summary>
	/// The singleton instance.
	/// </summary>
	protected static T Instance;

	virtual protected void Awake () {
		if ( Instance == null ) {
			Instance = this as T;
		}
		else {
			Debug.LogError ( "There can only be one instance of " + typeof ( T ).Name );

			gameObject.SetActive ( false );
		}
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

}