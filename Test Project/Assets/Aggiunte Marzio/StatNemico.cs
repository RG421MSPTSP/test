using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatNemico : StatEsseri
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Muori()
    {
        base.Muori();
        //effetti di morte e loot

        Destroy(gameObject);
    }
}
