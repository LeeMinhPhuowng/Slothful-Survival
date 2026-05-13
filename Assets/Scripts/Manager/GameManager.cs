using UnityEngine;
using TMPro;
using System.Drawing;

public class GameManager : MonoBehaviour
{
    [Header("Gameplay Components")]
    [SerializeField] PlayerSetter playerSetter;
    [SerializeField] PlayerLevelManager playerLevelManager;
    [SerializeField] Spawner spawner;
    [SerializeField] AugmentManager augmentManager;
    // [SerializeField] LevelLoader levelLoader;

    [Header("Game Sections")]
    [SerializeField] GameObject gameScene;
    [SerializeField] GameObject mainMenuScene;

    [Header("Analytics")]
    [SerializeField] TextMeshProUGUI enemyCount;
    [SerializeField] TextMeshProUGUI fps;

    public static int EnemyCount;

    float timer = 0;
    int fpsInt = 0;
    int frameCount = 0;

    private void OnEnable()
    {
        mainMenuScene.SetActive(true);
        gameScene.SetActive(false);

        playerSetter.gameObject.SetActive(false);
        playerLevelManager.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        augmentManager.gameObject.SetActive(false);
        // levelLoader.gameObject.SetActive(false);
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    private void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;
        if(timer >= 0.2f)
        {
            fpsInt = Mathf.RoundToInt(frameCount / timer);
            timer = 0f;
            frameCount = 0;
        }       
    }

    private void Tick()
    {
        enemyCount.text = EnemyCount.ToString();
        fps.text = fpsInt.ToString();
    }    

    public void OnMainMenuPlayButtonClicked()
    {
        mainMenuScene.SetActive(false);
        gameScene.SetActive(true);
        
        // levelLoader.gameObject.SetActive(true);
        playerSetter.gameObject.SetActive(true);
        augmentManager.gameObject.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        spawner.gameObject.SetActive(true);
        playerLevelManager.gameObject.SetActive(true);
        playerSetter.gameObject.SetActive(false);
        // levelLoader.OnPlayButtonClicked();
        playerSetter.OnPlayButtonClicked();
    }    
    

    public void OnPlayerButtonClicked()
    {
        gameScene.SetActive(true);
        playerSetter.gameObject.SetActive(true);
        //Chi co PlayerSetter duoc hoat dong
        playerLevelManager.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        augmentManager.gameObject.SetActive(false);
        // levelLoader.gameObject.SetActive(false);
    }

    public void OnCollectionButtonClicked()
    {

    }
    public void OnStoreButtonClicked()
    {

    }
    public void OnBackButtonClicked()
    {
        gameScene.SetActive(false);
        mainMenuScene.SetActive(true);
    }
}
