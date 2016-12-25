using UnityEngine;

namespace solsyssim {

    /// <summary>
    /// Script managing the behaviour of the Orbital axis and it's child orbital body.
    /// Sets the initial body parameter.
    /// Updates the scales of all parameters if necessary.
    /// Updates the orbit rotation and the body's day rotation.
    /// </summary>
    public class OrbitalBody : MonoBehaviour
    {
        [SerializeField] private GameObject _stellarParent;
        [SerializeField] private GameObject _body;
        [SerializeField] private Material _line;

        // TODO: Rename all correctly.
        // Variables related to the orbital axis.
        [SerializeField] private float sideralYear;
        [SerializeField] private float semiMajorAxis = 2f;
        [SerializeField] private float inclination = 0f;
        [SerializeField] [Range(0f, 1f)] private float eccentricity = 0f;
        [SerializeField] private float argAscending = 0f; // longitude
        [SerializeField] private float argPerihelion = 0f; // argument
        [SerializeField] private float startMeanAnomaly = 0f; // J2000
        [SerializeField] private float meanAnomaly;

        // TODO: Rename all correctly.
        // Variables related to the orbital body.
        [SerializeField] private float dayLength = 1f;
        [SerializeField] private float startDayRotation = 0f;
        [SerializeField] private float size = 1f;
        [SerializeField] private float rightAscension = 0f;
        [SerializeField] private float declination = 0f;
        private const float _eclipticTilt = 23.44f;
        private float _angularVelocity;

        // // Scales from the StellarSystem script.
        public float SizeScaled { private set; get; }
        public float OrbitScaled { private set; get; }

        // TODO: Rename all correctly.
        // Visual path related variables.
        public Color uiVisual;
        private bool _showPath = false;

        // registering to some events and initialising stuff
        private void Start()
        {
            SpaceTime.Instance.ScaleUpdated += UpdateScale;
            FindObjectOfType<InterfaceManager>().OrbitToggle += TogglePath;
            FindObjectOfType<InterfaceManager>().FullStart += TogglePath;

            SetScales();
            SetJ2000();
            AdvanceOrbit();
        }

        private void OnDestroy()
        {
            SpaceTime.Instance.ScaleUpdated -= UpdateScale;
        }

        // advance the body on it's orbit and it's rotation on itself separately
        private void Update()
        {
            AdvanceOrbit();
            AdvanceDayRotation();
        }

        // renders or not the orbit path
        private void OnRenderObject()
        {
            if (_showPath)
                DrawPath();
        }

        /// <summary>
        /// Initialises the orbital body state as per the J2000 data.
        /// </summary>
        private void SetJ2000() {
            transform.Rotate(new Vector3(0, 0, (90 - declination)));
            transform.Rotate(new Vector3(0, -rightAscension, 0));
            transform.Rotate(new Vector3(_eclipticTilt, 0, 0));
            transform.Rotate(new Vector3(0, startDayRotation, 0), Space.Self);
            _angularVelocity = Mathf.Deg2Rad * (360 / sideralYear); // Gow much degree is covered each day.
            meanAnomaly = startMeanAnomaly * Mathf.Deg2Rad;
        }


        /// <summary>
        /// Initialises the scales from the StellarSystem.
        /// </summary>
        private void SetScales() {
            OrbitScaled = semiMajorAxis * SpaceTime.Instance.OrbitScale;
            SizeScaled = size * SpaceTime.Instance.BodyScale;
            _body.transform.localScale = Vector3.one * SizeScaled;
        }

        /// <summary>
        /// Toggles the orbit path. Called on events.
        /// </summary>
        public void TogglePath() {
            _showPath = !_showPath;
        }

        /// <summary>
        /// Updates the scale. Called on event from the StellarSystem script.
        /// </summary>
        /// <param name="variable">Which scale.</param>
        public void UpdateScale(SpaceTime.Scale scale, float value) {
            switch (scale) {

                case SpaceTime.Scale.Body:
                    SizeScaled = size * value;
                    _body.transform.localScale = Vector3.one * SizeScaled;
                    break;

                case SpaceTime.Scale.Orbit:
                    OrbitScaled = semiMajorAxis * value;
                    break;
                
                case SpaceTime.Scale.Time:
                    break; 

                default:
                    Debug.LogWarning("Wrong variable name in Body.updateScales() .");
                    break;
            }
        }

        /// <summary>        
        /// Calculates how much the body has moved in it's orbit and rotate it's axis accordingly.
        /// </summary>        
        private void AdvanceOrbit() {
            meanAnomaly += _angularVelocity * SpaceTime.Instance.DeltatTime;
            if (meanAnomaly > 2f * Mathf.PI)  // we keep mean anomaly within 2*Pi
                meanAnomaly -= 2f * Mathf.PI;

            Vector3 orbitPos = GetPosition(meanAnomaly);
            Vector3 parentPos = _stellarParent.transform.position; // position of the parent to offset the calculated pos (used for moons)
            transform.position = new Vector3(orbitPos.x + parentPos.x, orbitPos.y + parentPos.y, orbitPos.z + parentPos.z);
        }

        /// <summary>        
        /// Rotates the body on itself as per it's day length.
        /// </summary>        
        private void AdvanceDayRotation() {
            float rot = SpaceTime.Instance.DeltatTime * (360 / dayLength);
            _body.transform.Rotate(new Vector3(0, -rot, 0)); // counter clockwise is the standard rotation considered
        }

        /// <summary>        
        /// Draws the orbit by calculating each next point in a circle.
        /// Then using the GL.Lines to draw a line between each point.
        /// </summary>        
        private void DrawPath() {
            const float PathDetail = 0.03f;  // TODO improve with size of orbit and distance/view to cam

            GL.PushMatrix();
            _line.color = uiVisual;
            _line.SetPass(0);
            GL.Begin(GL.LINES);

            Vector3 parentPos = _stellarParent.transform.position;

            // we just loop through a full circle by the path details and make a line for each step
            for (float theta = 0.0f; theta < (2f * Mathf.PI); theta += PathDetail) {
                float anomalyA = theta;
                Vector3 orbitPointA = GetPosition(anomalyA);
                GL.Vertex3(orbitPointA.x + parentPos.x, orbitPointA.y + parentPos.y, orbitPointA.z + parentPos.z);

                float anomalyB = theta + PathDetail;
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
        private float EccentricAnomaly(float M, int dp = 5) {
            // Mathematical Model is as follow:
            // E(n+1) = E(n) - f(E) / f'(E)
            // f(E) = E - e * sin(E) - M
            // f'(E) = 1 - e * cos(E)
            // we are happy when f(E)/f'(E) is small enough.

            int maxIter = 20;  // we make sure we won't loop too much
            int i = 0;
            float e = eccentricity;
            float precision = Mathf.Pow(10, -dp);
            float E, F;

            // If the eccentricity is high we guess the Mean anomaly for E, otherwise we guess PI.
            E = (e < 0.8) ? M : Mathf.PI;
            F = E - e * Mathf.Sin(M) - M;  //f(E)

            // We will interate until f(E) higher than our wanted precision (as devided then by f'(E)).
            while ((Mathf.Abs(F) > precision) && (i < maxIter)) {
                E = E - F / (1f - e * Mathf.Cos(E));
                F = E - e * Mathf.Sin(E) - M;
                i++;
            }

            return E;
        }

        /// <summary>
        /// Computes the True Anomaly. Angles to be passed in Radians.
        /// </summary>
        /// <returns>The True Anomaly.</returns>
        /// <param name="E">Eccentric Anomaly.</param>
        private float TrueAnomaly(float E) {
            // from wikipedia we can find several way to solve TA from E.
            // I tried sin(TA) = (sqrt(1-e*e) * sin(E))/(1 -e*cos(E)) but it didn't work properly for some reason,
            // so I sued the following as one of the my sources(jgiesen.de/Kepler) tan(TA) = (sqrt(1-e*e) * sin(E)) / (cos(E) - e).

            float e = eccentricity;
            float numerator = Mathf.Sqrt(1f - e * e) * Mathf.Sin(E);
            float denominator = Mathf.Cos(E) - e;
            float TA = Mathf.Atan2(numerator, denominator);

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
            float X = focusRadius * (Mathf.Cos(N) * Mathf.Cos(TA + w) - Mathf.Sin(N) * Mathf.Sin(TA + w)) * Mathf.Cos(i);
            float Y = focusRadius * Mathf.Sin(TA + w) * Mathf.Sin(i);
            float Z = focusRadius * (Mathf.Sin(N) * Mathf.Cos(TA + w) + Mathf.Cos(N) * Mathf.Sin(TA + w)) * Mathf.Cos(i);

            Vector3 orbitPoint = new Vector3(X, Y, Z);

            return orbitPoint;
        }
    
    }
}