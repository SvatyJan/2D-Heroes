using UnityEngine;

public class SpellbookUI : MonoBehaviour
{
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private SpellCaster spellCaster;

    private bool isOpen = false;

    private void Start()
    {
        bookPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBook();
        }
    }

    private void ToggleBook()
    {
        isOpen = !isOpen;
        bookPanel.SetActive(isOpen);

        // Time.timeScale = isOpen ? 0f : 1f; // optional pause
    }

    public void SelectSpellButton(Spell spell)
    {
        spellCaster.SelectSpell(spell);
        CloseBook();
    }

    private void CloseBook()
    {
        isOpen = false;
        bookPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}