using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Generic variables:")]
    public string itemName = "new item";
    public AudioClip clipToPlay;
    private AudioSource source;
    [TextArea(3,5)]
    public string description = "an item...";
    public float weight;
    public int quantity = 0;
    public ItemType itemType;

    public float coolDown = 1f;

    [Header("If consumable, it restores:")]
    public RestoreType restoreType;
    public int ammountToRestore = 0;

    [Header("If weapon, it does:")]
    //the attack radius of melee weapons or the bullet distance of ranged weapons
    public float attackRadius = 0.5f;
    public float damage = 0;
    public int numberOfBullets = 1;
    public GameObject damageCollider;

    [Header("If utility, it does:")]
    public bool isPlacable = false;
    public GameObject objectToPlace;
    public float minPlaceDistance = 0.5f;
    public float maxPlaceDistance = 5f;


    public void Consume(Item itemToConsume)
    {
        PlayerManager pm = FindObjectOfType<PlayerManager>();
        switch(restoreType)
        {
            case RestoreType.Health:
                pm.currentPlayerHealth += ammountToRestore;
                if (pm.currentPlayerHealth > pm.maxPlayerHealth) { pm.currentPlayerHealth = pm.maxPlayerHealth; }
                break;
            case RestoreType.Oxygen:
                pm.oxygen += ammountToRestore;
                if (pm.oxygen > 100) { pm.oxygen = 100; }
                break;
            case RestoreType.Food:
                pm.food += ammountToRestore;
                if (pm.food > 100) { pm.food = 100; }
                break;
            case RestoreType.Water:
                pm.water += ammountToRestore;
                if (pm.water > 100) { pm.water = 100; }
                break;
        }
        if(clipToPlay != null)
        {
            source = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
            source.PlayOneShot(clipToPlay);
        }

        itemToConsume.quantity -= 1;
        if(itemToConsume.quantity == 0)
        {
            pm.inventory.Remove(itemToConsume);
        }
        pm.CalculateWeight();
        pm.AddLog("Consumed: " + itemName);
    }

    public void Attack()
    {
        if(itemType == ItemType.melee)
        {
            Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 mousePos = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);

            double playerX = Math.Round(playerPos.x, MidpointRounding.AwayFromZero);
            double playerY = Math.Round(playerPos.y, MidpointRounding.AwayFromZero);
            double mouseX = Math.Round(mousePos.x, MidpointRounding.AwayFromZero);
            double mouseY = Math.Round(mousePos.y, MidpointRounding.AwayFromZero);

            Vector2 offset = Vector2.zero;
            float rotationOnZ = 0f;

            if(playerX < mouseX)
            {
                if(playerY < mouseY)
                {
                    //TopRight;
                    offset = new Vector2(0.5f, 0.5f);
                    rotationOnZ = 315f;
                }
                else if(playerY == mouseY)
                {
                    //Right;
                    offset = new Vector2(0.5f, 0f);
                    rotationOnZ = 270f;
                }
                else if (playerY > mouseY)
                {
                    //BottomRight;
                    offset = new Vector2(0.5f, -0.5f);
                    rotationOnZ = 225f;
                }
            }
            else if (playerX == mouseX)
            {
                if (playerY <= mouseY)
                {
                    //Top/center;
                    offset = new Vector2(0f, 0.5f);
                }
                else if (playerY >mouseY)
                {
                    //Bottom;
                    offset = new Vector2(0f, -0.5f);
                    rotationOnZ = 180f;
                }
            }
            else if (playerX > mouseX)
            {
                if (playerY < mouseY)
                {
                    //TopLeft;
                    offset = new Vector2(-0.5f, 0.5f);
                    rotationOnZ = 45f;
                }
                else if (playerY == mouseY)
                {
                    //Left;
                    offset = new Vector2(-0.5f, 0f);
                    rotationOnZ = 90f;
                }
                else if (playerY > mouseY)
                {
                    //BottomLeft;
                    offset = new Vector2(-0.5f, -0.5f);
                    rotationOnZ = 135f;
                }
            }
            offset.y -= 0.25f;
            Instantiate(damageCollider, playerPos+offset, Quaternion.Euler(0f,0f,rotationOnZ));
        }
        else if(itemType == ItemType.ranged)
        {
            Debug.Log("shot");
            Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

            for(int i = 0; i < numberOfBullets; i++)
                Instantiate(damageCollider, playerPos, Quaternion.identity);
        }
    }

    public void Place(Item itemToPlace)
    {
        if (isPlacable)
        {
            var player = GameObject.Find("Player");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPos = player.transform.position;
            var distance = Vector2.Distance(mousePos, playerPos);
            if (distance > minPlaceDistance && distance < maxPlaceDistance)
            {
                Instantiate(objectToPlace, mousePos, Quaternion.identity);

                itemToPlace.quantity -= 1;
                if (itemToPlace.quantity == 0)
                {
                    player.GetComponent<PlayerManager>().inventory.Remove(itemToPlace);
                }
                var pm = player.GetComponent<PlayerManager>();
                pm.CalculateWeight();
                pm.AddLog("Placed: " + itemToPlace.itemName);
            }
        }
    }
}

public enum ItemType
{
    melee, //use it to attack
    ranged,
    resource, //use it to make other things 
    consumable, //consume it for perks
    utility //place it and interact with it
}

public enum RestoreType
{
    Health,
    Oxygen,
    Food,
    Water
}

