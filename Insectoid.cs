using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Insectoid : MonoBehaviour
{

    public Vector3 velocity;
    public float maxVelocity = 1.0f;

    private Insectoid insect;

    //spawn location
    public Vector3 spawnLocation;

    //GLobal Direction Force
    public float globalForce = 1.0f;
    public Vector3 globalDirection = Vector3.zero;
    //Flocking Variables
    public bool participateInFlocking = true;
    //avoidance variables
    public float avoidRadius;
    public float repulsionForce;
    //Alignment variables
    public float alignRadius;
    public float alignForce;
    //Cohesion variables
    public float cohesionForce;
    public float cohesionRadius;
    /*
    //Energy variables
    public bool inFlight = true;
    public float energy = 100.0f;
    public float flightConsumptionRate = 0.01f;
    public float energyConsumptionRate = 0.01f;
    public float roostConsumptionRate = 0.0f;
    */
    //hunting ground variable
    //Hunting Variables
    public float closestHuntDistance = -1.0f;
    public Vector3 closestHuntLocation;
    //hunting grounds locations
    public HuntingGrounds[] huntGrounds;
    public HuntingGrounds thisHunt;

    //variable to show bat is participating in flocking

    public Insectoid[] allInsects;


    //roost locations
    public TemporaryRoosts[] tempRoosts;
    public Vector3[] allRoosts;

    //hunting grounds locations
    //public HuntingGrounds[] huntGrounds;


    // Start is called before the first frame update
    void Start()
    {
        participateInFlocking = true; // start with flocking
        //inFlight = true;// start in flight
        velocity = this.transform.forward * maxVelocity;
        insect = GetComponent<Insectoid>();
        allInsects = FindObjectsOfType<Insectoid>();
        //globalDirection = thisHunt.transform.position - insect.transform.position;

        //Debug.Log(thisHunt);
        
        //FindNearestHunting();
        //tempRoosts = FindObjectsOfType<TemporaryRoosts>();
        //huntGrounds = FindObjectsOfType<HuntingGrounds>();
        //int i = 0;
        //foreach(var roosties in tempRoosts)
        //{
        //allRoosts.Add(roostie.transform.position;)
        //Debug.Log(roosties.transform.position); //this gets what I want!!!
        //Debug.Log(roosties.roostLocation());

        //i++;

        //}
        //foreach (var hunties in huntGrounds)
        //{
        //    Debug.Log(hunties.transform.position);
        //    hunties.Add();
        //}

    }


    


    void GlobalBehavior()
    {

        /*if (thisHunt.windVelocity.magnitude>thisHunt.maxWindSpeed)
        {
            globalDirection = thisHunt.transform.position + thisHunt.windVelocity.normalized*thisHunt.maxWindSpeed - insect.transform.position;
        }
        else
        {
            globalDirection = thisHunt.transform.position + thisHunt.windVelocity - insect.transform.position;
        }*/
        globalDirection = thisHunt.transform.position - insect.transform.position;
        insect.velocity += globalDirection * globalForce * Time.deltaTime;
        //insect.velocity += Vector3.Lerp(Vector3.zero, globalDirection, Time.deltaTime) * globalForce;
    }

    void BoidAvoidance()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        // ex octrees etc

        var average = Vector3.zero;
        var found = 0;

        foreach (var one in thisHunt.localPrey)
        {
            var diff = one.transform.position - this.transform.position;
            if (diff.magnitude < avoidRadius && diff.magnitude != 0)
            {
                average += diff;
                found += 1;
            }
        }

        if (found > 0)
        {
            average = average / found;
            insect.velocity -= Vector3.Lerp(Vector3.zero, average, average.magnitude / avoidRadius) * repulsionForce;
        }

    }

    /*
    void FindNearestHunting()
    {
        foreach (var hunties in huntGrounds)
        {
            Vector3 temp = hunties.transform.position - insect.transform.position;
            if (temp.magnitude < closestHuntDistance || closestHuntDistance == -1.0f)
            {
                closestHuntDistance = temp.magnitude;
                closestHuntLocation = hunties.transform.position;
                thisHunt = hunties;
                //allRoosts.Add(roosties.transform.position);
                //Debug.Log(closestHuntDistance);
                //Debug.Log(closestHuntLocation);
            }


            //hunties.Add();
        }
        //globalDirection = closestHuntLocation;
    }
    */
    void BoidAlignment()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        // ex octrees etc


        var average = Vector3.zero;
        var found = 0;

        foreach (var one in thisHunt.localPrey)
        {
            var diff = one.transform.position - this.transform.position;
            if (diff.magnitude < alignRadius && diff.magnitude != 0)
            {
                average += one.velocity;
                found += 1;
            }
        }

        if (found > 0)
        {
            average = average / found;
            insect.velocity += Vector3.Lerp(insect.velocity, average, Time.deltaTime);
        }

    }

    void BoidCohesion()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        // ex octrees etc

        var average = Vector3.zero;
        var found = 0;

        foreach (var one in thisHunt.localPrey)
        {
            var diff = one.transform.position - this.transform.position;
            if (diff.magnitude < cohesionRadius && diff.magnitude != 0)
            {
                average += diff;
                found += 1;
            }
        }

        if (found > 0)
        {
            average = average / found;
            insect.velocity += Vector3.Lerp(Vector3.zero, average, average.magnitude / cohesionRadius);
        }

    }




    // want to implement

    //1. Bug hunting and eating
    //1.1) Find Nearest Hunting Ground and set global direction vector toward it
    //1.2) detect nearby insects within a certain range
    //1.3) hunt those insects

    // can be done through cone of detection or raycasting methods
    // what math will help determine position within a forward looking direction? 
    // vector direction towards prey relative to heading must be within a 50degree area of observation
    // need to split off from flock and regroup later

    //2. energy storage and rules for when to hunt and roost
    //2.1) find nearest roost and set global direction vector to it
    //2.2) enter roost area and stop moving. turn energy expenditure to minimum and turn flying off
    //energy variable that detracts over time based on flight and gets increased when insects are captured
    //how will the bats roost? will they fly to a roost area or just stay still 

    //3. states for flocking
    // night time using echolocation
    // light time using vision
    // when to disperse and when to regroup
    public float boundaryRadius = 10.0f;
    void BoundaryRepel()
    {
        
        if ((insect.transform.position - closestHuntLocation).magnitude > boundaryRadius)
        {

            //Debug.Log(closestHuntLocation);
            insect.velocity += Vector3.Lerp(insect.transform.position, closestHuntLocation,Time.deltaTime);//close
        }
    }

    void Disperse()
    {
        // remove from cohesion and alignment of other bats
        this.alignForce = 0;
        this.cohesionForce = 0;
        //enter hunt stage
    }

    void Hunt()
    {
        //search for insects within radius of detecion 
        participateInFlocking = false;
        //if magnitude towards hunting ground is less than threshold and hunting ground is not full break off and head towards hunting ground 
        //addBat to that hunting ground

        //make sure rest of flock readjusts to next nearest hunting ground!

        //while (bug is detected)
        //find their flight vector
        //find the best path to align with their flightvector
        //overtake and eat bug
        //add to energy reserve
    }





    void FixedUpdate()
    {
        //BoundaryRepel();

        BoidAvoidance();
        BoidAlignment();
        BoidCohesion();
        GlobalBehavior();

        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }

        

        this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);
    }
}
