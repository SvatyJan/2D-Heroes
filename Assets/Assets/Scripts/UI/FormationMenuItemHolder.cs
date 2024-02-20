using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMenuItemHolder : MonoBehaviour
{

    public List<GameObject> FormationMenuItems = new List<GameObject>();

    void Update()
    {
        foreach (GameObject FormationMenuItem in FormationMenuItems)
        {
            GameObject newMenuItem = Instantiate(FormationMenuItem);
            newMenuItem.transform.parent = this.transform;
        }
    }
}
