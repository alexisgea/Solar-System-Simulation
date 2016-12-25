using UnityEngine;
using System;

public class SpaceTime : MonoBehaviour {

	// Singleton instatiation
    private static SpaceTime _instance;
	public static SpaceTime Instance {
		get {
			if (_instance == null) {
                _instance = new SpaceTime();
            }
            return _instance;
        }
	}

	private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

	// stellar system scale public variables
    private float _bodyScale = BaseBodyScale;

    public float BodyScale {
        get { 
            return _bodyScale;
        }
        private set { 
            _bodyScale = Mathf.Clamp(value, MinDefaultScale, MaxDefaultScale);
            if(ScaleUpdated != null)
                ScaleUpdated.Invoke(Scale.Body, _bodyScale);
        }
    }

    private float _orbitScale = BaseOrbitScale;

    public float OrbitScale {
        get { return _orbitScale; }
        private set { 
            _orbitScale = Mathf.Clamp(value, MinDefaultScale, MaxDefaultScale);
            if(ScaleUpdated != null)
                ScaleUpdated.Invoke(Scale.Orbit, _orbitScale);
        }
    }

    private bool _timePause = false;
    private float _lastTimeScale = BaseTimeScale;
    private float _timeScale = BaseTimeScale;

    public float TimeScale {
        get { return _timeScale; }
        private set { 
            if(!_timePause){
                _timeScale = Mathf.Clamp(value, MinTimeScale, MaxTimeScale);
                if(ScaleUpdated != null)
                    ScaleUpdated.Invoke(Scale.Time, _timeScale);
            }
        }
    }

    private double _elapsedTime = 0;
	public double ElapsedTime {
        get { return _elapsedTime; }
        private set { _elapsedTime = value; }
    }

    public float DeltatTime {
        get { return Time.deltaTime * TimeScale; }
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
    public event Action<Scale, float> ScaleUpdated;


	private void Start() {
        ControlIntentions.Instance.Scaling += UpdateScale;
        ControlIntentions.Instance.PauseGame += PauseTime;        
    }

    private void OnDestroy() {
        ControlIntentions.Instance.Scaling -= UpdateScale;
        ControlIntentions.Instance.PauseGame -= PauseTime;                
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

    public void PauseTime(bool pause){
        if(pause){
            _lastTimeScale = TimeScale;
            TimeScale = MinTimeScale;
            _timePause = pause;
        } else {
            _timePause = pause;
            TimeScale = _lastTimeScale;
        }

    }

	public void ResetAllScales() {
        BodyScale = BaseBodyScale;
        OrbitScale = BaseOrbitScale;
        TimeScale = BaseTimeScale;
    }


}
