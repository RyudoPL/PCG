using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType  //enum of types of tiles which can be instantiatd in the dungeon
{
	essential,
	random,
	empty,
	chest,
	enemy,
	rubbish
}

public class DungeonManager : MonoBehaviour 
{
	[Serializable]
	public class PathTile //class for a tile
	{
		public TileType type; //type of the tile
		public Vector2 position; //position of the tile
		public List<Vector2> adjacentPathTiles; //list of adjacent tiles

		//constructor
		public PathTile(TileType t, Vector2 p, int min, int max, Dictionary<Vector2, TileType> currentTiles)
		{
			type = t;
			position = p;
			adjacentPathTiles = getAdjacentPath(min,max,currentTiles);
		}

		//function that makes a list of adjacent tiles
		public List<Vector2> getAdjacentPath(int minBound, int maxBound, Dictionary<Vector2,TileType> currentTiles) 
		{
			List<Vector2> pathTiles = new List<Vector2>(); //create a new list of adjacent tiles
			if(position.y + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x,position.y + 1)))
			{
				pathTiles.Add(new Vector2(position.x,position.y + 1));
			}
			if(position.x + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x + 1, position.y)))
			{
				pathTiles.Add(new Vector2(position.x + 1,position.y));
			}
			if(position.y - 1 > minBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y - 1)))
			{
				pathTiles.Add(new Vector2(position.x,position.y -1));
			}
			if(position.x - 1 >= minBound && !currentTiles.ContainsKey(new Vector2(position.x - 1, position.y)) && type != TileType.essential)
			{
				pathTiles.Add(new Vector2(position.x - 1,position.y));
			}
			return pathTiles;
		}
	}

	public Dictionary<Vector2,TileType> gridPositions = new Dictionary<Vector2,TileType>(); //creates a new dictionary of tiles
	public int minBound = 0, maxBound; //bounds of dungeon board
	public static Vector2 startPos; //start position of the essential path
	public Vector2 endPos; //end position of the essential path

	//create random dungeon
	public void StartDungeon()
	{
		gridPositions.Clear(); //clear the dictionary 
		maxBound = Random.Range(50,101); //randomise maximum bound of the dungeon
		BuildEssentialPath();
		BuildRandomPath();
	}

	//function that builds essential path in the dungeon / leads to an exit
	private void BuildEssentialPath()
	{
		int randomY = Random.Range(0, maxBound + 1); //choose random Y coordinate to set start position there
		PathTile ePath = new PathTile(TileType.essential,new Vector2(0,randomY),minBound,maxBound,gridPositions); //build the first essential path's tile
		startPos = ePath.position;
		int boundTracker = 0; //set a tracker which will go up every time essential path move one tile on x axis

		//repeat below until the tracker reaches maxBound value on X axis
		while(boundTracker < maxBound)
		{
			gridPositions.Add(ePath.position, TileType.empty);
			int adjacentTileCount = ePath.adjacentPathTiles.Count; //get the number of adjacent path tiles
			int randomIndex = Random.Range(0,adjacentTileCount); //choose the random tile from the adjacent path tiles
			Vector2 nextEPathPos;

			if(adjacentTileCount > 0)
			{
				nextEPathPos = ePath.adjacentPathTiles[randomIndex];
			}
			else
			{
				break;
			}

			PathTile nextEPath = new PathTile(TileType.essential,nextEPathPos,minBound,maxBound,gridPositions); //build the next tile in the essential path
			if(nextEPath.position.x > ePath.position.x || nextEPath.position.x == maxBound - 1 && Random.Range(0,2) == 1)
			{
				++boundTracker; //iterate bound tracker
			}
			//set the next tile as the base one
			ePath = nextEPath;
		}
		if(!gridPositions.ContainsKey(ePath.position))
		{
			gridPositions.Add(ePath.position,TileType.empty);
			endPos = new Vector2(ePath.position.x,ePath.position.y);
		}
	}

	//function that build the random path
	private void BuildRandomPath()
	{
		//create a new list
		List<PathTile> pathQueue = new List<PathTile>(); 

		//copy the existing list of tiles
		foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
		{
			Vector2 tilePos = new Vector2(tile.Key.x,tile.Key.y);
			pathQueue.Add(new PathTile(TileType.random,tilePos,minBound,maxBound,gridPositions));
		}

		pathQueue.ForEach(delegate(PathTile tile)
		{
			int adjacentTileCount = tile.adjacentPathTiles.Count;
				if(adjacentTileCount != 0)
				{
					if(Random.Range(0,5) == 1) //20% chance to build a random chamber
					{
						BuildRandomChamber(tile);
					}
					else if(Random.Range(0,5) == 1 || (tile.type == TileType.random && adjacentTileCount > 1))
					{
						int randomIndex = Random.Range(0,adjacentTileCount);
						Vector2 newRPathPos = tile.adjacentPathTiles[randomIndex];
						if(!gridPositions.ContainsKey(newRPathPos))
						{
							if(Random.Range(0,20) == 1) //1 out of 20 chance to spawn an enemy
							{
								gridPositions.Add(newRPathPos,TileType.enemy);
							}
							else if(Random.Range(0,10) == 1)
							{
								gridPositions.Add(newRPathPos,TileType.rubbish);
							}
							else
							{
								gridPositions.Add(newRPathPos, TileType.empty);
							}

							PathTile newRPath = new PathTile(TileType.random,newRPathPos,minBound,maxBound,gridPositions);
							pathQueue.Add(newRPath);
						}
					}

				}
			});
		}
	//function that build chambers randomly
	private void BuildRandomChamber(PathTile tile)
	{
		//size of the chamber
		int chamberSize = 3, adjacentTileCount = tile.adjacentPathTiles.Count, randomIndex = Random.Range(0,adjacentTileCount);
		Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex];

		for(int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
		{
			for(int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
			{
				Vector2 chamberTilePos = new Vector2(x,y);
				if(!gridPositions.ContainsKey(chamberTilePos) && chamberTilePos.x < maxBound && chamberTilePos.x > 0 && chamberTilePos.y < maxBound && chamberTilePos.y > 0)
				{
					if(Random.Range(0,70) == 1)
					{
						gridPositions.Add(chamberTilePos,TileType.chest);
					}
					else
					{
						gridPositions.Add(chamberTilePos,TileType.empty);
					}
				}
			}
		}
	}

}

