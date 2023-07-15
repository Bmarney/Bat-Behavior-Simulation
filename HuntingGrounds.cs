using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGrounds : MonoBehaviour
{

    public GameObject prefab;

    public float radius;

    public int spawnNumber;

    private HuntingGrounds selfHunt;

    private Insectoid[] insecties;
    private Insectoid bob;
    public Vector3 windVelocity = Vector3.zero;
    public float maxWindSpeed = 0.5f;

    public int lowerSpawnNumber;
    public int upperSpawnNumber;
    public bool needsReset = true;
    public int maxBatNumber;
    public List<Insectoid> localPrey = new List<Insectoid>();

    public void Spawn(int spawnNum)
    {
        for (int i = 0; i < spawnNum; i++)
        {
            this.spawnNumber = spawnNum;
            //spawn inside the semisphere 
            //need to keep them inside the semishere!
            Vector3 temp = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.5f, 5.0f), Random.Range(-5.0f, 5.0f));
            GameObject clone = Instantiate<GameObject>(prefab, this.transform.position + temp, Random.rotation);
            Insectoid myinsect = clone.GetComponent<Insectoid>();
            myinsect.thisHunt = selfHunt;
            AddPrey(myinsect);
            needsReset = true;
        }
    }

    //overloaded Spawn function
    public void Spawn()
    {

        spawnNumber = Random.Range(lowerSpawnNumber, upperSpawnNumber);
        for (int i = 0; i < spawnNumber; i++)
        {
            //spawn inside the semisphere 
            //need to keep them inside the semishere!
            

            Vector3 temp = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.5f, 5.0f), Random.Range(-5.0f, 5.0f));
            //Instantiate(prefab, this.transform.position + temp, Random.rotation); // can I spawn it and give it the location of this hunting ground to keep the insects in??

            GameObject clone = Instantiate<GameObject>(prefab, this.transform.position + temp, Random.rotation);
            Insectoid myinsect = clone.GetComponent<Insectoid>();
            myinsect.thisHunt = selfHunt;
            AddPrey(myinsect);
            //object you want to instantiate is null
            //Insectoid clone = Instantiate(bob, this.transform.position + temp, Random.rotation);
            //clone.spawnLocation = this.transform.position;
            //Debug.Log(clone.spawnLocation);
            needsReset = true;
        }

        /*
        foreach (Insectoid target in localPrey)
        {
            Debug.Log(target);
        }*/

    }

    

    public void ResetThings()
    {
        if (localPrey.Count != 0)
        {
            /*foreach (Insectoid bug in localPrey)
            {
                
                {
                    localPrey.Remove(bug);
                    Destroy(bug.gameObject);
                }
                //RemovePrey(bug);
            }*/
            for (int i = 0; i < localPrey.Count; i++)
            {
                Destroy(localPrey[i].gameObject);
                localPrey.Remove(localPrey[i]);

            }
        }
    }

    public void AddPrey(Insectoid prey)
    {
        //Debug.Log(prey);
        localPrey.Add(prey);
    }

    public void RemovePrey(Insectoid prey)
    {
        localPrey.Remove(prey);
    }
    // Start is called before the first frame update
    void Start()
    {
        selfHunt = GetComponent<HuntingGrounds>();
    }

    public bool full = false;
    public int batCount = 0;

    public void Add()
    {
        if (batCount == 0)
        {
            Spawn();
        }
        batCount++;
    }

    public void Subract() {
        if (batCount >= 0)
        {
            batCount--;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (batCount >= maxBatNumber)
        {
            full = true;
        }
        else {
            full = false;
        }

        if (batCount<= 0 )
        {
            ResetThings();
        }

        if (windVelocity.magnitude > maxWindSpeed)
        {
            windVelocity = windVelocity.normalized * maxWindSpeed;
        }
        if (windVelocity.magnitude !=0)
        {
            this.transform.position += windVelocity * Time.deltaTime;

        }
    }
}
