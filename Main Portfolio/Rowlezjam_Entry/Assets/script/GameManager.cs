using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public float[] pos; // 포탈을 탄 위치
    public int nowElement = 0; // 지금 원소 
    public int Hp = 30;  // HP
    public int Mp = 15;  // MP

    public bool[] havingElement;  // 가진 원소 종류
    public List<int> didEvents;  // 진행한 이벤트 목록
    public string MovedScene;  // 이동한 씬
    public PlayerData(GameManager manager)
    {

        nowElement = manager.nowElement;
        Hp = manager.Hp;
        Mp = manager.Mp;

        havingElement = (bool[])manager.havingElement.Clone();
        didEvents = manager.didEvents.ToList();
        MovedScene = manager.MovedScene;
        pos = new float[2];
        pos[0] = manager.PotalMovePosition.x;
        pos[1] = manager.PotalMovePosition.y;
    }
}

public class GameManager : MonoBehaviour
{
    private PlayerData data;
    private string filePath = string.Empty;

    public bool FileLoaded = false;
    MainCharacter mainchar;

    public int nowElement = 0;
    public int Hp = 30;
    public int Mp = 15;

    public bool[] havingElement;


    public int nowCatched = 0;
    [HideInInspector]
    public bool goleftmoved = false;
    [HideInInspector]
    public Vector3 PotalMovePosition = new Vector3(-0.325f, -0.3f, -1);
    [HideInInspector]
    public List<int> didEvents = new List<int> ();

    [HideInInspector]
    public bool MovingToSavePosition;
    [HideInInspector]
    public string MovedScene = "Tutorial";

    public void InitStates()
    {
        MovedScene = "Tutorial";
        PotalMovePosition = new Vector3(-0.325f, -0.3f, -1);
        MovingToSavePosition = false;
        goleftmoved = false;
        Hp = 30;
        Mp = 15;
        havingElement = new bool[] {true, false, false, false, false };
    }
    public void LoadFile()
    {
        data = SaveSystem.Load(filePath);
        if (data != null)
        {
            FileLoaded = true;
            nowElement = data.nowElement;
            Hp = data.Hp;
            Mp = data.Mp;
            havingElement = (bool[])data.havingElement.Clone();
            didEvents = data.didEvents.ToList();
            MovedScene = data.MovedScene;
            PotalMovePosition = new Vector2(data.pos[0], data.pos[1]);
        }
        else FileLoaded = false;
    }

    public void SaveFile()
    {
        data = new PlayerData(this);
        SaveSystem.Save(data, filePath);
    }
    private void Awake()
    {
        //Screen.SetResolution(64, 64, false);
        filePath = Application.persistentDataPath + "/dat.bin";
        DontDestroyOnLoad(gameObject);
        LoadFile();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("MPAutoHeal", 2.5f, 2.5f);
    }

    public void Damage(int damage) 
    {
        if (Hp - damage < 0)
            Hp = 0;
        else Hp -= damage;
    }

    public void Heal(int heal)
    {
        if (Hp + heal > 30)
            Hp = 30;
        else Hp += heal;
    }

    public void MPHeal(int mheal)
    {
        if (Mp + mheal > 15)
            Mp = 15;
        else Mp += mheal;
    }

    void MPAutoHeal() 
    {
        if (Mp < 15) Mp += 1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingMainChar(MainCharacter mc) {
        mainchar = mc;
    }
}


public static class SaveSystem
{
    public static void Save(PlayerData _data, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);

        formatter.Serialize(stream, _data);
        stream.Close();
    }

    public static PlayerData Load(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + filePath);
            return null;
        }
    }
}