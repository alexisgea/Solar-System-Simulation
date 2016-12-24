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
        private set { _bodyScale = Mathf.Clamp(value, MinDefaultScale, MaxDefaultScale); }
    }

    private float _orbitScale;

    public float OrbitScale {
        get { return _orbitScale; }
        private set { _orbitScale = Mathf.Clamp(value, MinDefaultScale, MaxDefaultScale); }
    }

    private float _timeScale;

    public float TimeScale {
        get { return _timeScale; }
        private set { _timeScale = Mathf.Clamp(value, MinTimeScale, MaxTimeScale); }
    }

    private double _elapsedTime = 0;
	public double ElapsedTime {
        get { return _elapsedTime; }
        private set { _elapsedTime = value; }
    }

    public enum Scale{Time, Orbit, Body};

	// constant parameters
    private const float BaseBodyScale = 0.01f;
    private const float BaseOrbitScale = 0.0001f;
    private const float BaseTimeScale = 0.5f;
    private const float MinTimeScale = 0.001f;
    private const float MaxTimeScale = 500.0f;
    private const float MinDefaultScale = 0.0001f; // probably replace default by specifics for body and orbits
    private const float MaxDefaultScale = 1.0f;

	// SpaceTime Signals

	private void Start() {
        ControlIntentions.Instance.Scaling += UpdateScale;
        ResetAllScales();
    }

    private void OnDestroy() {
        ControlIntentions.Instance.Scaling -= UpdateScale;
    }
	
	private void Update() {
        ElapsedTime += Time.deltaTime * TimeScale;
    }

	private void UpdateScale(Scale scale, float value) {

        switch(scale){
            
            case Scale.Body:
            BodyScale *= 1f + value;
            break;

            case Scale.Orbit:
            OrbitScale *= 1f + value;
            break;

            case Scale.Time:
            TimeScale *= 1f + value;
            break;

            default:
            Debug.LogWarning("Unknown scale when updating scales.");
            break;

        }
    }

	public void ResetAllScales() {
        BodyScale = BaseBodyScale;
        OrbitScale = BaseOrbitScale;
        TimeScale = BaseTimeScale;
    }


}
