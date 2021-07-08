using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeController : MonoBehaviour
{
    public GameObject[] explosives;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Target>().health = 0;
            //GetComponent<Destructible>().ChangeToDestructible();
            foreach (GameObject explosive in explosives)
            {
                explosive.GetComponent<ThrowableItemController>().Throw(Vector3.zero);
            }
        }
    }
}
