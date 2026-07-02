using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance { get; set; }

    void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //Objects Base
    public ObjectsDatabseSO databaseSO;
}
