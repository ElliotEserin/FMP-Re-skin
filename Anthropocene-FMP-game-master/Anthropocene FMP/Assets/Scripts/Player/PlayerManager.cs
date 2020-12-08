using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public float oxygen = 100f, rateOfOxygenDecrease = 0.4f; //oxygen
    public float food = 100f, rateOfFoodDecrease = 0.5f; //food
    public float water = 100f, rateOfWaterDecrease = 0.2f; //water
    [SerializeField]
    private float _currentPlayerHealth;
    public float currentPlayerHealth
    {
        get
        {
            return _currentPlayerHealth;
        }
        set
        {
            _currentPlayerHealth = value;
            CheckHealth();
        }
    }
    public float maxPlayerHealth = 100, rateOfHealthDecrease = 2f; //health
    public float currentPlayerWeight = 0, maxPlayerWeight = 100; //weight

    public GameObject pauseMenu, inventoryMenu; //menus
    public List<Item> inventory = new List<Item>(); //inventorys
    public Item leftHand, rightHand;
    public InventoryUI IUI;
    public TextMeshProUGUI logText;
   
    public CraftingUI craftingUI;

    public bool isCovered = false;
    
    private List<string>_log = new List<string>();
    public List<string> Log
    {
        get
        {
            return _log;
        }
        set
        {
            _log = value;
            UpdateLog();
        }
    }

    public int litterQuantity = 0;
    public bool uISelection = true;
    float lhTimer = 0f;
    float rhTimer = 0f;
    const int maxLogLength = 5;
    Interactable interactable;
    GameUI GUI;
    [SerializeField]
    Item[] miscLogs;

    private void Start()
    {
        _currentPlayerHealth = maxPlayerHealth;
        GUI = FindObjectOfType<GameUI>();
        IUI = FindObjectOfType<InventoryUI>();
        GUI.infoText.text = null;
        GUI.commandText.text = null;
    }

    void Update()
    {
        oxygen -= rateOfOxygenDecrease * Time.deltaTime;
        water -= rateOfWaterDecrease * Time.deltaTime;
        if (oxygen <= 0)
        {
            oxygen = 0;
            currentPlayerHealth -= rateOfHealthDecrease * Time.deltaTime;
            //no oxygen functionality
        }
        if (food <= 0)
        {
            food = 0;
            //no food functionality
        }
        if (water <= 0)
        {
            water = 0;
            //no water functionality
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactable != null && inventoryMenu.activeInHierarchy == false)
            {
                interactable.interact();
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !inventoryMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            SetTimeScaleForMenus();
        }
        if(Input.GetKeyDown(KeyCode.I) && !pauseMenu.activeInHierarchy)
        {
            inventoryMenu.SetActive(!inventoryMenu.activeInHierarchy);
            SetTimeScaleForMenus();
        }
        if(Input.GetMouseButtonDown(0) && lhTimer <= 0 && Time.timeScale == 1)
        {
            lhTimer = UseHand(leftHand);
            
        }
        if (Input.GetMouseButtonDown(1) && rhTimer <= 0 && Time.timeScale == 1)
        {
            rhTimer = UseHand(rightHand);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            uISelection = !uISelection;
        }

        if(lhTimer > 0)
        {
            lhTimer -= Time.deltaTime;
        }
        if (rhTimer > 0)
        {
            rhTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<Interactable>();
        if(interactable != null)
        {
            string command = "";
            if (interactable.itemNeeded != null) { command = " (" + interactable.itemNeeded.itemName + ")"; }
            GUI.commandText.text = "E" + command;
            if (interactable.item != null)
            {
                GUI.infoText.text = interactable.item.itemName;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = null;
        GUI.commandText.text = null;
        GUI.infoText.text = null;
    }

    void SetTimeScaleForMenus()
    {
        if (Time.timeScale == 1) { Time.timeScale = 0f; }
        else { Time.timeScale = 1f; }
    }

    float UseHand(Item hand)
    {
        if (hand != null)
        {
            if (hand.quantity == 0)
            {
                hand = null;
            }
        }

        if (hand != null)
        {
            switch (hand.itemType)
            {
                case ItemType.consumable:
                    hand.Consume(hand);
                    //if (IUI.positionInList > inventory.Count - 1) { IUI.positionInList = 0; }
                    break;
                case ItemType.ranged:
                case ItemType.melee:
                    hand.Attack();
                    Debug.Log("You Attacked!");
                    break;
                case ItemType.utility:
                    hand.Place(hand);
                    break;
                
            }

            if (hand.itemName == "Analyzer")
            { 
                print("scanning");
                RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward * 1000);
                if(hit2D.collider != null)
                {
                    print("ANALYSING: " + hit2D.collider.name);

                    if (hit2D.collider.tag == "Scanable" || hit2D.collider.tag == "Enemy")
                    {
                        Interactable interactable = hit2D.collider.gameObject.GetComponent<Interactable>();
                        interactable.interact();
                    }
                    else
                    {
                        int num = Random.Range(0, miscLogs.Length);
                        interact(miscLogs[num]);
                    }
                }
                else
                {
                    int num = Random.Range(0, miscLogs.Length);
                    interact(miscLogs[num]);
                }
                return hand.coolDown;
            }
        }

        return 0;
    }
    public void CalculateWeight()
    {
        currentPlayerWeight = 0f;

        foreach (Item item in inventory)
        {
            currentPlayerWeight += item.weight * item.quantity;
        }
    }

    void UpdateLog()
    {
        if (Log.Count > maxLogLength)
        {
            Log.Remove(Log[0]);
        }

        StringBuilder textToOutput = new StringBuilder();
        foreach (string logEntry in Log)
        {
            textToOutput.Append(logEntry + "\n");
        }

        logText.text = textToOutput.ToString();
    }

    public void AddLog(string textToAdd)
    {
        Log.Add(textToAdd);
        Log = Log;
    }

    public void interact(Item item)
    {
        if (item != null)
        {
            bool isInInventory = false;
            foreach (Item itemInInventory in inventory)
            {
                if (itemInInventory == item)
                {
                    item.quantity += 1;
                    isInInventory = true;
                }
            }
            if (!isInInventory)
            {
                inventory.Add(item);
                if (item.quantity <= 0)
                {
                    item.quantity = 1;
                }
            }

            CalculateWeight();
            AddLog("Picked up: " + item.itemName);
        }   
    }

    public void CheckHealth()
    {
        if(_currentPlayerHealth <= 0)
        {
            GameManagement.instance.Die();
        }
    }
}        
        
   
