using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class ManagerData
    {
        public float volum;
        public int difficulty;
        public int[] npcEvnetNum;
        public bool[,] Prograssive;
        public bool[] SceneEventOn;
    }

    private float volum = 1;
    private int difficulty;
    private PlayerState ps;
    private InventorySystem inven;
    private Vector3 movingEvnet;
    private int[] npcEvnetNum = new int[100];
    private bool[] SceneEventOn = new bool[100];
    private bool[,] Prograssive = new bool[100,100];
    private int filedata;
    private int deldata;
    private bool loaded = false;
    private int diffi1 = 0;
    private int diffi2 = 0;
    private int diffi3 = 0;

    // Start is called before the first frame update


    private void OnApplicationQuit()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        ps = gameObject.GetComponent<PlayerState>();
        inven = gameObject.GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (File.Exists(Application.persistentDataPath + "/TrolFile1.dat") || File.Exists(Application.persistentDataPath + "/TrolFile2.dat") || File.Exists(Application.persistentDataPath + "/TrolFile3.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (File.Exists(Application.persistentDataPath + "/TrolFile1.dat"))
            {
                FileStream file1 = File.Open(Application.persistentDataPath + "/TrolFile1.dat", FileMode.Open);
                if (file1 != null && file1.Length > 0)
                {
                    ManagerData sv = (ManagerData)bf.Deserialize(file1);
                    diffi1 = sv.difficulty;
                }
                file1.Close();
            }
            if (File.Exists(Application.persistentDataPath + "/TrolFile2.dat"))
            {
                FileStream file2 = File.Open(Application.persistentDataPath + "/TrolFile2.dat", FileMode.Open);
                if (file2 != null && file2.Length > 0)
                {
                    ManagerData sv = (ManagerData)bf.Deserialize(file2);
                    diffi2 = sv.difficulty;
                }
                file2.Close();
            }
            if (File.Exists(Application.persistentDataPath + "/TrolFile3.dat"))
            {
                FileStream file3 = File.Open(Application.persistentDataPath + "/TrolFile3.dat", FileMode.Open);

                if (file3 != null && file3.Length > 0)
                {
                    ManagerData sv = (ManagerData)bf.Deserialize(file3);
                    diffi3 = sv.difficulty;
                }
                file3.Close();
            }
        }
    }

    public int getDifData(int a) {
        switch(a){
            case 1:
                return diffi1;
                break;
            case 2:
                return diffi2;
                break;
            default:
                return diffi3;
                break;
        }
    }

    public void setVolum(float vol)
    {
        volum = vol;
    }

    public float getVolum()
    {
        return volum;
    }

    public int getDifficulty()
    {
        return difficulty;
    }

    public void setDatanum(int d)
    {
        filedata = d;
    }

    public int getDatanum()
    {
        return filedata;
    }

    public void setDifficulty(int diff)
    {
        difficulty = diff;
        ps.heal(100);
    }

    public void setMovingPoint(Vector3 movement)
    {
        movingEvnet = movement;
    }

    public Vector3 returnMovingPoint()
    {
        return movingEvnet;
    }

    public void setEventCode(int npcNum, int code)
    {
        npcEvnetNum[npcNum] = code;
    }
    public int getEventCode(int npcNum)
    {
        return npcEvnetNum[npcNum];
    }

    public void setSEvent(int num, bool isEvnetFinish)
    {
        SceneEventOn[num] = isEvnetFinish;
    }
    public bool getSEvent(int num)
    {
        return SceneEventOn[num];
    }

    public void setFilenum(int fileN)
    {
        filedata = fileN;
    }

    public void setDelenum(int fileN)
    {
        deldata = fileN;
    }

    public void setPrograss(int EventNum, int Events, bool isCleared)
    {
        Prograssive[EventNum, Events] = isCleared;
    }

    public bool getPrograss(int EventNum, int Events)
    {
        return Prograssive[EventNum, Events];
    }

    public void saveAll() {
        ps.save(filedata);
        inven.SaveItem(filedata);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/TrolFile" + filedata +".dat");
        ManagerData sv = new ManagerData();
        sv.volum = volum;
        sv.difficulty = difficulty;
        sv.npcEvnetNum = npcEvnetNum;
        sv.Prograssive = Prograssive;
        sv.SceneEventOn = SceneEventOn;
        bf.Serialize(file, sv);
        file.Close();
    }

    public void loadAll()
    {
        ps.load(filedata);
        inven.LoadItem(filedata);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/TrolFile" + filedata + ".dat", FileMode.Open);
        if (file != null && file.Length > 0)
        {
            ManagerData sv = (ManagerData)bf.Deserialize(file);
            volum = sv.volum;
            difficulty = sv.difficulty;
            npcEvnetNum = sv.npcEvnetNum;
            Prograssive = sv.Prograssive;
            SceneEventOn = sv.SceneEventOn;
        }
        loaded = true;
        file.Close();
    }

    public void removeFile()
    {
        if (File.Exists(Application.persistentDataPath + "/TrolFile" + deldata + ".dat"))
        {
            File.Delete(Application.persistentDataPath + "/TrolFile" + deldata + ".dat");
            File.Delete(Application.persistentDataPath + "/GuwaGuwa" + deldata + ".dat");
        }
    }

    public bool isLoad() {
        return loaded;
    }

    public void Reset()
    {
        npcEvnetNum = new int[100];
        SceneEventOn = new bool[100];
        Prograssive = new bool[100, 100];
        ps.Reset();
        inven.Reset();
    }
}
