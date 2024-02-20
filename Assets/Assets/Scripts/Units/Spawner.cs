using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject unitPrefab;
    public float spawnInterval = 10f;
    public GameObject spawnPoint;
    [SerializeField] bool spawnerActive = true;
    private int unitId = 0;


    private void Start()
    {
        if (spawnerActive)
        {
            InvokeRepeating("GenerateUnit", spawnInterval, spawnInterval);
        }        
    }

    private void GenerateUnit()
    {
        GameObject unit =  Instantiate(unitPrefab, spawnPoint.transform.position, Quaternion.identity);
        unit.name = unitPrefab.name + " " + unitId.ToString();

        Transform child = unit.transform.Find("Footman");
        unit.gameObject.name = unitPrefab.name + " " + unitId.ToString();
        //child.gameObject.name = unitPrefab.name + " " + unitId.ToString();
        unitId++;        
    }
}
