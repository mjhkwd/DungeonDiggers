using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {

	public Color colored;
	public Color default1;
	public static Button button;
	public Image image;

	void Awake(){
		button = GameObject.FindGameObjectWithTag ("Archer").GetComponent<Button> ();
		image = button.GetComponent<Image>();
		
		default1 = image.color;
	}

	public void selectArcher()
	{
		PlayerPrefs.SetString("CharacterSelected", "Archer");

		//button.colors.normalColor = new Color(0.22f, 0.22f, 0.22f, 1f);

		button = GameObject.FindGameObjectWithTag ("Archer").GetComponent<Button> ();
		image = button.GetComponent<Image>();

		colored.r = .8f;
		colored.g = 0.8f;
		colored.b = 0.8f;
		colored.a = 0.5f;
			
		image.color = colored;

		reset ("Barbarian");
		reset ("Knight");

		//Application.LoadLevel ("1");
	}
	public void selectBarbarian()
	{
		//this.
		PlayerPrefs.SetString("CharacterSelected", "Barbarian");
		//Application.LoadLevel ("1");
		button = GameObject.FindGameObjectWithTag ("Barbarian").GetComponent<Button> ();
		//button.colors.normalColor = new Color(0.22f, 0.22f, 0.22f, 1f);
		
		image = button.GetComponent<Image>();

		colored.r = .8f;
		colored.g = 0.8f;
		colored.b = 0.8f;
		colored.a = 0.5f;
		
		image.color = colored;
		
		reset ("Archer");
		reset ("Knight");
	}
	public void selectKnight()
	{
		//this.
		PlayerPrefs.SetString("CharacterSelected", "Knight");
		//Application.LoadLevel ("1");
		button = GameObject.FindGameObjectWithTag ("Knight").GetComponent<Button> ();
		//button.colors.normalColor = new Color(0.22f, 0.22f, 0.22f, 1f);
		
		image = button.GetComponent<Image>();

		
		colored.r = .8f;
		colored.g = 0.8f;
		colored.b = 0.8f;
		colored.a = 0.5f;
		
		image.color = colored;
		
		reset ("Archer");
		reset ("Barbarian");
	}
	public void reset(string a)
	{
		button = GameObject.FindGameObjectWithTag (a).GetComponent<Button> ();
		image = button.GetComponent<Image>();
		image.color = default1;
	}
}