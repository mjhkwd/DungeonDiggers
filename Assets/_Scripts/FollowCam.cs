using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
	public Vector3 poi;
	public GameObject person;
	

	void Awake ()
	{
	}

	void FixedUpdate ()
	{
		person = GameObject.FindGameObjectWithTag ("Player");
		
		if(person != null)
		{
			poi = person.transform.position;
			//poi.z -= 2.5f;
			poi.x -= 3.8f;
			poi.y = 100;

			this.camera.transform.position = poi;
		}
	}
}
