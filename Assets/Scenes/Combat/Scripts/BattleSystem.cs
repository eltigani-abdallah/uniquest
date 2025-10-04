using UnityEngine;
using System.Collections;

/// <summary>
/// STAGE 2: Integrate Unit classes and UI events.
/// Focus: Replace placeholders with real game logic and event-driven UI.
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST, RUN }

    [Header("Combat Stations")]
    public Transform playerBattleStation;  // Designated player position
    public Transform enemyBattleStation;   // Designated enemy position

    [Header("Unit Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    // Unit references
    private PlayerUnit playerUnit;
    private EnemyUnit enemyUnit;
    private BattleState currentState;

    // UI Events (new in Stage 2)
    public System.Action<string> OnDialogTextChanged;
    public System.Action<int, int> OnPlayerHPChanged;
    public System.Action<int, int> OnEnemyHPChanged;
    public System.Action<bool> OnActionsEnabled;

    void Start()
    {
        currentState = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Now instantiates Unit classes and hooks up UI events.
    /// </summary>
    IEnumerator SetupBattle()
    {
        // Spawn units at battle stations
        GameObject playerInstance = Instantiate(playerPrefab, playerBattleStation);
        GameObject enemyInstance = Instantiate(enemyPrefab, enemyBattleStation);

        playerUnit = playerInstance.GetComponent<PlayerUnit>();
        enemyUnit = enemyInstance.GetComponent<EnemyUnit>();

        SetDialogText("A wild " + enemyUnit.unitName + " appears!");
        yield return new WaitForSeconds(2f);

        currentState = BattleState.PLAYER_TURN;
        PlayerTurn();
    }

    // === UI Integration ===
    private void SetDialogText(string text) => OnDialogTextChanged?.Invoke(text);
    private void EnableActions(bool enable) => OnActionsEnabled?.Invoke(enable);
    private void UpdatePlayerHP() => OnPlayerHPChanged?.Invoke(playerUnit.currentHP, playerUnit.maxHP);
    private void UpdateEnemyHP() => OnEnemyHPChanged?.Invoke(enemyUnit.currentHP, enemyUnit.maxHP);

    // === Updated Action Handlers (now use Unit methods) ===
    IEnumerator PlayerAttack()
    {
        EnableActions(false);
        SetDialogText($"{playerUnit.unitName} attacks!");

        yield return new WaitForSeconds(1f);

        // Use actual Unit.TakeDamage() method
        bool isDead = enemyUnit.TakeDamage(playerUnit.attack);
        UpdateEnemyHP();

        if (isDead)
            EndBattle(true);
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        currentState = BattleState.ENEMY_TURN;
        SetDialogText($"{enemyUnit.unitName} attacks!");

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.attack);
        UpdatePlayerHP();

        if (isDead)
            EndBattle(false);
        else
        {
            currentState = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    /// <summary>
    /// New in Stage 2: Proper battle cleanup and scene transition.
    /// </summary>
    void EndBattle(bool playerWon)
    {
        currentState = playerWon ? BattleState.WON : BattleState.LOST;
        SetDialogText(playerWon ? "Victory!" : "Defeat...");
        EnableActions(false);
        StartCoroutine(ReturnToMapAfterDelay(3f));
    }

    IEnumerator ReturnToMapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }
}
