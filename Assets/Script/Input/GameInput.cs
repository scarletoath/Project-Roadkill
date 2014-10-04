using Tango;
using UnityEngine;
using System.Collections;

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

		private Vector3 _Previous2Position = Vector3.zero;
		private Vector3 _PreviousPosition = Vector3.zero;

		private Quaternion _Previous2Rotation = Quaternion.identity;
		private Quaternion _PreviousRotation = Quaternion.identity;

		private Vector3 _Velocity = Vector3.zero;

		private ButtonState _ClickState = ButtonState.None;

		public Vector3 Position {
			get {
				return _CurrentPosition;
			}
			set {
				if ( CanUpdatePoseData ) {
					_Previous2Position = _PreviousPosition;
					_PreviousPosition = _CurrentPosition;
					_CurrentPosition = Vector3.Lerp(Vector3.Lerp(_Previous2Position,_PreviousPosition,0.8f),value,0.6f);

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
					_Previous2Rotation = _PreviousRotation;
					_PreviousRotation = _CurrentRotation;
					_CurrentRotation = Quaternion.Slerp(Quaternion.Slerp(_Previous2Rotation,_PreviousRotation,0.8f),value,0.6f);
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
	/// A reference to the TangoApplication component.
	/// </summary>
	private TangoApplication TangoApp;
	private VIOProvider.VIOStatus TangoVIOStatus;
	private DepthTexture TangoDepthTex;

	private Texture2D ColorTex;
	private Texture2D DepthTex;

	private Queue _PosQueue;
	private Queue _RotQueue;

	private double Timestamp;

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

		// Prepare textures
		TangoDepthTex = GetComponent<DepthTexture> ();
		if ( TangoDepthTex == null ) {
			TangoDepthTex = gameObject.AddComponent<DepthTexture> ();
		}
		ColorTex = new Texture2D ( 1920 , 1080 , TextureFormat.RGB565 , false );
		ColorTex.filterMode = FilterMode.Bilinear;
		ColorTex.wrapMode = TextureWrapMode.Clamp;
		DepthTex = new Texture2D ( 1920 , 1080 );

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
		// Keep checking for TangoSDK availability if in simulation mode
		if ( IsSimulation ) {
			if ( TangoApp.Valid && TangoApp.IsInitialized () ) {
				IsSimulation = false;
			}
		}

		CanUpdatePoseData = true;

		UpdatePoseData ();

		CanUpdatePoseData = false;
	}

	public static PoseData Pose { get; private set; }

	public static Texture2D ColorTexture { get { return Instance.ColorTex; } }

	public static Texture2D DepthTexture { get { return Instance.DepthTex; } }

	public static bool GetIsSimulation () {
		return Instance.IsSimulation;
	}

	private void UpdatePoseData () {
		if ( IsSimulation ) {
			SimulationInput.GetLatestPose ( Pose );

			return;
		}

		// Get data from Tango data providers
		if ( VIOProvider.GetLatestPose ( ref TangoVIOStatus ) ) {
			Pose.Position = TangoVIOStatus.translation;
			Pose.Rotation = TangoVIOStatus.rotation;


		}
	}

	private void UpdateFrameData () {
		VideoOverlayProvider.RenderLatestFrame ( ColorTex.GetNativeTextureID () , ColorTex.width , ColorTex.height , ref Timestamp );

		DepthTex = TangoDepthTex.GetDepthTexture ( true , 8 , 1 );
	}

}