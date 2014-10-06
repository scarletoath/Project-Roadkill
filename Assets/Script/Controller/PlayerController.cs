using System.Collections;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {

	public const string TAG = "Player";

	public ParticleSystem BloodParticleSystem;
	public GameObject Spear;
    public GameObject[] Walls;
    public GameObject Caveman;

	/// <summary>
	/// The maximum vertical angle from looking straight ahead.
	/// </summary>
	public float MaxLookAngleY = 80.0f;

	private Renderer [] SpearRenderers;

	private Vector3 OriginalPos;
	private Vector3 OriginalSpearScale;

	private Vector3 TempVelocity;
	private Vector3 TempPos;
	private Vector3 TempEuler;

	// Use this for initialization
	void Start () {
		OriginalPos = transform.position;

		if ( BloodParticleSystem == null ) {
			Debug.Log ( "Blood Particle System cannot be null!" );
		}

		if ( Spear != null ) {
			OriginalSpearScale = Spear.transform.localScale;
			SpearRenderers = Spear.GetComponentsInChildren<Renderer> ();
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

		TempPos = transform.position + TempVelocity * Time.deltaTime * GameManager.GetBonuses ().MoveSpeedMultiplier;
		TempPos.y = OriginalPos.y;

        Vector3 originalpos = transform.position;
        transform.position = TempPos;
        
        if(HasPlayerHitWall())
            transform.position = originalpos;

		float height = Terrain.activeTerrain.SampleHeight ( transform.position );
		Vector3 pos = transform.position;
		pos.y = height - 20 + 2.5f;
		transform.position = pos;


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

    private bool HasPlayerHitWall()
    {
        foreach (GameObject wall in Walls)
        {
            if (wall.collider.bounds.Intersects(Caveman.collider.bounds))
            {
                return true;
            }
        }
        return false;
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
		Instance.BloodParticleSystem.Emit ( 50 );
	}

	public static void GlowSpear () {
		Instance.StartCoroutine ( Instance.GlowSpearInternal () );
	}

	IEnumerator GlowSpearInternal () {
		Quaternion OriginalRot = Spear.transform.localRotation;

		for ( float i = 0.0f ; i < 1.0f ; i += Time.deltaTime ) {
			foreach ( Renderer Renderer in SpearRenderers ) {
				Renderer.material.SetFloat ( "_Emission" , i * 0.67f );
			}
			Spear.transform.localRotation = Quaternion.Euler ( new Vector3 ( -360.0f * i , 0 , 0 ) ) * OriginalRot;
			yield return null;
		}

		foreach ( Renderer Renderer in SpearRenderers ) {
			Renderer.material.SetFloat ( "_Emission" , 0.67f );
		}
		yield return new WaitForSeconds ( 0.5f );

		for ( float i = 1.0f ; i > 0.0f ; i -= Time.deltaTime ) {
			foreach ( Renderer Renderer in SpearRenderers ) {
				Renderer.material.SetFloat ( "_Emission" , i * 0.67f );
			}
			Spear.transform.localRotation = Quaternion.Euler ( new Vector3 ( -360.0f * i , 0 , 0 ) ) * OriginalRot;
			yield return null;
		}
		foreach ( Renderer Renderer in SpearRenderers ) {
			Renderer.material.SetFloat ( "_Emission" , 0 );
		}
		Spear.transform.localRotation = OriginalRot;

		Spear.transform.localScale = new Vector3 (
			OriginalSpearScale.x ,
			OriginalSpearScale.y ,
			OriginalSpearScale.z * GameManager.GetBonuses ().SpearLevel
		);
	}


}