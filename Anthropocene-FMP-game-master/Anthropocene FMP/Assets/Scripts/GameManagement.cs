using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManagement : MonoBehaviour
{
    #region static inst
    public static GameManagement instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Multiple instances!");
            return;
        }

        instance = this;
    }
    #endregion

    List<SpriteRenderer> foregroundObjects = new List<SpriteRenderer>();
    public float timeBetweenDayNight = 5f; //minutes
    public float timeBetweenWeather = 5f; //minutes
    public Weather currentWeather;
    public ParticleSystem rain, storm, wind, blizzard;
    ParticleSystem currentParticleActive;
    PlayerManager playerManager;
    float timer = 0;
    public float timerMultiplier = 1;
    public float rainDamage = 1f;
    public float stormDamage = 2f;
    public float windDamage = 1f;
    public float blizzardDamage = 1f;

    public static bool hasDied;

    public GameObject[] enemies;

    [SerializeField]
    GameObject[] objectsToHideOnDeath;
    [SerializeField]
    GameObject deathUI;

    public AudioClip[] soundtrack;
    private AudioSource src;

    public bool day = true;

    private void OnApplicationQuit()
    {
        SaveAndLoadGame.instance.Save();
    }

    void Start()
    {
        hasDied = false;

        foreach(SpriteRenderer i in FindObjectsOfType<SpriteRenderer>())
        {
            if (i.sortingLayerName == "Foreground")
            {
                foregroundObjects.Add(i);
                i.sortingOrder = (int)(i.gameObject.transform.position.y*-100);
            }
        }

        playerManager = FindObjectOfType<PlayerManager>();

        //music setup
        src = GetComponent<AudioSource>();
        if (!src.playOnAwake)
        {
            src.clip = soundtrack[Random.Range(0, soundtrack.Length)];
            src.Play();
        }

        if(PlayerPrefs.GetInt("STATUS", 0) == 0)
        {
            StartCoroutine(SetStatus(1));
        }

        if (File.Exists(Application.dataPath + "/save.txt"))
            SaveAndLoadGame.instance.Load();

        InvokeRepeating("TimeOfDay", timeBetweenDayNight * 60 / timerMultiplier, timeBetweenDayNight * 60 / timerMultiplier);
        InvokeRepeating("SpawnEnemy", 60, 60);
    }

    IEnumerator SetStatus(int value)
    {
        yield return new WaitForSeconds(10);
        PlayerPrefs.SetInt("STATUS", value);

        SaveAndLoadGame.instance.Save();
    }

    private void Update()
    {      
        if (timer >= timeBetweenWeather * 60)
        {
            if(currentParticleActive != null)
                currentParticleActive.Stop();

            int chance = Random.Range(0, 100);
            
            if(chance <= 60)
            {
                currentWeather = Weather.clear;
                currentParticleActive = null;
            }
            else if(chance < 70)
            {
                currentWeather = Weather.raining;
                currentParticleActive = rain;
            }
            else if(chance < 75)
            {
                currentWeather = Weather.storm;
                currentParticleActive = storm;
            }
            else if(chance < 95)
            {
                currentWeather = Weather.windy;
                currentParticleActive = wind;
            }
            else
            {
                currentWeather = Weather.blizzard;
                currentParticleActive = blizzard;
            }
            if (currentParticleActive != null)
                currentParticleActive.Play();

            timer = 0;
        }
        else
        {
            timer += Time.deltaTime * timerMultiplier;
        }

        if (!playerManager.isCovered)
        {
            switch (currentWeather)
            {
                case Weather.raining:
                    playerManager.currentPlayerHealth -= rainDamage * Time.deltaTime;
                    break;
                case Weather.storm:
                    playerManager.currentPlayerHealth -= stormDamage * Time.deltaTime;
                    break;
                case Weather.windy:
                    playerManager.oxygen -= windDamage * Time.deltaTime;
                    break;
                case Weather.blizzard:
                    playerManager.food -= blizzardDamage * Time.deltaTime;
                    break;
                case Weather.clear:
                    break;
            }
        }

        if (!src.isPlaying)
        {
            src.clip = soundtrack[Random.Range(0, soundtrack.Length)];
            src.Play();
        }
    }

    public void Die()
    {
        if(!hasDied)
        {
            hasDied = true;
            Time.timeScale = 0f;

            foreach(GameObject objectToHide in objectsToHideOnDeath)
            {
                objectToHide.SetActive(false);
            }

            deathUI.SetActive(true);

            File.Delete(Application.dataPath + "/save.txt");
            File.Delete(Application.dataPath + "/save.meta");
            File.Delete(Application.dataPath + "/savePM.txt");
            File.Delete(Application.dataPath + "/savePM.meta");
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void TimeOfDay()
    {
        day = !day;
        if(day)
            playerManager.AddLog("It has just become day");
        else
            playerManager.AddLog("It has just become night");
    }

    public void SpawnEnemy()
    {
        if(!day)
        {
            int enemyToSpawn = Random.Range(0, enemies.Length);
            int randomX = Random.Range(-10, 11);
            int randomY = Random.Range(-10, 11);
            Instantiate(enemies[enemyToSpawn], playerManager.transform.position + new Vector3(randomX, randomY, 0), Quaternion.identity);
            Debug.Log("enemy spawned");
        }
    }
}

public enum Weather
{
    clear,
    raining,
    storm,
    windy,
    blizzard
}
