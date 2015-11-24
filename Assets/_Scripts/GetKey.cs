using UnityEngine;
using System.Collections;

public class GetKey : MonoBehaviour {
	public float hasKey = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		GameObject other = col.gameObject;
		if(other.tag == "Player")
		{
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			
			foreach (Renderer r in renderers)
			{
				r.enabled = false;
			}

			collider.enabled = false;
			//Collider[] colliders = GetComponentsInChildren<Collider>();

			//foreach (Collider c in collider)
			//{
			//	c.enabled = false;
			//}

			hasKey = 1;
			GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You have found the key!";
		}
	}
}
