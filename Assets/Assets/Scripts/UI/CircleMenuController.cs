using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class CircleMenuController : MonoBehaviour
{
    public GameObject CirleMenuObject;
    public GameObject CursorRootObject;
    public GameObject HighlightRootObject;

    private Vector2 mousePosition;
    public Camera cam;

    public TextMeshProUGUI CircleMenuItemLabel;
    public List<GameObject> CirlceMenuItems = new List<GameObject>();

    private void Start()
    {
        CirleMenuObject.active = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CirleMenuObject.active = true;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            CirleMenuObject.active = false;
        }

        if (Input.GetKey(KeyCode.F))
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.x -= Screen.width / 2;
            mousePos.y -= Screen.height / 2;

            int itemChangeAngle = (int)Mathf.Round(360 / CirlceMenuItems.Count);

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90f;
            CursorRootObject.transform.rotation = Quaternion.Euler(0, 0, angle);
            angle += 360;
            angle = angle % 360;

            float highlightAngle = Mathf.Round(angle / itemChangeAngle) * itemChangeAngle;
            highlightAngle += 360 / CirlceMenuItems.Count;
            HighlightRootObject.transform.rotation = Quaternion.Euler(0, 0, highlightAngle);

            int selectedItem = (int)Mathf.Round(angle / itemChangeAngle);
            
            selectedItem = selectedItem % (CirlceMenuItems.Count);

            CircleMenuItemLabel.text = CirlceMenuItems[selectedItem].name;
        }
    }
}
