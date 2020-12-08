using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class SaveAndLoadGame : MonoBehaviour
{
    public static SaveAndLoadGame instance;

    private void Awake()
    {
        instance = this;
    }

    SaveObject saveObj = new SaveObject();
    public GameObject player;

    public void Save()
    {
        saveObj.playerPos = player.transform.position;
        
        string jsonPM = JsonUtility.ToJson(player.GetComponent<PlayerManager>(), true);
        string json = JsonUtility.ToJson(saveObj, true);

        File.WriteAllText(Application.dataPath + "/save.txt", json);
        File.WriteAllText(Application.dataPath + "/savePM.txt", jsonPM);

        Debug.Log("SAVED");
    }

    public void Load()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string json = File.ReadAllText(Application.dataPath + "/save.txt");
            string jsonPM = File.ReadAllText(Application.dataPath + "/savePM.txt");

            saveObj = JsonUtility.FromJson<SaveObject>(json);
            player.transform.position = saveObj.playerPos;
            PlayerManager pm = player.GetComponent<PlayerManager>();

            StoreObject obj = new StoreObject();

            obj.inv = pm.inventoryMenu;
            obj.pause = pm.pauseMenu;
            obj.logText = pm.logText;

            JsonUtility.FromJsonOverwrite(jsonPM, pm);

            pm.inventoryMenu = obj.inv;
            pm.pauseMenu = obj.pause;
            pm.logText = obj.logText;

            Debug.Log("LOADED");

            PlayerPrefs.SetInt("STATUS", 1);
        }
        else
            PlayerPrefs.SetInt("STATUS", 0);
    }

    class SaveObject
    {
        public Vector3 playerPos;
    }

    class StoreObject
    {
        public GameObject inv, pause;
        public TextMeshProUGUI logText;
    }
}
