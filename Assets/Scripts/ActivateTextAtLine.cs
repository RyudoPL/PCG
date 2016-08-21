using UnityEngine;
using System.Collections;

public class ActivateTextAtLine : MonoBehaviour {


	public TextAsset theText;
	public int startLine;
	public int endLine;
	public TextBoxManager theTextBox;

	public bool requireButtonPress;
	private bool waitForPress;
	private bool firstTimeSpeak = true;

	public bool destroyWhenActivated;

	// Use this for initialization
	void Start () {
	
		theTextBox = FindObjectOfType<TextBoxManager> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (waitForPress && Input.GetKeyDown (KeyCode.E)) 
		{
			theTextBox.ReloadSctipt(theText);
			theTextBox.currentLine = startLine;
			theTextBox.endAtLine = endLine;
			theTextBox.EnableTextBox();
			
			if(destroyWhenActivated)
			{
				Destroy(gameObject);
			}
		}
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			//wait for a buttton press if we want to
			if(requireButtonPress)
			{
				waitForPress = true;
				return;
			}

			//check if it's the first time we speak with specific NPC
			if(firstTimeSpeak == true)
			{
				theTextBox.ReloadSctipt(theText);
				theTextBox.currentLine = startLine;
				theTextBox.endAtLine = endLine;
				theTextBox.EnableTextBox();
				firstTimeSpeak = false;
			}
				
			if(firstTimeSpeak == false)
			{
				
			}
				
			//destroy the object if we want to - ex. dying NPC
			if(destroyWhenActivated)
			{
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			waitForPress = false;
		}
	}
}
