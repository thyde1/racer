using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setup : MonoBehaviour
{
    public Dropdown PlayersDropdown;

    public Dropdown LapsDropdown;

    private void Start()
    {
        SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
    }

    public void StartGame()
    {
        var players = int.Parse(this.PlayersDropdown.options[this.PlayersDropdown.value].text);
        var laps = int.Parse(this.LapsDropdown.options[this.LapsDropdown.value].text);
        var track = "Debug";
        SceneManager.LoadSceneAsync(track, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.UnloadSceneAsync(this.gameObject.scene);
        var raceController = FindObjectOfType<RaceController>();
        raceController.Setup = this;
        raceController.enabled = true;
        FindObjectOfType<HudController>().enabled = true;
        SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
    }
}