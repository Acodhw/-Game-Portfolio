using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    public string startSceneName;

    private PlayerState state;
    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        state.StateReset();
    }

    private void Update()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //버튼 누르면 시작 씬으로 이동
    public void StartButton() {
        manager.ResetTimer();
        state.StateReset();
        GameObject.Find("GameManager").GetComponent<CodeSetting>().ResetSlots();
        LoadingSceneManager.LoadScene(startSceneName);
        
    }
}
