using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Stellar System script to manage the current scale of time, orbit and body size.
/// </summary>
public class StellarSystem : MonoBehaviour {

	// stellar system scale base variables
	private float bodyScale = 0.01f;
	public float BodyScale {
		get { return bodyScale; }
		set { bodyScale = value; }
	}

	private float orbitScale = 0.0001f;
	public float OrbitScale {
		get { return orbitScale; }
		set { orbitScale = value; }
	}

	private float timeScale = 0.5f;
	public float TimeScale {
		get { return timeScale; }
		set { timeScale = value; }
	}

	// scale min and max constants
	const float minTimeScale = 0.001f;
	const float maxTimeScale = 500.0f;
	const float minDefaultScale = 0.0001f;
	const float maxDefaultScale = 1.0f;

	// scales control togle
	bool userControl = false;

	public event Action<string, float> scaling;


	private void Start(){
		FindObjectOfType<InterfaceManager> ().fullStart += ControlToggle;
	}


	private void Update () {
		if (userControl)
			CheckInput ();

    }


	/// <summary>
	/// Toggle on and off of scales control.
	/// Listening to UI events.
	/// </summary>
	private void ControlToggle(){
		userControl = !userControl;
	}


	/// <summary>
	/// Checks the input for scale controls.
	/// </summary>
	private void CheckInput(){

		// set body scale
		if (Input.GetKey(KeyCode.LeftShift))
			BodyScale = updateScale("body", BodyScale);

		// set orbit scale
		else if (Input.GetKey(KeyCode.LeftControl))
			OrbitScale = updateScale("orbit", OrbitScale);

		// set time scale
		else if (Input.GetKey(KeyCode.Space))
			TimeScale = updateScale("time", TimeScale, minTimeScale, maxTimeScale);
	}


	/// <summary>
	/// Updates any of the scale and send a signal that it has been changed.
	/// </summary>
	/// <returns>The new scale factor.</returns>
	/// <param name="which">Which factor is changing.</param>
	/// <param name="scale">The current value of the scale.</param>
	/// <param name="min">Minimum value possible for that scale.</param>
	/// <param name="max">Max value possible for that scale..</param>
	private float updateScale (string which, float scale, float min = minDefaultScale, float max = maxDefaultScale){
		scale *= (1 + Input.GetAxis("Mouse ScrollWheel"));
		scale = Mathf.Clamp (scale, min, max);

		if(scaling != null)
			scaling(which, scale);

		return scale;
	}

}