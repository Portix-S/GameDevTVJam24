
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueDontDestroy : MonoBehaviour
{
    private static UniqueDontDestroy instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
