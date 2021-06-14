using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    //private GameObject mainUIRef;
        
    public void TakeDamage(float amount)
    {
        health -= amount;
    }
    //private void Start()
    //{
    //    mainUIRef = GameObject.FindGameObjectWithTag("MainUI");
    //}

    // Update is called once per frame
    void Update()
    {
        //mainUIRef.transform.Find("HPScrollbar").gameObject.GetComponent<Scrollbar>().value = health / maxHealth;
        //mainUIRef.transform.Find("HPScrollbar").gameObject.GetComponent<Image>().color = Color.Lerp(Color.black, Color.red, health / maxHealth);
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0f)
        {
            Destroy(gameObject, 0.02f);
        }
    }
}
