using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MeshStatsTool
{
#if UNITY_EDITOR
    /** 
    <summary> Adds a menu item to the Right Click Menu when 
    cursor is inside the project Tab. </summary>
    */
    [MenuItem("Assets/Mesh Statistics")]
    public static void GetStats()
    {
        var arr = Selection.objects;
        int totVerts = 0, totalTris = 0;
        foreach (var obj in arr)
        {
            // Checks if current object is a mesh.
            if (obj.GetType() == typeof(Mesh))
            {
                var mesh = obj as Mesh;
                int tris = mesh.triangles.Length / 3;
                int verts = mesh.vertices.Length;
                // Line output for individual objects.
                Debug.Log($"{obj.name} - Verts: {verts}"
                                     + $", Tris: {tris}");
                // Summation of vertices and triangles.
                totVerts += verts;
                totalTris += tris;
            }
        }
        // Line output for totals.
        Debug.Log($"Totals - Verts: {totVerts}"
                     + $", Tris: {totalTris}");
    }

    /**
    <summary>Disables the above menu item if there is less than
    1 item selected as well as if the selected items contains
    mesh objects.</summary>
    */
    [MenuItem("Assets/Mesh Statistics", true)]
    private static bool SelectionValid()
    {
        bool val = false;
        var arr = Selection.objects;
        /* 
        Checks if 1 or more objects are selected and at least
        one is of the correct type.
        */
        if (arr.Length >= 1 && CorrectType(arr))
        {
            val = true;
        }

        return val;
    }

    /**
    <summary>Returns true if the list of objects contains 1 or
    more mesh objects. </summary>
    */
    private static bool CorrectType(Object[] objects)
    {
        foreach (var obj in objects)
        {
            // Returns true if a Mesh object is found.
            if (obj.GetType() == typeof(Mesh))
            {
                return true;
            }
        }

        return false;
    }
#endif
}
