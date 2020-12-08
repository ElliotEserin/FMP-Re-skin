using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObjective : MonoBehaviour
{
    public enum affectType
    {
        Destroy,
        Enable,
        Disable,
        SetColliderToTrigger,
        Move
    }

    public Objective objectiveToComplete;
    [Header("OPTIONAL")]
    public GameObject objectToAffect;
    public affectType typeOfAffect;
    [Header("In case of move: ")]
    public Vector3 movePosition;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(objectiveToComplete != null)
            CompleteObjective();

        if(objectToAffect != null)
        {
            switch(typeOfAffect)
            {
                case affectType.SetColliderToTrigger:
                    objectToAffect.GetComponent<BoxCollider2D>().isTrigger = true;
                    break;
                case affectType.Move:
                    objectToAffect.transform.position = movePosition;
                    break;
            }
        }

        Destroy(gameObject);
    }

    private void CompleteObjective()
    {
        objectiveToComplete.complete = true;
    }
}
