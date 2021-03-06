﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setup : MonoBehaviour
{
    public Dropdown PlayersDropdown;
    public Dropdown LapsDropdown;
    public Dropdown TrackDropdown;

    private int players;
    private int laps;

    public void StartGame()
    {
        this.players = int.Parse(this.PlayersDropdown.options[this.PlayersDropdown.value].text);
        this.laps = int.Parse(this.LapsDropdown.options[this.LapsDropdown.value].text);
        var track = this.TrackDropdown.options[this.TrackDropdown.value].text;
        SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
        SceneManager.LoadSceneAsync(track, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.UnloadSceneAsync(this.gameObject.scene);
        var raceController = FindObjectOfType<RaceController>();
        raceController.Players = this.players;
        raceController.Laps = this.laps;
        raceController.enabled = true;
        SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
    }
}