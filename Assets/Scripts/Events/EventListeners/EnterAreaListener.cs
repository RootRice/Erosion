using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterAreaListener : EventListener
{
    [SerializeField] List<GameObject> actionsToPerform;
    private Collider playerCol;
    private void Awake()
    {
        playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();

        foreach(GameObject g in actionsToPerform)
        {
            actions.Add(g.GetComponent<EventAction>());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == playerCol)
        {
            foreach(EventAction action in actions)
            {
                action.TriggerAction();
            }
        }
    }

}
