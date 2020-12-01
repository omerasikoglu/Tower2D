using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MinionType")]
public class MinionTypeSO : ScriptableObject
{
    
    //[SerializeField] private MinionSetsListSO minionSetsList;

    public Transform prefab;
    public string nameString;
    public Sprite sprite;
    public int healthAmountMax;
    public float movementSpeed = 5f;
    public ResourceAmount[] summoningCostAmountArray;
    

    public string GetSummoningCostsToString()
    {
        string str = "";
        foreach (ResourceAmount resourceAmount in summoningCostAmountArray)
        {
            str += "<color=#" + resourceAmount.resourceType.colorHex + ">" + resourceAmount.amount + " " + resourceAmount.resourceType.nameString + "</color>\n";
        }
        return str;
    }
}
