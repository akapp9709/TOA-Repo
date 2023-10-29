using AJK;
using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    private CanvasRenderer canvasRenderer;
    [SerializeField] private Material shader;
    private PlayerManager _manager;

    public float health, stamina;

    private void Start()
    {
        PF_Generator.Singleton.OnComplete += InitilaizeUI;
        // Color mainCol = playerMaterial.GetColor("_Main_Color");
        // Color secondaryCol = playerMaterial.GetColor("_Secondary_Color");
    }

    private void InitilaizeUI()
    {
        _manager = PlayerManager.Singleton;
        UpdateShader();
        _manager.StateChange = UpdateShader;
    }

    private void UpdateShader()
    {
        health = _manager.currentHealth;
        stamina = _manager.currentStamina;
        shader.SetFloat("_Health_Value", (_manager.currentHealth / _manager.maxHealth) * 100);
        shader.SetFloat("_Stamina_Value", (_manager.currentStamina / _manager.maxStamina) * 100);
    }
}