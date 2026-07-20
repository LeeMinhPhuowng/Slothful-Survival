using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Experimental.Animations;
using Game.UI.Data;
using Game.UI.Service;
using Reflex.Attributes;

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
    // [SerializeField] Animator mainMenuPlayerAnimator;
    //Game Scene
    [SerializeField] GameObject gameScene;

    // private bool isKnight = true;
    // private bool isArcher = false;
    // private bool isMage = false;
    private int playerIndex;
    private GameObject _spawnedPlayer;
    public static PlayerSetter instance;

    [Inject] private readonly IInventoryService _inventoryService;

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

    private void Start()
    {
        SpawnPlayer();
    }
    public void OnKnightChosen()
    {
        playerIndex = 0;
        // mainMenuPlayerAnimator.Play("KnightIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    public void OnArcherChosen()
    {
        playerIndex = 1;
        // isKnight = false;
        // isMage = false;
        // isArcher = true;
        // mainMenuPlayerAnimator.Play("ArcherIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    public void OnMageChosen()
    {
        playerIndex = 2;
        // isKnight = false;
        // isMage = true;
        // isArcher = false;
        // mainMenuPlayerAnimator.Play("MageIdle");
        this.gameObject.SetActive(false);
        gameScene.SetActive(false);
    }

    private void SpawnPlayer()
    {
        //Instantiate Player
        CharacterInfoSO characterInfo = GameplayLaunchContext.CharacterConfig != null
            ? GameplayLaunchContext.CharacterConfig
            : playerInfos[playerIndex];

        if (characterInfo == null)
        {
            Debug.LogError("[PlayerSetter] Cannot spawn player because CharacterInfoSO is missing.");
            return;
        }

        // Use the Home room center if MapView calculated it, otherwise fallback to spawnPoint
        Vector3 playerSpawnPos = spawnPoint.position;
        if (MapView.HomeRoomSpawnPosition.HasValue)
        {
            playerSpawnPos = MapView.HomeRoomSpawnPosition.Value;
            Debug.Log($"[PlayerSetter] Spawning player at HomeRoom center: {playerSpawnPos}");
        }

        GameObject player = Instantiate(characterInfo.prefab, playerSpawnPos, Quaternion.identity, playerGameObject);
        _spawnedPlayer = player;
        GameObject hpBar = Instantiate(healthBar, healthBarCanvas.transform);
        
        if (Features.Inventory.GameplayInventoryBridge.Instance != null)
        {
            Features.Inventory.GameplayInventoryBridge.Instance.AddWeaponToActiveBag(characterInfo.startWeapon);
        }
        else
        {
            WeaponManager.Instance.AddWeapon(characterInfo.startWeapon);
        }

        hpBar.GetComponent<Follow>().SetTarget(player);

        //Set up stats
        PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
        playerInfo.InitializeFromCharacterInfoSO(characterInfo);

        if (_inventoryService != null)
        {
            foreach (var kvp in _inventoryService.EquippedItems)
            {
                var item = kvp.Value;
                if (item != null)
                {
                    playerInfo.MaxHealth += item.MaxHealth;
                    playerInfo.CurrentHealth = playerInfo.MaxHealth; // Reset health to new max
                    playerInfo.MoveSpeed += item.MoveSpeed;
                    playerInfo.Armor += item.Armor;
                    playerInfo.BonusAttack += item.Damage;
                }
            }
        }

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

        // Also teleport the player if already spawned
        if (_spawnedPlayer != null)
        {
            _spawnedPlayer.transform.position = position;
        }
    }
}
