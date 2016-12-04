using UnityEngine;
using System.Collections;

// TODO rotate the transform toward the camera then draws all asteroid base on it
// TODO add possibility to draw varrying shapes based on complexity
// TODO correct and improve the inclination of orb its

// This script creates a belt of asteroids and update all of them
// it will directly draw the shape with GL and make sure it faces the camera
// one belt can only have one style of shape (number of vertex per asteroid) and one material
// to vary shape and material just create multiple smaller belt with different settings
// Note: this is by far the fastest solution I found without using shader or GPU instancing
public class AsteroidBelt : MonoBehaviour {

	// belt variable parameters
	[SerializeField]
	private int _beltPopulation;
	[SerializeField]
	private int _minRange;
	[SerializeField]
	private int _maxRange;
	[SerializeField]
	private float _inclinationRange;
	[SerializeField]
	private float _eccentricityRange;	
	[SerializeField]
	private float _asteroidRadius = 1f;
	[SerializeField][Range(2,6)]
	private int _asteroidComplexity = 2;
	[SerializeField]
	private Material _beltMaterial;
	
	// the actual asteroids in an array
	private Asteroid[] _asteroids;

	// variable related to game scales variables (to be moved in a singleton)
	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 
	
	// Use this for initialization
	private void Start () {

		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		GenerateBelt();
	}
	
	// update every frame
	private void Update() {
		_totalTime += Time.deltaTime * _timeScale;
	}

	// called every frame on render
	private void OnRenderObject() {

		if (!_beltMaterial) {
			Debug.LogError("Please assign asteroid belt material on the inspector");
			return;
		}

		GL.PushMatrix();
		_beltMaterial.SetPass(0);

		Vector3 camPosition = Camera.main.transform.position;
		Quaternion camRotation = Camera.main.transform.rotation;
		Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

		for (int i = 0; i < _beltPopulation; i+=2) {
			UpdateAsteroid(i, camPosition, camRotation, frustumPlanes);
			UpdateAsteroid(i+1, camPosition, camRotation, frustumPlanes);
		}
		GL.PopMatrix();

	}

	// Creates an asteroidbelt based on the exposed parameters
	private void GenerateBelt(){

		_asteroids = new Asteroid[_beltPopulation];

		for (int i = 0; i < _beltPopulation; i++) {
			int semiMajorAxis = Random.Range(_minRange, _maxRange);
			float inclination = Random.Range(0f, _inclinationRange).RandomSign();
			float eccentricity = Random.Range(0f, _eccentricityRange);

			_asteroids[i] = AsteroidGenerator(semiMajorAxis, inclination, eccentricity, _asteroidComplexity, _asteroidRadius);
		}
	}

	// TODO improve the plane orientation, especially the perihelionArgument
	// the asteroid generator generate a random asteroid based on the belt parameters given to it
	private Asteroid AsteroidGenerator(int semiMajorAxis, float inclination = 0f, float eccentricity = 0f, int complexity = 3, float size = 1f){

		// generate asteroid shape
		Vector2[] shape = GetAsteroidShape(size, complexity);
		
		// orienting orbit plane // NOT WORKING
		float perihelionArgument = Random.Range(0f, 2*Mathf.PI);
		transform.Rotate(new Vector3(0, perihelionArgument, 0)); // first the perihelion
		transform.Rotate(new Vector3(inclination, 0, 0)); // then the inclination, otherwise all inclination are at the same relative place

		// getting transform parameters 
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;
		Vector3 center = semiMajorAxis * forward * eccentricity;

		// getting missing position parameters
		int semiMinorAxis = (int)(semiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2f)));
		float initialArgument = Random.Range(0f, 2*Mathf.PI);
		float period = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (semiMajorAxis * _orbitScale, 3) / (17.78f));  // forgot what was the 17.78f

		// reseting base transform for next use (extension method)
		transform.ResetTransformation();
		
		// generating asteroid
		return new Asteroid(shape, center, forward, right, semiMajorAxis, semiMinorAxis, initialArgument, period);
	}

	// TODO make it possible to generate more complex shape
	// TODO ensure the more complexe shape won't "cross"
	// generate a random shape based on complexity and size given
	// the first vertex is always the asteroid center (one less rotation to do)
	// thus we only need to compute n-1 vertex of complexity
	// all possible triangle forms can be made in the 2 consécutif quadrant
	// so to simplify the creation and ensure the correct clock order of vertex for drawing
	// we will always get the next vertex in the next quadrant in clockwise order
	// we will also ensure size (length and angles) are within parameters to be nice looking
	private Vector2[] GetAsteroidShape(float size, int complexity){

		bool vertexCorrect = false;

		int safety = 50; // max number of loop (it's a lot but only happens during generation)
		int safetyCounter = 0;	// for not getting stuck too long in loops

		int minSizeDivider = 5;
		float magMin = size / minSizeDivider; // the minimum magnitude accepted (avoid to small or too thin asteroid)

		float minAngle = 0.1f;
		float maxAngle = 0.9f;

		Vector2[] shape = new Vector2[complexity];

		// first vertex
		do{
			// we get one random point in any quadrant
			shape[0] = new Vector2(Random.Range(-size, size), Random.Range(-size, size));

			// we ensure it is far enough from the center and that no value is equal to 0 (to avoid confusion for next quadrant)
			bool magCheck = shape[0].magnitude > magMin;
			bool nullCheck = shape[0].y != 0 && shape[0].x != 0;

			vertexCorrect = magCheck && nullCheck;
			
			safetyCounter += 1;
			if(safetyCounter == safety){
				Debug.LogWarning("Exiting asteroid 1st vector generation - magCheck:" + magCheck + " - nullCheck:" + nullCheck);
			}
		} while (!vertexCorrect && safetyCounter < safety);
		
		safetyCounter = 0;// reset safety for next round

		// second vertex
		do {
			// we create the next point in the next clockwise quadrant
			if(shape[0].y > 0 && shape[0].x > 0)
				shape[1] = new Vector2(Random.Range(0f, size), Random.Range(-size, 0f));
			else if(shape[0].y > 0 && shape[0].x < 0)
				shape[1] = new Vector2(Random.Range(0f, size), Random.Range(0f, size));
			else if(shape[0].y < 0 && shape[0].x > 0)
				shape[1] = new Vector2(Random.Range(-size, 0f), Random.Range(-size, 0f));
			else if(shape[0].y < 0 && shape[0].x < 0)
				shape[1] = new Vector2(Random.Range(-size, 0f), Random.Range(0f, size));

			// we ensure it is far enough from center and first vertex as well as the angle in specified parameters
			bool magCheck = (shape[1].magnitude > magMin) && ((shape[1] - shape[0]).magnitude > magMin);
			bool angleCheck = Vector2.Dot(shape[0].normalized, shape[1].normalized) > minAngle && Vector2.Dot(shape[0].normalized, shape[1].normalized) < maxAngle;
			
			vertexCorrect = magCheck && angleCheck;

			safetyCounter += 1;
			if(safetyCounter == safety){
				Debug.LogWarning("Exiting asteroid 2nd vector generation - magCheck:" + magCheck + " angleCheck:" + angleCheck/* + ":" + Vector2.Dot(shape[0].normalized, shape[1].normalized)*/);
			}
		} while (!vertexCorrect && safetyCounter < safety);
		
		// we return the shape even if it's not correct
		return shape;
	}

	// Update the asteroid position and scale and then draws it on camera
	// also checks if the asteroid is in the view frustum
	private void UpdateAsteroid(int index, Vector3 camPosition, Quaternion camRotation, Plane[] frustumPlanes){
		Vector3 position = ComputeAsteroidPos(index);
		float scale = ComputeScale(position, camPosition);
		Bounds asteroidBounds = new Bounds(position, Vector3.one * _asteroidRadius * scale);

		if(GeometryUtility.TestPlanesAABB(frustumPlanes, asteroidBounds))
			DrawAsteroid (index, position, camRotation, scale);

	}

	// Calculates and return the asteroid position
	private Vector3 ComputeAsteroidPos(int index) {
		// first we calculate how far in the orbit the asterod is
		float theta = _asteroids[index].InitialArgument + (_totalTime / _asteroids[index].Period);
		
		// then we use an elipse parametric equation to get the point of that angle in the orbit
		// elipse focus F(1,2,3), major axis unit vector (forward) M(1,2,3), minor axis unit vector (right) m(1,2,3)
		// semi major axis a, semi minor axis b, theta position angle
		// pos = F + a*cos(theta)*M + b*sin(theta)*m
		// minus in front of theta as the asteroid orbit counter clockwise
		float firstMember = _asteroids[index].SemiMajorAxis * _orbitScale * Mathf.Cos (-theta);
		float secondMember = _asteroids[index].SemiMinorAxis * _orbitScale * Mathf.Sin (-theta);
		Vector3 spacePos = _asteroids[index].OrbitFocus + firstMember * _asteroids[index].MajorVector + secondMember * _asteroids[index].MinorVector;

		return spacePos;
	}	

	// scale is computed as the distance to the camera
	private float ComputeScale(Vector3 position, Vector3 camPos){
		return (position - camPos).magnitude;
	}

	// TODO improve by using GL.TRIANGLE_STRIP for improved complexity
	// TODO resolve the scale issue to draw 2 shapes at a time
	// draws the asteroid with GL lines
	// computes the vertices by rotating the shape to the cam
	private void DrawAsteroid(int index, Vector3 astPos, Quaternion camRot, float scale) {
		
		// rotation is VERY expensive, this step is taking more than one third of the script
		//////////////////
		Vector3 _vert1 = astPos;
		Vector3 _vert2 = astPos + (camRot * _asteroids[index].Shape[0] * scale);
		Vector3 _vert3 = astPos + (camRot * _asteroids[index].Shape[1] * scale);
		//////////////////
		
		GL.Begin(GL.TRIANGLES);
		GL.Vertex3(_vert1.x, _vert1.y, _vert1.z);
		GL.Vertex3(_vert2.x, _vert2.y, _vert2.z);
		GL.Vertex3(_vert3.x, _vert3.y, _vert3.z);
		GL.End();

		// we can draw the oposite asteroid without recomputing anything
		// _vert1 = -_vert1;
		// _vert2 = -_vert2;
		// _vert3 = -_vert3;
		
		// need to reverse the vertex to draw them facing correctly
		// GL.Begin(GL.TRIANGLES);
		// GL.Vertex3(_vert3.x, _vert3.y, _vert3.z);
		// GL.Vertex3(_vert1.x, _vert1.y, _vert1.z);
		// GL.Vertex3(_vert2.x, _vert2.y, _vert2.z);
		// GL.End();
	}

	// TODO check if this is actually necessary, if computation is done unscalled for position then it should not be relevant
	// calculates the orbit beriod of all asteroids
	// called if the orbitscale is changed
	// private void UpdateOrbitPeriods(){
	// 	for (int i = 0; i < _beltPopulation; i++)
	// 		_asteroids[i].Period = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_asteroids[i].SemiMajorAxis * _orbitScale, 3) / (17.78f));
	// }

	// TODO this needs to be in a singleton
	// updates the differents application scale
	// called through events
	public void UpdateScale(string variable, float value) {
        switch (variable) {
            case "time":
                _timeScale = value;
                break;

            case "orbit":
                _orbitScale = value;
				//UpdateOrbitPeriods();
                break;

            default:
                break;
        }
    }

}

// asteroid structure used in the belt
// only holds data and can only be set on creation
public struct Asteroid {

	public Vector2[] Shape {private set; get;}
	public Vector3 OrbitFocus {private set; get;}
	public Vector3 MajorVector {private set; get;}
	public Vector3 MinorVector {private set; get;}
	public int SemiMajorAxis {private set; get;}
	public int SemiMinorAxis {private set; get;}
	public float InitialArgument{private set; get;}
	public float Period{private set; get;}  // set should be private as well (public if we have to recompute periods)

	public Asteroid(Vector2[] shape, Vector3 orbitFocus, Vector3 majorVector, Vector3 minorVector, int semiMajorAxis, int semiMinorAxis, float initialArgument, float period){
		this.Shape = shape;
		this.OrbitFocus = orbitFocus;
		this.MajorVector = majorVector;
		this.MinorVector = minorVector;
		this.SemiMajorAxis = semiMajorAxis;
		this.SemiMinorAxis = semiMinorAxis;
		this.InitialArgument = initialArgument;
		this.Period = period;
	}

}

