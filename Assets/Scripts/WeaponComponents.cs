using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;


public class WeaponComponents : MonoBehaviour {

	public Sprite[] modules; //array of sprites

	private Weapon parent; //parent's transform
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () 
	{	
		parent = GetComponentInParent<Weapon>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		//choose the sprite for the weapon component at random
		spriteRenderer.sprite = modules[Random.Range(0,modules.Length)];
	}
	
	// Update is called once per frame
	void Update () 
	{
		//match the rotation with weapon's rotation
		transform.eulerAngles = parent.transform.eulerAngles;
	}

	public SpriteRenderer getSpriteRenderer()
	{
		return spriteRenderer;
	}
}
