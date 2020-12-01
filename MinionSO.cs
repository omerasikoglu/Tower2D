using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Minion")]
public class MinionSO : ScriptableObject
{
    public MinionSetSO minionSet;
    public string nameString;
    public Sprite sprite;

    private const float speed = 5f;
}
