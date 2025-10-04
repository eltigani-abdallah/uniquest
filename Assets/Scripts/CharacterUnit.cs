using UnityEngine;

public class CharacterUnit : MonoBehaviour
{
    [Header("Basic Info")]
    public string unitName;
    public int level = 1;

    [Header("Health & Mana")]
    public int maxHP = 100;
    public int currentHP = 100;
    public int maxMP = 50;
    public int currentMP = 50;

    [Header("Stats")]
    public int attack = 10;
    public int defense = 5;
    public int speed = 8;
    public int magicPower = 15;

    public virtual bool TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - defense);
        currentHP -= actualDamage;

        Debug.Log($"[Combat] {unitName} took {actualDamage} damage! HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log($"[Combat] {unitName} was defeated!");
            return true;
        }
        return false;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        Debug.Log($"[Combat] {unitName} healed for {amount} HP! HP: {currentHP}/{maxHP}");
    }

    public bool UseMana(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            Debug.Log($"[Combat] {unitName} used {amount} MP! MP: {currentMP}/{maxMP}");
            return true;
        }
        return false;
    }
}