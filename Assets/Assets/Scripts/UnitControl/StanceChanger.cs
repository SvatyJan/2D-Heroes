using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanceChanger : MonoBehaviour
{
    public enum Stance
    {
        AGRESSIVE = 1,
        DEFENSIVE = 2,
        PASSIVE = 3,
    }

    [SerializeField] public Stance stance;

    public void ChangeStance(GameObject player)
    {
        player.GetComponent<UnitController>().ChangeStance((UnitController.Stance)stance);
    }
}
