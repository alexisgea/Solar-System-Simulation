using UnityEngine;

/// <summary>
/// UI Class for the infos popup menu.
/// </summary>
public class InfosDisplay : MonoBehaviour
{
    [SerializeField] private AudioClip _open;
    [SerializeField] private AudioClip _close;
    private Animator _infosDisplayAnimator;

    private void Start()
    {
        _infosDisplayAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CheckControl();
    }

    /// <summary>
    /// Check if the player is openeing of closeing the menu.
    /// Plays the associated sound.
    /// </summary>
    public void CheckControl()
    {
        _infosDisplayAnimator.SetBool("opened", !_infosDisplayAnimator.GetBool("opened"));

        if (_infosDisplayAnimator.GetBool("opened"))
            GetComponent<AudioSource>().PlayOneShot(_open);
        else
            GetComponent<AudioSource>().PlayOneShot(_close);
    }

    /// <summary>
    /// Quits the app.
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }
}