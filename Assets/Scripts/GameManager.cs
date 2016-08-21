using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float turnDelay = 0.1f;							//Delay between each Player turn.
	public int healthPoints = 100;							//Starting value for Player health points.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.

	private BoardManager boardScript;
	private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
	private List<Follow> npc;
	private bool enemiesMoving;								//Boolean to check if enemies are moving.
	private DungeonManager dungeonScript;					
	private Player playerScript;
	private bool playerInDungeon;							//bool that informs whether player is in the dungeon

	public bool enemiesFaster = false;						//boolean to check if enemies should be faster
	public bool enemiesSmarter = false;						//boolean to check if enemies should be smarter
	public int enemySpawnRatio = 20;						//how often enemies should be spawn, ex. 1 out of 20 tiles

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			
			//if not, set instance to this
			instance = this;
		
		//If instance already exists and it's not this:
		else if (instance != this)
			
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);	
		
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
		
		//Assign enemies to a new List of Enemy objects.
		enemies = new List<Enemy>();
		npc = new List<Follow>();

		boardScript = GetComponent<BoardManager>();

		dungeonScript = GetComponent<DungeonManager>();

		playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		
		//Call the InitGame function to initialize the first level 
		InitGame();
	}
	
	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		//Call InitGame to initialize our level.
		InitGame();
	}
	
	//Initializes the game for each level.
	void InitGame()
	{
		//Clear any Enemy objects in our List to prepare for next level.
		enemies.Clear();

		//create the board
		boardScript.BoardSetup();

		//player is on world board, not in the dungeon when starting the game
		playerInDungeon = false;
	}
	
	//Update is called every frame.
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving)
			
			//If any of these are true, return and do not start MoveEnemies.
			return;
		
		//Start moving enemies.
		StartCoroutine (MoveEnemies ());

		//start moving enemies following the player
		if(npc.Count != 0)
		{
			StartCoroutine(MoveNPC());
		}
	}
	
	//GameOver is called when the player reaches 0 health points
	public void GameOver()
	{
		//Disable this GameManager.
		enabled = false;
	}

	//function that adds enemies to the list
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);

		//make the background music more tense
		SoundManager.instance.FormAudio(true);
	}

	public void AddNPCToList(Follow script)
	{
		npc.Add(script);
	}

	//function that removes enemies from the list
	public void RemoveEnemyFromList(Enemy script)
	{
		enemies.Remove(script);
		if(enemies.Count == 0)
		{
			SoundManager.instance.FormAudio(false);
		}
	}
	
	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		//it is enemies' turn, not player's
		enemiesMoving = true;

		yield return new WaitForSeconds(turnDelay);

		//if there are noe enemies, wait for the end of the turn
		if(enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		//create a list of enemies to destroy - not visible on camera or those who entered black spots
		List<Enemy> enemiesToDestroy = new List<Enemy>();
		for(int i = 0; i < enemies.Count; i++)
		{
			//check if player is in dungeon
			if(playerInDungeon)
			{
				//if enemy's sprite renderer is not visible
				if(!enemies[i].getSpriteRenderer().isVisible)
				{
					if(i == enemies.Count -1)
					{
						yield return new WaitForSeconds(enemies[i].moveTime);
					}
					continue;
				}
			}
			else
			{
				//if enemy's sprite renderer is not visible and its position is not visible on world board
				if((!enemies[i].getSpriteRenderer().isVisible) || (!boardScript.checkValidTile(enemies[i].transform.position)))
				{
					//destroy that specific enemy
					enemiesToDestroy.Add(enemies[i]);
					continue;
				}
			}

			enemies[i].MoveEnemy();

			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;

		for(int i = 0; i < enemiesToDestroy.Count; i++)
		{
			enemies.Remove(enemiesToDestroy[i]);
			Destroy(enemiesToDestroy[i].gameObject);
		}

		enemiesToDestroy.Clear();
	}

	IEnumerator MoveNPC()
	{
		npc[0].MoveNPC();

		yield return new WaitForSeconds(npc[0].moveTime);

	}

	//update the board by adding new tiles
	public void updateBoard(int horizontal, int vertical)
	{
		boardScript.addToBoard(horizontal,vertical);
	}

	//function that runs when player enters the dungeon
	public void enterDungeon()
	{
		
		dungeonScript.StartDungeon();
		boardScript.setDungeonBoard(dungeonScript.gridPositions,dungeonScript.maxBound,dungeonScript.endPos);
		playerScript.dungeonTransition = false;
		playerInDungeon = true;

		for(int i = 0; i < enemies.Count; i++)
		{
			Destroy(enemies[i].gameObject);
		}

		enemies.Clear();
			
	}

	public void exitDungeon() //function that runs when exiting the dungeon
	{
		boardScript.SetWorldBoard(); //build the world board
		playerScript.dungeonTransition = false;
		playerInDungeon = false; //player is not more in the dungeon
		enemies.Clear(); //clear the list of all enemies

		SoundManager.instance.FormAudio(false); //make the music less tense
	}

}