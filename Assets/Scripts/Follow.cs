using UnityEngine;
using System.Collections;

public class Follow : MovingObject {

	private Player player;
	public int wallDamage;
	// Use this for initialization
	protected override void Start()
	{
		GameManager.instance.AddNPCToList(this);
		player = FindObjectOfType<Player>();
		base.Start();
	}

	protected override bool AttemptMove <T> (int xDir, int yDir) 
	{
		bool hit = base.AttemptMove<T>(xDir,yDir);
		return hit;
	}

	protected override void OnCantMove <T> (T component) 
	{
		if(typeof(T) == typeof(Wall))
		{
			Wall blockingObject = component as Wall;
			blockingObject.Interact(wallDamage);
		}
		else if(typeof(T) == typeof(Enemy))
		{
			Enemy blockingObject = component as Enemy;

			//attack the enemy
			blockingObject.DamageEnemy(wallDamage);
		}
	}

	public void MoveNPC()
	{
		int xDir = 0;
		int yDir = 0;


			int xHeading = (int)player.transform.position.x - (int)transform.position.x;
			int yHeading = (int)player.transform.position.y - (int)transform.position.y;

			bool moveOnX = false;

			if(Mathf.Abs(xHeading) >= Mathf.Abs(yHeading))
			{
				moveOnX = true;
			}

			for(int attempt = 0; attempt < 2; attempt++)
			{
				if(moveOnX == true && xHeading < 0)
				{
					xDir = -1;
					yDir = 0;
				}
				else if(moveOnX == true && xHeading > 0)
				{
					xDir = 1;
					yDir = 0;
				}
				else if(moveOnX == false && yHeading < 0)
				{
					xDir = 0;
					yDir = -1;
				}
				else if(moveOnX == false && yHeading > 0)
				{
					xDir = 0;
					yDir = 1;
				}

				Vector2 start = transform.position;
				Vector2 end = start + new Vector2(xDir,yDir);
				base.boxCollider.enabled = false;
				RaycastHit2D hit = Physics2D.Linecast(start,end,base.blockingLayer);
				base.boxCollider.enabled = true;

				if(hit.transform != null)
				{
					if(hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Chest")
					{
						if(moveOnX == true)
						{
							moveOnX = false;
						}
						else
						{
							moveOnX = true;
						}
					}
					else
					{
						break;
					}
				}
		}

		AttemptMove<Enemy>(xDir,yDir);
	}
}
