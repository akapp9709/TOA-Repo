using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class RadarChart : MonoBehaviour
{
    public static int minStatLevel = 1, maxStatLevel = 15;
    [SerializeField] private CanvasRenderer radarMeshRenderer;
    [SerializeField] private Material radarMaterial;

    [SerializeField] private PlayerSO playerStats;
    public bool destroyAfterLoad = true;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (destroyAfterLoad)
            PF_Generator.Singleton.OnComplete += ClearRadar;

        playerStats.StatChange += UpdateVisuals;
        UpdateVisuals();
    }

    private void ClearRadar()
    {
        Destroy(radarMeshRenderer.gameObject);
        radarMeshRenderer = null;
    }


    private void UpdateVisuals()
    {
        if (radarMeshRenderer == null)
            return;

        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[5];
        Vector2[] uv = new Vector2[5];
        int[] triangles = new int[3 * 4];

        float radarSize = 85;
        Vector3 attackVert = Vector3.right * radarSize * playerStats.strengthLevel / maxStatLevel;
        int attackIndex = 2;
        Vector3 healthVert = Vector3.up * radarSize * playerStats.healthLevel / maxStatLevel;
        int healthIndex = 1;
        Vector3 staminaVert = Vector3.down * radarSize * playerStats.staminaLevel / maxStatLevel;
        int staminaIndex = 3;
        Vector3 defenseVert = Vector3.left * radarSize * playerStats.defenseLevel / maxStatLevel;
        int defenseIndex = 4;

        verts[0] = Vector3.zero;
        verts[healthIndex] = healthVert;
        verts[attackIndex] = attackVert;
        verts[staminaIndex] = staminaVert;
        verts[defenseIndex] = defenseVert;

        triangles[0] = 0;
        triangles[1] = healthIndex;
        triangles[2] = attackIndex;

        triangles[3] = 0;
        triangles[4] = attackIndex;
        triangles[5] = staminaIndex;

        triangles[6] = 0;
        triangles[7] = staminaIndex;
        triangles[8] = defenseIndex;

        triangles[9] = 0;
        triangles[10] = defenseIndex;
        triangles[11] = healthIndex;


        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshRenderer.SetMesh(mesh);
        radarMeshRenderer.SetMaterial(radarMaterial, null);
    }

}
