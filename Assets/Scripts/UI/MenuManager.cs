using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;

    public GameObject playerHUD;

    private void Update()
    {
        if (Player.Instance.pauseAction.WasPressedThisFrame())
        {
            OnPauseButton();
        }
    }

    public void OnPlayButton()
    {
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
        pauseMenuPanel.SetActive(false);
        playerHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
