using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    LevelLoader levelLoader;

    public void PlayGame() {
        //levelLoader.LoadSpecificLevel(2);
        Debug.Log("No level 1 yet!");
    }
    public void TestLevel() {
        levelLoader.LoadSpecificLevel(1);
    }

    public void Quit() {
        Application.Quit();
    }
}