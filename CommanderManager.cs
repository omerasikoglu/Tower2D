using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderManager : MonoBehaviour
{
    public static CommanderManager Instance { get; private set; }

    public event EventHandler<OnActiveSpellChangedEventArgs> OnActiveSpellChanged;

    public class OnActiveSpellChangedEventArgs:EventArgs
    {
        //public SpellTypeSO activeSpellType; 
    }

    [SerializeField] private Commander commander;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        
    }
}
