using Tango;
using UnityEngine;

public class GameInput : Singleton<GameInput> {

	/// <summary>
	/// The prefab to instantiate if the scene does not already contain the 
	/// TangoSDK prefab.
	/// </summary>
	public GameObject TangoPrefab;

	/// <summary>
	/// A reference to the TangoApplication component.
	/// </summary>
	private TangoApplication TangoApp;

	protected override void Awake () {
		base.Awake ();

		if ( TangoPrefab ) {
			TangoApp = GameObject.FindObjectOfType<TangoApplication> ();

			if ( TangoApp == null ) {
				TangoApp = ( Instantiate ( TangoPrefab , Vector3.zero , Quaternion.identity ) as GameObject ).GetComponent<TangoApplication> ();
				TangoApp.transform.parent = transform;
			}
			else if ( TangoApp.tag != "TangoSDK" ) {
				Debug.LogError ( "Are you sure the TangoApp is the correct one?" );
			}
		}
		else {
			Debug.LogError ( "TangoPrefab must be assigned for TangoSDK to be used!" );

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