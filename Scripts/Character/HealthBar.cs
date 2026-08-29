using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("サイズ設定")]
    [Range(0.1f, 5.0f)]
    [SerializeField] private float x_scale = 1.0f;
    [Range(0.1f, 5.0f)]
    [SerializeField] private float y_scale = 1.0f;

    [SerializeField] private Transform healthBarRoot; 
    [SerializeField] private SpriteRenderer HP_bar;
    [SerializeField] private SpriteRenderer HP_bg;

    private Quaternion defaultRotation;

    public void Init()
    {
        //healthBarRoot.forward = Camera.main.transform.forward;
        defaultRotation = Camera.main.transform.rotation;
        UpdateRotation();
        healthBarRoot.localScale = new Vector3(x_scale, y_scale, 1.0f);
    }

    public void UpdateHealthBar(float ratio)
    {
        if(ratio < 0) { ratio = 0.0f; }
        HP_bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }

    public void UpdateRotation()
    {
        healthBarRoot.rotation = defaultRotation; // fixed angle matching camera
    }
}
