using UnityEngine;

public class CutSceneObj : MonoBehaviour
{
    private GameManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void StartCutscene()
    {
        manager.SetOnCutScene(true);
    }

    public void EndCutscene()
    {
        manager.SetOnCutScene(false);
    }
}
