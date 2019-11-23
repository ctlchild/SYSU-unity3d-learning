using UnityEngine;
using UnityEngine.UI;

public class myGUI : MonoBehaviour
{
    public float health = .5f;
    private float resultHealth;
    public Slider healthSlider;

    private Rect HealthBar;
    private Rect HealthUp;
    private Rect HealthDown;

    void Start()
    {
        HealthBar = new Rect(50, 50, 200, 20);
        HealthUp = new Rect(100, 80, 40, 20);
        HealthDown = new Rect(160, 80, 40, 20);
        resultHealth = health;
    }

    void OnGUI()
    {
        if (GUI.Button(HealthUp, "+"))
            resultHealth = resultHealth + 0.1f > 1.0f ? 1.0f : resultHealth + 0.1f;
        if (GUI.Button(HealthDown, "-"))
            resultHealth = resultHealth - 0.1f < 0.0f ? 0.0f : resultHealth - 0.1f;
        
        healthSlider.value = health = Mathf.Lerp(health, resultHealth, 0.05f);
        
        GUI.HorizontalScrollbar(HealthBar, 0.0f, health, 0.0f, 1.0f);
    }
}