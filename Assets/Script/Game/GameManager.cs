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

public class GameManager : Singleton<GameManager> {

	public class BonusValues {
		public int spearScale = 1;
		public float walkSpeed = 1;
		public bool quietFeet = false;
	}

	private const string CREATURE_CONTAINER_NAME = "Creatures";

	private const int MAX_SPEAR_LEVEL = 7;
	private const int MAX_WALK_SPEED = 20;

	public int NumCreaturesToSpawn = 20;

	public Creature [] Creatures;

	private Transform CreatureContainer;

	private LinkedList<Creature> SpawnedCreatures;
	private List<Creature> DeadCreatures;

	private GameObject TempGameObject;
	private Vector3 TempPos;
	private Quaternion TempRot;

	private Dictionary<CreatureType,int> DeadCreatureCount;

	public static BonusValues bonuses;

	// Use this for initialization
	void Start () {
		SpawnedCreatures = new LinkedList<Creature> ();
		DeadCreatures = new List<Creature> ();
		bonuses = new BonusValues ();
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

		SpawnCreatures ();
		StartCoroutine ( respawnCreatures () );
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
        regenerateCreatures();
	}

	IEnumerator respawnCreatures () {
		while ( true ) {
			yield return new WaitForSeconds ( 5.0f );
			regenerateCreatures ();
		}
	}

	private void regenerateCreatures () {
		for ( int i=0 ; i < NumCreaturesToSpawn - NumAliveCreatures ; i++ ) {
			TempPos.z = Random.Range (20 , 220 );
            TempPos.x = Random.Range(-100, 200);

			TempRot.eulerAngles = new Vector3 ( 0 , Random.Range ( 0 , 360.0f ) , 0 );

            if (Terrain.activeTerrain.SampleHeight(TempPos) < 18.5f) ;


			TempGameObject = ( Instantiate ( Creatures [ Random.Range ( 0 , Creatures.Length ) ] , TempPos , TempRot ) as Creature ).gameObject;
			TempGameObject.transform.parent = CreatureContainer;
			SpawnedCreatures.AddLast ( TempGameObject.GetComponent<Creature> () );
		}
	}

	void CheckBonuses () {
		Debug.Log ( "Check Bonuses called" );

		// First kill, then every 3 kills
		if ( bonuses.spearScale < MAX_SPEAR_LEVEL && bonuses.spearScale * 3 - 2 == DeadCreatureCount [ CreatureType.Elephant ] ) {
			LevelUpSpear ();
		}

		// Walk speed increase every 3 hippo kills
		if ( DeadCreatureCount [ CreatureType.Hippo ] > 0 && DeadCreatureCount [ CreatureType.Hippo ] % 3 == 0 && bonuses.walkSpeed == 1 ) {
			LevelUpWSPD ();
		}

		// Quiet foot every 3 bunny kills
		if ( DeadCreatureCount [ CreatureType.Bunny ] > 0 && DeadCreatureCount [ CreatureType.Bunny ] % 3 == 0 ) {
			ActivateQuietFoot ();
		}

		// Achievements
		if ( Achievements.IsKillCountAchievement ( NumDeadCreatures ) ) {
			CameraUIScript.achievements = ( ( Achievements.KillCount ) NumDeadCreatures ).ToString ().Replace ( '_' , ' ' );
		}
	}

	void LevelUpSpear () {
		if ( bonuses.spearScale >= MAX_SPEAR_LEVEL )
			return;

		bonuses.spearScale++;
		CameraUIScript.speartext = "Spear\nLevel\n" + bonuses.spearScale;
		PlayerController.GlowSpear ();
	}

	void LevelUpWSPD () {
		if ( bonuses.walkSpeed >= MAX_WALK_SPEED )
			return;
		bonuses.walkSpeed++;
		CameraUIScript.WSPDtext = "WSPD\nx" + bonuses.walkSpeed + "\n";
		StartCoroutine ( WSPDCountdown () );
	}

	void ActivateQuietFoot () {
		CameraUIScript.quietfoottext = "Sneak\nOn\n";
		StartCoroutine ( QuietFootCountdown () );
	}

	IEnumerator QuietFootCountdown () {
		if ( bonuses.quietFeet ) yield break;
		bonuses.quietFeet = true;
		string txt = CameraUIScript.quietfoottext;
		for ( int i = 10 ; i >= 1 ; i-- ) {
			CameraUIScript.quietfoottext = txt + i;
			yield return new WaitForSeconds ( 1.0f );

		}
		CameraUIScript.quietfoottext = "Sneak\nOff\n";
		bonuses.quietFeet = false;
	}

	IEnumerator WSPDCountdown () {
		string txt = CameraUIScript.WSPDtext;
		for ( int i = 10 ; i >= 1 ; i-- ) {
			CameraUIScript.WSPDtext = txt + i;
			yield return new WaitForSeconds ( 1.0f );

		}

		CameraUIScript.WSPDtext = "WSPD\nx1";
		bonuses.walkSpeed = 1;
	}

}
