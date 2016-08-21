using UnityEngine;
using System.Collections;

public class DestroyableObject : MonoBehaviour {

	protected SpriteRenderer spriteRenderer;

	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public virtual void Interact()
	{
		
	}

	public virtual void Interact(int loss)
	{

	}

	protected virtual void ChangeSprite(SpriteRenderer spriteRenderer)
	{
		
	}
}
