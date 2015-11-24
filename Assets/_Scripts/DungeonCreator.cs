/// <summary>
/// 
/// Algorithm:
/// 
/// Initiate a 2D array, all 0
/// Make x number of squares at random sizes -- Max declared in inspector
/// Set room values to 1
/// If multiple squares are in the same spot, squash them together to form one room
/// 
/// Find shortest path between 2 rooms points
/// This will be a corridor
/// Set corridor value to 3
/// 
/// Set wall values to 2
/// 
/// 
/// similar algorithm to https://learninggeekblog.wordpress.com/2013/10/30/procedural-dungeon-generation-part1/
/// 
///
/// </summary>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room
{
	public int x = 0;
	public int y = 0;
	public int w = 0;
	public int h = 0;
	public Room connectedTo = null;
}

public class SpawnList
{
	public int x;
	public int y;
	public bool byWall;
	public bool spawnedObject;
}

[System.Serializable]
public class SpawnOption
{
	public int minSpawnCount;
	public int maxSpawnCount;
	public bool spawnByWall;
	public GameObject gameObject;
}


public class DungeonCreator : MonoBehaviour
{
	public GameObject startPrefab;
	public GameObject exitPrefab;
	public List<SpawnList> spawnedObjectLocations = new List<SpawnList>();
	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public int maximumRoomCount = 5;
	public int minimumRoomCount = 10;
	public bool generate_on_load = true;
	public int minRoomSize = 5;
	public int maxRoomSize = 10;
	public float tileScaling = 1f;
	public SpawnOption[] spawnOptions;
	public static string[] enemies = {"enemyDog", "enemySkeleton"};
	public int[] difficulty;
	public int scaler = 1;


	public static DungeonCreator S;
	
	void Awake()
	{
		S = this;
//		for(int i=0; i<difficulty.Length; i++){
//			difficulty[i] = scaler;
//			scaler++;
//			minimumRoomCount = minimumRoomCount*difficulty[GameObject.FindGameObjectWithTag("exit").GetComponent<PassLevel>().level];
//			maximumRoomCount = maximumRoomCount*difficulty[GameObject.FindGameObjectWithTag("exit").GetComponent<PassLevel>().level];
//		}
	}

	class Dungeon
	{
		public static int map_size = 256;
		public static int[,] map = new int[map_size, map_size];
		
		public static List<Room> rooms = new List<Room>();
		
		public static Room goalRoom;
		public static Room startRoom;
		
		public int min_size;
		public int max_size;
		
		public int maximumRoomCount;
		public int minimumRoomCount;
		
		
		public void Generate()
		{
			for (var x = 0; x < map_size; x++) {
				for (var y = 0; y < map_size; y++) {
					map [x, y] = 0;
				}
			}
			rooms = new List<Room> ();
			
			int room_count = Random.Range (this.minimumRoomCount, this.maximumRoomCount);
			int min_size = this.min_size;
			int max_size = this.max_size;
			
			for (var i = 0; i < room_count; i++) {
				Room room = new Room ();
				room.x = Random.Range (1, map_size - min_size - 1);
				room.y = Random.Range (1, map_size - max_size - 1);
				room.w = Random.Range (min_size, max_size);
				room.h = Random.Range (min_size, max_size);
				
				bool doesCollide = this.DoesCollide (room, 0);
				
				if (doesCollide) {
					i--;
				} else {
					room.w--;
					room.h--;
					
					rooms.Add (room);
				}
			}
			
			SquashRooms ();
			
			//corridor making
			for (int i = 0; i < room_count; i++) {
				Room roomA = rooms [i];
				Room roomB = FindClosestRoom (roomA);
				
				if (roomB != null) {
					var pointA = new Room ();
					pointA.x = Random.Range (roomA.x, roomA.x + roomA.w);
					pointA.y = Random.Range (roomA.y, roomA.y + roomA.h);
					
					var pointB = new Room ();
					pointB.x = Random.Range (roomB.x, roomB.x + roomB.w);
					pointB.y = Random.Range (roomB.y, roomB.y + roomB.h);
					
					roomA.connectedTo = roomB;
					
					while ((pointB.x != pointA.x) || (pointB.y != pointA.y)) {
						if (pointB.x != pointA.x) {
							if (pointB.x > pointA.x)
								pointB.x--;
							else
								pointB.x++;
						} else if (pointB.y != pointA.y) {
							if (pointB.y > pointA.y)
								pointB.y--;
							else
								pointB.y++;
						}
						
						map [pointB.x, pointB.y] = 3;
					}
				}
			}
			
			//room making
			for (int i = 0; i < room_count; i++) {
				Room room = rooms [i];
				for (int x = room.x; x < room.x + room.w; x++) {
					for (int y = room.y; y < room.y + room.h; y++) {
						map [x, y] = 1;
					}
				}
			}
			
			
			//wall maker
			for (int x = 0; x < map_size; x++) {
				for (int y = 0; y < map_size; y++) {
					if (map [x, y] == 1 || map [x, y] == 3) {
						for (var xx = x - 1; xx <= x + 1; xx++) {
							for (var yy = y - 1; yy <= y + 1; yy++) {
								if (map [xx, yy] == 0)
									map [xx, yy] = 2;
							}
						}
					}
				}
			}
			
			//find far far away room
			goalRoom = FindAwayRoom (rooms [0]);
			if (goalRoom != null) {
				goalRoom.x = goalRoom.x + (goalRoom.w / 2);
				goalRoom.y = goalRoom.y + (goalRoom.h / 2);
			}
			//starting point
			startRoom = rooms [0];
			startRoom.x = startRoom.x + (startRoom.w / 2);
			startRoom.y = startRoom.y + (startRoom.h / 2);

			GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "Next Level";

		}
		
		private bool DoesCollide (Room room, int ignore)
		{
			for (int i = 0; i < rooms.Count; i++)
			{
				if (i == ignore) continue;
				var check = rooms[i];
				if (!((room.x + room.w + 2 < check.x) || (room.x > check.x + check.w + 2) || (room.y + room.h + 2 < check.y) || (room.y > check.y + check.h + 2))) return true;
			}
			
			return false;
		}
		
		private void SquashRooms()
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < rooms.Count; j++)
				{
					Room room = rooms[j];
					while (true)
					{
						Room old_position = new Room();
						
						old_position.x = room.x;
						old_position.y = room.y;
						
						if (room.x > 1) room.x--;
						if (room.y > 1) room.y--;
						if ((room.x == 1) && (room.y == 1)) break;
						if (this.DoesCollide(room, j))
						{
							room.x = old_position.x;
							room.y = old_position.y;
							break;
						}
					}
				}
			}
		}
		
		
		
		private Room FindClosestRoom(Room room)
		{
			Room mid = new Room();
			mid.x = room.x + (room.w);
			mid.y = room.y + (room.h);
			
			Room closest = null;
			float closest_distance = 10000;
			
			for (var i = 0; i < rooms.Count; i++)
			{
				Room check = rooms[i];
				Room check_mid = new Room();
				check_mid.x = check.x + (check.w);
				check_mid.y = check.y + (check.h);
				
				if (check != room)
				{  
					//var distance = Mathf.Min(Mathf.Abs(mid.x - check_mid.x) - (room.w / 2) - (check.w / 2), Mathf.Abs(mid.y - check_mid.y) - (room.h / 2) - (check.h / 2));
					float distance = lineDistance(check,mid);
					if (distance < closest_distance && check.connectedTo == null)
					{
						closest_distance = distance;
						closest = check;
					}
				}
			}
			
			
			return closest;
		}
		
		private float lineDistance( Room point1, Room point2 )
		{
			var xs = 0;
			var ys = 0;
			
			xs = point2.x - point1.x;
			xs = xs * xs;
			
			ys = point2.y - point1.y;
			ys = ys * ys;
			
			return Mathf.Sqrt( xs + ys );
		}
		
		
		
		private Room FindAwayRoom(Room room)
		{
			Room mid = new Room();
			mid.x = room.x + (room.w / 2);
			mid.y = room.y + (room.h / 2);
			
			Room closest = null;
			float closest_distance = 0;
			int i = 0;
			for (i = 0; i < rooms.Count; i++)
			{
				var check = rooms[i];
				if (check != room)
				{  
					
					var check_mid = new Room();
					check_mid.x = check.x + (check.w / 2);
					check_mid.y = check.y + (check.h / 2);
					
					//var distance = Mathf.Min(Mathf.Abs(mid.x - check_mid.x) - (room.w / 2) - (check.w / 2), Mathf.Abs(mid.y - check_mid.y) - (room.h / 2) - (check.h / 2));
					var distance = lineDistance(check,mid);
					
					if (distance > closest_distance)
					{
						closest_distance = distance;
						closest = check;
					}
				}
			}
			if (closest != null)
			{
				closest = rooms[i -1 ];
			}
			return closest;
		}
		
	}
	
	public void ClearOldDungeon()
	{
		int childs = transform.childCount;
		for (var i = childs - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
		foreach (GameObject key in GameObject.FindGameObjectsWithTag("key"))
		{
			Destroy(key);
		}
	}
	
	
	public void Generate()
	{
		Dungeon dungeon = new Dungeon();
		
		dungeon.min_size = minRoomSize;
		dungeon.max_size = maxRoomSize;
		dungeon.minimumRoomCount = minimumRoomCount;
		dungeon.maximumRoomCount = maximumRoomCount;
		
		dungeon.Generate();
		
		//Dungeon.map = floodFill(Dungeon.map,1,1);
		
		for (var y = 0; y < Dungeon.map_size; y++)
		{
			for (var x = 0; x < Dungeon.map_size; x++)
			{
				int tile = Dungeon.map[x,y];
				GameObject created_tile;
				Vector3 tile_location;

				tile_location = new Vector3(x* tileScaling,0,y* tileScaling);
				
				
				created_tile = null;
				if (tile == 1)
				{
					created_tile = GameObject.Instantiate(floorPrefab,tile_location,Quaternion.identity ) as GameObject;
				}
				
				if (tile == 2)
				{
					created_tile = GameObject.Instantiate(wallPrefab,tile_location,Quaternion.identity ) as GameObject;
				}
				
				if (tile == 3)
				{
					created_tile = GameObject.Instantiate(floorPrefab,tile_location,Quaternion.identity ) as GameObject;
				}
				
				if (created_tile)
				{
					created_tile.transform.parent = transform;
				}
			}
		}
		
		GameObject end_point;
		GameObject start_point;

		end_point = GameObject.Instantiate(exitPrefab,new Vector3(Dungeon.goalRoom.x * tileScaling ,0,Dungeon.goalRoom.y * tileScaling),Quaternion.identity) as GameObject;
		start_point = GameObject.Instantiate(startPrefab,new Vector3(Dungeon.startRoom.x * tileScaling ,1,Dungeon.startRoom.y * tileScaling),Quaternion.identity) as GameObject;
		
		
		end_point.transform.parent = transform;
		start_point.transform.parent = transform;
		
		//Spawn Objects;
		List<SpawnList> spawnedObjectLocations = new List<SpawnList>();
		for (int x = 0; x < Dungeon.map_size; x++)
		{
			for (int y = 0; y < Dungeon.map_size; y++)
			{
				if(Dungeon.map[x,y] == 1)
				{
					var location = new SpawnList();
					location.x = x;
					location.y = y;
					if (Dungeon.map[x + 1,y] == 2 || Dungeon.map[x - 1,y] == 2 || Dungeon.map[x,y + 1] == 2 || Dungeon.map[x,y - 1] == 2){
						location.byWall = true;
					}
					spawnedObjectLocations.Add(location);
					
				}
			}
		}
		
		for (int i = 0; i < spawnedObjectLocations.Count; i++)
		{
			SpawnList temp = spawnedObjectLocations[i];
			int randomIndex = Random.Range(i, spawnedObjectLocations.Count);
			spawnedObjectLocations[i] = spawnedObjectLocations[randomIndex];
			spawnedObjectLocations[randomIndex] = temp;
		}
		
		int objectCountToSpawn = 0;
		foreach (SpawnOption objectToSpawn in spawnOptions)
		{
			objectCountToSpawn = Random.Range(objectToSpawn.minSpawnCount,objectToSpawn.maxSpawnCount);
			while (objectCountToSpawn > 0)
			{
				for (int i = 0;i < spawnedObjectLocations.Count;i++)
				{
					bool createHere= false;
					
					if (!spawnedObjectLocations[i].spawnedObject)
					{
						if (objectToSpawn.spawnByWall)
						{
							if (spawnedObjectLocations[i].byWall)
							{
								createHere = true;
							}
						}
						else
						{
							createHere= true;
						}
					}
					if (createHere)
					{
						SpawnList spawnLocation = spawnedObjectLocations[i];
						GameObject newObject;

						newObject = GameObject.Instantiate(objectToSpawn.gameObject,new Vector3(spawnLocation.x * tileScaling ,.7f,spawnLocation.y * tileScaling),Quaternion.identity) as GameObject;
						
						newObject.transform.parent = transform;
						spawnedObjectLocations[i].spawnedObject = newObject;


						if(objectToSpawn.gameObject.name == "Key")
						{
							//PassLevel.S.reset();
							PassLevel levelz = GameObject.FindGameObjectWithTag("exit").GetComponent<PassLevel>();
							levelz.reset();
						}


						objectCountToSpawn--;
						break;
					}
				}
			}
		}
		/// ///////////
		string enemyIndex = enemies[Random.Range (0,enemies.Length)];

		if (GameObject.FindGameObjectWithTag (enemyIndex)) {
			GameObject.FindGameObjectWithTag (enemyIndex).GetComponent<Enemy> ().hasKey = true;
		} else {
			if(enemyIndex == "enemyDog"){
				GameObject.FindGameObjectWithTag("enemySkeleton").GetComponent<Enemy>().hasKey = true;
			}
			if(enemyIndex == "enemySkeleton"){
				GameObject.FindGameObjectWithTag("enemyDog").GetComponent<Enemy>().hasKey = true;
			}
		}

	}
	
	int[][] floodFill(int[][] map,int x,int y)
	{
		if (map[x][y] == 1 || map[x][y] == 3)
		{
			map[x][y] = 4;
		}
		else
		{
			return map;
		}
		map = floodFill(map,x +1,y);
		map = floodFill(map,x,y+1);
		map = floodFill(map,x -1 ,y);
		map = floodFill(map,x ,y -1);
		return map;
	}
	
	
	void Start ()
	{
		if (generate_on_load)
		{
			ClearOldDungeon();
			Generate();
			GameObject.FindGameObjectWithTag ("Console Log").guiText.text = "Welcome to Dungeon Diggers. How far can you make it?";
			GameObject.FindGameObjectWithTag ("uiLevel").guiText.text = "Level: 1";
		}

		aerialCam();
	}

	void Update ()
	{

	}

	void aerialCam()
	{
		float countRows = 0;
		float countCols = 0;
		int[] trackRows = new int[256];
		int[] trackCols = new int[256];

		for(int i=0; i<256; i++){
			for(int j=0; j<256; j++){
				if(Dungeon.map[i,j] != 0 && trackCols[j] == 0)
				{
					countRows += 1;
					trackCols[j] = 1;
				}

				if(Dungeon.map[j,i] != 0 && trackRows[j] == 0)
				{
					countCols += 1;
					trackRows[j] = 1;
				}
			}
		}
		Camera aerialCamTran = GameObject.Find ("_Aerial_Camera").camera;

		Vector3 pos = new Vector3 ();

		pos = aerialCamTran.transform.position;

		pos.x = countCols / 2f;
		pos.z = countRows / 2f;

		aerialCamTran.transform.position = pos;

		while(aerialCamTran.camera.orthographicSize<(countCols/3f))
		{
			aerialCamTran.camera.orthographicSize ++;
		}

		while(aerialCamTran.camera.orthographicSize<(countRows/2f))
		{
			aerialCamTran.camera.orthographicSize ++;
		}
	}
}
