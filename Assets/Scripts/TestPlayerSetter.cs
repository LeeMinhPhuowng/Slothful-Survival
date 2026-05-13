using UnityEngine;

public class TestPlayerSetter : MonoBehaviour
{
    [SerializeField] CharacterInfoSO infoSO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInfo.instance.InitializeFromCharacterInfoSO(infoSO);
    }
}
