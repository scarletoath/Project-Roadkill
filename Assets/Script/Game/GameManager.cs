﻿using System.Collections.Generic;
using System.Collections;
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
	const int MAX_WALK_SPEED = 20;

	int[] deadCreatures;
	static int numCreatureTypes = System.Enum.GetNames(typeof(CreatureType)).Length;

	const int MAX_SPAWN_CREATURES = 10;


	public static BonusValues bonuses;

	// Use this for initialization
	void Start () {
		SpawnedCreatures = new LinkedList<Creature> ();
		DeadCreatures = new List<Creature> ();
		bonuses = new BonusValues ();
		deadCreatures = new int[numCreatureTypes];

		TempGameObject = GameObject.Find ( CREATURE_CONTAINER_NAME );
		if ( TempGameObject == null ) {
			CreatureContainer = new GameObject ( CREATURE_CONTAINER_NAME ).transform;
		}
		else {
			CreatureContainer = TempGameObject.transform;
		}

		SpawnCreatures ();
		StartCoroutine (respawnCreatures ());
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

			Instance.deadCreatures[(int)Creature.Type]++;
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

	IEnumerator respawnCreatures()
	{
		while (true) 
		{
			yield return new WaitForSeconds(5.0f);
			regenerateCreatures();
		}
	}

	private void regenerateCreatures()
	{
		for (int i=0; i<MAX_SPAWN_CREATURES - NumAliveCreatures; i++) 
		{
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
		if ( (deadCreatures[(int)CreatureType.Elephant] == 1 && bonuses.spearScale == 1) ||
		    (deadCreatures[(int)CreatureType.Elephant] == 4 && bonuses.spearScale == 2) ||
		    (deadCreatures[(int)CreatureType.Elephant] == 8 && bonuses.spearScale == 3) )
		{
			LevelUpSpear();
		}

		if (deadCreatures [(int)CreatureType.Bird] == 1 && bonuses.walkSpeed == 1) 
		{
			LevelUpWSPD();
		}

		if (deadCreatures [(int)CreatureType.Bunny] == 1) 
		{
			ActivateQuietFoot();
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
		if (bonuses.walkSpeed >= MAX_WALK_SPEED)
						return;
		bonuses.walkSpeed++;
	}

	void ActivateQuietFoot()
	{
		StartCoroutine (QuietFootCountdown ());
	}

	IEnumerator QuietFootCountdown()
	{
		bonuses.quietFeet = true;
		yield return new WaitForSeconds(10.0f);
		bonuses.quietFeet = false;
	}
}