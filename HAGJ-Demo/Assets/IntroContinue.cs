using UnityEngine;

public class IntroContinue : MonoBehaviour {
    public void ContinueToLevelOne() {
        Loader.Load(Loader.Scene.Level1);
    }
}