using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.GetComponent<CapsuleCollider2D>().enabled)
        {
            print("BossDead");
            DialogueLua.SetVariable("BossDead", true);
            gameObject.GetComponent<Boss>().enabled = false;
        }
    }
}
