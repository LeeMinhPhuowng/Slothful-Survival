using UnityEngine;

public class Follow : MonoBehaviour
{
    GameObject target;
    public Vector3 offset = new Vector3(0, 0, 0);
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if(target == null || rectTransform == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + offset);
        rectTransform.position = screenPos;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
