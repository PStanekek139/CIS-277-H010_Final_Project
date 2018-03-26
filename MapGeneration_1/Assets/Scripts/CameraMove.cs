using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
	//this camera
	public Camera mainCamera;


	//for camera zoom
	float camDistance = 0f;
	float camMax = 100f;
	float camMin = 0.1f;
	float scrollSpeed = 5f;


	//for camera movement
	float moveSpeed = 100.0f;
	private float totalMove = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//camera movement
		Vector3 inp = GetInput();
		totalMove = Mathf.Clamp(totalMove * 0.5f, 1f, 1000f);
		inp = inp * moveSpeed;
		inp = inp * Time.deltaTime;
		Vector3 newPos = transform.position;
		transform.Translate(inp);

		//camera zoom
		camDistance = mainCamera.orthographicSize;
		camDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
		camDistance = Mathf.Clamp(camDistance, camMin, camMax);
		mainCamera.orthographicSize = camDistance;


	}

	private Vector3 GetInput() 
	{ //0 = not active
        Vector3 direction = new Vector3();
        if ((Input.GetKey (KeyCode.W)) || (Input.GetKey (KeyCode.UpArrow))){
            direction += new Vector3(0, 1 , 0);
        }
		if ((Input.GetKey (KeyCode.S)) || (Input.GetKey (KeyCode.DownArrow))){
            direction += new Vector3(0, -1, 0);
        }
		if ((Input.GetKey (KeyCode.A)) || (Input.GetKey (KeyCode.LeftArrow))){
            direction += new Vector3(-1, 0, 0);
        }
		if ((Input.GetKey (KeyCode.D)) || (Input.GetKey (KeyCode.RightArrow))){
            direction += new Vector3(1, 0, 0);
        }
        return direction;
    }
}
