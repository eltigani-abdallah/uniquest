using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text

public class EnemyUnit : CharacterUnit
{
    public int expReward = 50;

    void Start()
    {
        unitName = "Goblin";
        maxHP = 60;
        currentHP = 60;
        attack = 8;
        defense = 3;
        magicPower = 5;
    }
}