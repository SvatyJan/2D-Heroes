using UnityEngine;

public class FormationChanger : MonoBehaviour
{
    public enum Formation
    {
        CIRCLE = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }

    [SerializeField] public Formation formation;

    public void ChangeFormation(GameObject player)
    {
        player.GetComponent<UnitController>().ChangeFormation((UnitController.Formation)formation);
    }
}
