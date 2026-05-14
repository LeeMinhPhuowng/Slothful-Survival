using UnityEngine;
using DG.Tweening;
using TMPro;

public class LevelUpVFX : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject levelUpTextPrefab; // Prefab with TextMeshPro
    [SerializeField] private GameObject auraPrefab;        // Prefab with ParticleSystem
    
    public void PlayVFX()
    {
        // 1. Spawn and Play Aura
        if (auraPrefab != null)
        {
            // Spawn at player's position, parented to player so it follows
            GameObject aura = Instantiate(auraPrefab, transform.position, Quaternion.identity, transform);
            
            // Get ParticleSystem and play it
            ParticleSystem ps = aura.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                // Destroy the aura object after its duration
                Destroy(aura, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                // Fallback if no PS found
                Destroy(aura, 2f);
            }
        }

        // 2. Spawn floating text
        if (levelUpTextPrefab != null)
        {
            Vector3 startPos = transform.position; 
            GameObject textGo = Instantiate(levelUpTextPrefab, startPos, Quaternion.identity);
            
            // Use a Sequence to manage both animations and ensure destruction even if game pauses
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true); // This is key! It makes the sequence ignore Time.timeScale = 0
            
            seq.Join(textGo.transform.DOMoveY(startPos.y + 2.5f, 1f).SetEase(Ease.OutSine));
            seq.Join(textGo.GetComponent<TextMeshPro>().DOFade(0, 1f));
            
            seq.OnComplete(() => {
                if (textGo != null) Destroy(textGo);
            });
        }
    }
}
