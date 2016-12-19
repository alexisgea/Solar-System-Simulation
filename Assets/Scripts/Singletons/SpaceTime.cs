using UnityEngine;

public class SpaceTime : MonoBehaviour {

	// Singleton instatiation
    private static SpaceTime _instance;
	public static SpaceTime Instance {
		get {
			if (_instance == null){
                _instance = new SpaceTime();
            }
            return _instance;
        }
	}

	private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

	// stellar system scale public variables
    private float _bodyScale;

    public float BodyScale {
        get { return _bodyScale; }
        private set { _bodyScale = value; }
    }

    private float _orbitScale;

    public float OrbitScale {
        get { return _orbitScale; }
        private set { _orbitScale = value; }
    }

    private float _timeScale;

    public float TimeScale {
        get { return _timeScale; }
        private set { _timeScale = value; }
    }

    private double _elapsedTime = 0;
	public double ElapsedTime {
        get { return _elapsedTime; }
        private set { _elapsedTime = value; }
    }

	// constant parameters
    private const float BaseBodyScale = 0.01f;
    private const float BaseOrbitScale = 0.0001f;
    private const float BaseTimeScale = 0.5f;
    private const float MinTimeScale = 0.001f;
    private const float MaxTimeScale = 500.0f;
    private const float MinDefaultScale = 0.0001f;
    private const float MaxDefaultScale = 1.0f;

	// SpaceTime Signals

	private void Start() {
        ResetAllScales();
    }
	
	private void Update() {
        ElapsedTime += Time.deltaTime * TimeScale;
    }

	private float UpdateScale(string which, float scale, float min = MinDefaultScale, float max = MaxDefaultScale) {
        scale *= (1 + Input.GetAxis("Mouse ScrollWheel"));
        scale = Mathf.Clamp(scale, min, max);

        return scale;
    }

	private void ResetAllScales() {
        BodyScale = BaseBodyScale;
        OrbitScale = BaseOrbitScale;
        TimeScale = BaseTimeScale;
    }


}
