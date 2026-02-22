using UnityEngine;

public class SpellInputController : MonoBehaviour
{
    [SerializeField] private SpellCaster spellCaster;
    [SerializeField] private Player player;
    [SerializeField] private Camera cam;

    private void Update()
    {
        if (spellCaster.SelectedSpell == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            spellCaster.CastCurrentSpell(player, worldPos);
            spellCaster.ClearSpell();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            spellCaster.ClearSpell();
        }
    }
}