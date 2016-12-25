using UnityEngine;

namespace solsyssim {

    /// <summary>
    /// This class instantiate a BodyLabel as chidl for each orbital body in the StellarSystem.
    /// </summary>
    public class BodiesOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _stellarSystem;
        [SerializeField] private GameObject _label;

        private void Start()
        {
            InstantiateLabels();
        }

        /// <summary>
        /// Instantiates a BodyLabel as child for each orbital body in the StellarSystem.
        /// </summary>
        private void InstantiateLabels()
        {
            int i = 0;
            foreach (Transform body in _stellarSystem.transform)
            {
                if (body.GetComponent<OrbitalBody>() != null)
                {
                    GameObject newLabel = Instantiate(_label);
                    newLabel.GetComponent<BodyLabel>().Owner = body.gameObject;
                    newLabel.transform.SetParent(transform);
                }

                i++;
            }
        }

    }
}