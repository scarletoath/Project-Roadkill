using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class Achievements {

	public enum KillCount {
		FIRST_BLOOD = 1 ,
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

	public AudioClip[] announcerSounds;
	private bool isplaying = false;

	public AudioClip GetAnnouncerSound ( int index ) {
		return announcerSounds [ index ];
	}

	public static bool IsKillCountAchievement ( int KillCount ) {
		return System.Enum.IsDefined ( typeof ( KillCount ) , KillCount );
	}

}

[System.Serializable]
public class Bonuses {

	public const int MAX_SPEAR_LEVEL = 4;
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

	public GameObject [] SpearPrefabs;
	public AudioClip [] SpearSounds;

	public AudioClip SpearLevelUpSound;

	public delegate void OnSpearLevelUpHandler ();
	public event OnSpearLevelUpHandler OnSpearLevelUp;

	public delegate void OnIncreaseExpHandler ();
	public event OnIncreaseExpHandler OnIncreaseExp;

	public delegate void OnQuietFeetChangeHandler ( bool IsBonusActive );
	public event OnQuietFeetChangeHandler OnQuietFeetChange;

	public delegate void OnMoveSpeedChangeHandler ( bool IsBonusActive );
	public event OnMoveSpeedChangeHandler OnMoveSpeedChange;

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
		if ( SpearLevel >= MAX_SPEAR_LEVEL ) {
			ExpCounter = 0;
			return;
		}

		ExpCounter += exp;

		if ( ExpCounter >= MAX_EXP_BEFORE_LEVEL_UP ) {
			ExpCounter = 0;
			IncreaseSpearLevel ();
		}
		else {
			if ( OnIncreaseExp != null ) {
				OnIncreaseExp ();
			}
		}
	}

	public GameObject GetCurrentSpearPrefab () {
		return SpearPrefabs [ SpearLevel - 1 ];
	}

	public AudioClip GetCurrentSpearSound () {
		return SpearSounds [ SpearLevel - 1 ];
	}

	public int GetMoveSpeedTimeRemaining () {
		return MoveSpeedTimer;
	}

	public int GetQuietFeetTimeRemaining () {
		return QuietFeetTimer;
	}

	public void IncreaseSpearLevel () {
		if ( SpearLevel >= MAX_SPEAR_LEVEL ) {
			ExpCounter = 0;
			return;
		}

		SpearLevel++;

		if ( OnSpearLevelUp != null ) {
			OnSpearLevelUp ();
		}
	}

	public IEnumerator IncreaseMoveSpeedLevel () {
		if ( MoveSpeedMultiplier >= MAX_MOVE_SPEED_MULTIPLIER ) {
			yield break;
		}

		MoveSpeedMultiplier++;

		if ( OnMoveSpeedChange != null ) {
			OnMoveSpeedChange ( true );
		}

		while ( MoveSpeedTimer > 0 ) {
			yield return new WaitForSeconds ( 1.0f );

			MoveSpeedTimer--;
		}

		ResetMoveSpeed ();

		if ( OnMoveSpeedChange != null ) {
			OnMoveSpeedChange ( false );
		}
	}

	public IEnumerator EnableQuietFeet () {
		if ( HasQuietFeet ) {
			yield break;
		}

		HasQuietFeet = true;

		if ( OnQuietFeetChange != null ) {
			OnQuietFeetChange ( true );
		}

		while ( QuietFeetTimer > 0 ) {
			yield return new WaitForSeconds ( 1.0f );

			QuietFeetTimer--;
		}

		ResetQuietFeet ();

		if ( OnQuietFeetChange != null ) {
			OnQuietFeetChange ( false );
		}
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

	private const string KEY_LIFETIME_KILLED_CREATURES_COUNT = "TotalKills";

	public int NumCreaturesToSpawn = 10;
	public float SpawnInterval = 5.0f;

	public Creature [] Creatures;

	public Bonuses Bonuses;

	public Achievements achievements;
	private bool isplaying = false;
	private int currentAnnouncerSound = -1;

	public AudioSource BGMSource;
	public float TimeBeforeFadeBGM = 5.0f;

	private Transform CreatureContainer;

	private LinkedList<Creature> SpawnedCreatures;
	private List<Creature> DeadCreatures;

	private GameObject TempGameObject;
	private Vector3 TempPos;
	private Quaternion TempRot;

	private Dictionary<CreatureType,int> DeadCreatureCount;
	private int LifetimeKilledCreaturesCount;
	private float LastKilledCreatureTime;

    public GameObject BloodSplatter;

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

		if ( PlayerPrefs.HasKey ( KEY_LIFETIME_KILLED_CREATURES_COUNT ) ) {
			LifetimeKilledCreaturesCount = PlayerPrefs.GetInt ( KEY_LIFETIME_KILLED_CREATURES_COUNT );
		}
		else {
			PlayerPrefs.SetInt ( KEY_LIFETIME_KILLED_CREATURES_COUNT , 0 );
		}

		Bonuses.OnSpearLevelUp += PlayerController.UpdateSpear;
		Bonuses.OnSpearLevelUp += PlayUpgradeSpearSound;

		SpawnInitialCreatures ();
		StartCoroutine ( RespawnCreaturesAfterDelay () );
	}

	// Update is called once per frame
	void Update () {
		CheckFadeBGM ();
	}

	void OnApplicationQuit () {
		LifetimeKilledCreaturesCount += NumDeadCreatures;
		PlayerPrefs.SetInt ( KEY_LIFETIME_KILLED_CREATURES_COUNT , LifetimeKilledCreaturesCount );
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

	public static int NumLifetimeKilledCreatures {
		get {
			return Instance.LifetimeKilledCreaturesCount;
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
			Instance.CheckAndPlayAnnouncerSounds ();
            Instance.makeSplatter(Creature.gameObject.transform.position);

			Instance.audio.PlayOneShot ( Instance.Bonuses.GetCurrentSpearSound () , 15 );

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

    private void makeSplatter(Vector3 pos)
    {
        if (Terrain.activeTerrain.SampleHeight(pos) > 19.9 &&
            Terrain.activeTerrain.SampleHeight(pos + Vector3.forward * 5f ) > 19.9 &&
            Terrain.activeTerrain.SampleHeight(pos + Vector3.back * 5f) > 19.9 &&
            Terrain.activeTerrain.SampleHeight(pos + Vector3.right * 5f) > 19.9 &&
            Terrain.activeTerrain.SampleHeight(pos + Vector3.left * 5f) > 19.9)
        {
            Vector3 splatterpos = pos;
            splatterpos.y = 0.0001f;
            Instantiate(Instance.BloodSplatter, splatterpos, Quaternion.Euler(90, 0, 0));
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
			TempPos.z = Random.Range ( 10 , 100 ) + PlayerController.Position.z;
			TempPos.x = Random.Range ( 10 , 60 ) * flip () + PlayerController.Position.x;



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

	private int flip () {
		return Random.Range ( 0 , 2 ) == 1 ? 1 : -1;
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
		audio.PlayOneShot ( Bonuses.SpearLevelUpSound , 5 );
	}

	private void CheckBonuses () {
		// Walk speed increase every 3 hippo kills
		if ( IsLastCreatureKilledType ( CreatureType.Hippo ) && DeadCreatureCount [ CreatureType.Hippo ] % 3 == 0 && Bonuses.MoveSpeedMultiplier == 1 ) {
			StartCoroutine ( Bonuses.IncreaseMoveSpeedLevel () );
		}

		// Quiet foot every 3 bunny kills
		if ( IsLastCreatureKilledType ( CreatureType.Bird ) && DeadCreatureCount [ CreatureType.Bird ] % 3 == 0 ) {
			StartCoroutine ( Bonuses.EnableQuietFeet () );
		}
	}


	private void CheckAndPlayAnnouncerSounds () {
		if ( Achievements.IsKillCountAchievement ( GameManager.NumDeadCreatures ) )
			switch ( ( Achievements.KillCount ) GameManager.NumDeadCreatures ) {
				case Achievements.KillCount.FIRST_BLOOD:
					StartCoroutine ( playAnnouncerSound ( 0 ) );
					break;
				case Achievements.KillCount.KILLING_SPREE:
				case Achievements.KillCount.DOMINATING:
				case Achievements.KillCount.MEGA_KILL:
				case Achievements.KillCount.UNSTOPPABLE:
				case Achievements.KillCount.WICKED_SICK:
				case Achievements.KillCount.MONSTER_KILL:
				case Achievements.KillCount.GOD_LIKE:
				case Achievements.KillCount.HOLY_SHIT:
				case Achievements.KillCount.OWNAGE:
					StartCoroutine ( playAnnouncerSound ( GameManager.NumDeadCreatures + 2 ) );
					break;
				default:
					break;


			}

		if ( GameManager.NumDeadCreatures > 11 )
			StartCoroutine ( playAnnouncerSound ( 13 ) );
	}

	IEnumerator playAnnouncerSound ( int sound_index ) {
		if ( isplaying && currentAnnouncerSound == sound_index ) yield break;
		AudioClip sound = achievements.GetAnnouncerSound ( sound_index );
		this.audio.PlayOneShot ( sound , 15.0f );
		isplaying = true;
		currentAnnouncerSound = sound_index;
		yield return new WaitForSeconds ( sound.length );
		isplaying = false;
		currentAnnouncerSound = -1;
	}

	private bool IsLastCreatureKilledType ( CreatureType Type ) {
		return DeadCreatures [ DeadCreatures.Count - 1 ].Type == Type;
	}

	private void ClearStoredData () {
		PlayerPrefs.DeleteAll ();
	}

}