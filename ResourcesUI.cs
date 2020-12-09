using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcesUI : MonoBehaviour
{
    private ResourceTypeListSO resourceTypeList;
    private Dictionary<ResourceTypeSO, Transform> resourceTypeTransformDictionary;
    private void Awake()
    {
        resourceTypeList = Resources.Load<ResourceTypeListSO>(typeof(ResourceTypeListSO).Name);
        resourceTypeTransformDictionary = new Dictionary<ResourceTypeSO, Transform>();

        Transform orbTemplate = transform.Find("orbTemplate");
        orbTemplate.gameObject.SetActive(false);

        // int index = 0;
        foreach (ResourceTypeSO resourceType in resourceTypeList.list)
        {
            Transform resourceTransform = Instantiate(orbTemplate, transform);
            resourceTransform.gameObject.SetActive(true);
            resourceTransform.Find("orb").Find("orbSprite").GetComponent<Image>().color = resourceType.color;
            //float offsetAmount = -150f;
            //resourceTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetAmount * index, 0);

            //resourceTransform.Find("image").GetComponent<Image>().sprite = resourceType.sprite;
            resourceTypeTransformDictionary[resourceType] = resourceTransform;

            //index++;
        }

    }
    private void Start()
    {
        ResourceManager.Instance.OnResourceAmountChanged += ResourceManager_OnResourceAmountChanged;
        UpdateResourceAmount();
    }

    private void ResourceManager_OnResourceAmountChanged(object sender, System.EventArgs e)
    {
        UpdateResourceAmount();
    }


    private void UpdateResourceAmount()
    {
        foreach (ResourceTypeSO resourceType in resourceTypeList.list)
        {
            Transform resourceTransform = resourceTypeTransformDictionary[resourceType];
            int resourceAmount = ResourceManager.Instance.GetResourceAmount(resourceType);
            resourceTransform.Find("text").GetComponent<TextMeshProUGUI>().SetText(resourceAmount.ToString());
            UpdateOrb(resourceTransform.Find("orb").Find("orbSprite"), resourceAmount);
        }
    }
    private void UpdateOrb(Transform orbTransform, int resourceAmount)
    {
        orbTransform.GetComponent<Image>().fillAmount = GetResourceAmountNormalized(resourceAmount);
    }
    private float GetResourceAmountNormalized(int resourceAmount)
    {
        return (float)resourceAmount / 5;
    }
}
