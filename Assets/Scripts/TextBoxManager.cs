using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TextBoxManager : MonoBehaviour {

	//reference to the text box UI
	public GameObject textBox;

	//reference to our text file asset
	public TextAsset textFile;
	public Text thisText;
	public string[] textLines;
	public int currentLine;
	public int endAtLine;

	public float typeSpeed;

	private Player player;
	public bool isActive;
	public bool stopPlayerMovement;

	private bool isTyping = false;
	private bool cancelTyping = false;

	// Use this for initialization
	void Start () {
	
		player = FindObjectOfType<Player>();

		if (textFile != null) 
		{
			textLines = (textFile.text.Split('\n'));
		}

		if (endAtLine == 0) 
		{
			endAtLine = textLines.Length - 1;
		}


	}
	
	// Update is called once per frame
	void Update () {
	
		if (!isActive) {
			return;
		}

		//thisText.text = textLines [currentLine];

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (!isTyping) {
				currentLine += 1;

				if (currentLine > endAtLine) 
				{
					DisableTextBox ();
				} 
				else 
				{
					StartCoroutine(TextScroll(textLines[currentLine]));
				}
			} else if (isTyping && !cancelTyping) {
				cancelTyping = true;
			}

		}
	}

	private IEnumerator TextScroll(string lineOfText)
	{
		int letter = 0;
		thisText.text = "";
		isTyping = true;
		cancelTyping = false;

		while (isTyping && !cancelTyping && (letter < lineOfText.Length - 1)) 
		{
			thisText.text += lineOfText[letter];
			letter += 1;
			yield return new WaitForSeconds(typeSpeed);
		}
		thisText.text = lineOfText;
		isTyping = false;
		cancelTyping = false;
	}


	//enable text box UI and disable player's movement if needed
	public void EnableTextBox()
	{
		textBox.SetActive (true);
		isActive = true;

		if (stopPlayerMovement) 
		{
			player.canMove = false;
		}

		StartCoroutine(TextScroll(textLines[currentLine]));

	}

	//disable text box UI / enable player's movement
	public void DisableTextBox()
	{
		textBox.SetActive(false);
		isActive = false;

		player.canMove = true;
	}

	public void ReloadSctipt(TextAsset theText)
	{
		if(theText != null)
		{
			textLines = new string[1];
			textLines = (theText.text.Split('\n'));
		}
	}
}
