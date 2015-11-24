using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeaponType
{
	none,		//The default/no weapon
	dagger,		//Low damage, low range
	sword,		//medium damage, medium range
	bow,		//medium damage, high range
	hammer		//high damage, low range
}

public enum CharacterType
{
	Archer,		//Long Range, 	Fast,			Low damage
	Barbarian,	//Short Range, 	Slow,			High Damage
	Knight		//Medium Range, Medium speed,	Medium Damage
}

[System.Serializable]
public class CharacterDefinition
{
	public CharacterType charType;
	public Color charColor;
	public float charSpeed;
	public float charHealth;
	public WeaponType defaultWeapon;
}

[System.Serializable]
public class WeaponDefinition
{
	public WeaponType type = WeaponType.none;
	public float damage = 0;
	public float delayBetweenShots = 0;
	public float range = 0;
}

public class Player : MonoBehaviour
{
	static public Dictionary<CharacterType, CharacterDefinition> DEFS;

	private float step = .1f;
	private float speed = 20f;
	private float health = 10f;
	private Color color;

	public float defaultHealth;
	public float defaultSpeed;

	public float distance;
	public float lastAttack;
	public int i;

	public Color[] originalColors;
	public Material[] materials;		//All the materials of this and its children
	public int showDamageForFrames = 2;		//# frames to show damage
	public int remainingDamageFrames = 0;		//Damage frames left

	public static Player S;

	
	public WeaponType weapon = WeaponType.dagger;
	public CharacterType type = CharacterType.Knight;
	//[SerializeField]
	public CharacterDefinition[] characterDefinitions;
	public WeaponDefinition[] weaponDefinitions;

	void Awake ()
	{
		S = this;


		DEFS = new Dictionary<CharacterType, CharacterDefinition>();
		foreach(CharacterDefinition def in characterDefinitions)
		{
			DEFS[def.charType] = def;
		}

		switch (PlayerPrefs.GetString ("CharacterSelected"))
		{
			case "Archer":
				i = 2;
				type = CharacterType.Archer;
				GameObject.Find("Sword").SetActive(false);
				GameObject.Find("Hammer").SetActive(false);
			break;
			case "Barbarian":
				i = 3;
				type = CharacterType.Barbarian;
				GameObject.Find("Sword").SetActive(false);
				GameObject.Find("Bow").SetActive(false);
			break;
			case "Knight":
				i = 1;
				type = CharacterType.Knight;
				GameObject.Find("Bow").SetActive(false);
				GameObject.Find("Hammer").SetActive(false);
			break;
		}

		speed = DEFS[type].charSpeed;
		defaultSpeed = speed;
		health = DEFS[type].charHealth;
		defaultHealth = health;
		color = DEFS[type].charColor;
		weapon = DEFS[type].defaultWeapon;
		lastAttack = weaponDefinitions [i].delayBetweenShots;
	}

	void Start()
	{
		//this.renderer.material.color = color;
		GameObject.Find ("Torso").renderer.material.color = color;
		materials = GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int j=0; j<materials.Length; j++) {
			originalColors[j] = materials[j].color;		
		}
		updateStats();

	}

	void Update ()
	{
		GameObject.FindGameObjectWithTag ("uiHealth").guiText.text = "Health: " + (int)health;
		InfoHolder.S.saveHealth = health;
		InfoHolder.S.saveSpeed = speed;

		lastAttack--;
		float xAxis = Input.GetAxis ("Horizontal");
		float zAxis = Input.GetAxis ("Vertical");
		
		//Change the transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * step * Time.deltaTime;
		pos.z += zAxis * speed * step * Time.deltaTime;
		transform.position = pos;

		Vector3 moveDirection = new Vector3 (xAxis, 0, zAxis);
		if(moveDirection != Vector3.zero){
			Quaternion newRotation = Quaternion.LookRotation(moveDirection);
			transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime*8);
		}
		//transform.rotation = Quaternion.Euler (0, xAxis*90 ,0);

		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if(remainingDamageFrames == 0){
				UnShowDamage();
			}
		}


		//Register attacks by player and check if in range of enemy
		foreach (GameObject enem in GameObject.FindGameObjectsWithTag("enemyDog")){
			distance = Vector3.Distance (transform.position, enem.transform.position);
			if(Input.GetKeyDown (KeyCode.Space)){
				if(distance <= weaponDefinitions[i].range){
					//deal damage to enemy
					if(lastAttack <=0){
						attack (enem);
					}
				}

			}
		}

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("enemySkeleton")){
			distance = Vector3.Distance (transform.position, enemy.transform.position);
			if(Input.GetKeyDown (KeyCode.Space)){
				if(distance <= weaponDefinitions[i].range){
					//deal damage to enemy
					if(lastAttack <= 0){
						attack (enemy);
						GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You have dealt " + (int)weaponDefinitions[i].damage + " damage.";
					}
					//lowerHealth = GameObject.Find ("Skeleton").GetComponent<Enemy>();
					//lowerHealth.health -= attacking.damage;
				}
			}
		}
	}

	public void attack(GameObject enem){
		//subtract health
		enem.GetComponent<Enemy> ().ShowDamage ();
		enem.GetComponent<Enemy>().health -= weaponDefinitions[i].damage;
		lastAttack = weaponDefinitions [i].delayBetweenShots;
	}

	public void takeDamage(GameObject attacker)
	{
		//flash player red
		ShowDamage ();
		health -= attacker.GetComponent<Enemy> ().damage;
		if(health <= 0){
			GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You Died!";
			PlayerPrefs.DeleteKey("CharacterSelected");
			Destroy (this.gameObject);
			Destroy (this);
			GameObject.FindGameObjectWithTag("exit").GetComponent<PassLevel>().level = 0;
			Application.LoadLevel("_Scene_2");
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

	void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;		
		}
		remainingDamageFrames = showDamageForFrames;
	}
	
	void UnShowDamage(){
		for (int i=0; i<materials.Length; i++) {
			materials[i].color = originalColors[i];		
		}
	}

	void updateStats()
	{
		health = InfoHolder.S.saveHealth;
		speed = InfoHolder.S.saveSpeed;
	}
}

