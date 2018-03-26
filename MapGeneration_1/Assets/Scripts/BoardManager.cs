using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using UnityEngine.UI;
using System.IO;


	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.


			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}

		public Text Hor;
		public Text Vert;

		public const int TILE_FLOOR = 0;	
		public const int TILE_WALL = 1;

		public const int MAX_LEAF_SIZE = 20;								//max leaf size
		public List<Leaf> _leafs = new List<Leaf>();						//list of al Leaf objects
		public Leaf l;														//helper Leaf object

		public static List<Rect> _rooms = new List<Rect>();						//list of Rooms
		public static List<Rect> _halls = new List<Rect>();						//List of halls


		public int columns = 60; 										//columns of map
		public int rows = 60;											//Rows of map

		public int[,] cellsArray;										//Array of cells (contains 0 or 1 for Floor or Wall)
		public int[,] cellsArrayTemp;									//Temporary copy of cellsArray - used for flipping map by X or Y

		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.

		//private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();		//A list of possible locations to place tiles.

		public Camera mainCamera;						//Store a reference to our Main Camera 

		int maxIterations = 1;											//maximum map-splitting iterations

		//centers camera based on map size
		void CenterCamera ()
		{
			//calculate initial camera position and scale based on number of columns and rows
			int x = (int)((float)columns / 2);
			int y = (int)((float)rows / 2);
			float z = (float)((((Math.Max(columns,rows)) / 10)) * 5) + 20.0f;
			Vector3 cameraPosition = new Vector3(x, y, -10f);
			mainCamera.transform.SetPositionAndRotation(cameraPosition, Quaternion.identity);
			mainCamera.orthographicSize = z;
		}



		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();

			//Loop through x axis (columns).
			for(int x = 0; x < columns; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 0; y < rows; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}

		// Initialize all cells as floors
		void InitializeCells ()
		{
			//loop through x axis (columns).
			for (int x = 0; x < columns; x++) 
			{
				//Within each column, loop through y axis (rows).
				for (int y = 0; y < rows; y++) 
				{
					//At each cell, initialize cell to 0 (for Floor)
					cellsArray[x,y] = TILE_WALL;
				}
			}
		}

	void InitializeCellsTemp ()
	{
		//loop through x axis (columns).
			for (int x = 0; x < columns; x++) 
			{
				//Within each column, loop through y axis (rows).
				for (int y = 0; y < rows; y++) 
				{
					//At each cell, initialize cell to 0 (for Floor)
					cellsArrayTemp[x,y] = TILE_WALL;
				}
			}
	}

		public class Leaf
		{
		private int MIN_SIZE = 6;
			
			//position and size
			public int x;
			public int y;
			public int width;
			public int height;

			//child leaves
			public Leaf leftChild;
			public Leaf rightChild;

			//room and halls
			public Rect room;
			//public List halls;

			//constructors
			public Leaf(){}
			public Leaf (int x_, int y_, int width_, int height_)
			{
				//initialize Leaf
				x = x_;
				y = y_;
				width = width_;
				height = height_;

			}




		//split a leaf into children
		public Boolean split ()
		{
			//test if already split
			if ((leftChild != null) || (rightChild != null)) {
				return false;
			}	
					
			//split direction is based on width and height
			//if equal, split randomly
			bool splitHor = false;
			//Debug.Log("Parent x, y, width, height: " + x + ", " + y + ", " + width + "," + height);
			//if ((width > height) && (width / height >= 1.25)) {
			if (width > height)  {
				splitHor = false;
				//Debug.Log ("splitHor: false");
			//} else if ((height > width) && (height / width >= 1.25)) {
			} else if (height > width)  {
				splitHor = true;
				//Debug.Log ("splitHor: true");
			}
			
	
			//if too small, return false
			int max = Math.Max (width, height);
			//Debug.Log("Max: " + max);
			if (max <= (MIN_SIZE * 2)) {
				//Debug.Log ("Cancel split");
				return false;
			}
			
			if (splitHor) {
				max = height -3;
			} else {
				max = width -3;
			}

			//choose where to split
			int split = Random.Range (MIN_SIZE, max);
			//Debug.Log("Split: " + split);
			//create children based on horizontal or vertical split
			if (splitHor) {
				leftChild = new Leaf (x, y, width, split);
				rightChild = new Leaf (x, y + split, width, height - split);
				//Debug.Log("LeftChild x, y, width, height: " + x + ", " + y + ", " + width + "," + split);
				//Debug.Log("RightChild x, y, width, height: " + x + ", " + (y + split) + ", " + width + "," + (height-split));
			}
			if (!splitHor) {
				leftChild = new Leaf(x, y, split, height);
				//Debug.Log("LeftChild x, y, width, height: " + x + ", " + y + ", " + split + "," + height);
				rightChild = new Leaf(x + split, y, width - split, height);
				//Debug.Log("RightChild x, y, width, height: " + (x + split) + ", " + y + ", " + (width-split) + "," + height);
			}
			return true; //success
		} //end function split()

		public void createRooms ()
		{
			//generates rooms for leaf and all children
			if ((leftChild != null) || (rightChild != null)) {
				//has children, so create rooms in them
				if (leftChild != null) {
					leftChild.createRooms ();
				}
				if (rightChild != null) {
					rightChild.createRooms ();
				}
				
				//create hall if both children present
				if (leftChild != null && rightChild != null) {
					Debug.Log("Pre-Hall: L_Child x,y: " + leftChild.x + ", " + leftChild.y);
					Debug.Log("Pre-Hall: R_Child x,y: " + rightChild.x + ", " + rightChild.y);
					createHall (leftChild.getRoom (), rightChild.getRoom ());
				}
			} else {
				//create room in this leaf
				Vector2 roomSize;
				Vector2 roomPos;

				roomSize = new Vector2(Random.Range(3, width - 2), Random.Range(3, height - 2));
								
				int roomX = (int)Random.Range(1, width - roomSize.x - 1);
				int roomY = (int)Random.Range(1, height - roomSize.y - 1);
				roomPos = new Vector2(roomX, roomY);
				Debug.Log("New Room: x, y, size x, size y = " + (x + roomPos.x) + ", " + (y + roomPos.y) + ", " + roomSize.x + ", " + roomSize.y);
				room = new Rect(x + roomPos.x, y + roomPos.y, roomSize.x, roomSize.y);
				_rooms.Add(room);
			}
		} //end createRooms()
		
		public Rect getRoom ()
		{	
			
			if (room != null && (room.width != 0 && room.height != 0)) {
				return room;
			} else {
				Debug.Log ("Parent Leaf x,y: " + this.x + ", " + this.y);
				Rect lRoom = new Rect (0, 0, 0, 0);
				Rect rRoom = new Rect (0, 0, 0, 0);
				Rect noResult = new Rect (0, 0, 0, 0);
				
				if (leftChild != null) {
					Debug.Log ("Left Child Not Null!");
					Debug.Log ("Left Child x,y=" + leftChild.x + ", " + leftChild.y);
					Debug.Log ("Left Child Room x,y=" + leftChild.room.x + ", " + leftChild.room.y);
					lRoom = leftChild.getRoom ();
					Debug.Log ("lRoom x,y=" + lRoom.x + ", " + lRoom.y);
				}
				if (rightChild != null) {
					Debug.Log ("Right Child Not Null!");
					Debug.Log ("Right Child x,y=" + rightChild.x + ", " + rightChild.y);
					Debug.Log ("Right Child Room x,y=" + rightChild.room.x + ", " + rightChild.room.y);
					rRoom = rightChild.getRoom ();
					Debug.Log ("rRoom x,y=" + rRoom.x + ", " + rRoom.y);
				}
				Debug.Log ("getRoom Enter Non-recursive part");
				
				if (lRoom.width == 0 && rRoom.width == 0) {
					//return null
					return noResult;
				} else if ((int)rRoom.width == 0) {
					return lRoom;
				} else if ((int)lRoom.width == 0) {
					return rRoom;
				} else if (Random.Range (1, 2) > 1) {
					return lRoom;
				} else {
					return rRoom;
				}



			}
			
		} //end getRoom()

		public void createHall (Rect l, Rect r)
		{
			

			//Debug.Log ("Room1 (x,y,w,h): " + l.x + ", " + l.y + ", " + l.width + ", " + l.height);
			//Debug.Log ("Room2 (x,y,w,h): " + r.x + ", " + r.y + ", " + r.width + ", " + r.height);
			
			//no hallways if either room does not truly exist
			if (((int)l.width == 0 && (int)l.height == 0) || ((int)r.width == 0 && (int)r.height == 0)) {
				//do nothing
				Debug.Log("********************BAD HALLWAY HERE****************");
			} else {
				Vector2 point1 = new Vector2 ((int)(Random.Range (l.xMin + 1, l.xMax - 1)), (int)(Random.Range (l.yMin + 1, l.yMax - 2)));
				Vector2 point2 = new Vector2 ((int)(Random.Range (r.xMin + 1, r.xMax - 1)), (int)(Random.Range (r.yMin + 1, r.yMax - 2)));
				Debug.Log ("point1=" + point1 + ", point2=" + point2);

				int w = (int)(point2.x - point1.x);
				int h = (int)(point2.y - point1.y);


				if (w < 0) {
					if (h < 0) {
						if (Random.Range (1, 2) > 1) {
							_halls.Add (new Rect (point2.x, point1.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point2.x, point2.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						} else {
							_halls.Add (new Rect (point2.x, point2.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point1.x, point2.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						}

					} else if (h > 0) {
						if (Random.Range (1, 2) > 1) {
							_halls.Add (new Rect (point2.x, point1.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point2.x, point1.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						} else {
							_halls.Add (new Rect (point2.x, point2.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point1.x, point1.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						}
					} else {
						_halls.Add (new Rect (point2.x, point2.y, Math.Abs (w), 1));
					}
				} else if (w > 0) {
					if (h < 0) {
						if (Random.Range (1, 2) > 1) {
							_halls.Add (new Rect (point1.x, point2.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point1.x, point2.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						} else {
							_halls.Add (new Rect (point1.x, point1.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point2.x, point2.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						}
					} else if (h > 0) {
						if (Random.Range (1, 2) > 1) {
							_halls.Add (new Rect (point1.x, point1.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point2.x, point1.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						} else {
							_halls.Add (new Rect (point1.x, point2.y, Math.Abs (w)+1, 1));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
							_halls.Add (new Rect (point1.x, point1.y, 1, Math.Abs (h)));
							//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
						}
					} else { // h must now equal 0
						_halls.Add (new Rect (point1.x, point1.y, Math.Abs (w), 1));
						//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
					}
				} else { // w must now equal 0
					if (h < 0) {
						_halls.Add (new Rect (point2.x, point2.y, 1, Math.Abs (h)));
						//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
					} else if (h > 0) {
						_halls.Add (new Rect (point1.x, point1.y, 1, Math.Abs (h)));
						//Debug.Log ("New Hall: x=" + _halls [_halls.Count - 1].x + ", y=" + _halls [_halls.Count - 1].y + ", w=" + _halls [_halls.Count - 1].width + ", h=" + _halls [_halls.Count - 1].height);
					}
				}
			} //end if for hallway existance
		} //end createHall

		} //end class leaf






		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);

			//Return the randomly selected Vector3 position.
			return randomPosition;
		}


		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}


		//SetupScene initializes the map and calls the previous functions to lay out the game board
		public void SetupScene ()
	{

		Hor = GameObject.Find ("lblHorizontalCurrent").GetComponent<Text> ();
		columns = (Int32.Parse (Hor.text));

		Vert = GameObject.Find ("lblVerticalCurrent").GetComponent<Text> ();
		rows = (Int32.Parse (Vert.text));

		//delete any old map tiles, if they exist
		DeleteMapTiles ();


		cellsArray = new int[columns, rows];
		cellsArrayTemp = new int[columns, rows];
		//initialize room and hall data
		_leafs = new List<Leaf> ();						//list of al Leaf objects
		_rooms = new List<Rect> ();						//list of Rooms
		_halls = new List<Rect> ();						//List of halls		

		//Reset our list of gridpositions.
		InitialiseList ();

		//Instantiate wall tile arrays
		InitializeCells ();
		InitializeCellsTemp ();

		//center camera
		CenterCamera ();

		//create outer walls
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				if ((y == -1) || (y == rows) || (x == -1) || (x == columns)) {
					GameObject tileChoice = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					Vector3 tilePosition = new Vector3 (x, y, 0f);
					Instantiate (tileChoice, tilePosition, Quaternion.identity);
				}
			}
		}

		//create first leaf (full map) and add to List
		Leaf root = new Leaf (0, 0, columns, rows);
		_leafs.Add (root);

		//loop through leaves and split until no more splits happen
		bool didSplit = true;
		while (didSplit) {
			didSplit = false;
			//create list for loop
			List<Leaf> _leafsToAdd = new List<Leaf> ();
			foreach (Leaf l in _leafs) {
				//check if leaf does not have children
				if ((l.leftChild == null) && (l.rightChild == null)) {
					//if leaf is either too large or 75% chance
					if ((l.width > MAX_LEAF_SIZE) || (l.height > MAX_LEAF_SIZE) || ((Random.Range (1, 4)) > 1)) {
						//try to split
						if (l.split ()) {
							//if split successful, add children to list
							_leafsToAdd.Add (l.leftChild);
							_leafsToAdd.Add (l.rightChild);
							didSplit = true;
						}
					}

				}
			} //end foreach
			foreach (Leaf l in _leafsToAdd) {
				_leafs.Add (l);
			}

		} //end while

		//create Rooms
		root.createRooms ();
		
		//cycle through each room, and set the value of each tile to floor
		
		foreach (Leaf l in _leafs) {
			if (l.room != null && l.room.size.x != 0) {
				//Debug.Log ("Room Found!");
				//Debug.Log ("Room: x, y, size x, size y = " + l.room.x + ", " + l.room.y + ", " + l.room.size.x + ", " + l.room.size.y);
				//Debug.Log ("Leaf: x(i), y(j), width, height = " + l.x + ", " + l.y + ", " + l.width + ", " + l.height);
				for (int i = l.x; i < (l.x + l.width); i++) {
					for (int j = l.y; j < (l.y + l.height); j++) {
						//if cell is part of room, display floor
						//else display wall
						bool isWall = false;
						//if x < room's x, then WALL
						
						if (i < l.room.x) {
							isWall = true;
						}
		
						//if x > (room's x + room's size -1), then WALL

						if (i > (l.room.x + l.room.size.x - 1)) {
							isWall = true;
						}
	
						//if y < room's y, then WALL
						
						if (j < l.room.y) {
							isWall = true;
						}
	
						//if y > (room's y + room's size - 1), then WALL
						if (j > (l.room.y + l.room.size.y - 1)) {
							isWall = true;
						}
						//Debug.Log ("l.room.x, l.room.y = " + l.room.x + ", " + l.room.y);		
						//Debug.Log ("i, j, = " + i + ", " + j);
						//Debug.Log ("isWall = " + isWall);

						if (isWall) {
							//set to wall (may change during hallway check)
							cellsArray [i, j] = TILE_WALL;
							//GameObject tileChoice = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
							//Vector3 tilePosition = new Vector3 (i, j, 0f);
							//Instantiate (tileChoice, tilePosition, Quaternion.identity);
						} else {
							//set to floor in cellsArray
							cellsArray [i, j] = TILE_FLOOR;
							//GameObject tileChoice = floorTiles [Random.Range (0, floorTiles.Length)];
							//Vector3 tilePosition = new Vector3 (i, j, 0f);
							//Instantiate (tileChoice, tilePosition, Quaternion.identity);
						}
	
					}
				}
			} // end 'if' for room check				

		} //end foreach leaf

		//now set hallway tiles as floor
		foreach (Rect hall in _halls) {
			for (int i = (int)hall.x; i < (int)(hall.x + hall.width); i++) {
				for (int j = (int)hall.y; j < (int)(hall.y + hall.height); j++) {
					//Debug.Log ("Hallway Tile:" + i + ", " + j);
					
					//NEW TEST CODE START
//					if (hall.width > hall.height) {
//						if ((cellsArray [i, j - 2] != TILE_FLOOR) && (cellsArray [i, j + 2] != TILE_FLOOR)) {
//							cellsArray [i, j] = TILE_FLOOR;
//						}
//					} else if (hall.width < hall.height) {
//						if ((cellsArray [i-2, j] != TILE_FLOOR) && (cellsArray [i+2, j] != TILE_FLOOR)) {
//							cellsArray [i, j] = TILE_FLOOR;
//						}
//					} else {
//						cellsArray [i, j] = TILE_FLOOR;
//					}
//					
					//NEW TEST CODE END

					//ORIGINAL STATEMENT:
					cellsArray [i, j] = TILE_FLOOR;
					
				}
			}
		} //end foreach hallway

		//now instantiate tiles
		for (int x = 0; x < columns; x++) {
			//Within each column, loop through y axis (rows).
			for (int y = 0; y < rows; y++) {
				//instantiate wall or floor
				if (cellsArray [x, y] == TILE_FLOOR) { //floor
					GameObject tileChoice = floorTiles [Random.Range (0, floorTiles.Length)];
					Vector3 tilePosition = new Vector3 (x, y, 0f);
					Instantiate (tileChoice, tilePosition, Quaternion.identity);

				} else { //wall
					GameObject tileChoice = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					Vector3 tilePosition = new Vector3 (x, y, 0f);
					Instantiate (tileChoice, tilePosition, Quaternion.identity);
				}
			}
		}
		} //end setupScene()


	//destroy any instantiated tiles
	public void DeleteMapTiles ()
	{
		GameObject[] tiles = GameObject.FindGameObjectsWithTag ("Tiles");
		foreach (GameObject tile in tiles) {
			//destroy walls and floors
			Destroy(tile);
			Debug.Log("Destroying Tile!");
		}
	}

	public Boolean ExportMap (string filename)
	{
		Hor = GameObject.Find ("lblHorizontalCurrent").GetComponent<Text> ();
		columns = (Int32.Parse (Hor.text));

		Vert = GameObject.Find ("lblVerticalCurrent").GetComponent<Text> ();
		rows = (Int32.Parse (Vert.text));

		string rowText = "";

		//open file and generate top outer wall
		for (int y = -1; y < columns + 1; y++) {
			rowText += "*";
		}
		using(StreamWriter writetext = new StreamWriter(filename)){
				writetext.WriteLine(rowText);
			}
		

		//export each row
		for (int y = rows-1; y >= 0; y--) {
			//Within each column, loop through y axis (rows).
			//start by adding first (left) outer map wall
			rowText = "*";
			for (int x = 0; x < columns; x++) {
				//instantiate wall or floor
				if (cellsArray [x, y] == TILE_FLOOR) { //floor
					rowText += " ";

				} else { //wall
					rowText += "*";
				}
			}
			rowText += "*";
			//string now contains full row including outer wall
			//now export to file
			using(StreamWriter writetext = new StreamWriter(filename, append: true)){
				writetext.WriteLine(rowText);
			}

		}
		//add bottom outer wall
		rowText = "";
		for (int y = -1; y < columns + 1; y++) {
			rowText += "*";
		}
		using(StreamWriter writetext = new StreamWriter(filename, append: true)){
				writetext.WriteLine(rowText);
		}
		

		return true;
	}




	}

