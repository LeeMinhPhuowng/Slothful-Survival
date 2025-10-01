using UnityEngine;

[CreateAssetMenu(fileName = "New Info", menuName = "Character/New Character Info")]
public class CharacterInfoSO : ScriptableObject
{
    [Header("Base Character Stat")]
    public int maxHealth;
    public int moveSpeed;
    public GameObject startWeapon;
    [Header("Character Prefab")]
    public GameObject prefab;
}
