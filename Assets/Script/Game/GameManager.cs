using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Achievements {

	public enum KillCount {
		KILLING_SPREE = 3 ,
		DOMINATING = 4 ,
		MEGA_KILL = 5 ,
		UNSTOPPABLE = 6 ,
		WICKED_SICK = 7 ,
		MONSTER_KILL = 8 ,
		GOD_LIKE = 9 ,
		HOLY_SHIT = 10 ,
		OWNAGE = 11 ,
	}

	public static bool IsKillCountAchievement ( int KillCount ) {
		return System.Enum.IsDefined ( typeof ( KillCount ) , KillCount );
	}

}

[System.Serializable]
public class Bonuses {

	public const int MAX_SPEAR_LEVEL = 7;
	public const int MAX_MOVE_SPEED_MULTIPLIER = 20;

	public int MoveSpeedDuration = 10;
	public int QuietFeetDuration = 10;

	public AudioClip SpearLevelUpSound;

	public int SpearLevel { get; private set; }
	public float MoveSpeedMultiplier { get; private set; }
	public bool HasQuietFeet { get; private set; }

	private int MoveSpeedTimer;
	private int QuietFeetTimer;

	public Bonuses () {
		SpearLevel = 1;
		MoveSpeedMultiplier = 1;
		HasQuietFeet = false;

		ResetMoveSpeed ();
		ResetQuietFeet ();
	}

	public int GetMoveSpeedTimeRemaining () {
		return MoveSpeedTimer;
	}

	public int GetQuietFeetTimeRemaining () {
		return QuietFeetTimer;
	}

	public void IncreaseSpearLevel () {
		if ( SpearLevel >= MAX_SPEAR_LEVEL ) {
			return;
		}

		SpearLevel++;
		PlayerController.GlowSpear ();
	}

	public IEnumerator IncreaseMoveSpeedLevel () {
		if ( MoveSpeedMultiplier >= MAX_MOVE_SPEED_MULTIPLIER ) {
			yield break;
		}

		MoveSpeedMultiplier++;

		while ( MoveSpeedTimer > 0 ) {
			yield return new WaitForSeconds ( 1.0f );

			MoveSpeedTimer--;
		}

		ResetMoveSpeed ();
	}

	public IEnumerator EnableQuietFeet () {
		if ( HasQuietFeet ) {
			yield break;
		}

		HasQuietFeet = true;

		while ( QuietFeetTimer > 0 ) {
			yield return new WaitForSeconds ( 1.0f );

			QuietFeetTimer--;
		}

		ResetQuietFeet ();
	}

	private void ResetMoveSpeed () {
		MoveSpeedMultiplier = 1;
		MoveSpeedTimer = MoveSpeedDuration;
	}

	private void ResetQuietFeet () {
		HasQuietFeet = false;
		QuietFeetTimer = QuietFeetDuration;
	}

}

public class GameManager : Singleton<GameManager> {

	private const string CREATURE_CONTAINER_NAME = "Creatures";

	public int NumCreaturesToSpawn = 10;
	public float SpawnInterval = 5.0f;

	public Creature [] Creatures;

	public Bonuses Bonuses;

	private Transform CreatureContainer;

	private LinkedList<Creature> SpawnedCreatures;
	private List<Creature> DeadCreatures;

	private GameObject TempGameObject;
	private Vector3 TempPos;
	private Quaternion TempRot;

	private Dictionary<CreatureType,int> DeadCreatureCount;

	// Use this for initialization
	void Start () {
		SpawnedCreatures = new LinkedList<Creature> ();
		DeadCreatures = new List<Creature> ();
		DeadCreatureCount = new Dictionary<CreatureType , int> ();
		foreach ( CreatureType Type in System.Enum.GetValues ( typeof ( CreatureType ) ) ) {
			DeadCreatureCount.Add ( Type , 0 );
		}

		TempGameObject = GameObject.Find ( CREATURE_CONTAINER_NAME );
		if ( TempGameObject == null ) {
			CreatureContainer = new GameObject ( CREATURE_CONTAINER_NAME ).transform;
		}
		else {
			CreatureContainer = TempGameObject.transform;
		}

		SpawnInitialCreatures ();
		StartCoroutine ( RespawnCreaturesAfterDelay () );
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

	public static Bonuses GetBonuses () {
		return Instance.Bonuses;
	}

	/// <summary>
	/// Kills the specified creature.
	/// </summary>
	/// <param name="Creature">Returns whether the killing was successful (a dead or non-existing creature cannot be killed).</param>
	/// <returns></returns>
	public static bool KillCreature ( Creature Creature ) {
		LinkedListNode<Creature> CreatureEntry = Instance.SpawnedCreatures.Find ( Creature );

		if ( CreatureEntry != null ) {
			Instance.SpawnedCreatures.Remove ( Creature );
			Instance.DeadCreatures.Add ( CreatureEntry.Value );
			Instance.DeadCreatureCount [ Creature.Type ]++;
			Instance.CheckBonuses ();

			return true;
		}
		else {
			return false;
		}
	}

	private void SpawnInitialCreatures () {
		// Add existing creatures to list
		Creature Creature;
		foreach ( GameObject CreatureObject in GameObject.FindGameObjectsWithTag ( Creature.TAG ) ) {
			Creature = CreatureObject.GetComponent<Creature> ();
			if ( Creature != null ) {
				SpawnedCreatures.AddLast ( Creature );
			}
		}

		// Spawn new ones
		RespawnCreatures ();
	}

	IEnumerator RespawnCreaturesAfterDelay () {
		while ( true ) {
			yield return new WaitForSeconds ( SpawnInterval );

			RespawnCreatures ();
		}
	}

	private void RespawnCreatures () {
		for ( int i = 0 ; i < NumCreaturesToSpawn - NumAliveCreatures ; i++ ) {
			TempPos.z = Random.Range ( 0 , 100 );
			TempPos.x = Random.Range ( -50 , 50 );

			TempRot.eulerAngles = new Vector3 ( 0 , Random.Range ( 0 , 360.0f ) , 0 );

			TempGameObject = ( Instantiate ( Creatures [ Random.Range ( 0 , Creatures.Length ) ] , TempPos , TempRot ) as Creature ).gameObject;
			TempGameObject.transform.parent = CreatureContainer;
			SpawnedCreatures.AddLast ( TempGameObject.GetComponent<Creature> () );
		}
	}

	private void CheckBonuses () {
		Debug.Log ( "Check Bonuses called" );

		// First kill, then every 3 kills
		if ( Bonuses.SpearLevel < Bonuses.MAX_SPEAR_LEVEL && Bonuses.SpearLevel * 3 - 2 == DeadCreatureCount [ CreatureType.Elephant ] ) {
			Bonuses.IncreaseSpearLevel ();
			audio.PlayOneShot ( Bonuses.SpearLevelUpSound );
		}

		// Walk speed increase every 3 hippo kills
		if ( IsLastCreatureKilledType ( CreatureType.Hippo ) && DeadCreatureCount [ CreatureType.Hippo ] % 3 == 0 && Bonuses.MoveSpeedMultiplier == 1 ) {
			StartCoroutine ( Bonuses.IncreaseMoveSpeedLevel () );
		}

		// Quiet foot every 3 bunny kills
		if ( IsLastCreatureKilledType ( CreatureType.Bunny ) && DeadCreatureCount [ CreatureType.Bunny ] % 3 == 0 ) {
			StartCoroutine ( Bonuses.EnableQuietFeet () );
		}
	}

	private bool IsLastCreatureKilledType ( CreatureType Type ) {
		return DeadCreatures [ DeadCreatures.Count - 1 ].Type == Type;
	}

}