using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLavaController : MonoBehaviour
{
    private HashSet<Material> materials = new HashSet<Material>();

    public void Burn(float radius)
    {
        foreach(Material mat in materials)
        {
            if (mat.HasFloat("LavaRadius")) {
                mat.SetFloat("LavaRadius", radius);
            }
        }
    }

    public void ObstacleSpawned(GameObject obstacle)
    {
        foreach (MeshRenderer mesh in obstacle.GetComponentsInChildren<MeshRenderer>())
        {
            materials.Add(mesh.material);
        }
    }
}
