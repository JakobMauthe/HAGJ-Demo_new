using UnityEngine;

public class IntroContinue : MonoBehaviour {
    public void ContinueToLevelOne() {
        Loader.Load(Loader.Scene.Level1);
    }
    public void ContinueToLevelTwo() {
        Loader.Load(Loader.Scene.Level2);
    }
    public void ContinueToMainMenu() {
        Loader.Load(Loader.Scene.MainMenu);
    }
}