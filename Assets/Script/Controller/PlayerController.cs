using UnityEngine;

public class PlayerController : Singleton<PlayerController> {

	public const string TAG = "Player";

	public ParticleSystem BloodParticleSystem;

	/// <summary>
	/// The maximum vertical angle from looking straight ahead.
	/// </summary>
	public float MaxLookAngleY = 80.0f;

	private Vector3 OriginalPos;

	private Vector3 TempVelocity;
	private Vector3 TempPos;
	private Vector3 TempEuler;

	// Use this for initialization
	void Start () {
		OriginalPos = transform.position;

		if ( BloodParticleSystem == null ) {
			Debug.Log ( "Blood Particle System cannot be null!" );
		}
	}

	// Update is called once per frame
	void Update () {
		// Constrain height
		TempVelocity = GameInput.Pose.Velocity;
		if ( TempVelocity.sqrMagnitude < 0.01f ) {
			TempVelocity = Vector3.zero;
		}
		TempVelocity.y = 0;

		TempPos = transform.position + TempVelocity * Time.deltaTime * 30;
		TempPos.y = OriginalPos.y;
		transform.position = TempPos;

		// Clamp Y-rotation to prevent gimbal lock
		TempEuler = GameInput.Pose.Rotation.eulerAngles;
		if ( TempEuler.x > 180 && TempEuler.x - 360 < -MaxLookAngleY ) {
			TempEuler.x = -MaxLookAngleY;
		}
		else if ( TempEuler.x < 180 && TempEuler.x > MaxLookAngleY ) {
			TempEuler.x = MaxLookAngleY;
		}
		transform.eulerAngles = TempEuler;
	}

	public static GameObject Object {
		get {
			return Instance.gameObject;
		}
	}

	public static Vector3 Position {
		get {
			return Instance.transform.position;
		}
	}

	public static bool IsPlayerObject ( GameObject GameObject ) {
		return GameObject == Instance.gameObject || GameObject.tag == TAG;
	}

	public static void SplatterBlood () {
		Instance.BloodParticleSystem.Emit ( 100 );
	}

}