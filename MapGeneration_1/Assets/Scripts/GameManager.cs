using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;


using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.


	private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the map

	private UIManager UIScript;								//Store a reference to our UIManager which will manager the UI panel

	public Button GenerateMapButton;						//Store a reference to our Generate Map button
	public Button ExportMapButton;							//Store a reference to out Export Map button
	public Text SaveMessage;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. 
			Destroy(gameObject);	

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);


		//Get a component reference to the attached BoardManager script
		boardScript = GetComponent<BoardManager>();

		//create onClick listeners and setup UI
		GenerateMapButton.onClick.AddListener(GenerateOnClick);
		ExportMapButton.interactable = false;
		SaveMessage.text = "";
		ExportMapButton.onClick.AddListener(ExportOnClick);

		
	}

	//Initializes the map.
	void InitGame()
	{
		//Call the SetupScene function of the BoardManager script
		boardScript.SetupScene();
	}
	
	//generate new map
	void GenerateOnClick ()
	{
		

		Debug.Log("Generating Map!");
		SaveMessage.text = "";
		ExportMapButton.interactable = false;
		InitGame();
		ExportMapButton.interactable = true;
	}

	//export map using timestamp as filename
	void ExportOnClick ()
	{
		
		Debug.Log ("Exporting Map: ");
		ExportMapButton.interactable = false;
		string fileName = "MAP_" + (DateTime.Now).ToString ("yyyyMMddHHmmssffff") + ".txt";
				
		//pass to export function on BoardManager
		bool result = boardScript.ExportMap (fileName);
	
		//if successful, update display
		if (result) {
			SaveMessage.text = "Map Saved as: " + fileName;
		} else {
			SaveMessage.text = "Error saving map!";
		}
		ExportMapButton.interactable = true;

	}



	//Update is called every frame.
	void Update()
	{
		if (Input.GetKey("escape"))
			Application.Quit();
	}


}
