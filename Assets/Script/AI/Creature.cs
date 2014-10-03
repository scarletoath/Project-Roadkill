using UnityEngine;

public abstract class Creature : MonoBehaviour {

	public const string TAG = "Creature";

	public float MoveSpeed = 1.0f;

	protected bool IsDead = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		TryMove ();
	}

	void OnCollisionEnter ( Collision Collision ) {
		DoOnCollisionEnter ( Collision );
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

	/// <summary>
	/// Tries to move the creature.
	/// </summary>
	virtual protected void TryMove () {
		if ( CanMove ) {
			transform.Translate ( 0 , 0 , Time.deltaTime * MoveSpeed , Space.Self );
		}
	}

}