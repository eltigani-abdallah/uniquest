using UnityEngine;
using System.Collections;


public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST, RUN }

    [Header("Combat Station")]
    public Transform playerBatleStation;//Player spawn point on the battlefileld
    public Transform enemyBattleStation;//Enemy spawn point on the battlefield

    [Header("Unit Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;


    private BattleState currentState;
    private PlayerUnit playerUnit;
    private EnemyUnit enemyUnit;
    private GameObject playerInstance;
    private GameObject enemyInstance;

}