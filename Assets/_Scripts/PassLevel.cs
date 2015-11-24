using UnityEngine;
using System.Collections;

public class PassLevel : MonoBehaviour
{
	public static PassLevel S;
	public GetKey gottenKey;
	public int passedLevel = 0;
	public int level;
	public static int onThisLevel = 1;

	void Start ()
	{
		S = this;
		level = onThisLevel;
	}

	public void reset()
	{
		gottenKey = GameObject.FindGameObjectWithTag("key").GetComponent<GetKey>();
		level = onThisLevel;
		S = this;
	}

	void Update () {
		if (passedLevel == 1)
		{
			level++;
			onThisLevel ++;
			GameObject.FindGameObjectWithTag ("uiLevel").guiText.text = "Level: " + onThisLevel;
			gottenKey.hasKey = 0;
			if(level%2==0)
			{
				DungeonCreator.S.minRoomSize++;
				DungeonCreator.S.maxRoomSize++;
				DungeonCreator.S.minimumRoomCount++;
				DungeonCreator.S.maximumRoomCount++;

				for(int k=1;k<DungeonCreator.S.spawnOptions.Length;k++)
				{
					DungeonCreator.S.spawnOptions[k].minSpawnCount++;
					DungeonCreator.S.spawnOptions[k].maxSpawnCount++;
				}
			}
			DungeonCreator.S.ClearOldDungeon();
			DungeonCreator.S.Generate();
		}
	}

	void OnCollisionEnter(Collision col)
	{
		GameObject other = col.gameObject;
		if(other.tag == "Player")
		{
			if(gottenKey != null && gottenKey.hasKey == 1)
			{
				passedLevel = 1;
			}
			else
			{
				GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "You must have the key to continue!";
			}
		}
	}
}
