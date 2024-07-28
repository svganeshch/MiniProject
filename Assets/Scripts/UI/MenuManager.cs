using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject deathPanel;

    public GameObject playerHUD;

    public AudioClip buttonAudioClip;

    AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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

    public void OnRetryButton()
    {
        audioSource.PlayOneShot(buttonAudioClip);

        SceneManager.LoadScene(0);
    }

    public void SetDeathMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        deathPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        audioSource.PlayOneShot(buttonAudioClip);

        Application.Quit();
    }
}
