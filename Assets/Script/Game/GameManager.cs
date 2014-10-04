using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	private const string CREATURE_CONTAINER_NAME = "Creatures";

	public int NumCreaturesToSpawn = 10;

	public Creature [] Creatures;

	private Transform CreatureContainer;

	private LinkedList<Creature> SpawnedCreatures;
	private List<Creature> DeadCreatures;

	private GameObject TempGameObject;
	private Vector3 TempPos;
	private Quaternion TempRot;

	public class BonusValues
	{
		public int spearScale = 1;
		public float walkSpeed = 1;
		public bool quietFeet = false;
	}

	const int MAX_SPEAR_LEVEL = 7;
	const int MAX_WALK_SPEED = 7;

	public static BonusValues bonuses;

	// Use this for initialization
	void Start () {
		SpawnedCreatures = new LinkedList<Creature> ();
		DeadCreatures = new List<Creature> ();
		bonuses = new BonusValues ();

		TempGameObject = GameObject.Find ( CREATURE_CONTAINER_NAME );
		if ( TempGameObject == null ) {
			CreatureContainer = new GameObject ( CREATURE_CONTAINER_NAME ).transform;
		}
		else {
			CreatureContainer = TempGameObject.transform;
		}

		SpawnCreatures ();
	}

	// Update is called once per frame
	void Update () {

	}

	public static int NumTotalCreatures {
		get {
			return NumAliveCreatures + NumDeadCreatures;
		}
	}

	public static int NumAliveCreatures {
		get {
			return Instance.SpawnedCreatures.Count;
		}
	}

	public static int NumDeadCreatures {
		get {
			return Instance.DeadCreatures.Count;
		}
	}

	/// <summary>
	/// Kills the specified creature.
	/// </summary>
	/// <param name="Creature">Returns whether the killing was successful (a dead or non-existing creature cannot be killed).</param>
	/// <returns></returns>
	public static bool KillCreature ( Creature Creature ) {
		LinkedListNode<Creature> CreatureEntry = Instance.SpawnedCreatures.Find ( Creature );

		if ( CreatureEntry != null ) {
			Debug.Log ("killcreature");

			Instance.SpawnedCreatures.Remove ( Creature );
			Instance.DeadCreatures.Add ( CreatureEntry.Value );
			Instance.CheckBonuses();

			return true;
		}
		else {
			return false;
		}
	}

	private void SpawnCreatures () {
		// Add existing creatures to list
		Creature Creature;
		foreach ( GameObject CreatureObject in GameObject.FindGameObjectsWithTag ( Creature.TAG ) ) {
			Creature = CreatureObject.GetComponent<Creature> ();
			if ( Creature != null ) {
				SpawnedCreatures.AddLast ( Creature );
			}
		}

		// Spawn new ones
		for ( int i = 0 ; i < NumCreaturesToSpawn ; i++ ) {
			TempPos.z = Random.Range ( 0 , 500 );
			TempPos.x = 10 * Mathf.Sin ( Mathf.Deg2Rad * TempPos.z );

			TempRot.eulerAngles = new Vector3 ( 0 , Random.Range ( 0 , 360.0f ) , 0 );

			TempGameObject = ( Instantiate ( Creatures [ Random.Range ( 0 , Creatures.Length ) ] , TempPos , TempRot ) as Creature ).gameObject;
			TempGameObject.transform.parent = CreatureContainer;
			SpawnedCreatures.AddLast(TempGameObject.GetComponent<Creature>());
		}
	}

	void CheckBonuses()
	{
		Debug.Log ("Check Bonuses called");
		if (DeadCreatures.Count == 3 || DeadCreatures.Count == 5 || DeadCreatures.Count == 7) 
		{
			LevelUpSpear();
		}
		
	}

	void LevelUpSpear()
	{
		if (bonuses.spearScale >= MAX_SPEAR_LEVEL)
						return;

		bonuses.spearScale++;
		PlayerController.GlowSpear ();
	}

	void LevelUpWSPD()
	{

	}


}