using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class PlayerUnit : CharacterUnit
{
    public int experience;
    public int expToNextLevel = 100;

    void Start()
    {
        unitName = "Hero";
    }
}