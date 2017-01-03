using UnityEngine;
using System;

namespace solsyssim {

    //TODO investigate the idea to have a GameManager Singleton instance
    // which will reference SpaceTime, ControlIntentions and other "singleton"
    // thus there will be only one singleton

    /// <summary>
    /// handles all scales and dimention of space and time for the current gameinstance
    /// </summary>
    public class SpaceTime : MonoBehaviour {

        // Singleton instatiation
        private static SpaceTime _instance;
        public static SpaceTime Instance {
            get {
                // if (_instance == null) {
                //     if (_instance == null){
                //         Debug.LogError("Singleton not yet instanciated.");
                //         _instance = new SpaceTime();
                //     }
                // }
                return _instance;
            }
        }

        private void Awake() {
            if (_instance != null && _instance != this) {
                Debug.LogError("Double instance of ControlIntentions Singleton!");
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }

        // enum of all the different accessible scales
        public enum Scale{Time, Orbit, Body};

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

        // time is a particular scale as it is used to pause the game 
        // it also has a totaltime counter and the last scaled deltatime
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

        // registering to events
        private void Start() {
            ControlIntentions.Instance.Scaling += UpdateScale;
            ControlIntentions.Instance.PauseGame += PauseTime;        
        }
    
        private void OnDestroy() {
            ControlIntentions.Instance.Scaling -= UpdateScale;
            ControlIntentions.Instance.PauseGame -= PauseTime;                
        }
        
        // updating the elapsed time each frame depending on frame and scale
        private void Update() {
            ElapsedTime += Time.deltaTime * TimeScale;
        }

        // listening to events from inputs
        // modify the correct scale accordingly
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

        /// <summary>
        /// Sets timescale to the minimum when pause is pressed.
        /// </summary>
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

        /// <summary>
        /// Resets the different scales to their default constant values.
        /// </summary>
        public void ResetAllScales() {
            BodyScale = BaseBodyScale;
            OrbitScale = BaseOrbitScale;
            TimeScale = BaseTimeScale;
        }

    }
}