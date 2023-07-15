using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{

    public Vector3 velocity;
    public float maxVelocity;

    private Boid boid;

    

    
    //GLobal Direction Force
    public float globalForce = 1.0f;
    public Vector3 globalDirection = Vector3.zero;
    public Vector3 goalPoint = Vector3.zero;
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

    //Roost Variables
    public Vector3 myRoost; 
    public Vector3 homeRoostLocation;
    public float closestRoostDistance = -1.0f;
    public Vector3 closestRoostLocation;
    public bool isRoosting;
    public bool foundRoost = false;
    public bool tempRest = false;

    //Hunting Variables
    public float closestHuntDistance = -1.0f;
    public Vector3 closestHuntLocation;
    public bool foundHunt = false;

    //Insect and Prey Variables
    public Insectoid prey;
    public float minPreyDistance = -1;
    public List<Insectoid> myPrey = new List<Insectoid>();
    public Vector3 tempShit;
    public float minCatchDistance = 0.1f;
    public float circleAroundDistance = 10.0f;
    public bool targetLocked = false;

    //Time vairables
    public float timer = 0.0f;
    public float nightLength = 20.0f;
    public float dayLength = 10.0f;
    public int numberOfDays = 1;    
    public float lastMealTime;
    public float roostTime = 15;
    public float returnHuntingTime;
    public int cooldown = 0;

    //variable to show bat is participating in flocking

    public Boid[] boids;
    
    //Energy variables
    public bool inFlight = true;
    public float energyLevel = 1500.0f;
    public float huntThreshold = 1500.0f;
    public float flightConsumptionMultiplier = 0.75f; // flight multiplier
    public float energyConsumptionRate = 1.0f; // per second cost of living
    public float roostConsumptionRate = 0.0f;
    public int numberBugsEatenToday = 0;
    public float fullEnergyThreshold = 2000.0f;
    public bool isFull;
    public float noCatchTimeThreshold = 100.0f;
    public float minEnergyThreshold = 300;
    /// <summary>
    /// THING I NEED TO DO
    /// ENERGETICS MODEL AND BEHAVIOURS BASED ON ENERGy
    /// STATE MACHINE INSTEAD OF IF STATEMENTS IF POSSIBLE

    /// </summary>

    //roost locations
    public TemporaryRoosts[] tempRoosts;
    public TemporaryRoosts targetRoost;
    public Vector3[] allRoosts;
    public bool returnHome = false;

    //hunting grounds locations
    public HuntingGrounds[] huntGrounds;
    public HuntingGrounds targetHunt;
    public HuntingGrounds myHunt;
    public bool globalReset = true;
    public bool isHunting = false;
    public bool isSearching = false;
    

    // Start is called before the first frame update
    void Start()
    {
        participateInFlocking = true; // start with flocking
        inFlight = true;// start in flight
        velocity = this.transform.forward * maxVelocity;
        boid = GetComponent<Boid>();
        boids = FindObjectsOfType<Boid>();
        //prey = GetComponent<Insectoid>();
        tempRoosts = FindObjectsOfType<TemporaryRoosts>();
        huntGrounds = FindObjectsOfType<HuntingGrounds>();
        nightLength = 50.0f;
        dayLength = 25.0f;
        noCatchTimeThreshold = 20.0f;
        roostTime = 15;
        avoidRadius = 1.0f;
        repulsionForce = 1.75f;
        cohesionRadius = 5.0f;
        alignRadius = 5.0f;
        flightConsumptionMultiplier = .05f; // flight multiplier
        energyConsumptionRate = 1.0f;


}

    void FindNearestRoost()
    {
        closestRoostDistance = -1.0f;
        foreach (var roosties in tempRoosts)
        {
            Vector3 temp = roosties.transform.position - boid.transform.position;
            if (temp.magnitude < closestRoostDistance || closestRoostDistance == -1.0f)
            {
                this.closestRoostDistance = temp.magnitude;
                this.closestRoostLocation = roosties.transform.position;
                this.targetRoost = roosties;

                
            }

            

        }
        goalPoint = targetRoost.transform.position;
    }


   


    bool AngleCheck(Vector3 one, Vector3 two)
    {
        //Debug.Log(Mathf.Acos(Vector3.Dot(one, two)/(one.magnitude*two.magnitude)));
        if (Mathf.Acos(Vector3.Dot(one, two) / (one.magnitude * two.magnitude)) < 0.4363f)
        {
            return true;

        }
        else return false;
    }

    void VelocityReset()
    {
        boid.velocity = Vector3.zero;
    }

    public int i = 1;
    void HuntBehavior()
    {
        
        
        if (foundHunt == false)
        {
            FindNearestHunting();
            foundHunt = true;
        }
        
        if ((targetHunt.transform.position - boid.transform.position).magnitude < 20 && targetHunt.full != true && this.participateInFlocking == true)
        {
            this.participateInFlocking = false;
            myHunt = targetHunt;
            //currently ierelevant 
            //Disperse(); // set align and cohesion force multiplier to 0 
            if (isHunting == false && isFull == false)
            {
                targetHunt.Add();
            }
            
            isHunting = true;
            //globalReset = true;
        }
        else if(isHunting == false && targetHunt.full == true)// && globalReset == true)
        {
            
            FindNearestHunting();
           
        }
        
        /*if (myHunt.full == true)
        {
            foundHunt = false;
        }*/
    }


    //return a vector3 and set that to the goal point
    void FindNearestHunting()
    {
        
        //if (this.participateInFlocking == true)
        //{

        
        
            closestHuntDistance = -1.0f;
        
            foreach (var hunties in huntGrounds)
            {
            
                Vector3 temp = hunties.transform.position - boid.transform.position;
                if ((hunties.full == false) && (temp.magnitude < closestHuntDistance || closestHuntDistance == -1.0f))
                {

                    this.closestHuntDistance = temp.magnitude;
                    this.closestHuntLocation = hunties.transform.position;
                    this.targetHunt = hunties;
                    //allRoosts.Add(roosties.transform.position);
                    
                }
            }


        //hunties.Add();
        //}
        goalPoint = closestHuntLocation;
    }
    void GlobalBehavior()
    {

        if (startCondition == false)
        {
            boid.velocity += new Vector3(0, 1, 0);
        }
        if(isRoosting == false && isHunting == false && startCondition==true &&returnHome ==false)
        {
            globalDirection = targetHunt.transform.position - boid.transform.position;
        }
        else if (isRoosting == true && returnHome == false)
        {
            globalDirection = closestRoostLocation - boid.transform.position; // if roosts can move, this should be changed
        }
        if (returnHome == true)
        {
            globalDirection = homeRoostLocation - transform.position;
        }

        if (targetLocked == true)
        {
            globalDirection = (goalPoint - transform.position).normalized*maxVelocity;
        }

        if (targetLocked == false && isRoosting == false && participateInFlocking == false &&isFull == false)
        {
            globalDirection = (goalPoint - transform.position).normalized * maxVelocity;
        }
        //globalDirection = goalPoint - transform.position;

        if (startCondition == true)
        {
            boid.velocity += globalDirection * globalForce * Time.deltaTime;
        }
        
    }

    void BoidAvoidance()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        // ex octrees etc

        var average = Vector3.zero;
        var found = 0;
        //radius, layer bit shifted
        //returns array of colliders
        //Collider[] = Physics.OverlapSphere(boid.transform.position, 5.0f, 1 << 7);
        //Physics.OverlapSphere(boid.transform.position, 5.0f, 1 << 7);

        //this is much faster than n^2 checking from all boids
        foreach (var boidCollider in Physics.OverlapSphere(transform.position, avoidRadius, 1 << 7))
        {
            var diff = boidCollider.gameObject.transform.position - transform.position;
            //if (diff.magnitude < avoidRadius)
            //{
                average += diff;
                found += 1;
            //}
        }
        /*
        foreach (var boid in boids.Where(b => b != boid))
        {
            var diff = boid.transform.position - this.transform.position;
            if (diff.magnitude < avoidRadius)
            {
                average += diff;
                found += 1;
            }
        }
        */
        if (found > 0)
        {
            average /= found;
            boid.velocity -= Vector3.Lerp(Vector3.zero, average, average.magnitude / avoidRadius) * repulsionForce;
        }

    }

    void BoidAlignment()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        //ex octrees etc

        
        var average = Vector3.zero;
        var found = 0;

        foreach (var boidCollider in Physics.OverlapSphere(transform.position, alignRadius, 1 << 7))
        {
            //var diff = boidCollider.gameObject.transform.position - this.transform.position;
            //if (diff.magnitude < avoidRadius)
            //{
            average += boidCollider.gameObject.GetComponent<Boid>().velocity;
            found += 1;
            //}
        }
        /*
        foreach (var boid in boids.Where(b => b != boid))
        {
            var diff = boid.transform.position - this.transform.position;
            if (diff.magnitude < alignRadius)
            {
                average += boid.velocity;
                found += 1;
            }
        }
        */
        if (found > 0)
        {
            average /= found; //changed from average = average/found
            velocity += Vector3.Lerp(velocity, average, Time.deltaTime);
        }

    }

    void BoidCohesion()
    {
        //can use better data types to keep track of boids and should implement it with something else!!
        // ex octrees etc

        var average = Vector3.zero;
        var found = 0;
        /*
        foreach (var boid in boids.Where(b => b != boid))
        {
            var diff = boid.transform.position - this.transform.position;
            if (diff.magnitude < cohesionRadius && boid.participateInFlocking == true)
            {
                average += diff;
                found += 1;
            }
        }
        */
        foreach (var boidCollider in Physics.OverlapSphere(transform.position, cohesionRadius, 1 << 7))
        {
            var diff = boidCollider.gameObject.transform.position - transform.position;
            if (boidCollider.gameObject.GetComponent<Boid>().participateInFlocking == true)
            {
                average += diff;
                found += 1;
            }
        }


        if (found > 0)
        {
            average /= found;
            velocity += Vector3.Lerp(Vector3.zero, average, average.magnitude / cohesionRadius);
        }

    }

    //change things based on hunger levels or time
    //make them act with rules not scripting
    // give a bat a metabolic energy rate say 2000
    // 1 per second time.delta time
    // magnitude of instantateous speed affects the speed of energy loss +=0.5*speed
    //add 200 calories to the moths
    //when below a certain value say 300 have them fly towards a roost
    //keep track of time and have them try to return home by dawn


    


    

    //the goal is to model bat and prey interaction in a multi agent simulation
    //motivation can be spread of white nose in bats, getting it from prey or intersecting flocks
    //movement patterns
    //having a good bat model of hunting allows us to see feeding resourses, intercolony resources
    //demos go from simple to more complex stuff
    //multiple bats have different home colonies
    //when one bat has white nose and it gets close enought to any other bat then it transfers the white

    // want to implement



    //2. energy storage and rules for when to hunt and roost
    //2.1) find nearest roost and set global direction vector to it
    //2.2) enter roost area and stop moving. turn energy expenditure to minimum and turn flying off
    //energy variable that detracts over time based on flight and gets increased when insects are captured
    //how will the bats roost? will they fly to a roost area or just stay still 

    //3. states for flocking
    // night time using echolocation
    // light time using vision
    // when to disperse and when to regroup



    
    public bool rehunt = true;
    //When Bats get close enough have them split off up to the max amount, spawn in prey when first splits off, then reorient the group
    //currently not based on perception!!
    void Hunt()
    {
        cooldown--;
        foreach (Insectoid target in myHunt.localPrey)
        {

            
            tempShit = target.transform.position;
            
            //using target.transform.position doesnt work for this if statement for lord knows what reason!
            if ((minPreyDistance == -1 || (tempShit - boid.transform.position).magnitude < minPreyDistance) && AngleCheck(boid.velocity, target.transform.position - boid.transform.position) == true)
            {


                repulsionForce = 0.5f;
                minPreyDistance = (target.transform.position - boid.transform.position).magnitude;
                prey = target;
                

            }
            //if (AngleCheck(boid.velocity, target.transform.position - boid.transform.position) == false)
            //{
            //    minPreyDistance = -1;
            //}

            if(((boid.transform.position - myHunt.transform.position).magnitude > circleAroundDistance) && minPreyDistance ==-1 &&isFull == false)
            {
                goalPoint = myHunt.transform.position;
                velocity = myHunt.transform.position - transform.position;
                //globalDirection = myHunt.transform.position - boid.transform.position;
                //Debug.Log("Circling back");
            }


        }

        if (myHunt.localPrey.Count == 0)
        {
            //Debug.Log("we did it!!!");

            //here is where the booleans are needed for one behaviour to another
            /*if (false)// behaviours say go to roost
            {
                FindNearestRoost();
                isRoosting = true;
                participateInFlocking = true;
            }*/
            //FindNearestRoost();
            //isRoosting = true;
            if (true) // behaviours say hunting continues
            {
                repulsionForce = 1.0f;
                FindNearestHunting();
                isHunting = false;
                participateInFlocking = true;
            }
            
            
        }
        //AngleCheck(boid.velocity, prey.velocity);
        

        //this line is an important check to make sure that the insect prey wasnt already eaten by another bat
        if (prey == null)
        {
            minPreyDistance = -1;
        }
        // with the information about the velocity of the prey the Bat becomes a fucking sniper!
        if (minPreyDistance != -1)
        {
            if (minPreyDistance < 3.0f && cooldown <=0)
            {
                //this simulates the immediate approach phase where a bat zero's in on its target and catches it with incredible percision
                boid.velocity = (prey.transform.position - boid.transform.position).normalized * maxVelocity;
            }
            else
            {
                if (prey.velocity.magnitude > prey.maxVelocity)
                {
                    goalPoint = prey.transform.position + prey.velocity.normalized * prey.maxVelocity;
                }
                else
                {
                    goalPoint = prey.transform.position + prey.velocity;
                }
               
                //globalDirection = (prey.transform.position + prey.velocity - boid.transform.position).normalized * maxVelocity;

            }
            
        }

        if (minPreyDistance != -1)
        {
            targetLocked = true;
        }
        else
        {
            targetLocked = false;
        }
        
        if (minPreyDistance != -1 && (prey.transform.position - boid.transform.position).magnitude < minCatchDistance && cooldown <=0)
        {
            //Debug.Log("Catch!");
            //Destroy(prey);
            energyLevel += 200;
            lastMealTime = timer;
            numberBugsEatenToday++;
            myHunt.RemovePrey(prey);
            cooldown = 30;

            //uninstantiate()
            Destroy(prey.gameObject);
            minPreyDistance = -1;
        }

        /*
        //search for insects within radius of detecion 
        if ((closestHuntLocation - boid.transform.position).magnitude > targetHunt.transform.localScale.magnitude)
        {

            //Debug.Log(closestHuntLocation);
            boid.velocity = Vector3.zero;
            //boid.velocity += Vector3.Lerp(boid.transform.position, closestHuntLocation, Time.deltaTime);//close
        }
        */


        //if magnitude towards hunting ground is less than threshold and hunting ground is not full break off and head towards hunting ground 
        //addBat to that hunting ground

        //make sure rest of flock readjusts to next nearest hunting ground!

        //while (bug is detected)
        //find their flight vector
        //find the best path to align with their flightvector
        //overtake and eat bug
        //add to energy reserve
    }

    

    public bool newDay = false;

    public bool startCondition = false;

    void FixedUpdate()
    {
        //keep track of time
        //maybe dont do this for every boid but rather as its own class once ready for that
        timer += Time.deltaTime;


        
        BoidAvoidance();
        if (participateInFlocking == true)
        {
            BoidAlignment();
            BoidCohesion();
        }
        if (isRoosting == true)
        {

            if (foundRoost == false)
            {
                FindNearestRoost();
                foundRoost = true;
            }

        }

        //start off with moving upwards
        if (transform.position.y > 20 && startCondition == false)
        {
            isSearching = true;
            startCondition = true;
            FindNearestHunting();
            //repulsionForce = 5.0f;
            //cohesionForce = 0.5f;
            globalForce = 2.0f;

        }
       
        
        

       
        

        

        //DO SOMETHING IF NIGHT TIME IS OVER

        //number of days starts at 1
        if (timer > nightLength * (numberOfDays - 1) + dayLength * (numberOfDays - 1) && timer < nightLength * numberOfDays + dayLength * (numberOfDays - 1))
        {
            //start behaviours
            //newDay = false;
            
            if (tempRest == false && isFull == false)
            {
                isRoosting = false;
            }
            

            /*if (timer - lastMealTime > 20)
            {
                Debug.Log("HUNGRY");

                FindNearestRoost();
                isRoosting = true;
                participateInFlocking = true;

                if (true)
                {

                }

            }*/

            // do something if not catching any bugs
            if (timer - lastMealTime > noCatchTimeThreshold && (targetHunt.transform.position - boid.transform.position).magnitude > 20)
            {
                //Debug.Log("Hungry2222");
                if (tempRest == false)
                {
                    returnHuntingTime = timer + roostTime;
                    FindNearestRoost();
                    isRoosting = true;
                    tempRest = true;
                    participateInFlocking = true;
                    Debug.Log("Hungry");
                }
                
            }

            if (tempRest == true)
            {
                if (timer > returnHuntingTime)
                {
                    globalForce = 2.0f;
                    tempRest = false;
                    prey = null;
                    lastMealTime = timer;
                    minPreyDistance = -1;
                    myHunt = null;
                    targetHunt = null;
                    isHunting = false;
                    participateInFlocking = true;
                    FindNearestHunting();
                }
            }
            

            // do something if really low on energy
            if (energyLevel < minEnergyThreshold)
            {
                Debug.Log("dying");
            }

            if (energyLevel > fullEnergyThreshold)//&&isFull == false would save computation
            {
                if (isFull == false)
                {
                    targetHunt.batCount -= 1;
                }
                
                isFull = true;
                globalForce = 2.0f;
                FindNearestRoost();
                isRoosting = true;
                participateInFlocking = true;
            }

            if (isFull == true)
            {
                if (energyLevel < fullEnergyThreshold)
                {
                    isFull = false;
                    lastMealTime = timer;
                    isRoosting = false;
                    prey = null;
                    minPreyDistance = -1;
                    myHunt = null;
                    targetHunt = null;
                    isHunting = false;
                    participateInFlocking = true;
                    tempRest = false;
                    FindNearestHunting();
                }
            }

            if (isSearching == true)
            {
                HuntBehavior();
            }
            

            //SET RULES BASED ON ENERGY IN THIS NIGHT BEHAVIOUR STREAM

            //Debug.Log("nighttime");
            //FindNearestHunting();
            //isHunting = false;
            //participateInFlocking = true;


        }
        else if (timer > nightLength*numberOfDays + dayLength * (numberOfDays - 1) && timer < nightLength * numberOfDays + dayLength * numberOfDays)
        {

            if (targetHunt != null)
            {
                targetHunt.batCount -= 1;
            }
            globalForce = 5.0f;
            prey = null;
            minPreyDistance = -1;
            myHunt = null;
            targetHunt = null;
            isHunting = false;
            //reset all HUNTING GROUNDS!!!
            foreach (var hunties in huntGrounds)
            {
                
                hunties.batCount = 0;
                hunties.full = false;
                hunties.ResetThings();// however I do this I cant get rid of the exceptions from trying to delete multiple bugs at the same time

                /*if (hunties.localPrey.Count != 0)
                {
                    foreach (Insectoid bug in hunties.localPrey)
                    {
                        hunties.RemovePrey(bug);
                    }
                }*/
            }
            //Debug.Log("daytime");
            //FindNearestRoost();
            returnHome = true;
            isRoosting = true;
            participateInFlocking = true;
            
        }
        else
        {
            numberOfDays += 1;
            numberBugsEatenToday = 0;
            lastMealTime = timer;
            startCondition = false;
            returnHome = false;
            startCondition = false;
            FindNearestHunting();
        }

        GlobalBehavior();
        if (participateInFlocking == false)
        {
            Hunt();
        }


        // CONTROL BEHAVRIOURS BASED ON ENERGY LEVELS AND PERHAPS TIME SINCE LAST EATING!!



        //simTime += 1;
        if (velocity.magnitude > maxVelocity)
        { 
            velocity = velocity.normalized * maxVelocity;
        }

        //Debug.Log(Time.deltaTime * flightConsumptionMultiplier * velocity.magnitude * velocity.magnitude);

        //needs work!
        energyLevel -= (energyConsumptionRate  +  flightConsumptionMultiplier * velocity.magnitude * velocity.magnitude)*Time.deltaTime;

        if (energyLevel < 50.0f && participateInFlocking == true)
        {
            
            
        }

        //makes the bats sit still while roosting
        if (isRoosting == true &&returnHome==false)
        {
            if ((targetRoost.transform.position - boid.transform.position).magnitude < 4.0f)
            {
                VelocityReset();
            }
        }
        if (returnHome == true)
        {
            if ((myRoost - boid.transform.position).magnitude < 3.0f)
            {
                VelocityReset();
            }
        }
             
        transform.position += velocity * Time.deltaTime;
        

        if (boid.velocity.magnitude != 0)
        {
            this.transform.rotation = Quaternion.LookRotation(velocity);
        }
        
    }
}
