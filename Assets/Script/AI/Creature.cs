using UnityEngine;

public enum CreatureState {

	/// <summary>
	/// An invalid state.
	/// </summary>
	None ,

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

public abstract class Creature : MonoBehaviour {

	public const string TAG = "Creature";

	public float MoveSpeed = 1.0f;
	public float MaxTurnSpeed = 1.57f;

	public float DestroyTime = 1.0f;

	public Material BloodMaterial;

	protected bool IsDead = false;

	private Renderer [] Renderers;
	private Animator Animator;

	void Awake () {
		CurrentTarget = null;
		Renderers = GetComponentsInChildren<Renderer> ();
		Animator = GetComponentInChildren<Animator> ();
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
		DoOnCollisionEnter ( Collision );
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
		if ( NewState != CurrentState ) {
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
		Debug.Log ( Collision.gameObject.name + " hit a CreatureBase!" );
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
		Animator.SetTrigger ( AnimationState.ToString () );
		Animator.speed = AnimationSpeed;
	}

	#region STATE ACTIONS
	virtual protected void DoIdle () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			Debug.Log ( name + " is idling." );
		}
	}

	virtual protected void DoLookAt () {
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			Debug.Log ( name + " is looking at " + CurrentTarget + "." );
		}
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
		if ( IsStateJustChanged ) {
			IsStateJustChanged = false;

			Debug.Log ( name + " is escaping from " + CurrentTarget + "." );
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

}