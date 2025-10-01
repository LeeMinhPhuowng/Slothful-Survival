using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Experimental.Animations;
public class ClassOptions : MonoBehaviour
{
    [SerializeField] List<CharacterInfoSO> playerInfos;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform playerGameObject;
    //Health Bar
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject healthBarCanvas;
    //Camera
    [SerializeField] CinemachineStateDrivenCamera drivenCamera;
    [SerializeField] CinemachineCamera runCamera;
    [SerializeField] CinemachineCamera idleCamera;


    private void Start()
    {
        this.gameObject.SetActive(true);
        AugmentManager.Instance.gameObject.SetActive(false);
        PlayerLevelManager.Instance.gameObject.SetActive(false);
        HealthBarCanvas.Instance.gameObject.SetActive(false);
        Spawner.Instance.gameObject.SetActive(false);
    }

    public void OnKnightChosen()
    {
        SpawnPlayer(0);
    }

    public void OnArcherChosen()
    {
        SpawnPlayer(1);
    }

    public void OnMageChosen()
    {
        SpawnPlayer(2);
    }

    private void SpawnPlayer(int infoIndex)
    {
        //Instantiate Player
        CharacterInfoSO characterInfo = playerInfos[infoIndex];
        GameObject player = Instantiate(characterInfo.prefab, spawnPoint.position, Quaternion.identity, playerGameObject);
        GameObject hpBar = Instantiate(healthBar, healthBarCanvas.transform);
        WeaponManager.Instance.AddWeapon(characterInfo.startWeapon);
        hpBar.GetComponent<Follow>().SetTarget(player);

        //Set up stats
        PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
        playerInfo.InitializeFromCharacterInfoSO(characterInfo);

        //Set up in inspector
        drivenCamera.AnimatedTarget = player.GetComponent<Animator>();
        runCamera.Follow = player.transform;
        idleCamera.Follow = player.transform;
        this.gameObject.SetActive(false);
        Spawner.Instance.gameObject.SetActive(true);
        AugmentManager.Instance.gameObject.SetActive(true);
        PlayerLevelManager.Instance.gameObject.SetActive(true);
        HealthBarCanvas.Instance.gameObject.SetActive(true);
    }
}
