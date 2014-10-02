using Tango;
using UnityEngine;

public enum ButtonState {
	None ,
	Released ,
	Pressed ,
	HoldingDown ,
}

public class GameInput : Singleton<GameInput> {

	/// <summary>
	/// Stores interpreted pose information from the TangoSDK.
	/// </summary>
	public class PoseData {

		private Vector3 _CurrentPosition = Vector3.zero;
		private Quaternion _CurrentRotation = Quaternion.identity;

		private Vector3 _PreviousPosition = Vector3.zero;
		private Quaternion _PreviousRotation = Quaternion.identity;

		private Vector3 _Velocity = Vector3.zero;

		private ButtonState _ClickState = ButtonState.None;

		public Vector3 Position {
			get {
				return _CurrentPosition;
			}
			set {
				if ( CanUpdatePoseData ) {
					_PreviousPosition = _CurrentPosition;
					_CurrentPosition = value;

					_Velocity = ( _CurrentPosition - _PreviousPosition ) / Time.deltaTime;
				}
			}
		}

		public Quaternion Rotation {
			get {
				return _CurrentRotation;
			}
			set {
				if ( CanUpdatePoseData ) {
					_PreviousRotation = _CurrentRotation;
					_CurrentRotation = value;

					// Clamp Y-rotation to prevent gimbal lock
					Vector3 Euler = _CurrentRotation.eulerAngles;
					if ( Euler.x > 180 && Euler.x - 360 < -Instance.MaxLookAngleY ) {
						Euler.x = -Instance.MaxLookAngleY;
					}
					else if ( Euler.x < 180 && Euler.x > Instance.MaxLookAngleY ) {
						Euler.x = Instance.MaxLookAngleY;
					}
					_CurrentRotation.eulerAngles = Euler;
				}
			}
		}

		public Vector3 Velocity {
			get {
				return _Velocity;
			}
		}

	}

	/// <summary>
	/// Indicates whether pose data is updated if the structure's data is 
	/// attempted to be changed.
	/// </summary>
	private static bool CanUpdatePoseData = false;

	/// <summary>
	/// The prefab to instantiate if the scene does not already contain the 
	/// TangoSDK prefab.
	/// </summary>
	public GameObject TangoPrefab;

	/// <summary>
	/// Indicates whether to use mouse and keyboard for input. It is 
	/// automatically set if TangoSDK is not valid.
	/// </summary>
	public bool IsSimulation = false;

	/// <summary>
	/// The maximum vertical angle from looking straight ahead.
	/// </summary>
	public float MaxLookAngleY = 80.0f;

	/// <summary>
	/// A reference to the TangoApplication component.
	/// </summary>
	private TangoApplication TangoApp;
	private VIOProvider.VIOStatus VIOStatus;

	private SimulationInput SimulationInput;

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

		// Auto set simulation mode if Tango not available
		// Tango init occurs during Instantiate->Awake, so if available, it is init-ed by now
		if ( !TangoApp.Valid || !TangoApp.IsInitialized () ) {
			IsSimulation = true;

			SimulationInput = gameObject.GetComponent<SimulationInput> ();
		}

		// Disable cursor
		Screen.showCursor = false;

		// Initialize data structures
		Pose = new PoseData ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		CanUpdatePoseData = true;

		UpdatePoseData ();

		CanUpdatePoseData = false;
	}

	public static PoseData Pose { get; private set; }

	private void UpdatePoseData () {
		if ( IsSimulation ) {
			SimulationInput.GetLatestPose ( Pose );

			return;
		}

		// Get data from Tango data providers
		if ( VIOProvider.GetLatestPose ( ref VIOStatus ) ) {
			Pose.Position = VIOStatus.translation;
			Pose.Rotation = VIOStatus.rotation;
		}
	}

}