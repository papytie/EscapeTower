using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    void Awake() {
        Game.GameManager = this;
        CreateAllManagers();
    }

    void Start() {
        Game.Player = FindAnyObjectByType<PlayerController>();
    }

    private void CreateAllManagers() {
        Game.GameSettings = SRResources.GameSettings;

        var soundManager = FindObjectOfType<SoundManager>();
        if(soundManager == null) {
            soundManager = new GameObject("SoundManager", typeof(SoundManager)).GetComponent<SoundManager>();
        }
        Game.SoundManager = soundManager;
    }
}
