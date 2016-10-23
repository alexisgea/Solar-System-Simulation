using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// UI base class to display the current state of the game, focus, date and scale.
/// </summary>
public class StateDisplay : MonoBehaviour {

	public GameObject timeScaleDisplay;
	public GameObject orbitScaleDisplay;
	public GameObject bodyScaleDisplay;
	public GameObject focusDisplay;

	// potentialy changeable through UI in the future?
	private float timeScale;


	private void Start () {
		FindObjectOfType<StellarSystem> ().scaling += UpdateScale;
		FindObjectOfType<CamControl> ().focusing += UpdateFocus;
		//SetScales ();

	}


	/// <summary>
	/// Sets the scales info on the UI.
	/// The three commented-out scales were not so interesting but are kept for potential UI improvement.
	/// </summary>
	private void SetScales(){
		timeScale = FindObjectOfType<StellarSystem> ().TimeScale;
//		timeScaleDisplay.GetComponent<Text> ().text = "Time Scale: " + timeScale.ToString("F4");
//		bodyScaleDisplay.GetComponent<Text> ().text = "Body Scale: " + FindObjectOfType<StellarSystem> ().BodyScale.ToString("F4");
//		orbitScaleDisplay.GetComponent<Text> ().text = "Orbit Scale: " + FindObjectOfType<StellarSystem> ().OrbitScale.ToString("F4");
	}


	/// <summary>
	/// Updates the scales info on the UI.
	/// </summary>
	/// <param name="variable">Which scale.</param>
	/// <param name="value">Scale value.</param>
	private void UpdateScale (string variable, float value){

		switch (variable) {
		case "time":
			timeScale = value;
			timeScaleDisplay.GetComponent<Text> ().text = "Time Scale: " + timeScale.ToString("F4");
			break;
		case "body":
			bodyScaleDisplay.GetComponent<Text> ().text = "Body Scale: " + value.ToString("F4");
			break;
		case "orbit":
			orbitScaleDisplay.GetComponent<Text> ().text = "Orbit Scale: " + value.ToString("F4");
			break;
		default:
			Debug.Log ("Wrong variable name in StateDiplay.UpdateScale() .");
			break;

		}
	}

	/// <summary>
	/// Updates the UI with the focused body's name.
	/// </summary>
	/// <param name="body">Body transform.</param>
	public void UpdateFocus (Transform body){

		string focusName = body.name;
		if (focusName == "Body")
			focusName = body.parent.name;	
		focusDisplay.GetComponent<Text> ().text = "Focus: " + focusName;
	}

}
