using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	private int dayLength;   //in minutes
	private int dayStart;
	private int nightStart;   //also in minutes
	private int currentTime;
	public float cycleSpeed;
	private Animator anim;

	void Start() 
	{
		dayLength = 1440;
		dayStart = 300;
		nightStart = 1200;
		currentTime = 720;
		anim = GetComponent<Animator>();
		StartCoroutine(TimeOfDay());
	}

	//change color's alpha according to a time of the day
	void Update() 
	{
		if (currentTime > 0 && currentTime < dayStart) {
			anim.SetBool("Sunset",false);
			anim.SetBool("Sunrise",true);
		} else if (currentTime >= dayStart && currentTime < nightStart) {
			anim.SetBool("Sunrise",false);
			anim.SetBool("Sunset",true);

		} else if (currentTime >= nightStart && currentTime < dayLength) {
			
		} else if (currentTime >= dayLength) 
		{
			currentTime = 0;
		}
		float currentTimeF = currentTime;
		float dayLengthF = dayLength;
	}

	IEnumerator TimeOfDay()
	{
		while (true) 
		{
			currentTime += 1;
			int hours = Mathf.RoundToInt( currentTime / 60);
			int minutes = currentTime % 60;
			Debug.Log (hours+":"+minutes);
			yield return new WaitForSeconds(1F/cycleSpeed);
		}
	}
}
