using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public List<Objective> objectives;
    public Objective currentObjective;
    private Objective previousObjective;

    public AudioSource source;
    public AudioClip notification;

    public TextMeshProUGUI tutorialBox;

    private PlayerManager playerManager;

    bool tutorialFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        objectives[0].active = true;
        currentObjective = objectives[0];

        if (PlayerPrefs.GetInt("STATUS", 0) == 0)
        {
            Debug.Log("RESETTING TUTORIAL");
            foreach (Objective obj in objectives)
            {
                obj.active = false;
                obj.complete = false;
            }
        }
        else
        {
            Debug.Log("FINISHING TUTORIAL");
            objectives.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentObjective != previousObjective)
        {
            previousObjective = currentObjective;
            tutorialBox.SetText(currentObjective.objective);
        }
        if(currentObjective.complete)
        {
            objectives.Remove(currentObjective);
            if (objectives.Count > 0)
                currentObjective = objectives[0];
            else
            {
                if (!tutorialFinished)
                    InvokeRepeating("DisplayCoords", 0.25f, 0.25f);
                tutorialFinished = true;
            }
        }

        switch(currentObjective.goal)
        {
            case Goal.interact:
                if(playerManager.inventory.Count > 0)
                {
                    source.PlayOneShot(notification);
                    currentObjective.complete = true;
                }
                break;
            case Goal.inventory:
                if(playerManager.inventoryMenu.activeInHierarchy)
                {
                    source.PlayOneShot(notification);
                    currentObjective.complete = true;
                }
                break;
            case Goal.crafting:
                if(!playerManager.uISelection)
                {
                    source.PlayOneShot(notification);
                    currentObjective.complete = true;
                    transform.SetAsFirstSibling();
                }
                break;
            case Goal.equipItem:
                if(playerManager.leftHand != null || playerManager.rightHand != null)
                {
                    source.PlayOneShot(notification);
                    currentObjective.complete = true;
                }
                break;
            case Goal.NOTYETIMPLEMENTED:
                NotYetImplementedOrTimerBased();
                break;
        }
    }

    IEnumerator NotYetImplementedOrTimerBased()
    {
        yield return new WaitForSeconds(10);
        source.PlayOneShot(notification);
        currentObjective.complete = true;
    }

    void DisplayCoords()
    {
        tutorialBox.SetText(((Vector2)playerManager.transform.position).ToString());
    }
}

public enum Goal
{
    move,
    interact,
    inventory,
    crafting,
    equipItem,
    NOTYETIMPLEMENTED
}


