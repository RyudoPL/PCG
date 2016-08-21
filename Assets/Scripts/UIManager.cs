using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public static UIManager instance = null;	
	public Text woodNumber;
	public Text coalNumber;
	public Text fishNumber;
	private int totalNumber;

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

		totalNumber = 0;
	}

	public void UpdateUI(string tag)
	{
		totalNumber++;

		switch(tag)
		{
		case "Wood":
			woodNumber.text = totalNumber.ToString();
			break;

		case "Coal":
			coalNumber.text = totalNumber.ToString();
			break;

		case "Fish":
			fishNumber.text = totalNumber.ToString();
			break;
		}
	}

	public void GetResourceNumber()
	{
		
	}
}
