using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : MonoBehaviour
{
    public GameObject shieldRing;
    public float shieldRadius = 5f;
    public PlayerManager pm;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(shieldRing, transform.position, Quaternion.identity);
        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = shieldRadius;
        pm = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, pm.transform.position) <= shieldRadius)
        {
            if (pm.oxygen < 100)
                pm.oxygen += pm.rateOfOxygenDecrease * 2 * Time.deltaTime;
            else
                pm.oxygen = 100;

            pm.isCovered = true;
        }
        else if (pm.isCovered == true)
        {
            pm.isCovered = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shieldRadius);
    }
}
