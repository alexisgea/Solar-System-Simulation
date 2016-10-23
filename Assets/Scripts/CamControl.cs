using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;


/// <summary>
/// Camera Control Script
/// The camera is fixed at the end of a pole itself attached on a central axis
/// The camera can move along the pole as a zoom function
/// The pole can rotate on it's central axis (the Y) as an "orbit cam"
/// The pole can rotate on itself on the X axis to move up and down as an "orbit cam"
/// 
/// The "orbit cam" center is set to one of the celestial body and constantly update it's position to it
/// It is possible to celect a new celestial body
/// </summary>
public class CamControl : MonoBehaviour {

	// needed references
    public GameObject pole;
    public Camera cam;

	// cam control togle
	private bool userControl = false;

	// first menu related variables
	private bool initializeCam = false;

	// audio reference and variables
	public AudioClip selectionSound;
    
	// cam focus variable and event action
	private Transform selectedBody;
	public event Action<Transform> focusing;


	private void Start () {
		
		// set the first body transform to istelf (null) to avoid error with cam zoom function
        selectedBody = transform;

		FindObjectOfType<InterfaceManager> ().fullStart += ControlToggle;
		FindObjectOfType<InterfaceManager> ().fullStart += CamInitialization;

	}



	private void Update () {

		// we always set the cam axis position to the selected celestial object's position.
        transform.position = selectedBody.position;

		if (initializeCam)
			PanCam ();

		if (userControl) {
			checkRotation ();
			checkZoom ();
			checkSelection ();

			// if the player starts moving before cam panning is finished we let him do
			if (initializeCam && Input.anyKey || Input.GetAxis ("Mouse ScrollWheel") != 0)
				initializeCam = false;
		}

    }


	/// <summary>
	/// Toggle on and off of camera control.
	/// Listening to UI events.
	/// </summary>
	private void ControlToggle(){
		userControl = !userControl;
	}


	/// <summary>
	/// Enables the movement when closing the first menu.
	/// Listening to event.
	/// </summary>
	private void CamInitialization(){
		initializeCam = true;
	}


	/// <summary>
	/// Came movement after we close the first menu using lerp functions.
	/// </summary>
	private void PanCam(){

		const float camFinalPos = -20;
		const float camFinalRot = 25;
		const float lerpIntensity = 0.1f;
		const float lerpEnd = 0.1f;
		
		Vector3 pos = cam.transform.localPosition;
		Vector3 rot = pole.transform.localEulerAngles;

		// this is a funcky bit to properly rotate the camera with EulerAngles
		float rotX = rot.x;
		if (rotX > camFinalRot)
			rotX -= 360;
		float newRotX = Mathf.Lerp (rotX, camFinalRot, lerpIntensity);
		if (newRotX < 0)
			newRotX += 360;

		cam.transform.localPosition = Vector3.Lerp(pos, new Vector3(0 , 0, camFinalPos), lerpIntensity); //Mathf.SmoothStep()  // Time.smoothDeltaTime
		pole.transform.localEulerAngles = new Vector3(newRotX,0,0);

		// if we are close enough to the desired position we stop the lerping
		if (Mathf.Abs(pos.z - camFinalPos) <= lerpEnd)
			initializeCam = false;
	}


	/// <summary>
	/// Checks for rotation input and update cam
	/// </summary>
    private void checkRotation()
    {
		const float camSpeed = 25f;

		// rotation of the cam around the center horizontally (on the y axis of Axis)
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(new Vector3(0, -camSpeed * Time.deltaTime, 0));
        else if (Input.GetKey(KeyCode.Q))
            transform.Rotate(new Vector3(0, camSpeed * Time.deltaTime, 0));

        // rotation of the cam around the center vertically (on the x axis of Pole)
        if (Input.GetKey(KeyCode.Z))
            pole.transform.Rotate(new Vector3(camSpeed * Time.deltaTime, 0, 0));
        else if (Input.GetKey(KeyCode.S))
            pole.transform.Rotate(new Vector3(-camSpeed * Time.deltaTime, 0, 0));
    }


	/// <summary>
	/// Checks for zoom input and update the cam position
	/// </summary>
    private void checkZoom()
    {
		// cam moving in and out as a zoom to the center
		const float zoomSpeed = 2f;
		const float zoomMin = -0.1f;
		const float zoomMax = -500.0f;

		if (!Input.GetKey (KeyCode.LeftShift) && !Input.GetKey (KeyCode.LeftControl) && !Input.GetKey (KeyCode.Space)){
			Vector3 pos = cam.transform.localPosition;
			float Z = pos.z + zoomSpeed * Input.GetAxis ("Mouse ScrollWheel") * cam.transform.localPosition.magnitude;
			pos.z = Mathf.Clamp (Z, zoomMax, zoomMin);
			cam.transform.localPosition = pos;
		}
    }


	/// <summary>
	/// Checks if a new celestial body has been selected with a raycast.
	/// In both case if yes we send a signal and play a selection sound.
	/// </summary>
    private void checkSelection()
    {

		const float rayLength = 1000000f;

		if (Input.GetMouseButton (0)) {

			// first we cast a Ray on the UI to see if we catched one of the icon
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, rayLength)) {
				if (hit.collider != null) {
					selectedBody = hit.collider.transform;

					if (focusing != null)
						focusing (selectedBody);
					
					if(!GetComponent<AudioSource> ().isPlaying)
						GetComponent<AudioSource> ().PlayOneShot (selectionSound);
				}
			

			// if we didn't catch an Icon, we cast a 3D ray to catch a colission sphere on one of the body
			} else {

				PointerEventData pointer = new PointerEventData (EventSystem.current);
				pointer.position = Input.mousePosition;

				List<RaycastResult> raycastResults = new List<RaycastResult> ();
				EventSystem.current.RaycastAll (pointer, raycastResults);

				if (raycastResults.Count > 0 && raycastResults [0].gameObject.tag == "BodyLabel") {
					// As the UI is on top it should always be on 0? Or something like that, to be checked.
					selectedBody = raycastResults [0].gameObject.GetComponent<BodyLabel> ().Owner.transform;

					if (focusing != null)
						focusing (selectedBody);

					if(!GetComponent<AudioSource> ().isPlaying)
						GetComponent<AudioSource> ().PlayOneShot (selectionSound);
				}
			}

		}

    }

}
