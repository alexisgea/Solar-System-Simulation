using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class for the orbital body icon and name appearing on the UI.
/// </summary>
public class BodyLabel : MonoBehaviour {

	// constant to put the label offscreen
	Vector3 offScreen = new Vector3 (-1000, -1000, 0);

	// reference and variables
	public GameObject nameField;
	private OrbitalBody ownerBody;
	private bool showIcon = false;
	//private bool showName = false; // for the future

	// the orbital body owning the label
	private GameObject owner;
	public GameObject Owner { 
		set {
			owner = value;
			nameField.GetComponent<Text> ().text = owner.name;
		}
		get {
			return owner;
		}
	}


	private void Start(){
		
		ownerBody = Owner.GetComponent<OrbitalBody> ();
		transform.position = offScreen;

		FindObjectOfType<InterfaceManager> ().iconToggle += ToggleIcon;
		FindObjectOfType<InterfaceManager> ().fullStart += ToggleIcon;

		GetComponent<Image> ().color = Owner.GetComponent<OrbitalBody> ().uiVisual;
		Sprite newSprite = Resources.Load<Sprite> (Owner.name.ToLower () + "Icon");
		if (newSprite != null)
			GetComponent<Image> ().sprite = newSprite;
	}


	private void Update () {
		if(showIcon)
			UpdatePosition ();
	}


	/// <summary>
	/// Toggles the icon, called from events.
	/// </summary>
	private void ToggleIcon(){
		showIcon = !showIcon;
	}


	/// <summary>
	/// Updates the position from worl to UI.
	/// </summary>
	private void UpdatePosition(){

		const float sizeFactor = 50.0f;

		Vector3 screenPos = Camera.main.WorldToScreenPoint(Owner.transform.position);
		Vector3 camPos = Camera.main.transform.position;
		Vector3 ownerPos = Owner.transform.position;
		float distanceToCam = Vector3.Distance (camPos, ownerPos);

		// if toggle is off or if behind us or if orbital body is close or to far then we hide it (put it offscreen)
		if (screenPos.z < 0 || distanceToCam < sizeFactor * ownerBody.SizeScaled || distanceToCam > sizeFactor * ownerBody.OrbitScaled || !showIcon) {
			if(transform.position != offScreen)
				transform.position = offScreen;
		} else { // otherwise we update the position on screen
			transform.position = screenPos;
		}
	}

}
