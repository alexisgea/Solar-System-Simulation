using System;
using UnityEngine;

/// <summary>
/// Class to manage difference UI element and send signal.
/// This was for giving a single point of event subscription to all other game elements.
/// </summary>
public class InterfaceManager : MonoBehaviour
{
    public event Action FullStart;

    // These two will be used with potential UI inprovement.
    public event Action IconToggle;

    public event Action OrbitToggle;

    private bool _launched = false;

    /// <summary>
    /// Called through the info display animator to notify other game component the game has started.
    /// </summary>
    public void ExitLauncSequence()
    {
        if (!_launched)
        {
            FullStart();
            _launched = true;
        }
    }
}