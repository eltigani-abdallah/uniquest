using UnityEngine;
using System.Collections;

/// <summary>
/// STAGE 1: Basic state machine with turn transitions.
/// Focus: Establish battle flow without UI/animations.
/// </summary>
public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    [Header("Dependencies")]
    public GameObject playerPrefab;  // Temporary - will replace with proper unit spawning
    public GameObject enemyPrefab;

    private BattleState currentState;
    private GameObject playerInstance;
    private GameObject enemyInstance;

    void Start()
    {
        currentState = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Minimal setup: Instantiate units and start turn cycle.
    /// TODO: Add proper positioning, animations, and UI hooks.
    /// </summary>
    IEnumerator SetupBattle()
    {
        // Temporary instantiation (no battle stations yet)
        playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        enemyInstance = Instantiate(enemyPrefab, new Vector3(5, 0, 0), Quaternion.identity);

        Debug.Log("Battle started! Player vs Enemy");
        yield return new WaitForSeconds(1f);

        currentState = BattleState.PLAYER_TURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        Debug.Log("Player's turn. Current state: " + currentState);
        // TODO: Hook up UI buttons to call OnPlayerAttack(), etc.
    }

    // === Placeholder Action Handlers ===
    public void OnPlayerAttack()
    {
        if (currentState != BattleState.PLAYER_TURN) return;
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        Debug.Log("Player attacks!");
        yield return new WaitForSeconds(1f);

        // Simulate damage (no actual Unit classes yet)
        bool enemyDefeated = Random.Range(0, 2) == 1; // 50% chance to "defeat" enemy

        if (enemyDefeated)
        {
            EndBattle(true);
        }
        else
        {
            currentState = BattleState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy attacks!");
        yield return new WaitForSeconds(1f);

        // Simulate player defeat check
        bool playerDefeated = Random.Range(0, 2) == 1;
        if (playerDefeated)
        {
            EndBattle(false);
        }
        else
        {
            currentState = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    void EndBattle(bool playerWon)
    {
        currentState = playerWon ? BattleState.WON : BattleState.LOST;
        Debug.Log(playerWon ? "Victory!" : "Defeat...");
        // TODO: Return to overworld scene
    }
}
