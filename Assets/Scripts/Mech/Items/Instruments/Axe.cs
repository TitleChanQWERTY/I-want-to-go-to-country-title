using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

public class Axe : Instrument 
{
    private PlantController tree = null;
    private UnityEvent myEvent;

    private int HitCount { get; set; }
    private int TimeChop { get; set; }

    public Axe(int level, int durability)
    {
        Level = level;
        Durability = durability;
        
        switch (level) 
        {
            default: 
                HitCount = 10;
                TimeChop = 5;
                break;
        }
    }

    private void ChopTree()
    {
        tree.ChoppingTree(HitCount);
    }

    public override void Use()
    {
        if (tree == null) return;

        if (tree.IsCanChoppingTree())
        {
            UIController.GetInstance().ProgressBar(TimeChop, ChopTree);
        }
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                           startPoint.forward,
                           out hit,
                           MechConstants.DISTANCE_TO_TREE))
        {
            Transform hitObj = hit.transform;

            if (hitObj.CompareTag(TagConstants.TREE))
            {
                tree = hitObj.GetComponent<PlantController>();
            }
            else
            {
                tree = null;
                UIController.GetInstance().StopProgressBar();
            }
        }
        else
        {
            tree = null;
            UIController.GetInstance().StopProgressBar();
        }

        return base.Updating(obj, prefab);
    }
}
