using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] 
public class Stat
{
    [SerializeField]//perch� sia visibile anche dall'inspector senza essere public
   private int valoreBase;

    public int GetValore()
    {
        return valoreBase;
    }
}
