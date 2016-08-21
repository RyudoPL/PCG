using UnityEngine;
using System.Collections;

public class Chest : DestroyableObject {

	public Sprite openSprite; //sprite which is enabled when chest is opened

	public Item randomItem; //item which is inside the chest
	public Weapon weapon;

	//interact with a chest
	public override void Interact()
	{
		ChangeSprite(base.spriteRenderer);
		GameObject toInstantiate;

		if(Random.Range(0,2) == 1) //50% chance to spawn an item and 50% chance to spawn a weapon
		{
			randomItem.RandomItemInit(); //generate a random item
			toInstantiate = randomItem.gameObject;
		}
		else
		{
			toInstantiate = weapon.gameObject;
		}
		GameObject instance = Instantiate(toInstantiate,new Vector3(transform.position.x,transform.position.y,0f),Quaternion.identity) as GameObject;
		instance.transform.SetParent(transform.parent);

		gameObject.layer = 10; //change chest's layer in order for the player to be rendered on top of it

	}

	protected override void ChangeSprite(SpriteRenderer spriteRenderer)
	{
		spriteRenderer.sprite = openSprite;
		spriteRenderer.sortingLayerName = "Items";
	}
}
