using System;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public void ActivateUI(GameObject gameObjectToActivate)
    {
        gameObjectToActivate.SetActive(true);
    }

    public void DeactivateUI(GameObject gameObjectToDeactivate)
    {
        gameObjectToDeactivate.SetActive(false);
    }

    public void GoToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
