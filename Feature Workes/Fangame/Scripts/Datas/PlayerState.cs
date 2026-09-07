using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public class FilesData
{
    public int level1 = 0;
    public int hp1 = 0;
    public int dif1 = 0;
    public string SceneName1 = "";
    public int level2 = 0;
    public int hp2 = 0;
    public int dif2 = 0;
    public string SceneName2 = "";
    public int level3 = 0;
    public int hp3 = 0;
    public int dif3 = 0;
    public string SceneName3 = "";
}
public class PlayerState : MonoBehaviour
{
    [System.Serializable]
    public class StateData
    {
        public int level;
        public int hp;
        public int exp;
        public int money;
        public int skill1;
        public int skill2;
        public bool getShots;
        public float savedPointX;
        public float savedPointY;
        public float savedPointZ;
        public string SceneName;
        public bool[] canSelectSkills;
    }

    public string savedscene;
    public bool[] canSelectSkills = new bool[20];
    private FilesData fd = new FilesData();
    public bool getShots;
    private int skill1;
    private int skill2;
    public int money = 0;
    private int maxhp = 100;
    public int hp = 100;
    private int maxMana = 3;
    private int mana = 3;
    public int level = 1;
    private readonly int[] MaxExp = { 0, 45, 75, 130, 210, 380, 500, 780, 1100, 1450, 1750, 1950, 2200, 3500, 4900, 6500, 7800, 9800, 12050, 14860, 18480, 23580, 28580, 34880, 38950, 48850, 57875, 67852, 78850, 80000, 128320, 167895, 200852, 238580, 298895, 345750, 397850, 435575, 498850, 587850, -1 };
    private int exp;
    public Vector3 savedPos;
    public PlayerControl pc;
    public GameManager gmm;


    // Start is called before the first frame update
    void Start()
    {
        gmm = gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        maxhp = ((level - 1) * 10) + 100 - (gmm.getDifficulty() * 30 + ((level - 1) * (gmm.getDifficulty() + 5)));
        switch (level)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                maxMana = 3;
                break;
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                maxMana = 4;
                break;
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
                maxMana = 5;
                break;
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
                maxMana = 6;
                break;            
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
                maxMana = 7;
                break;            
            case 30:
            case 31:
            case 32:
            case 33:
                maxMana = 8;
                break;
            case 34:
            case 35:
            case 36:
            case 37:
                maxMana = 9;
                break;
            default:
                maxMana = 10;
                break;
        }

        if (MaxExp[level] != -1 && exp >= MaxExp[level])
        {
            levelup();
        }

        if (maxMana < mana)
            mana = maxMana;
        if (hp > maxhp)
            hp = maxhp;


        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/GuwaGuwa1.dat"))
        {
            FileStream file1 = File.Open(Application.persistentDataPath + "/GuwaGuwa1.dat", FileMode.Open);
            if (file1 != null && file1.Length > 0)
            {
                StateData sv = (StateData)bf.Deserialize(file1);
                fd.level1 = sv.level;
                fd.hp1 = sv.hp;
                fd.SceneName1 = SceneNameChanger(sv.SceneName);
            }
            file1.Close();
        }
        else
        {
            fd.level1 = 0;
            fd.hp1 = 0;
            fd.SceneName1 = "";
        }
        if (File.Exists(Application.persistentDataPath + "/GuwaGuwa2.dat"))
        {
            FileStream file2 = File.Open(Application.persistentDataPath + "/GuwaGuwa2.dat", FileMode.Open);
            if (file2 != null && file2.Length > 0)
            {
                StateData sv = (StateData)bf.Deserialize(file2);
                fd.level1 = sv.level;
                fd.hp1 = sv.hp;
                fd.SceneName1 = SceneNameChanger(sv.SceneName);
            }
            file2.Close();
        }
        else
        {
            fd.level2 = 0;
            fd.hp2 = 0;
            fd.SceneName2 = "";
        }
        if (File.Exists(Application.persistentDataPath + "/GuwaGuwa3.dat"))
        {
            FileStream file3 = File.Open(Application.persistentDataPath + "/GuwaGuwa3.dat", FileMode.Open);


            if (file3 != null && file3.Length > 0)
            {
                StateData sv = (StateData)bf.Deserialize(file3);
                fd.level1 = sv.level;
                fd.hp1 = sv.hp;
                fd.SceneName1 = SceneNameChanger(sv.SceneName);
            }
            file3.Close();
        }
        else
        {
            fd.level2 = 0;
            fd.hp2 = 0;
            fd.SceneName2 = "";
        }
    }

    private string SceneNameChanger(string scene) 
    {
        switch (scene)
        {
            //chapter1
            case "ToCastle":
                return "시련의 길";
                break;

            case "Castle_BeforeTest_Door":            
                return "성문 앞";
                break;

            case "Castle_BeforeTest":
            case "Castle_AfterTest":
            case "Castle_DeadBrave":
                return "셀러토 제국의 성";
                break;

            case "Testing_Roby":
                return "사과의 시련";
                break;

            case "Pakuru_tower":
            case "Control_tower":
            case "Fighting_tower":
            case "Maze_Tower":
                return "시련의 탑";
                break;

            //chapter2

            case "Field1":
            case "Field2":
            case "Field3":
            case "Field4":
                return "셀러토 초원";
                break;

            case "Desert1":
            case "Desert2":
            case "Desert3":
            case "Desert4":
                return "데절트 사막";
                break;

            case "Desert_Village":
                return "사마크 마을";
                break;

            case "Ocean1":
            case "Ocean2":
            case "Ocean3":
            case "Ocean4":
                return "박구의 바다";
                break;

            case "Ocean_Village":
                return "박구마을";
                break;

            case "Forest1":
            case "Forest2":
            case "Forest3":
            case "Forest4":
                return "성초의 숲";
                break;

            case "Forest_Village":
                return "성초의 마을";
                break;

            case "Hapta_Tower":
                return "독각 타워";
                break;
            case "cyu_Tower":
                return "정형준 타워";
                break;
            case "Sleep_Tower":
                return "뜰타워";
                break;

            case "Pantom_Stage1":
            case "Pantom_Stage2":
                return "봉인의 정원";
                break;

            //chapter3

            case "Pantom_Fight":
                return "팬텀 토벌";
                break;

            //chapter4

            case "Tample_Field1":
            case "Tample_Field2":
            case "Tample_Field3":
                return "신전으로";
                break;

            case "Sak_Village_HeroSak":
            case "Sak_Village_DeadBrave":
                return "에포르 마을";
                break;

            case "Apple_Tample_Events":
                return "마인애플 신전";
                break;
            case "Lettuce_Tample_Events":
                return "카운터 신전";
                break;
            case "Broccoli_Tample_Events":
                return "꾸몽 신전";
                break;
            case "Egg_Tample_Events":
                return "아이리스 신전";
                break;
            case "Water_Tample_Events":
                return "파크모 신전";
                break;
            case "Sweetpotato_Tample_Events":
                return "유성 신전";
                break;           
            case "Forever_Tower":
                return "영원의 탑";
                break;
            case "Before_Final":
                return "최종장";
                break;

            default :
                return "진행 없음";
                break;
        }
    }
    public void SetCan_Shot(bool c)
    {
        getShots = c;
    }

    public bool GetCan_Shot()
    {
        return getShots;
    }
    public void SetMoney(int cash)
    {
        money = cash;
    }

    public int GetMoney()
    {
        return money;
    }

    public void heal(int healpoint)
    {
        if (hp + healpoint > maxhp)
            hp = maxhp;
        else
            hp += healpoint;
    }

    public void damage(int damage)
    {
        if (hp - damage < 0)
            hp = maxhp;
        else
            hp -= damage;
    }
    public int getHP()
    {
        return hp;
    }

    public void setMana(int m)
    {
        mana = m;
    }

    public int getMana()
    {
        return mana;
    }

    public int getMaxMana()
    {
        return maxMana;
    }

    public int getMaxHP()
    {
        return maxhp;
    }

    public void ExpUp(int exp_gage)
    {
        exp += exp_gage;
    }

    public int getEXP()
    {
        return exp;
    }

    public int getMaxEXP(int level)
    {
        return MaxExp[level];
    }

    public int getLevel()
    {
        return level;
    }

    private void levelup()
    {        
        int i = maxhp;
        exp -= MaxExp[level];
        level += 1;
        maxhp = ((level - 1) * 10) + 100 - (gmm.getDifficulty() * 30 + ((level - 1) * (gmm.getDifficulty() + 5)));
        hp += maxhp - i;
    }

    public void setskills(int s1, int s2)
    {
        skill1 = s1;
        skill2 = s2;
    }

    public int getskill1()
    {
        return skill1;
    }

    public int getskill2()
    {
        return skill2;
    }

    public void save(int fileNum)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GuwaGuwa" + fileNum + ".dat");
        StateData sv = new StateData();
        sv.level = level;
        sv.hp = hp;
        sv.exp = exp;
        sv.money = money;
        sv.skill1 = skill1;
        sv.skill2 = skill2;
        sv.getShots = getShots;
        sv.canSelectSkills = canSelectSkills;
        sv.savedPointX = pc.transform.position.x;
        sv.savedPointY = pc.transform.position.y;
        sv.savedPointZ = pc.transform.position.z;
        sv.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bf.Serialize(file, sv);
        file.Close();
    }

    public void load(int fileNum)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/GuwaGuwa" + fileNum + ".dat", FileMode.Open);
        if (file != null && file.Length > 0)
        {
            StateData sv = (StateData)bf.Deserialize(file);
            level = sv.level;
            hp = sv.hp;
            exp = sv.exp;
            money = sv.money;
            skill1 = sv.skill1;
            skill2 = sv.skill2;
            getShots = sv.getShots;
            savedscene = sv.SceneName;
            canSelectSkills = sv.canSelectSkills;
            savedPos = new Vector3(sv.savedPointX, sv.savedPointY, sv.savedPointZ);
        }
        file.Close();
    }

    public FilesData fData()
    {
        return fd;
    }

    public void setCanSelectSkills(int num, bool cansel)
    {
        canSelectSkills[num] = cansel;
    }

    public bool getCanSelectSkills(int num)
    {
        return canSelectSkills[num];
    }

    public void Reset()
    {
        getShots = false;
        skill1 = 0;
        skill2 = 0;
        money = 0;
        maxhp = 100;
        hp = 100;
        maxMana = 3;
        mana = 3;
        level = 1;
        exp = 0;
        canSelectSkills = new bool[20];
    }
}
