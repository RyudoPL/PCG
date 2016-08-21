using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	//enemy's attack
	public int playerDamage;
	//enemy's health points
	private int hp = 3;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	private SpriteRenderer spriteRenderer;

	protected override void Start()
	{
		//add this script to the list of enemies
		GameManager.instance.AddEnemyToList(this);
		
		animator = GetComponent<Animator>();

		//find player's position
		target = GameObject.FindGameObjectWithTag("Player").transform;

		spriteRenderer = GetComponent<SpriteRenderer>();

		base.Start();
	}

	//check whether enemy can move
	protected override bool AttemptMove <T> (int xDir, int yDir) 
	{
		//check the difficulty and if its easy skip every second turn of the enemy
		if(skipMove && !GameManager.instance.enemiesSmarter)
		{
			skipMove = false;
			return skipMove;
		}

		base.AttemptMove<T>(xDir,yDir);

		skipMove = true;
		return skipMove;
	}

	//control enemy's movement
	public void MoveEnemy()
	{
		int xDir = 0;
		int yDir = 0;

		if(GameManager.instance.enemiesSmarter)
		{
			int xHeading = (int)target.position.x - (int)transform.position.x; //check difference between enenemy and player on x axis
			int yHeading = (int)target.position.y - (int)transform.position.y;//check difference between enenemy and player on y axis

			bool moveOnX = false;

			if(Mathf.Abs(xHeading) >= Mathf.Abs(yHeading)) //if difference on x axis is bigger than on y axis, move enemy on x axis
			{
				moveOnX = true;
			}

			//enemy has two attempts to find the shortest route to the player
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

				//disable box collider to send a raycast
				base.boxCollider.enabled = false;
				//check if enemy can move in x,y direction
				RaycastHit2D hit = Physics2D.Linecast(start,end,base.blockingLayer);
				base.boxCollider.enabled = true;

				if(hit.transform != null)
				{
					//if there is a wall or chest where enemy wants to move, change the direction
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
		}
		else
		{
			if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
			{
				yDir = target.position.y > transform.position.y ? 1 : -1;
			}
			else
			{
				xDir = target.position.x > transform.position.x ? 1: -1;
			}
		}
		AttemptMove<Player>(xDir,yDir);
	}
		
	//function that runs when enemy cannot move
	protected override void OnCantMove <T> (T component) 
	{
		//check if player is next to the enemy
		Player hitPlayer = component as Player;
		//deal damage to the enemy
		hitPlayer.LoseHealth(playerDamage);
		animator.SetTrigger("enemyAttack");
	}

	public SpriteRenderer getSpriteRenderer () {
		return spriteRenderer;
	}

	//receive damage
	public void DamageEnemy (int loss) 
	{
		//subtract loss from enemy's hp
		hp -= loss;

		if (hp <= 0) 
		{
			//remove enemy if its hp is 0
			GameManager.instance.RemoveEnemyFromList (this);
			//destroy the enemy
			Destroy (gameObject);
		}
	}
		
}
