using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI staminaText = default;

    private void OnEnable()
    {
        FirstPersonController.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        FirstPersonController.OnStaminaChange -= UpdateStamina;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateStamina(100);
    }

    private void UpdateStamina(float currentStamina)
    {
        staminaText.text = currentStamina.ToString("00");
    }
}
