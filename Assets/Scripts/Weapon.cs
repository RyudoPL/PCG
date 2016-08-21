using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public bool inPlayerInventory = false; //boolean that checks if an item is already in player's inventory

	private Player player; //reference to the player object
	private WeaponComponents[] weaponComps; //array of weapon components
	private bool weaponUsed = false; //boolean thay check if weapon is being used

	// Update is called once per frame
	void Update () 
	{
		//check if player has a weapon in its inventory
		if(inPlayerInventory)
		{
			transform.position = player.transform.position;
			if(weaponUsed == true)
			{
				//variables that hold weapon rotation
				float degreeY = 0, degreeZ = -90f, degreeZMax = 275f;
				Vector3 returnVector = Vector3.zero;

				if(Player.isFacingRight)
				{
					degreeY = 0;
					returnVector = Vector3.zero;
				}
				//when player is facing left
				else if(!Player.isFacingRight)
				{
					degreeY = 180;
					returnVector = new Vector3(0,180,0);
				}
				//weapon animation controlled by the code
				transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,degreeY,degreeZ), Time.deltaTime * 20.0f);

				//if rotation on z axis is the same as max degree then set the rotation to original position
				if(transform.eulerAngles.z <= degreeZMax)
				{
					transform.eulerAngles = returnVector;
					weaponUsed = false;
					EnableSpriteRenderer(false);
				}
			}
		}
	}

	//function that enables communication between player, weapon and weapon components
	public void AcquireWeapon()
	{
		player = GetComponentInParent<Player>();
		weaponComps = GetComponentsInChildren<WeaponComponents>();
	}

	//function that is called when weapon is being used
	public void UseWeapon()
	{
		EnableSpriteRenderer(true);
		weaponUsed = true;
	}

	//function that enables or disables SpriteRenderer on all child components
	public void EnableSpriteRenderer(bool isEnabled)
	{
		foreach(WeaponComponents comp in weaponComps)
		{
			comp.getSpriteRenderer().enabled = isEnabled;
		}
	}

	//function that get sprites from the chosen weapon component
	public Sprite GetComponentImage(int index)
	{
		return weaponComps[index].getSpriteRenderer().sprite;
	}


}
