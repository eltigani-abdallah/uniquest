using UnityEngine;

public class ActionScript : MonoBehaviour
{
    private BattleSystem battleSystem;

    void Start()
    {
        battleSystem = FindFirstObjectByType<BattleSystem>();
        if (battleSystem == null)
            Debug.LogError("BattleSystem not found in scene!");
    }
    // === PUBLIC METHODS FOR UI BUTTONS ===
    public void Attack()
    {
        Debug.Log("[Combat] Attack action");
        battleSystem?.OnPlayerAttack();
    }

    public void Magic()
    {
        Debug.Log("[Combat] Magic action");
        battleSystem?.OnPlayerMagic();
    }

    public void Item()
    {
        Debug.Log("[Combat] Item action");
        battleSystem?.OnPlayerUseItem(0); // 0 = health potion
    }

    public void Run()
    {
        Debug.Log("[Combat] Run action");
        battleSystem?.OnRunButton();
    }
}