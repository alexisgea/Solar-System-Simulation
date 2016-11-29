using UnityEngine;
using System.Collections;

public class AsteroidBelt : MonoBehaviour {

	[SerializeField]
	private Material _astMat;

	[SerializeField]
	private int _fieldSize;

	[SerializeField]
	private float _astRadius = 1f;

	private Transform[] _astPos;
	private float[] _astSemiMajAxis; // semi major axis in km
	private float[] _astStartOrbitRot; // start position in radian
	private float[] _astInclinaison; // orbit inclination in radian
	private float[] _orbitPeriod;


	private Vector3 _astPlaneCenter  = new Vector3(0, 0, 0);
	private Vector3 _astPlaneRight = new Vector3(1, 0, 0);
	private Vector3 _astPlaneUp  = new Vector3(0, 1, 0);
	private Vector3 _astPlaneBack  = new Vector3(0, 0, 1);
	
	private Vector2[] _vertOffset1;
	private Vector2[] _vertOffset2;
	private Vector2[] _vertOffset3;

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

		_astPos = new Transform[_fieldSize];
		_astSemiMajAxis = new float[_fieldSize];
		_astStartOrbitRot = new float[_fieldSize];
		_astInclinaison = new float[_fieldSize];
		_orbitPeriod = new float[_fieldSize];
		_vertOffset1 = new Vector2[_fieldSize];
		_vertOffset2 = new Vector2[_fieldSize];
		_vertOffset3 = new Vector2[_fieldSize];

		for (int j = 0; j < _fieldSize; j++) {
			_astPos[j] = transform;
			_astSemiMajAxis [j] = Random.Range (327800f, 476800f);
			_astStartOrbitRot [j] = Mathf.Deg2Rad * Random.Range(0f, 360f);
			_astInclinaison [j] = Mathf.Deg2Rad * Random.Range(-20f, 20f);
			_orbitPeriod[j] = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_astSemiMajAxis [j] * _orbitScale, 3) / (17.78f));
			_vertOffset1 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));
			_vertOffset2 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));
			_vertOffset3 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));

		}

	}
	

	private void Update() {
		_totalTime += Time.deltaTime * _timeScale;
	}

	private void OnRenderObject() {

		if (!_astMat) {
			Debug.LogError("Please assign asteroid belt material on the inspector");
			return;
		}

		GL.PushMatrix();
		_astMat.SetPass(0);

		Vector3 camPosition = Camera.main.transform.position;
		Quaternion camRotation = Camera.main.transform.rotation;
		Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

		for (int j = 0; j < _fieldSize; j+=2) {
			UpdateAsteroid(j, camPosition, camRotation, frustumPlanes);
			UpdateAsteroid(j+1, camPosition, camRotation, frustumPlanes);
		}
		GL.PopMatrix();

	}

	private void UpdateAsteroid(int j, Vector3 camPosition, Quaternion camRotation, Plane[] frustumPlanes){
		_astPos[j].position = ComputeAstPos(j);
		float scale = ComputeScale(j, camPosition);
		Bounds asteroidBouds = new Bounds(_astPos[j].position, Vector3.one * _astRadius * scale);
		
		if(GeometryUtility.TestPlanesAABB(frustumPlanes, asteroidBouds))
			DrawAsteroid (j, _astPos[j].position, camRotation, scale);

	}

	private Vector3 ComputeAstPos(int j) {
		//float T = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_astSemiMajAxis [j] * _orbitScale, 3) / (17.78f));
		float T = _orbitPeriod[j];
		float rot = _astStartOrbitRot[j] + (_totalTime / T); // float rot = s[j] + (totalTime / T[j]);

		// float x = _astSemiMajAxis[j] * _orbitScale * Mathf.Cos (_astInclinaison[j]) * Mathf.Cos (rot);  // maybe cos(i) and not sin(i)
		// float y = _astSemiMajAxis[j] * _orbitScale * Mathf.Sin (_astInclinaison[j]) * Mathf.Cos (rot);
		// float z = _astSemiMajAxis[j] * _orbitScale * Mathf.Sin (rot);
		
		float firstMember = _astSemiMajAxis[j] * _orbitScale * Mathf.Cos (rot);
		float secondMember = _astSemiMajAxis[j] * _orbitScale * Mathf.Sin (rot);

		float x = _astPlaneCenter.x + firstMember * _astPlaneRight.x + secondMember * _astPlaneBack.x;
		float y = _astPlaneCenter.y + firstMember * _astPlaneRight.y + secondMember * _astPlaneBack.y;
		float z = _astPlaneCenter.z + firstMember * _astPlaneRight.z + secondMember * _astPlaneBack.z;

		return new Vector3(x, y, z);
	}	

	private float ComputeScale(int j, Vector3 camPos){
		Vector3 diff = _astPos[j].position - camPos;
		return diff.magnitude;
	}

	private void DrawAsteroid(int j, Vector3 astPos, Quaternion camRot, float scale) {
		
		// this step seems to take half the script time
		//////////////////
		_vert1 = astPos + (camRot * _vertOffset1[j] * scale);
		_vert2 = astPos + (camRot * _vertOffset2[j] * scale);
		_vert3 = astPos + (camRot * _vertOffset3[j] * scale);
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
		for (int j = 0; j < _fieldSize; j++)
			_orbitPeriod[j] = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_astSemiMajAxis [j] * _orbitScale, 3) / (17.78f));
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
}
