using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
	public float health;
	public float speed;
	public float damage;
	public float range;
	public float lastAttackSetter;
	public float lastAttackChanging;

	public Color[] originalColors;
	public Material[] materials;		//All the materials of this and its children
	public int showDamageForFrames = 2;		//# frames to show damage
	public int remainingDamageFrames = 0;		//Damage frames left

	public GameObject keyPrefab;
	public bool hasKey = false;

	public GameObject player;
	public float dist;

	public bool following = false;
	public int counter = 0;

	public LineRenderer line;



	void Start ()
	{
		materials = GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int j=0; j<materials.Length; j++) {
			originalColors[j] = materials[j].color;		
		}

		player = GameObject.FindGameObjectWithTag("Player");
		
		line = GetComponent<LineRenderer>();
		if(this.tag=="enemyDog")
		{
			health = 25f;
			speed = .75f;
			damage = 2f;
			range = 4f;
			lastAttackSetter = 90f;
		}
		if(this.tag=="enemySkeleton")
		{
			health = 45f;
			speed = .75f;
			damage = 4f;
			range = 2f;
			lastAttackSetter = 120f;
		}
		lastAttackChanging = lastAttackSetter;
	}

	void Update ()
	{

		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if(remainingDamageFrames == 0){
				UnShowDamage();
			}
		}

		if (player!=null) {
			lastAttackChanging--;
			dist = Vector3.Distance (this.transform.position, player.transform.position);
					
			if (dist <= range && following) {
				if (lastAttackChanging <= 0) {
					Player.S.GetComponent<Player> ().takeDamage (this.gameObject);
					GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You have taken " + (int)damage + " damage.";
					lastAttackChanging = lastAttackSetter;
				}
			}	

			if (dist > range) {
				following = false;
			}
	
			if (following) {
				Vector3 lookDir = (player.transform.position - transform.position).normalized;
				lookDir.y = 0;
				Quaternion newRotation = Quaternion.LookRotation(lookDir);
				transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime*8);

				transform.position = Vector3.Lerp (transform.position, player.transform.position, Time.deltaTime * speed);
			}
			if (!following) {	
				if (counter % 75 == 0) {
					this.transform.Rotate (0, 45, 0, Space.Self);
				}
			}
			counter++;
			if (health <= 0) {
				if (hasKey) {
					GameObject key = (GameObject)Instantiate (keyPrefab);
					GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "A key has been dropped!";
					key.transform.position = this.gameObject.transform.position;
					PassLevel.S.reset ();
				}
				Destroy (this.gameObject);
				Destroy (this);
				GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You killed an enemy!";
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			following = true;
		}
	}

	//Returns a list of all Materials on this GameObject or its children
	static public Material[] GetAllMaterials(GameObject go){
		List<Material> mats = new List<Material> ();
		if (go.renderer != null) {
			mats.Add (go.renderer.material);		
		}
		foreach (Transform t in go.transform) {
			mats.AddRange (GetAllMaterials(t.gameObject));		
		}
		return(mats.ToArray ());
	}
	
	public void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;		
		}
		remainingDamageFrames = showDamageForFrames;
	}
	
	public void UnShowDamage(){
		for (int i=0; i<materials.Length; i++) {
			materials[i].color = originalColors[i];		
		}
	}
}
