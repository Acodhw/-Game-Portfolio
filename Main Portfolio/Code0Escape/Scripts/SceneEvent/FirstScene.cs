using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 유니티에서는 기본적으로 씬이 이동할 때 오브젝트가 삭제되지 않게 하는 방법이 있다
// 보통은 게임이 진행되어도 삭제되면 안되는 게임메니져가 이를 활용한다
// 하지만 이 오브젝트를 타이틀에 적용시키면 타이틀에 돌아올때마다 게임메니져가 복사되며 에러가 난다
// 그래서 일단 게임 시작할 때 임시 씬을 만들어서 삭제되면 안되는 오브젝트들을 미리 로드한 후 관리한다
public class FirstScene : MonoBehaviour
{
    [SerializeField]
    private string nextScene;
    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

}
