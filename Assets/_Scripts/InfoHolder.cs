using UnityEngine;
using System.Collections;

public class InfoHolder : MonoBehaviour
{
	public static InfoHolder S;

	public float saveHealth;
	public float saveSpeed;
	//public float damage;
	//public float delay;

	void Awake ()
	{
		S = this;
		saveSpeed = Player.S.defaultSpeed;
		saveHealth = Player.S.defaultHealth;
	}

	void Update ()
	{
	
	}
}
