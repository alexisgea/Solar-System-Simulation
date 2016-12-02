using UnityEngine;
using System.Collections;

public class AsteroidBelt : MonoBehaviour {

	[SerializeField]
	private int _beltPopulation;
	[SerializeField]
	private int _maxRange;
	[SerializeField]
	private int _minRange;
	[SerializeField]
	private float _inclinationRange;
	[SerializeField]
	private float _eccentricityRange;	
	[SerializeField]
	private float _asteroidRadius = 1f;
	[SerializeField][Range(3,6)]
	private int _asteroidComplexity = 3;
	[SerializeField]
	private Material _beltMaterial;

	private Asteroid[] _asteroids;

	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 

	private Vector3 _vert1;
	private Vector3 _vert2;
	private Vector3 _vert3;
	
	// Use this for initialization
	private void Start () {

		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		GenerateBelt();

	}
	
	private void Update() {
		_totalTime += Time.deltaTime * _timeScale;
	}

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

	private void UpdateAsteroid(int i, Vector3 camPosition, Quaternion camRotation, Plane[] frustumPlanes){
		Vector3 position = ComputeAsteroidPos(i);
		float scale = ComputeScale(i, position, camPosition);
		Bounds asteroidBounds = new Bounds(position, Vector3.one * _asteroidRadius * scale);

		if(GeometryUtility.TestPlanesAABB(frustumPlanes, asteroidBounds))
			DrawAsteroid (i, position, camRotation, scale);

	}

	private Vector3 ComputeAsteroidPos(int i) {
		float rot = _asteroids[i].InitialArgument + (_totalTime / _asteroids[i].Period);
		
		float firstMember = _asteroids[i].SemiMajorAxis * _orbitScale * Mathf.Cos (-rot);
		float secondMember = _asteroids[i].SemiMinorAxis * _orbitScale * Mathf.Sin (-rot);
		Vector3 spacePos = _asteroids[i].OrbitFocus + firstMember * _asteroids[i].MajorVector + secondMember * _asteroids[i].MinorVector;

		return spacePos;
	}	

	private float ComputeScale(int j, Vector3 position, Vector3 camPos){
		Vector3 diff = position - camPos;
		return diff.magnitude;
	}

	private void DrawAsteroid(int i, Vector3 astPos, Quaternion camRot, float scale) {
		
		// this step seems to take half the script time
		// would it be possible to only rotate one asteroid and then base all the other on the same rotation?
		//////////////////
		_vert1 = astPos + (camRot * _asteroids[i].Shape[0] * scale);
		_vert2 = astPos + (camRot * _asteroids[i].Shape[1] * scale);
		_vert3 = astPos + (camRot * _asteroids[i].Shape[2] * scale);
		//////////////////

		GL.Begin(GL.TRIANGLES);
		GL.Vertex3(_vert1.x, _vert1.y, _vert1.z);
		GL.Vertex3(_vert2.x, _vert2.y, _vert2.z);
		GL.Vertex3(_vert3.x, _vert3.y, _vert3.z);
		GL.End();

		// _vert1 = -_vert1;
		// _vert2 = -_vert2;
		// _vert3 = -_vert3;
		
		// GL.Begin(GL.TRIANGLES);
		// GL.Vertex3(_vert3.x, _vert3.y, _vert3.z);
		// GL.Vertex3(_vert1.x, _vert1.y, _vert1.z);
		// GL.Vertex3(_vert2.x, _vert2.y, _vert2.z);
		// GL.End();
	}


	private void RecomputePeriods(){
		for (int i = 0; i < _beltPopulation; i++)
			_asteroids[i].Period = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_asteroids[i].SemiMajorAxis * _orbitScale, 3) / (17.78f));
	}

	public void UpdateScale(string variable, float value) {
        switch (variable) {
            case "time":
                _timeScale = value;
                break;

            case "orbit":
                _orbitScale = value;
				RecomputePeriods();
                break;

            default:
                break;
        }
    }

	private void GenerateBelt(){

		_asteroids = new Asteroid[_beltPopulation];

		for (int i = 0; i < _beltPopulation; i++) {
			int semiMajorAxis = Random.Range(_minRange, _maxRange);
			float inclination = Random.Range(0f, _inclinationRange).RandomSign();
			float eccentricity = Random.Range(0f, _eccentricityRange);

			_asteroids[i] = AsteroidGenerator(semiMajorAxis, inclination, eccentricity, _asteroidComplexity, _asteroidRadius);
		}

		

	}

	private Asteroid AsteroidGenerator(int semiMajorAxis, float inclination = 0f, float eccentricity = 0f, int complexity = 3, float size = 1f){

		// generate asteroid shape
		Vector2[] shape = new Vector2[complexity];
		for(int i=0; i<complexity; i++){
			float x = Random.Range(size/2, size).RandomSign();
			float y = Random.Range(size/2, size).RandomSign();
			shape[i] = new Vector2(x, y);
		}
		
		// orienting orbit plane // NOT WORKING
		float perihelionArgument = Random.Range(0f, 2*Mathf.PI);
		transform.Rotate(new Vector3(inclination, perihelionArgument, 0));

		// getting transform parameters 
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;
		Vector3 center = semiMajorAxis * forward * eccentricity;

		// getting missing position parameters
		int semiMinorAxis = (int)(semiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2f)));
		float initialArgument = Random.Range(0f, 2*Mathf.PI);
		float period = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (semiMajorAxis * _orbitScale, 3) / (17.78f));  // forgot what was the 17.78f

		// reseting base transform
		transform.ResetTransformation();
		
		// generating asteroid
		return new Asteroid(shape, center, forward, right, semiMajorAxis, semiMinorAxis, initialArgument, period);
	}

}


class Asteroid {

	public Vector2[] Shape {private set; get;}
	public Vector3 OrbitFocus {private set; get;}
	public Vector3 MajorVector {private set; get;}
	public Vector3 MinorVector {private set; get;}
	public int SemiMajorAxis {private set; get;}
	public int SemiMinorAxis {private set; get;}
	public float InitialArgument{private set; get;}
	public float Period{set; get;}  // set should be private as well

	public Asteroid(Vector2[] shape, Vector3 orbitFocus, Vector3 majorVector, Vector3 minorVector, int semiMajorAxis, int semiMinorAxis, float initialArgument, float period){
		Shape = shape;
		OrbitFocus = orbitFocus;
		MajorVector = majorVector;
		MinorVector = minorVector;
		SemiMajorAxis = semiMajorAxis;
		SemiMinorAxis = semiMinorAxis;
		InitialArgument = initialArgument;
		Period = period;
	}

}

