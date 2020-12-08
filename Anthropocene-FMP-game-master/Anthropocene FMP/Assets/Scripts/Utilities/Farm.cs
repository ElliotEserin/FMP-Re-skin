using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Farm : DynamicMovement
{
    public GameObject[] crops;
    public float growTime = 4f; //minutes 
    DateTime startTime = DateTime.UtcNow;
    PlayerManager playerManager;

    private void OnBecameVisible()
    {
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();
            updateSortOrder();
        }
        GrowUp();
    }

    private void OnBecameInvisible()
    {
        GrowUp();
    }

    public void GrowUp()
    {
        DateTime currentTime = DateTime.UtcNow;
        if ((currentTime - startTime).TotalMinutes >= growTime)
        {
            updateSortOrder();
            foreach(GameObject crop in crops)
            {
                crop.SetActive(true);
            }
            startTime.AddMinutes(growTime);
        }
    }
}
