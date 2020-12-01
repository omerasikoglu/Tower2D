using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public event EventHandler OnResourceAmountChanged;

    [SerializeField] private List<ResourceAmount> startingResourceAmountList;

    private Dictionary<ResourceTypeSO, int> resourceAmountDictionary;

    private void Awake()
    {
        Instance = this;
        resourceAmountDictionary = new Dictionary<ResourceTypeSO, int>();
        ResourceTypeListSO resourceTypeList = Resources.Load<ResourceTypeListSO>(typeof(ResourceTypeListSO).Name);
        foreach (ResourceTypeSO resourceType in resourceTypeList.list)  //oyun başı tüm kaynakları 0lama
        {
            resourceAmountDictionary[resourceType] = 0;

        }
        foreach (ResourceAmount resourceAmount in startingResourceAmountList)   //başlangıç fazladan kaynak ekleme
        {
            AddResource(resourceAmount.resourceType, resourceAmount.amount);
        }

    }
    public void AddResource(ResourceTypeSO resourceType, int amount)
    {
        if (resourceAmountDictionary[resourceType] != 5)
        {
            resourceAmountDictionary[resourceType] += amount;
            OnResourceAmountChanged?.Invoke(this, EventArgs.Empty);    //NULL CHECK - null değilse sağ taraf gerçekleşir
                                                                       //BUNLA AYNI ŞEY - if(OnResourceAmountChanged!=null){OnResourceAmountChanged(this,EventArgs.Empty)}
        }
    }

    public int GetResourceAmount(ResourceTypeSO resourceType)   //gönderilen kaynaktan sende kaç tane var onu döndürür
    {
        return resourceAmountDictionary[resourceType];
    }
    public bool CanAfford(ResourceAmount[] resourceAmountArray)
    {
        foreach (ResourceAmount resourceAmount in resourceAmountArray)
        {
            if (GetResourceAmount(resourceAmount.resourceType) >= resourceAmount.amount) { }
            //önce tüm kaynaklara sırasıyla bakar en son true döndürür
            else return false;

        }
        return true;
    }
    public void SpendResources(ResourceAmount[] resourceAmountArray)    //kaynak eksilmesi bina dikince
    {
        foreach (ResourceAmount resourceAmount in resourceAmountArray)
        {
            resourceAmountDictionary[resourceAmount.resourceType] -= resourceAmount.amount;

        }
        OnResourceAmountChanged?.Invoke(this, EventArgs.Empty);
    }

}




