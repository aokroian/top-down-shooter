using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDestroyer : MonoBehaviour
{

    public List<GameObject> additionalEnvironments;
    public float destroyDistance = 50f;
    public GameObject player;

    public HashSet<GameObject> environments = new HashSet<GameObject>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            environments.Add(transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in additionalEnvironments)
        {
            environments.Add(obj);
        }

        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        List<GameObject> forDelete = new List<GameObject>();
        while (environments.Count > 0)
        {
            foreach (GameObject obj in environments)
            {
                if (Vector3.Distance(obj.transform.position, player.transform.position) > destroyDistance)
                {
                    forDelete.Add(obj);
                    GameObject.Destroy(obj);
                }
                yield return null;
            }
            foreach (GameObject del in forDelete)
            {
                environments.Remove(del);
            }
            forDelete.Clear();
        }
        gameObject.SetActive(false);
        Debug.Log("Deactivated");
    }
}
