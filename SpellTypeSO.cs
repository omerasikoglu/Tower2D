﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpellType")]
public class SpellTypeSO : ScriptableObject
{
    public string nameString;
    public Sprite sprite;
    public float cooldownTime;
    public ResourceAmount[] castAmountArray;
    public string GetSpellCastCostsToString()
    {
        string str = "";
        foreach (ResourceAmount resourceAmount in castAmountArray)
        {
            str += "<color=#" + resourceAmount.resourceType.colorHex + ">" + resourceAmount.amount + " " + resourceAmount.resourceType.nameString + "</color>\n";
        }
        return str;
    }
}
