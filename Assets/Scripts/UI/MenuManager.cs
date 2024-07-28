using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;

    public GameObject playerHUD;

    public AudioClip buttonAudioClip;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Player.Instance.pauseAction.WasPressedThisFrame())
        {
            OnPauseButton();
        }
    }

    public void OnPlayButton()
    {
        audioSource.PlayOneShot(buttonAudioClip);

        mainMenuPanel.SetActive(false);
        playerHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnPauseButton()
    {
        Cursor.lockState = CursorLockMode.None;
        playerHUD.SetActive(!playerHUD.activeSelf);

        pauseMenuPanel.SetActive(!pauseMenuPanel.activeSelf);

        Debug.Log("pause menu triggered");
    }

    public void OnResumeButton()
    {
        audioSource.PlayOneShot(buttonAudioClip);

        pauseMenuPanel.SetActive(false);
        playerHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnQuitButton()
    {
        audioSource.PlayOneShot(buttonAudioClip);

        Application.Quit();
    }
}
