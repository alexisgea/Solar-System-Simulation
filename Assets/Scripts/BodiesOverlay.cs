using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class instantiate a BodyLabel as chidl for each orbital body in the solar system.
/// </summary>
public class BodiesOverlay : MonoBehaviour {

	public GameObject stellarSystem;
	public GameObject label;


	private void Start () {
		InstantiateLabels ();

	}


	/// <summary>
	/// Instantiates a BodyLabel as chidl for each orbital body in the solar system.
	/// </summary>
	private void InstantiateLabels(){
		int i = 0;
		foreach (Transform body in stellarSystem.transform) {

			GameObject newLabel = Instantiate (label);
			newLabel.GetComponent<BodyLabel> ().Owner = body.gameObject;
			newLabel.transform.SetParent (transform);

			i++;
		}
	}


}