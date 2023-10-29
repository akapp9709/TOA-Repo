using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.GameCenter;

public class SpecialAttackHandler : MonoBehaviour
{
    private InputHandler _input;
    private SpecialAttackAnimHandler _animHandler;
    [HideInInspector] public List<Transform> enemiesInRange = new List<Transform>();
    [HideInInspector] public Vector3 centerPos;

    public float range = 5f;
    public int maxEnemies = 3;

    // Start is called before the first frame update
    void Start()
    {
        _animHandler = GetComponentInChildren<SpecialAttackAnimHandler>();

        var controls = new PlayerControls();
        controls.PlayerActions.Enable();
        controls.PlayerActions.SpecialAttack.started += InitiateSpecialAttack;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitiateSpecialAttack(InputAction.CallbackContext context)
    {
        var list = GetEnemiesInRange();
        enemiesInRange = SelectRandomEnemies(list);
        centerPos = GetCentrePosition(enemiesInRange);
        _animHandler.StartSpecialAttack();

        foreach (var enemy in enemiesInRange)
        {
            Debug.DrawLine(transform.position + Vector3.up, enemy.position + Vector3.up, Color.red, 3f);
        }

    }

    private List<Transform> GetEnemiesInRange()
    {
        List<Transform> enemies = new List<Transform>();
        var arr = Physics.OverlapSphere(transform.position, range);

        foreach (var obj in arr)
        {
            if (obj.CompareTag("Enemy"))
            {
                enemies.Add(obj.transform);
            }
        }

        return enemies;
    }

    private List<Transform> SelectRandomEnemies(List<Transform> startList)
    {
        List<Transform> selectedEnemies = new List<Transform>();

        while (selectedEnemies.Count < maxEnemies && startList.Count > 0)
        {
            int randomIndex = Random.Range(0, startList.Count);
            var selectedEnemy = startList[randomIndex];
            selectedEnemies.Add(selectedEnemy);
            startList.RemoveAt(randomIndex);
        }

        return selectedEnemies;
    }

    private Vector3 GetCentrePosition(List<Transform> list)
    {
        Vector3 pos = Vector3.zero;
        var totalPos = new Vector3();

        foreach (var trans in list)
        {
            totalPos += trans.position;
        }

        pos = totalPos / (float)list.Count;
        return pos;
    }
}
