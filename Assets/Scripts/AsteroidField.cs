using UnityEngine;
using System.Collections;

public class AsteroidField : MonoBehaviour {

	[SerializeField]
	private Asteroid _asteroid;

	[SerializeField]
	private int _fieldSize;

	private Asteroid[] _field;
	private float[] _astSemiMajAxis; // semi major axis in km
	private float[] _astStartOrbitRot; // start position in radian
	private float[] _astInclinaison; // orbit inclination in radian

	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 

	// Use this for initialization
	private void Start () {
		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		_field = new Asteroid[_fieldSize * 2];
		_astSemiMajAxis = new float[_fieldSize];
		_astStartOrbitRot = new float[_fieldSize];
		_astInclinaison = new float[_fieldSize];

		for (int i = 0; i < _fieldSize; i++) {
			_field[i] = Instantiate(_asteroid);
			_field[i].GetComponent<MeshFilter>().mesh = CreateAsteroidMeshes();

			_field[i + _fieldSize] = Instantiate(_asteroid);
			_field[i + _fieldSize].GetComponent<MeshFilter>().mesh = CreateAsteroidMeshes();

			_astSemiMajAxis [i] = Random.Range (327800f, 476800f);
			_astStartOrbitRot [i] = Mathf.Deg2Rad * Random.Range(0f, 360f);
			_astInclinaison [i] = Mathf.Deg2Rad * Random.Range(-20f, 20f);
			_field[i].transform.position = ComputeAstPos(i);
			_field[i + _fieldSize].transform.position = -ComputeAstPos(i);

			_field[i].CamCheck(Camera.main.transform);
			_field[i + _fieldSize].CamCheck(Camera.main.transform);
		}
	}
	
	// Update is called once per frame
	private void Update () {
		_totalTime += Time.deltaTime * _timeScale;
		UpdateField();
	}

	private Mesh CreateAsteroidMeshes(){
		Mesh mesh = new Mesh();
 
         Vector3[] vertices = new Vector3[]
         {
             new Vector3( 1, 0,  1),
             new Vector3( 1, 0, -1),
             new Vector3(-1, 0,  1),
             new Vector3(-1, 0, -1),
         };
 
         Vector2[] uv = new Vector2[]
         {
             new Vector2(1, 1),
             new Vector2(1, 0),
             new Vector2(0, 1),
             new Vector2(0, 0),
         };
 
         int[] triangles = new int[]
         {
             0, 1, 2,
             2, 1, 3,
         };
 
         mesh.vertices = vertices;
         mesh.uv = uv;
         mesh.triangles = triangles;
		
		return mesh;

	}

	private void UpdateField() {

		Transform cam = Camera.main.transform;

		for (int i = 0; i < _fieldSize; i++) {
			_field[i].transform.position = ComputeAstPos(i);
			_field[i + _fieldSize].transform.position = -ComputeAstPos(i);

			_field[i].CamCheck(cam);
			_field[i + _fieldSize].CamCheck(cam);
		}
		
	}

	private Vector3 ComputeAstPos(int i) {
		float T = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_astSemiMajAxis [i] * _orbitScale, 3) / (17.78f));
		float rot = _astStartOrbitRot[i] + (_totalTime / T); // float rot = s[j] + (totalTime / T[j]);
		float x = _astSemiMajAxis[i] * _orbitScale * Mathf.Cos (_astInclinaison[i]) * Mathf.Cos (rot);  // maybe cos(i) and not sin(i)
		float y = _astSemiMajAxis[i] * _orbitScale * Mathf.Sin (_astInclinaison[i]) * Mathf.Cos (rot);
		float z = _astSemiMajAxis[i] * _orbitScale * Mathf.Sin (rot);

		return new Vector3(x, y, z);
	}

	public void UpdateScale(string variable, float value) {
        switch (variable) {
            case "time":
                _timeScale = value;
                break;

            case "orbit":
                _orbitScale = value;
                break;

            default:
                break;
        }
    }

}
