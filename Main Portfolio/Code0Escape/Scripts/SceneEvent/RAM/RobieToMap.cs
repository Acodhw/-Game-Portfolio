using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobieToMap : MonoBehaviour
{
    public string SceneName;


    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            LoadingSceneManager.LoadScene(SceneName);
        }
    }
}
