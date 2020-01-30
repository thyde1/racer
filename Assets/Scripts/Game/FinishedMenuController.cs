using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedMenuController : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}