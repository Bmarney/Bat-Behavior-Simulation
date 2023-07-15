using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeRoost : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject prefab;

    public float radius;

    public int number;
    HomeRoost myHome;



    void Start()
    {
        myHome = GetComponent<HomeRoost>();
        for (int i = 0; i <number; i++)
        {
            //Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation);
            Vector3 spawn = transform.position + Random.insideUnitSphere * radius;
            GameObject clone = Instantiate<GameObject>(prefab, spawn, Random.rotation);
            Boid myBat = clone.GetComponent<Boid>();
            myBat.homeRoostLocation = transform.position;
            myBat.myRoost = spawn;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
