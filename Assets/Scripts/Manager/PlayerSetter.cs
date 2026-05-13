using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Experimental.Animations;
public class PlayerSetter : MonoBehaviour
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
    //Main Menu Player Anim
    [SerializeField] Animator mainMenuPlayerAnimator;
    //Game Scene
    [SerializeField] GameObject gameScene;

    private bool isKnight = true;
    private bool isArcher = false;
    private bool isMage = false;
    private int playerIndex;
    public static PlayerSetter instance;

    private void Awake()
    {
        instance = this;
    }
    /*
    private void Start()
    {
        this.gameObject.SetActive(true);
        AugmentManager.Instance.gameObject.SetActive(false);
        PlayerLevelManager.Instance.gameObject.SetActive(false);
        HealthBarCanvas.Instance.gameObject.SetActive(false);
        Spawner.Instance.gameObject.SetActive(false);
    }
    */
    public void OnKnightChosen()
    {
        playerIndex = 0;
        mainMenuPlayerAnimator.Play("KnightIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    public void OnArcherChosen()
    {
        playerIndex = 1;
        isKnight = false;
        isMage = false;
        isArcher = true;
        mainMenuPlayerAnimator.Play("ArcherIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    public void OnMageChosen()
    {
        playerIndex = 2;
        isKnight = false;
        isMage = true;
        isArcher = false;
        mainMenuPlayerAnimator.Play("MageIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    private void SpawnPlayer()
    {
        //Instantiate Player
        CharacterInfoSO characterInfo = playerInfos[playerIndex];
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
    }

    public void OnPlayButtonClicked()
    {
        SpawnPlayer();
    }

    public void SetSpawnPosition(Vector3 position)
    {
        if (spawnPoint != null)
        {
            spawnPoint.position = position;
        }
    }
}
