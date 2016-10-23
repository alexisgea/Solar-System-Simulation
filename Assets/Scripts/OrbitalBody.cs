using UnityEngine;
using System.Collections;

/// <summary>
/// Script managing the behaviour of the Orbital axis and it's child orbital body
/// Sets the initial body parameter
/// Updates the scales of all parameters if necessary
/// Updates the orbit rotation and the body's day rotation
/// </summary>
public class OrbitalBody : MonoBehaviour {

	// references to objects
	public GameObject stellarParent;
	public GameObject body;
	public Material lineMat;

	// variables related to the orbital axis
    public float sideralYear;
	public float semiMajorAxis = 2f;
	public float inclination = 0f;
	[Range(0f,1f)]
	public float eccentricity = 0f;
	public float argAscending = 0f; // longitude
	public float argPerihelion = 0f; // argument
	public float startMeanAnomaly = 0f; // J2000
	private float meanAnomaly;

	// variables related to the orbital body
    public float dayLength = 1f;
	public float startDayRotation = 0f;
    public float size = 1f;
    public float tilt;
	public float rightAscension = 0f;
	public float declination = 0f;
	const float eclipticTilt = 23.44f;
	private float angularVelocity;

	// scales from the stellar system
	private float timeScale;
	public float SizeScaled { private set; get; }
	public float OrbitScaled { private set; get; }

	// visual path related variables
	public Color uiVisual;
	private bool showPath = false;


    private void Start () {

		FindObjectOfType<StellarSystem> ().scaling += UpdateScale;
		FindObjectOfType<InterfaceManager> ().orbitToggle += TogglePath;
		FindObjectOfType<InterfaceManager> ().fullStart += TogglePath;
		SetScales();
		SetJ2000 ();
		AdvanceOrbit ();

    }


	private void Update () {

		AdvanceOrbit ();
		AdvanceDayRotation ();

	}


	private void OnRenderObject(){
		if (showPath)
			DrawPath ();
	}


	/// <summary>
	/// Initialise the orbital body state as per the J2000 data.
	/// </summary>
	private void SetJ2000(){
		transform.Rotate(new Vector3(0, 0, (90-declination)));
		transform.Rotate(new Vector3(0, -rightAscension, 0));
		transform.Rotate(new Vector3(eclipticTilt, 0, 0));
		transform.Rotate(new Vector3(0, startDayRotation, 0), Space.Self);
		angularVelocity = Mathf.Deg2Rad * (360 / sideralYear); // how much degree is covered each day
		meanAnomaly = startMeanAnomaly * Mathf.Deg2Rad;
	}


	/// <summary>
	/// Initialise the scales from the StellarSystem.
	/// </summary>
	private void SetScales(){
		timeScale = FindObjectOfType<StellarSystem> ().TimeScale;
		OrbitScaled = semiMajorAxis * FindObjectOfType<StellarSystem> ().OrbitScale;
		SizeScaled = size * FindObjectOfType<StellarSystem> ().BodyScale;
		body.transform.localScale = Vector3.one * SizeScaled;
		
	}


	/// <summary>
	/// Toggles the orbit path.
	/// Called on events.
	/// </summary>
	public void TogglePath(){
		showPath = !showPath;
	}


	/// <summary>
	/// Updates the scale.
	/// Called on event from the solar system script.
	/// </summary>
	/// <param name="variable">Which scale.</param>
	/// <param name="value">Scale value.</param>
	public void UpdateScale (string variable, float value){

		switch (variable) {
		case "time":
			timeScale = value;
			break;
		case "body":
			SizeScaled = size * value;
			body.transform.localScale = Vector3.one * SizeScaled;
			break;
		case "orbit":
			OrbitScaled = semiMajorAxis * value;
			break;
		default:
			Debug.Log ("Wrong variable name in Body.updateScales() .");
			break;

		}
	}


	/// <summary>
	/// Calculate how much the body has moved in it's orbit and rotate it's axis accordingly
	/// </summary>
	private void AdvanceOrbit(){

		meanAnomaly += angularVelocity * Time.deltaTime * timeScale;
		if (meanAnomaly > 2f * Mathf.PI)  // we keep mean anomaly within 2*Pi
			meanAnomaly -= 2f * Mathf.PI;

		Vector3 orbitPos = GetPosition(meanAnomaly);
		Vector3 parentPos = stellarParent.transform.position; // position of the parent to offset the calculated pos (used for moons)
		transform.position = new Vector3 (orbitPos.x + parentPos.x, orbitPos.y + parentPos.y, orbitPos.z + parentPos.z);
	}


	/// <summary>
	/// Rotate the body on itself as per it's day length
	/// </summary>
	private void AdvanceDayRotation(){
		float rot = Time.deltaTime * timeScale * (360 / dayLength);
		body.transform.Rotate(new Vector3(0, -rot, 0)); // counter clockwise is the standard rotation considered
	}


	/// <summary>
	/// Draws the orbit by calculating each next point in a circle
	/// Then using the GL.Lines to draw a line between each point
	/// </summary>
	private void DrawPath(){

		const float pathDetail = 0.01f;

		GL.PushMatrix();
		lineMat.color = uiVisual;
		lineMat.SetPass(0);
		GL.Begin(GL.LINES);

		Vector3 parentPos = stellarParent.transform.position;

		// we just loop through a full circle by the path details and make a line for each step
		for(float theta = 0.0f; theta < (2f*Mathf.PI); theta += pathDetail) {
			float anomalyA = theta;
			Vector3 orbitPointA = GetPosition(anomalyA);
			GL.Vertex3(orbitPointA.x + parentPos.x, orbitPointA.y + parentPos.y, orbitPointA.z + parentPos.z);

			float anomalyB = theta + pathDetail;
			Vector3 orbitPointB = GetPosition(anomalyB);
			GL.Vertex3(orbitPointB.x + parentPos.x, orbitPointB.y + parentPos.y, orbitPointB.z + parentPos.z);
		}

		GL.End();
		GL.PopMatrix();

	}


	/// <summary>
	/// Computes the Eccentric Anomaly. Angles to be passed in Radians.
	/// The Newton method used to solve E implies getting a first guess and itterating until the value is precise enough.
	/// </summary>
	/// <returns>The Eccentric Anomaly.</returns>
	/// <param name="e">Eccentricity.</param>
	/// <param name="M">Mean anomaly.</param>
	/// <param name="dp">Decimal precision.</param>
	private float EccentricAnomaly(float M, int dp=5){

		//E(n+1) = E(n) - f(E) / f'(E)
		//f(E) = E - e * sin(E) - M
		//f'(E) = 1 - e * cos(E)
		//we are happy when f(E)/f'(E) is small enough

		int maxIter = 20;  // we make sure we won't loop too much
		int i = 0;
		float e = eccentricity;
		float precision = Mathf.Pow (10, -dp);
		float E, F;

		// if the eccentricity is high we guess the Mean anomaly for E, otherwise we guess PI
		E = (e < 0.8)? M : Mathf.PI;
		F = E - e * Mathf.Sin (M) - M;  //f(E)

		// we will interate until f(E) higher than our wanted precision (as devided then by f'(E))
		while ((Mathf.Abs (F) > precision) && (i < maxIter)) {
			E = E - F / (1f - e * Mathf.Cos (E));
			F = E - e * Mathf.Sin (E) - M;
			i++;
		}

		return E;
	}


	/// <summary>
	/// Computes the True Anomaly. Angles to be passed in Radians.
	/// </summary>
	/// <remarks>
	/// We could add a decimal precision parameter to round the TA.
	/// </remarks>
	/// <returns>The True Anomaly.</returns>
	/// <param name="e">Eccentricity.</param>
	/// <param name="E">Eccentric Anomaly.</param>
	private float TrueAnomaly(float E) {

		// from wikipedia we can find several way to solve TA from E
		// I tried sin(TA) = (sqrt(1-e*e) * sin(E))/(1 -e*cos(E)) but it didn't work properly for some reason
		// so I sued the following as one of the my sources(jgiesen.de/Kepler) tan(TA) = (sqrt(1-e*e) * sin(E)) / (cos(E) - e)

		float e = eccentricity;
		float numerator = Mathf.Sqrt (1f - e*e) * Mathf.Sin (E);
		float denominator = Mathf.Cos (E) - e;
		float TA = Mathf.Atan2 (numerator, denominator);

//		float numerator = Mathf.Sqrt (1f - e * e) * Mathf.Sin (E);
//		float denominator = 1f - e * Mathf.Cos (E);
//		float TA = Mathf.Asin (numerator / denominator);

		return TA;
	}


	/// <summary>
	/// Compute a point's position in a given orbit. All angles are to be passed in Radians.
	/// </summary>
	/// <returns>The point position.</returns>
	/// <param name="M">Mean anomaly.</param>
	public Vector3 GetPosition(float M) {

		float e = eccentricity;
		float a = OrbitScaled; // semiMajorAxis
		float N = argAscending * Mathf.Deg2Rad; // not const as might vary with precession
		float w = argPerihelion * Mathf.Deg2Rad;
		float i = inclination * Mathf.Deg2Rad;

		float E = EccentricAnomaly(M);
		float TA = TrueAnomaly(E);
		float focusRadius = a * (1 - Mathf.Pow(e, 2f)) / (1 + e * Mathf.Cos(TA));

		// parametric equation of an elipse using the orbital elements
		float X =  focusRadius * (Mathf.Cos(N) * Mathf.Cos(TA + w) - Mathf.Sin(N) * Mathf.Sin(TA + w)) * Mathf.Cos(i);
		float Y = focusRadius * Mathf.Sin(TA + w) * Mathf.Sin(i);
		float Z = focusRadius * (Mathf.Sin(N) * Mathf.Cos(TA+w) + Mathf.Cos(N) * Mathf.Sin(TA + w)) * Mathf.Cos(i);

		Vector3 orbitPoint = new Vector3 (X, Y, Z);

		return orbitPoint;
	}
		
}
