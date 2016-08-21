using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

	
public class BoardManager : MonoBehaviour
{
	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.
		
		
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8; //number of columns
	public int rows = 8; //number of rows
	public int visionMax = 8;
	public int visionMin = 7;
	public GameObject[] floorTiles; //array of floor tiles
	public GameObject[] wallTiles; //array of wall tiles
	public GameObject[] outerWallTiles; //array of outer wall tiles
	public GameObject[] grassTiles; //array of grass tiles
	public GameObject[] npcTiles; //array of NPCs
	public GameObject[] treesTiles; //array of trees
	public GameObject[] lakeTile; //array of lake tiles
	public GameObject[] rubbishTiles; //array of other/rubbish tiles
	public GameObject[] tentTiles; //array of tent tiles
	public GameObject chestTile; //chest tile
	public GameObject exit; //exit prefab
	public GameObject enemy; //enemy prefab
	public GameObject bandit;

	private Transform boardHolder; 
	private Transform dungeonBoardHolder;
	private Player player;
	private Dictionary<Vector2,Vector2> gridPositions = new Dictionary<Vector2, Vector2>(); //dictionary of world board tiles
	private Dictionary<Vector2,Vector2> dungeonGridPositions; //dictionary of dungeon tiles

	//build the base world board
	public void BoardSetup()
	{
		boardHolder = new GameObject("Board").transform; //create new GameObject which will store all the tiles

		for(int x = 0; x < columns; x++)
		{
			for(int y = 0;y < rows; y++)
			{
				gridPositions.Add(new Vector2(x,y),new Vector2(x,y));
				GameObject toInstantiate = grassTiles[Random.Range(0,grassTiles.Length)];
				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
		}

		player = GameObject.FindObjectOfType<Player>();
	}

	public void addToBoard(int horizontal, int vertical)
	{
		if(horizontal == 1) //player goes right
		{
			int x = (int)Player.position.x;
			int sightX = x + visionMax; //reveal two tiles in front of the player

			for(x += visionMin; x <= sightX; x++)
			{
				int y = (int)Player.position.y;
				int sightY = y + visionMin; //reveal one tile above and one below the player
				for(y -= visionMin; y <= sightY; y++)
				{
					addTiles(new Vector2(x,y));
				}
			}
		}
		else if (horizontal == -1) //player goes left
		{
			int x = (int)Player.position.x;
			int sightX = x - visionMax;
			for (x -= visionMin; x >= sightX; x--) 
			{
				int y = (int)Player.position.y;
				int sightY = y + visionMin;
				for (y -= visionMin; y <= sightY; y++) 
				{
					addTiles(new Vector2 (x, y));
				}
			}
		}
		else if (vertical == 1) //player goes up
		{
			int y = (int)Player.position.y;
			int sightY = y + visionMax;
			for (y += visionMin; y <= sightY; y++)
			{
				int x = (int)Player.position.x;
				int sightX = x + visionMin;
				for (x -= visionMin; x <= sightX; x++) 
				{
					addTiles(new Vector2 (x, y));
				}
			}
		}
		else if (vertical == -1) //player goes down
		{
			int y = (int)Player.position.y;
			int sightY = y - visionMax;
			for (y += visionMin; y >= sightY; y--)
			{
				int x = (int)Player.position.x;
				int sightX = x + visionMin;
				for (x -= visionMin; x <= sightX; x++) 
				{
					addTiles(new Vector2 (x, y));
				}
			}
		}
	}

	//add tiles to the world board
	private void addTiles(Vector2 tileToAdd)
	{
		if(!gridPositions.ContainsKey(tileToAdd))
		{
			gridPositions.Add(tileToAdd,tileToAdd);
			GameObject toInstantiate = grassTiles[Random.Range(0,grassTiles.Length)];
			GameObject instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
			instance.transform.SetParent(boardHolder);

			if(Random.Range(0,3) == 1) //33% chance to spawn a random wall tile
			{
				toInstantiate = wallTiles[Random.Range(0,wallTiles.Length)];
				instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);

			}
			else if(Random.Range(0,50) == 1) //1/50% chance to spawn exit prefab
			{
				toInstantiate = exit;
				instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
			else if(Random.Range(0,GameManager.instance.enemySpawnRatio) == 1)
			{
				toInstantiate = enemy;
				instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
			/*else if(Random.Range(0,10) == 1)
			{
				toInstantiate = bandit;
				instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}*/
			else if(Random.Range(0,10) == 1)   //1/10 chance to spawn a random tree
			{
				toInstantiate = treesTiles[Random.Range(0,treesTiles.Length)];
				instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
			else if(Random.Range(0,50) == 1)
			{
				bool lakeHit = false;
				Collider2D[] lakeTiles = Physics2D.OverlapCircleAll(tileToAdd,5f);
				foreach(Collider2D lake in lakeTiles)
				{
					if(lake.tag == "Lake" || lake.tag == "Tent")
					{
						lakeHit = true;
					}
				}

				if(lakeHit == false)
				{
					BuildLake(tileToAdd);
				}
			}
			else if(Random.Range(0,100) == 1)
			{
				toInstantiate = npcTiles[Random.Range(0,npcTiles.Length)];

				instance = Instantiate(toInstantiate,new Vector3(tileToAdd.x,tileToAdd.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
			}
			else if(Random.Range(0,10) == 1)
			{
				bool tentHit = false;
				Collider2D[] tentTiles = Physics2D.OverlapCircleAll(tileToAdd,5f);
				foreach(Collider2D tent in tentTiles)
				{
					if(tent.tag == "Tent" || tent.tag == "Lake")
					{
						tentHit = true;
					}
				}

				if(tentHit == false)
				{
					BuildTent(tileToAdd);
				}
			}
		}
	}

	//build random dungeon board
	public void setDungeonBoard(Dictionary<Vector2,TileType> dungeonTiles,int bound, Vector2 endPos)
	{
		//disable the world board - to prevent movement there
		boardHolder.gameObject.SetActive(false);

		//create new game object which will hold all dungeon tiles
		dungeonBoardHolder = new GameObject("Dungeon").transform;
		GameObject toInstantiate, instance;

		foreach(KeyValuePair<Vector2, TileType> tile in dungeonTiles)
		{
			toInstantiate = floorTiles[Random.Range(0,floorTiles.Length)];
			instance = Instantiate(toInstantiate,new Vector3(tile.Key.x,tile.Key.y,0f), Quaternion.identity) as GameObject;
			instance.transform.SetParent(dungeonBoardHolder);

			if(tile.Value == TileType.chest)
			{
				toInstantiate = chestTile;
				instance = Instantiate(toInstantiate,new Vector3(tile.Key.x,tile.Key.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(dungeonBoardHolder);
			}
			else if(tile.Value == TileType.enemy)
			{
				toInstantiate = enemy;
				instance = Instantiate(toInstantiate,new Vector3(tile.Key.x,tile.Key.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(dungeonBoardHolder);
			}
			else if(tile.Value == TileType.rubbish)
			{
				toInstantiate = rubbishTiles[Random.Range(0,rubbishTiles.Length)];
				instance = Instantiate(toInstantiate,new Vector3(tile.Key.x,tile.Key.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(dungeonBoardHolder);
			}
		}

		//put outer wall tiles around the dungeon
		for(int x = -1; x < bound + 1; x++)
		{
			for(int y = -1; y < bound; y++)
			{
				if(!dungeonTiles.ContainsKey(new Vector2(x,y)))
				{
					toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
					instance = Instantiate(toInstantiate,new Vector3(x,y,0f),Quaternion.identity) as GameObject;
					instance.transform.SetParent(dungeonBoardHolder);
				}
			}
		}

		toInstantiate = exit;
		instance = Instantiate(toInstantiate,new Vector3(endPos.x,endPos.y,0f),Quaternion.identity) as GameObject;
		instance.transform.SetParent(dungeonBoardHolder);
	}

	public void SetWorldBoard()
	{
		Destroy(dungeonBoardHolder.gameObject);
		boardHolder.gameObject.SetActive(true);
	}

	//function that checks whether a tile is in the dictionary / is visible
	public bool checkValidTile(Vector2 pos)
	{
		if(gridPositions.ContainsKey(pos))
		{
			return true;
		}
		return false;
	}

	//spawn a random lake
	private void BuildLake(Vector2 tileToAdd)
	{
		Vector2 origin;
		int offset = 2;

		if(player.transform.position.y >= tileToAdd.y && player.transform.position.x <= tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x, tileToAdd.y - offset);
		}
		else if(player.transform.position.y < tileToAdd.y && player.transform.position.x <= tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x, tileToAdd.y);
		}
		else if(player.transform.position.y >= tileToAdd.y && player.transform.position.x > tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x - offset, tileToAdd.y - offset);
		}
		else
		{
			origin = new Vector2(tileToAdd.x - offset, tileToAdd.y + offset);
		}
			

		if(gridPositions.ContainsKey(origin))
		{
			gridPositions.Remove(origin);
		}
		gridPositions.Add(origin,origin);
		int lakeSize = 3;
		int arrayIndex = 0;

		for(int x = (int)origin.x; x < origin.x + lakeSize; x++)
		{
			for(int y = (int)origin.y; y < origin.y + lakeSize; y++)
			{
				Vector2 lakeTilePos = new Vector2(x,y);
				if(gridPositions.ContainsKey(lakeTilePos))
				{
					gridPositions.Remove(lakeTilePos);
				}
				gridPositions.Add(lakeTilePos,lakeTilePos);
				GameObject toInstantiate = lakeTile[arrayIndex];
				GameObject instance = Instantiate(toInstantiate,new Vector3(lakeTilePos.x,lakeTilePos.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
				arrayIndex++;
			}
		}

	}

	//spawn a tent
	private void BuildTent(Vector2 tileToAdd)
	{
		Vector2 origin;
		int offset = 1;

		if(player.transform.position.y >= tileToAdd.y && player.transform.position.x <= tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x, tileToAdd.y - offset);
		}
		else if(player.transform.position.y < tileToAdd.y && player.transform.position.x <= tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x, tileToAdd.y);
		}
		else if(player.transform.position.y >= tileToAdd.y && player.transform.position.x > tileToAdd.x)
		{
			origin = new Vector2(tileToAdd.x - offset, tileToAdd.y - offset);
		}
		else
		{
			origin = new Vector2(tileToAdd.x - offset, tileToAdd.y + offset);
		}


		if(gridPositions.ContainsKey(origin))
		{
			gridPositions.Remove(origin);
		}
		gridPositions.Add(origin,origin);
		int tentSize = 2;
		int arrayIndex = 0;

		for(int x = (int)origin.x; x < origin.x + tentSize; x++)
		{
			for(int y = (int)origin.y; y < origin.y + tentSize; y++)
			{
				Vector2 tentTilePos = new Vector2(x,y);
				if(gridPositions.ContainsKey(tentTilePos))
				{
					gridPositions.Remove(tentTilePos);
				}
				gridPositions.Add(tentTilePos,tentTilePos);
				GameObject toInstantiate = tentTiles[arrayIndex];
				GameObject instance = Instantiate(toInstantiate,new Vector3(tentTilePos.x,tentTilePos.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
				toInstantiate = grassTiles[Random.Range(0,grassTiles.Length)];
				instance = Instantiate(toInstantiate,new Vector3(tentTilePos.x,tentTilePos.y,0f),Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardHolder);
				arrayIndex++;
			}
		}
	}
}
