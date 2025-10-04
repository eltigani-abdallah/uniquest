using UnityEngine;
using System.Collections;

/// <summary>
/// Manages turn-based battle mechanics including state transitions, combat actions,
/// and UI updates. Handles player/enemy turns, damage calculations, and battle outcomes.
/// </summary>


public class BattleSystem : MonoBehaviour
{
    // Battle state machine enum
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST, RUN }

    // Transform references for battle positions
    [Header("Combat Stations")]
    public Transform playerBattleStation;  // Where player unit appears in battle
    public Transform enemyBattleStation;   // Where enemy unit appears in battle

    // Prefabs for instantiating units
    [Header("Unit Prefabs")]
    public GameObject playerPrefab;  // Player character prefab
    public GameObject enemyPrefab;   // Enemy character prefab

    // Current battle state and unit references
    private BattleState currentState;
    private PlayerUnit playerUnit;
    private EnemyUnit enemyUnit;
    private GameObject playerInstance;
    private GameObject enemyInstance;

    // Event delegates for UI and animation systems
    public System.Action<string> OnDialogTextChanged;      // Updates battle dialog text
    public System.Action<int, int> OnPlayerHPChanged;      // Updates player HP bar (current, max)
    public System.Action<int, int> OnEnemyHPChanged;       // Updates enemy HP bar (current, max)
    public System.Action<bool> OnActionsEnabled;           // Enables/disables player action buttons
    public System.Action<string> OnPlayerAnimation;        // Triggers player animations
    public System.Action<string> OnEnemyAnimation;         // Triggers enemy animations

    /// <summary>
    /// Initializes battle state and starts setup coroutine
    /// </summary>

    
    void Start()
    {
        currentState = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Coroutine that handles battle initialization:
    /// 1. Instantiates player and enemy units
    /// 2. Plays entrance animations
    /// 3. Sets initial dialog
    /// 4. Transitions to player turn after delay
    /// </summary>
    IEnumerator SetupBattle()
    {
        // Instantiate player unit if prefab and station are assigned
        if (playerPrefab != null && playerBattleStation != null)
        {
            playerInstance = Instantiate(playerPrefab, playerBattleStation.position, Quaternion.identity);
            playerUnit = playerInstance.GetComponent<PlayerUnit>();
        }

        // Instantiate enemy unit if prefab and station are assigned
        if (enemyPrefab != null && enemyBattleStation != null)
        {
            enemyInstance = Instantiate(enemyPrefab, enemyBattleStation.position, Quaternion.identity);
            enemyUnit = enemyInstance.GetComponent<EnemyUnit>();
        }

        // Play entrance animations
        OnPlayerAnimation?.Invoke("Battle_Enter");
        OnEnemyAnimation?.Invoke("Battle_Enter");

        SetDialogText("A wild enemy appears!");
        yield return new WaitForSeconds(2f);

        currentState = BattleState.PLAYER_TURN;
        PlayerTurn();
    }

    /// <summary>
    /// Begins player's turn and enables action selection
    /// </summary>
    
    public void PlayerTurn()
    {
        SetDialogText("Choose an action!");
        EnableActions(true);
    }

    // ===== ACTION HANDLERS (Called from UI buttons) =====
    public void OnPlayerAttack()
    {
        if (currentState != BattleState.PLAYER_TURN) return;
        StartCoroutine(PlayerAttack());
    }

    public void OnPlayerMagic()
    {
        if (currentState != BattleState.PLAYER_TURN) return;
        StartCoroutine(PlayerMagic());
    }

    public void OnPlayerUseItem(int itemIndex)
    {
        if (currentState != BattleState.PLAYER_TURN) return;
        StartCoroutine(PlayerUseItem(itemIndex));
    }

    public void OnRunButton()
    {
        if (currentState != BattleState.PLAYER_TURN) return;
        StartCoroutine(PlayerRun());
    }

    // ===== COMBAT ACTION COROUTINES =====
    /// <summary>
    /// Handles player attack sequence:
    /// 1. Plays attack animation
    /// 2. Calculates damage to enemy
    /// 3. Updates UI
    /// 4. Checks for battle end or continues to enemy turn
    /// </summary>
    
    IEnumerator PlayerAttack()
    {
        EnableActions(false);
        SetDialogText($"{playerUnit.unitName} attacks!");

        OnPlayerAnimation?.Invoke("Attack");
        yield return new WaitForSeconds(0.5f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.attack);
        OnEnemyAnimation?.Invoke("Hit");
        UpdateEnemyHP();

        yield return new WaitForSeconds(1f);

        if (isDead)
            EndBattle(true);
        else
            StartCoroutine(EnemyTurn());
    }

    /// <summary>
    /// Handles enemy turn logic:
    /// 1. Plays enemy attack animation
    /// 2. Calculates damage to player
    /// 3. Updates UI
    /// 4. Checks for battle end or returns to player turn
    /// </summary>
    IEnumerator EnemyTurn()
    {
        currentState = BattleState.ENEMY_TURN;
        SetDialogText($"{enemyUnit.unitName} attacks!");

        OnEnemyAnimation?.Invoke("Attack");
        yield return new WaitForSeconds(0.5f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.attack);
        OnPlayerAnimation?.Invoke("Hit");
        UpdatePlayerHP();

        yield return new WaitForSeconds(1f);

        if (isDead)
            EndBattle(false);
        else
        {
            currentState = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    /// <summary>
    /// Handles magic attack sequence:
    /// 1. Checks mana availability
    /// 2. Plays magic animation if successful
    /// 3. Applies magic damage to enemy
    /// 4. Updates UI and continues battle flow
    /// </summary>
    IEnumerator PlayerMagic()
    {
        EnableActions(false);

        if (playerUnit.UseMana(10))
        {
            SetDialogText($"{playerUnit.unitName} casts a spell!");
            OnPlayerAnimation?.Invoke("Magic");
            yield return new WaitForSeconds(0.8f);

            bool isDead = enemyUnit.TakeDamage(playerUnit.magicPower);
            OnEnemyAnimation?.Invoke("Hit");
            UpdateEnemyHP();

            yield return new WaitForSeconds(1f);

            if (isDead)
                EndBattle(true);
            else
                StartCoroutine(EnemyTurn());
        }
        else
        {
            SetDialogText("Not enough mana!");
            yield return new WaitForSeconds(1f);
            PlayerTurn();
        }
    }

    /// <summary>
    /// Handles item usage (currently hardcoded for healing potion):
    /// 1. Plays item use animation
    /// 2. Applies healing effect
    /// 3. Updates UI and continues battle
    /// </summary>
    IEnumerator PlayerUseItem(int itemIndex)
    {
        EnableActions(false);
        SetDialogText("Used a healing potion!");

        OnPlayerAnimation?.Invoke("UseItem");
        yield return new WaitForSeconds(0.5f);

        playerUnit.Heal(30);
        UpdatePlayerHP();

        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    /// <summary>
    /// Handles escape attempt:
    /// 1. Plays run animation
    /// 2. Ends battle with "run" state
    /// </summary>
    IEnumerator PlayerRun()
    {
        EnableActions(false);
        SetDialogText("Got away safely!");

        OnPlayerAnimation?.Invoke("Run");
        yield return new WaitForSeconds(1f);

        EndBattle(false); // Note: Uses false to distinguish from victory
    }
    ///
    /// <summary>
    /// Ends the battle and triggers victory/defeat sequence:
    /// 1. Sets final dialog text
    /// 2. Plays victory/defeat animation
    /// 3. Returns to overworld after delay
    /// </summary>
    /// <param name="playerWon">True if player won, false if lost or ran</param>
    void EndBattle(bool playerWon)
    {
        currentState = playerWon ? BattleState.WON : BattleState.LOST;

        if (playerWon)
        {
            SetDialogText("Victory! You won the battle!");
            OnPlayerAnimation?.Invoke("Victory");
        }
        else
        {
            SetDialogText("You were defeated...");
            OnPlayerAnimation?.Invoke("Defeat");
        }

        EnableActions(false);
        StartCoroutine(ReturnToMapAfterDelay(3f));
    }

    /// <summary>
    /// Loads overworld scene after specified delay
    /// </summary>
    IEnumerator ReturnToMapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }

    // ===== UI UPDATE METHODS =====
    private void UpdatePlayerHP()
    {
        OnPlayerHPChanged?.Invoke(playerUnit.currentHP, playerUnit.maxHP);
    }

    private void UpdateEnemyHP()
    {
        OnEnemyHPChanged?.Invoke(enemyUnit.currentHP, enemyUnit.maxHP);
    }

    private void SetDialogText(string text)
    {
        OnDialogTextChanged?.Invoke(text);
    }

    private void EnableActions(bool enable)
    {
        OnActionsEnabled?.Invoke(enable);
    }

    // ===== UTILITY METHODS =====
    /// <summary>
    /// Checks if player has enough mana for an action
    /// </summary>
    public bool HasEnoughMana(int requiredMana)
    {
        return playerUnit != null && playerUnit.currentMP >= requiredMana;
    }
}
