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
	public const int MAX_EXP_BEFORE_LEVEL_UP = 100;
	public int ExpCounter = 0;
	public float ExpRatio {
		get {
			return ( float ) ExpCounter / ( float ) MAX_EXP_BEFORE_LEVEL_UP;
		}
	}

	public int MoveSpeedDuration = 10;
	public int QuietFeetDuration = 10;

	public AudioClip SpearLevelUpSound;

	public delegate void OnSpearLevelUpHandler ();
	public event OnSpearLevelUpHandler OnSpearLevelUp;

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

	public void AddExp ( int exp ) {
		ExpCounter += exp;

		if ( ExpCounter > MAX_EXP_BEFORE_LEVEL_UP ) {
			ExpCounter = 0;
			IncreaseSpearLevel ();
		}
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

		if ( OnSpearLevelUp != null ) {
			OnSpearLevelUp ();
		}
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

	public AudioSource BGMSource;
	public float TimeBeforeFadeBGM = 5.0f;

	private Transform CreatureContainer;

	private LinkedList<Creature> SpawnedCreatures;
	private List<Creature> DeadCreatures;

	private GameObject TempGameObject;
	private Vector3 TempPos;
	private Quaternion TempRot;

	private Dictionary<CreatureType,int> DeadCreatureCount;
	private float LastKilledCreatureTime;

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

		Bonuses.OnSpearLevelUp += PlayerController.GlowSpear;
		Bonuses.OnSpearLevelUp += PlayUpgradeSpearSound;

		SpawnInitialCreatures ();
		StartCoroutine ( RespawnCreaturesAfterDelay () );
	}

	// Update is called once per frame
	void Update () {
		CheckFadeBGM ();
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

			if ( !Instance.BGMSource.isPlaying ) {
				Instance.BGMSource.volume = 1;
				Instance.BGMSource.Play ();
			}
			Instance.LastKilledCreatureTime = Instance.TimeBeforeFadeBGM;

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
			TempPos.z = Random.Range ( 0 , 100 ) + PlayerController.Position.z;
			TempPos.x = Random.Range ( -50 , 50 ) + PlayerController.Position.x;

			TempRot.eulerAngles = new Vector3 ( 0 , Random.Range ( 0 , 360.0f ) , 0 );

			int CreatureIndex = Random.Range ( 0 , Creatures.Length );

			if ( Terrain.activeTerrain.SampleHeight ( TempPos ) < 18.5f ) {
				CreatureIndex = 5;
			}
			else if ( CreatureIndex == 5 ) {
				CreatureIndex = Random.Range ( 0 , 5 );
			}

			TempGameObject = ( Instantiate ( Creatures [ CreatureIndex ] , TempPos , TempRot ) as Creature ).gameObject;
			TempGameObject.transform.parent = CreatureContainer;
			SpawnedCreatures.AddLast ( TempGameObject.GetComponent<Creature> () );
		}
	}

	private void CheckFadeBGM () {
		if ( !BGMSource.isPlaying ) {
			return;
		}

		if ( LastKilledCreatureTime > 0 ) {
			LastKilledCreatureTime -= Time.deltaTime;
		}

		if ( LastKilledCreatureTime <= 0 ) {
			if ( BGMSource.volume > 0 ) {
				BGMSource.volume -= Time.deltaTime / 2;
			}
			else {
				BGMSource.Stop ();
			}
		}
	}

	private void PlayUpgradeSpearSound () {
		audio.PlayOneShot ( Bonuses.SpearLevelUpSound );
	}

	private void CheckBonuses () {
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