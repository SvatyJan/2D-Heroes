using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FormationMenuController : MonoBehaviour
{
    public GameObject Menu;
    public GameObject Cursor;
    public GameObject RootHighlight;

    public Camera cam;

    public TextMeshProUGUI CircleMenuItemLabel;
    public List<GameObject> CirlceMenuItems = new List<GameObject>();

    private int selectedItem = 0;

    private void Start()
    {
        Menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Menu.SetActive(true);
        }

        if (Input.GetKey(KeyCode.F))
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.x -= Screen.width / 2;
            mousePos.y -= Screen.height / 2;

            int itemChangeAngle = (int)Mathf.Round(360 / CirlceMenuItems.Count);

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90f;
            Cursor.transform.rotation = Quaternion.Euler(0, 0, angle);
            angle += 360;
            angle = angle % 360;

            float highlightAngle = Mathf.Round(angle / itemChangeAngle) * itemChangeAngle;
            highlightAngle += 360 / CirlceMenuItems.Count;
            RootHighlight.transform.rotation = Quaternion.Euler(0, 0, highlightAngle);

            selectedItem = (int)Mathf.Round(angle / itemChangeAngle);
            selectedItem = selectedItem % (CirlceMenuItems.Count);

            CircleMenuItemLabel.text = CirlceMenuItems[selectedItem].name;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            CirlceMenuItems[selectedItem].GetComponent<FormationChanger>().ChangeFormation(this.gameObject);
            Menu.SetActive(false);
        }
    }
}
