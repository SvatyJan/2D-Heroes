using UnityEngine;

public class BuildingSystemController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public GameObject buildingAvailableArrowIndicator;

    [SerializeField] private bool isBuilt = false;
    [SerializeField] private bool isHeroNearby = false;

    [SerializeField] private KeyCode buildKey = KeyCode.B;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildingAvailableArrowIndicator.SetActive(false);
        SetBuildingTransparency(150f);
    }

    private void Update()
    {
        if (isHeroNearby && !isBuilt && Input.GetKeyDown(buildKey))
        {
            Build();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBuilt)
        {
            buildingAvailableArrowIndicator.SetActive(true);
            isHeroNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            buildingAvailableArrowIndicator.SetActive(false);
            isHeroNearby = false;
        }
    }

    public void Build()
    {
        if (!isBuilt)
        {
            isBuilt = true;
            buildingAvailableArrowIndicator.SetActive(false);
            SetBuildingTransparency(255);
        }
    }

    private void SetBuildingTransparency(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha / 255f;
        spriteRenderer.color = color;
    }
}
