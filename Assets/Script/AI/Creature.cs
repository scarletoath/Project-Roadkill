using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType {
	None ,

	Bird ,
	Bunny ,
	Dragon ,
	Elephant ,
	Gnu ,
	Hippo ,
}

public enum CreatureState {

	/// <summary>
	/// An invalid state.
	/// </summary>
	None ,
	/// <summary>
	/// A special state to indicate to a called function to use whatever state 
	/// the Creature is currently in.
	/// </summary>
	Current ,

	/// <summary>
	/// The creature is waiting in position, doing nothing.
	/// </summary>
	Idle ,

	/// <summary>
	/// The creature is looking at something without moving.
	/// </summary>
	Looking ,
	/// <summary>
	/// The creature is moving around on its own, without any stimuli.
	/// </summary>
	Moving ,
	/// <summary>
	/// The creature is running away from something.
	/// </summary>
	Escaping ,
	/// <summary>
	/// The creature is running towards something.
	/// </summary>
	Chasing ,

	/// <summary>
	/// The creature is dying.
	/// </summary>
	Dying ,
	/// <summary>
	/// The creature is dead.
	/// </summary>
	Dead ,

}

public enum CreatureAnimationState {
	None ,

	Idle ,
	Move ,

	Die ,
}

[System.Serializable]
public class CreatureSound {

	/// <summary>
	/// The CreatureState that the CreatureSound is for.
	/// </summary>
	public CreatureState State;
	/// <summary>
	/// The minimum time between playing multiple instances of this CreatureSound.
	/// </summary>
	public float MinInterval = 0.0f;
	/// <summary>
	/// The maximum time between playing multiple instances of this CreatureSound.
	/// </summary>
	public float MaxInterval = 0.0f;
	/// <summary>
	/// The AudioClips that are associated with this CreatureSound.
	/// </summary>
	public AudioClip [] Clips;

	/// <summary>
	/// Gets a random clip.
	/// </summary>
	/// <returns>The randomized AudioClip.</returns>
	public AudioClip Get () {
		return Get ( Random.Range ( 0 , Clips.Length ) );
	}

	/// <summary>
	/// Gets the clip with the specified index.
	/// </summary>
	/// <param name="Index">The index of the clip to retrieve.</param>
	/// <returns>The AudioClip with the specified index. Returns null if Index is out of range or Clips is empty.</returns>
	public AudioClip Get ( int Index ) {
		if ( Clips.Length == 0 || Index < 0 || Index >= Clips.Length ) {
			return null;
		}

		return Clips [ Index ];
	}

}

public abstract class Creature : MonoBehaviour {

	public const string TAG = "Creature";

	public static readonly int TYPE_COUNT = System.Enum.GetNames ( typeof ( CreatureType ) ).Length;

	public CreatureType Type = CreatureType.None;

	//[Header ( "Movement" )]

	public float MoveSpeed = 1.0f;
	public float MaxTurnSpeed = 1.57f;


	//[Header ( "AI" )]

	//[Tooltip ( "Set to Infinity if not scared." )]
	public float DetectionRange = 25.0f;
	public float EscapeDistance = 25.0f;
	public bool IsLookAtPlayerAfterEscape = true;


	//[Header ( "On Death" )]

	public float DestroyTime = 1.0f;
	public Material BloodMaterial;


	//[Header ( "Sounds" )]

	public CreatureSound [] Sounds;

	protected bool IsDead = false;

	protected Vector3 EscapeDir;
	protected float CurrentEscapeDistance;

	protected Vector3 TempVec;
	protected float DotProduct;

	private Dictionary<CreatureState,CreatureSound> SoundDict;
	private bool IsSoundPlaying;

	private Renderer [] Renderers;

	private Animator Animator;
	private CreatureAnimationState CurrentAnimationState = CreatureAnimationState.None;

	void Awake () {
		CurrentTarget = null;
		Renderers = GetComponentsInChildren<Renderer> ();
		Animator = GetComponentInChildren<Animator> ();

		if ( audio == null ) {
			gameObject.AddComponent<AudioSource> ();
		}
		audio.loop = false;
		audio.playOnAwake = false;

		SoundDict = new Dictionary<CreatureState , CreatureSound> ();
		foreach ( CreatureSound Sound in Sounds ) {
			SoundDict [ Sound.State ] = Sound;
		}
	}

	// Use this for initialization
	virtual protected void Start () {
		ChangeState ( CreatureState.Idle );
	}

	// Update is called once per frame
	virtual protected void Update () {
		CheckState ();
	}

	void OnCollisionEnter ( Collision Collision ) {
		if ( !IsDead ) {
			DoOnCollisionEnter ( Collision );
		}
	}

	public void LookAt ( GameObject Object ) {
		ChangeState ( CreatureState.Looking );

		CurrentTarget = Object;
	}

	public GameObject CurrentTarget { get; private set; }

	/// <summary>
	/// The current state of the creature.
	/// </summary>
	public CreatureState CurrentState { get; private set; }

	/// <summary>
	/// Indicates whether the creature just changed its state.
	/// </summary>
	public bool IsStateJustChanged { get; protected set; }

	/// <summary>
	/// Changes the state the creature is in. Does nothing if NewState is CurrentState.
	/// </summary>
	/// <param name="NewState">The state to change to.</param>
	protected void ChangeState ( CreatureState NewState , GameObject NewTarget = null ) {
		if ( NewState != CurrentState && NewState != CreatureState.Current && NewState != CreatureState.None ) {
			CurrentState = NewState;

			if ( CurrentState == CreatureState.Looking ||
				CurrentState == CreatureState.Escaping ||
				CurrentState == CreatureState.Chasing ) {

				if ( NewTarget ) {
					CurrentTarget = NewTarget;
				}
				else {
					Debug.LogError ( CurrentState + " state on " + name + "must have a non-null target!" );
				}
			}

			IsStateJustChanged = true;
		}
	}

	/// <summary>
	/// "Uses up" the state, effectively marking as not a new state change.
	/// </summary>
	protected void UseState () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;
		}
	}

	/// <summary>
	/// Indicates whether the creature is allowed to move.
	/// </summary>
	virtual protected bool CanMove {
		get {
			return true;
		}
	}

	/// <summary>
	/// Performs actions when the Creature is collided with.
	/// </summary>
	/// <param name="Collision"></param>
	virtual protected void DoOnCollisionEnter ( Collision Collision ) {
		if ( PlayerController.IsPlayerObject ( Collision.gameObject ) ) {
			ChangeState ( CreatureState.Dying );
			IsDead = true;

			Destroy ( gameObject , DestroyTime );
			GameManager.KillCreature ( this );

			ChangeMaterial ( BloodMaterial );
			PlayerController.SplatterBlood ();
			GameInput.Vibrate ();
		}
	}

	protected void ChangeMaterial ( Material NewMaterial ) {
		// Do nothing is NewMaterial is null
		if ( NewMaterial == null ) {
			return;
		}

		foreach ( Renderer Renderer in Renderers ) {
			Renderer.material = NewMaterial;
		}
	}

	/// <summary>
	/// Triggers the specified animation.
	/// </summary>
	/// <param name="AnimationState">The animation state to trigger to.</param>
	/// <param name="AnimationSpeed">The speed the new animation is to be played at.</param>
	protected void TriggerAnimation ( CreatureAnimationState AnimationState , float AnimationSpeed = 1.0f ) {
		if ( AnimationState != CurrentAnimationState ) {
			Animator.SetTrigger ( AnimationState.ToString () );
			Animator.speed = AnimationSpeed;

			CurrentAnimationState = AnimationState;
		}
	}

	protected bool PlaySound ( CreatureState State , float Volume = 1.0f , bool OverrideCurrent = false ) {
		// Don't play new sound if not overriding and there is a sound playing
		if ( !OverrideCurrent && IsSoundPlaying ) {
			return false;
		}

		if ( SoundDict.ContainsKey ( State ) ) {
			CreatureSound Sound = SoundDict [ State ];
			float Interval = Random.Range ( Sound.MinInterval , Sound.MaxInterval );

			return PlaySound ( Sound.Get () , Volume , Interval , OverrideCurrent );
		}
		else {
			return false;
		}
	}

	protected bool PlaySound ( AudioClip Clip , float Volume = 1.0f , float LoopDelay = 0.0f , bool OverrideCurrent = false ) {
		// Don't play new sound if not overriding and there is a sound playing
		if ( !OverrideCurrent && IsSoundPlaying ) {
			return false;
		}

		audio.PlayOneShot ( Clip , Volume );
		IsSoundPlaying = true;

		StartCoroutine ( OnSoundEnd ( Clip.length + LoopDelay ) );

		return true;
	}

	protected void CheckPlayerProximity () {
		// Infinity means not scared or not caring about detecting player
		if ( DetectionRange == Mathf.Infinity || GameManager.GetBonuses ().HasQuietFeet ) {
			return;
		}

		EscapeDir = transform.position - PlayerController.Position;
		EscapeDir.y = 0;

		// Escape if player is in frontal cone of sight and too near
		if ( EscapeDir.sqrMagnitude < DetectionRange * DetectionRange ) {
			// TempVec = local displacement from this creature

			TempVec = transform.InverseTransformDirection ( EscapeDir );
			DotProduct = Vector3.Dot ( TempVec , Vector3.forward );

			if ( DotProduct < 0 ) {
				TempVec.y = 0;

				ChangeState ( CreatureState.Escaping , PlayerController.Object );
			}
		}
	}

	#region STATE ACTIONS
	virtual protected void DoIdle () {
		if ( IsStateJustChanged ) {
			TriggerAnimation ( CreatureAnimationState.Idle );

			IsStateJustChanged = false;
		}

		CheckPlayerProximity ();

		PlaySound ( CreatureState.Idle );
	}

	virtual protected void DoLookAt () {
		UseState ();

		CheckPlayerProximity ();

		// TempVec = world displacement of target from this creature

		TempVec = CurrentTarget.transform.position - transform.position;
		TempVec.y = 0;

		// Idle if looking directly at player
		if ( Vector3.Angle ( transform.forward , TempVec ) == 0 ) {
			ChangeState ( CreatureState.Idle );

			return;
		}

		transform.forward = Vector3.RotateTowards (
			transform.forward ,
			TempVec ,
			MaxTurnSpeed * Time.deltaTime ,
			0.0f
		);
	}

	/// <summary>
	/// Tries to move the creature.
	/// </summary>
	virtual protected void TryMove () {
		if ( CanMove ) {
			transform.Translate ( 0 , 0 , Time.deltaTime * MoveSpeed , Space.Self );
		}
	}

	virtual protected void DoEscapeFrom () {
		CheckPlayerProximity ();

		if ( IsStateJustChanged ) {
			CurrentEscapeDistance = 0;

			PlaySound ( CreatureState.Escaping , 10.0f , true );
			TriggerAnimation ( CreatureAnimationState.Move );

			IsStateJustChanged = false;
		}

		transform.forward = Vector3.RotateTowards (
			transform.forward ,
			EscapeDir ,
			MaxTurnSpeed * Time.deltaTime ,
			0.0f
		);

		transform.Translate ( 0 , 0 , MoveSpeed * Time.deltaTime , Space.Self );
		CurrentEscapeDistance += MoveSpeed * Time.deltaTime;

		PlaySound ( CreatureState.Moving );

		if ( CurrentEscapeDistance >= EscapeDistance ) {
			ChangeState ( IsLookAtPlayerAfterEscape ? CreatureState.Looking : CreatureState.Idle , PlayerController.Object );
		}
	}

	virtual protected void DoChase () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			Debug.Log ( name + " is chasing " + CurrentTarget + "." );
		}
	}

	virtual protected void DoDying () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			PlaySound ( CreatureState.Dying , 5.0f , true );

			Debug.Log ( name + " is dying." );
		}
	}

	virtual protected void DoDead () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			Debug.Log ( name + " is dead." );
		}
	}
	#endregion

	/// <summary>
	/// Checks the creature's current state and performs the respective actions.
	/// </summary>
	private void CheckState () {
		switch ( CurrentState ) {
			case CreatureState.Idle: DoIdle (); break;

			case CreatureState.Looking: DoLookAt (); break;

			case CreatureState.Moving: TryMove (); break;
			case CreatureState.Escaping: DoEscapeFrom (); break;
			case CreatureState.Chasing: DoChase (); break;

			case CreatureState.Dying: DoDying (); break;
			case CreatureState.Dead: DoDead (); break;
		}
	}

	private IEnumerator OnSoundEnd ( float SoundDuration ) {
		yield return new WaitForSeconds ( SoundDuration );

		IsSoundPlaying = false;
	}

}