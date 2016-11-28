using UnityEngine;
using System.Collections;

public class QuadField : MonoBehaviour {

	[SerializeField]
	private AstQuad _asteroid;

	[SerializeField]
	private Material _astMat;

	[SerializeField]
	private int _fieldSize;

	[SerializeField]
	private float _astSize;
	
	private AstQuad[] _field;

	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 
	
	// Use this for initialization
	private void Start () {
		
		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		_field = new AstQuad[_fieldSize];

		for (int j = 0; j < _fieldSize; j++) {

			float a = Random.Range (327800f, 476800f);
			float s = Mathf.Deg2Rad * Random.Range(0f, 360f);
			float i = Mathf.Deg2Rad * Random.Range(-20f, 20f);
			Matrix4x4 shape = new Matrix4x4();
			for(int k = 0; k < 4; k++){
				shape.SetColumn(k, new Vector4(Random.Range(-_astSize, _astSize), Random.Range(-_astSize, _astSize), 0, 0));
			}

			_field[j] = Instantiate(_asteroid);
			_field[j].transform.parent = transform;
			_field[j].Initialize(_astMat, shape, a, s, i);

		}

	}
	

	private void Update() {
		_totalTime += Time.deltaTime * _timeScale;
		UpdateField();
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

	private void UpdateField() {

		for (int j = 0; j < _fieldSize; j++) {
			_field[j].UpdatePos(_orbitScale, _totalTime);
		}
		
	}


}
