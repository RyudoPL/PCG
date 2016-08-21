using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Lake : DestroyableObject 
{
	
	public override void Interact ()
	{
		UIManager.instance.UpdateUI("Fish");
	}
}
