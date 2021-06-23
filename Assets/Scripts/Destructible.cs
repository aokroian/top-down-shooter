using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;

    private Dictionary<string, Vector3> partsPos = new Dictionary<string, Vector3>()
    {

    };
    private Dictionary<string, Quaternion> partsRot = new Dictionary<string, Quaternion>()
    {
    };

    private void Start()
    {

    }
    public void ChangeToDestructible()
    {

        GameObject destructible = Instantiate(destructiblePrefab, transform.position, transform.rotation);

        //// writing source parts
        //foreach (Transform child in transform)
        //{
        //    if (child.name == "metarig") continue;

        //    Debug.Log(child.name);

        //    Vector3 pos = child.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.localPosition;
        //    Quaternion rot = child.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.rotation;
        //    if (pos != null)
        //    {
        //        partsPos.Add(child.name, transform.TransformPoint(pos));
        //        partsRot.Add(child.name, rot);
        //    }

        //}
        //// writing part pos and rot
        //foreach (Transform child in destructible.transform)
        //{
        //    child.position = partsPos[child.name];
        //    child.rotation = partsRot[child.name];

        //    child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //}
        Destroy(gameObject);


    }


}
