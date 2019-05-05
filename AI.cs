using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 1,0,0 goes right
 * -1, 0, 0 goes left
 * 0, 0, 1 goes up
 * 0, 0, -1 goes left
 * */
public class AI : MonoBehaviour
{

    public PlayerScript player;
    public PlayerScript opponent;
    TileScript[] theTiles;

    TileScript possibleTemp;    //possible tiles to go to
    TileScript possibleTemp2;
    private Vector3 sphereDirection;    
    private float playerDistance;   
    private float opponentDistance;
    private float xDifference;
    private float zDifference;
    private float tempDist;
    private float tempDist2;
    Vector3 oppPosition;
    Vector3 playPosition;
    Vector3 ballVelocity;
    float oppX, oppZ, playX, playZ;
    bool opponentFire = false;
    float playerfromBall;

    private void Start()
    {
        theTiles = GameObject.FindObjectsOfType<TileScript>();
        
    }

    TileScript goingToTile = null;
    void Update()
    {
       
        tempDist = 0f;  //temp distance variables to see which tile is farther away from the sphere.
        tempDist2 = 0f;
        SphereScript []theSpheres = GameObject.FindObjectsOfType<SphereScript>();
        CreateSphereScript [] cSpheres = GameObject.FindObjectsOfType<CreateSphereScript>();
        oppX = opponent.transform.position.x;   //just shorthand variables
        oppZ = opponent.transform.position.z;
        playX = player.transform.position.x;
        playZ = player.transform.position.z;
        xDifference = Mathf.Abs(playX - oppX);  //used absolute values for the differences in x and z position so that which side they're on doesn't matter.
        zDifference = Mathf.Abs(playZ - oppZ);
        oppPosition = opponent.transform.position;
        playPosition = player.transform.position;
        TileScript ts = player.currentTile;
        playerDistance = Vector3.Distance(playPosition, oppPosition);

        if(goingToTile == null) //first frame
        {
            //print(goingToTile + " " + 2);
            for(int i = 0; i < ts.neighboringTiles.Count; i++)
            {   //go to tile closer to opponent
                opponentDistance = Vector3.Distance(ts.neighboringTiles[i].transform.position, oppPosition);
                if(opponentDistance < playerDistance)
                {
                    goingToTile = ts.neighboringTiles[i];
                }
            }
    
            player.moveToTile(goingToTile);
        }
        else if(ts == goingToTile)  //everything else, hunt the opponent
        {
            //print(goingToTile+" "+1);
            for (int i = 0; i < ts.neighboringTiles.Count; i++)//goes to neighboring tile that's closer to opponent
            {
                opponentDistance = Vector3.Distance(ts.neighboringTiles[i].transform.position, oppPosition);
                if (opponentDistance < playerDistance)
                {
                    goingToTile = ts.neighboringTiles[i];
                }
            }

            if (!player.moving) //if its stuck just go somewhere.
            {
                for (int i = 0; i < ts.neighboringTiles.Count; i++)
                {
                    if (ts.neighboringTiles[i].transform.position == ts.transform.position - new Vector3(1, 0, 0))
                    {
                        goingToTile = (ts.neighboringTiles[i]);
                    }
                    else if (ts.neighboringTiles[i].transform.position == ts.transform.position - new Vector3(1, 0, 0))
                    {
                        goingToTile = (ts.neighboringTiles[i]);
                    }
                    else if (ts.neighboringTiles[i].transform.position == ts.transform.position - new Vector3(1, 0, 0))
                    {
                        goingToTile = (ts.neighboringTiles[i]);
                    }
                    else
                    {
                        goingToTile = (ts.neighboringTiles[i]);
                    }
                }
                //player.moveToTile(goingToTile);
            }
            if(playerDistance < 2.5f)   //just stay put if too close to opponent.
            {
                goingToTile = player.currentTile;
            }
            for(int j = 0; j < theSpheres.Length; j++)  //beginning of evasion checking to avoid enemy spheres.
            {
                if(theSpheres[j].owner != player.thePlayer) //enemy sphere
                {
                    playerfromBall = Vector3.Distance(player.transform.position, theSpheres[j].transform.position); //how far player is from ball
                    
                  
                    GameObject go = theSpheres[j].gameObject;
                    //next if is to check if it needs to dodge, if sphere is going up and its below the player and within a close x distance, needs to dodge
                    if (go.GetComponent<Rigidbody>().velocity == new Vector3(0,0,1) * -2 && go.transform.position.z + 0.5f > playZ && Mathf.Abs(playX - go.transform.position.x) < 0.75f)
                    {
                        Debug.Log("In the downshot loop:");
                        
                        for(int k = 0; k < ts.neighboringTiles.Count; k++)
                        {
                            
                            //try and go left maybe.
                            if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(1, 0, 0))
                            {
                              //  Debug.Log("First k to side 1");
                                possibleTemp = ts.neighboringTiles[k];
                                tempDist = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position); 
                            }
                            //ball is on the left of player, try and go right
                            else if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(-1, 0, 0))
                            {
                               // Debug.Log("2nd k to side 2");
                                possibleTemp2 = ts.neighboringTiles[k];
                                tempDist2 = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);                                
                            }
                            else 
                            {
                                //has to go up or down, not ideal, but only choice.
                                goingToTile = (ts.neighboringTiles[k]);
                               // Debug.Log("Going to random one");
                            }

                            Debug.Log("tempDist1 is: " + tempDist);
                            Debug.Log("tempDist2 is: " + tempDist2);
                            //since it's set to 0 in update, won't go in here if neither option left or right available.
                            if(tempDist > tempDist2)
                            {
                                goingToTile = possibleTemp;
                            }
                            else //if (tempDist2 < tempDist)
                            {
                                goingToTile = possibleTemp2;
                            }
                            //below is the random Debug statement that was causing the error FYI.
                           //Debug.Log("Going to tile is" + goingToTile);
                           //Debug.Log("Trying to go to tile: " + goingToTile.name); //
                        }
                    }//end of first check if sphere traveling up and player above sphere and need to dodge.
                    //second check to see if it needs to dodge; if sphere is going down and its above the player and within a close x distance.
                    else if (go.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, -1) * -2 && go.transform.position.z-0.5f < playZ && Mathf.Abs(playX - go.transform.position.x) < 0.75f)
                    {
                        Debug.Log("In the upshot inner loop:");
                        
                        for (int k = 0; k < ts.neighboringTiles.Count; k++)
                        {
                            if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(1, 0, 0))
                            {
                                possibleTemp = ts.neighboringTiles[k];
                                tempDist = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(-1, 0, 0))
                            {
                                possibleTemp2 = ts.neighboringTiles[k];
                                tempDist2 = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else
                            {
                                goingToTile = (ts.neighboringTiles[k]);
                            }
                        }
                        if (tempDist > tempDist2)
                        {
                            goingToTile = possibleTemp;
                        }
                        else //if (tempDist2 < tempDist)
                        {
                            goingToTile = possibleTemp2;
                        }
                        //Debug.Log("Trying to go to tile: " + goingToTile.name);
                    }//end of second check if sphere traveling down and player below sphere and needs to dodge.
                    //Third check to see if sphere traveling to the right and player is to right of sphere and needs to dodge.
                    else if (go.GetComponent<Rigidbody>().velocity == new Vector3(1, 0, 0) * -2 && go.transform.position.x - 0.5f > playX && Mathf.Abs(playZ - go.transform.position.z) < 0.75f)
                    {
                        Debug.Log("In the rightshot inner loop:");

                        for (int k = 0; k < ts.neighboringTiles.Count; k++)
                        {
                            if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(0, 0, 1))
                            {
                                possibleTemp = ts.neighboringTiles[k];
                                tempDist = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(0, 0, -1))
                            {
                                possibleTemp2 = ts.neighboringTiles[k];
                                tempDist2 = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else
                            {
                                goingToTile = (ts.neighboringTiles[k]);
                            }
                            if (tempDist > tempDist2)
                            {
                                goingToTile = possibleTemp;
                            }
                            else //if (tempDist2 < tempDist)
                            {
                                goingToTile = possibleTemp2;
                            }
                        }
                        //Debug.Log("Trying to go to tile: " + goingToTile.name);
                    }//end of third check if sphere traveling right and player to the right of sphere and needs to dodge.
                    //Fourth check to see if sphere traveling to the left and player is to left of sphere and needs to dodge.
                    else if (go.GetComponent<Rigidbody>().velocity == new Vector3(-1, 0, 0) * -2 && go.transform.position.x + 0.5f < playX && Mathf.Abs(playZ - go.transform.position.z) < 0.75f)
                    {
                        Debug.Log("In the leftshot inner loop:");

                        for (int k = 0; k < ts.neighboringTiles.Count; k++)
                        {
                            if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(0, 0, 1))
                            {
                                possibleTemp = ts.neighboringTiles[k];
                                tempDist = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else if (ts.neighboringTiles[k].transform.position == ts.transform.position - new Vector3(0, 0, -1))
                            {
                                possibleTemp2 = ts.neighboringTiles[k];
                                tempDist2 = Vector3.Distance(go.transform.position, ts.neighboringTiles[k].transform.position);
                            }
                            else
                            {
                                goingToTile = (ts.neighboringTiles[k]);
                            }
                            if (tempDist > tempDist2)
                            {
                                goingToTile = possibleTemp;
                            }
                            else //if (tempDist2 < tempDist)
                            {
                                goingToTile = possibleTemp2;
                            }
                        }
                        //Debug.Log("Trying to go to tile: " + goingToTile.name);
                    }//end of 4th check if sphere traveling left and player to the left of sphere and needs to dodge.
                }//enemy spheres loop
            }//end of looping through spheres.
            
         
        }//end of big else statement setting moveToTile.

        //I only set the player.moveToTile once in Update at the very end; goingToTile changes throughout based on the different circumstances.
        player.moveToTile(goingToTile);
        setPath(player.currentTile, goingToTile);
     
        //firing logic
        if(playerDistance < 5f)//range for firing
        {
            //if opponent to the right of player and they are further away x wise than z we fire right.
            if(oppX < playX && xDifference > zDifference)
            {
            player.createSphere(new Vector3(1, 0, 0));
            }
            //fire left same logic if to the left.
            else if(oppX > playX && xDifference > zDifference)
            {
                player.createSphere(new Vector3(-1, 0, 0));
            }
            //firing up and now we check if their z axis difference is greater than x axis difference.
            else if(oppZ < playZ && zDifference > xDifference)
            {
                player.createSphere(new Vector3(0, 0, 1));
            }
            //same for firing down.
            else if(oppZ > playZ && zDifference > xDifference)
            {
                player.createSphere(new Vector3(0, 0, -1));
            }
        }
       
              
        /*
  * 1,0,0 goes right
  * -1, 0, 0 goes left
  * 0, 0, 1 goes up
  * 0, 0, -1 goes left
  * */

    }

    public void setPath(TileScript startNode, TileScript endNode)
    {
        TileScript start = startNode;

        int currentIteration = ControllerScript.iteration++;

        start.graph_iteration = currentIteration;

        Queue<TileScript> theQueue = new Queue<TileScript>();

        theQueue.Enqueue(start);

        start.graph_distance = 0;
        start.backPointer = null;

        

        while (theQueue.Count > 0)
        {
            TileScript current = theQueue.Dequeue();
            if(current == endNode)
            {break;}
            current.graph_iteration = currentIteration;
            for (int i = 0; i < current.neighboringTiles.Count; i++)
            {
                if (current.neighboringTiles[i].graph_iteration != currentIteration)
                {
                    current.neighboringTiles[i].graph_iteration = currentIteration;
                    theQueue.Enqueue(current.neighboringTiles[i]);
                    current.neighboringTiles[i].graph_distance = current.graph_distance + 1;
                    current.neighboringTiles[i].backPointer = current;
                }
            }
        }
        TileScript temp = endNode;
        while(temp != startNode)
        {
            path.Add(temp);
            temp = temp.backPointer;
        }
        path.Add(startNode);
    }


    public void setPathB(TileScript startNode, TileScript endNode)
    {
        TileScript start = startNode;

        int currentIteration = ControllerScript.iteration++;

        start.graph_iteration = currentIteration;

        Queue<TileScript> theQueue = new Queue<TileScript>();

        theQueue.Enqueue(start);

        start.graph_distance = 0;
        start.backPointer = null;



        while (theQueue.Count > 0)
        {
            TileScript current = theQueue.Dequeue();
            if (current == endNode)
            { break; }
            current.graph_iteration = currentIteration;
            for (int i = 0; i < current.neighboringTiles.Count; i++)
            {
                if (current.neighboringTiles[i].graph_iteration != currentIteration)
                {
                    current.neighboringTiles[i].graph_iteration = currentIteration;
                    theQueue.Enqueue(current.neighboringTiles[i]);
                    current.neighboringTiles[i].graph_distance = current.graph_distance + 1;
                    current.neighboringTiles[i].backPointer = current;
                }
            }
        }
        TileScript temp = endNode;
        while (temp != startNode)
        {
            path.Add(temp);
            temp = temp.backPointer;
        }
        path.Add(startNode);


        int index = 3;
        while(path.Count > 3)
        {
            path.RemoveAt(index);
        }
    }

    public void setPath2Spots(TileScript startNode, TileScript endNode)
    {
        TileScript start = startNode;

        int currentIteration = ControllerScript.iteration++;

        start.graph_iteration = currentIteration;

        Queue<TileScript> theQueue = new Queue<TileScript>();

        theQueue.Enqueue(start);

        start.graph_distance = 0;
        start.backPointer = null;

        while (theQueue.Count > 0)
        {
            TileScript current = theQueue.Dequeue();
            current.graph_iteration = currentIteration;

            //print(current.name + " " + current.graph_distance);

            for (int i = 0; i < current.neighboringTiles.Count; i++)
            {
                if (current.neighboringTiles[i].graph_iteration != currentIteration)
                {
                    current.neighboringTiles[i].graph_iteration = currentIteration;
                    theQueue.Enqueue(current.neighboringTiles[i]);
                    current.neighboringTiles[i].graph_distance = current.graph_distance + 1;
                    current.neighboringTiles[i].backPointer = current;
                }
            }
        }


        /*dijkstra #2!!!*/


        start = endNode;

        currentIteration = ControllerScript.iteration++;

        start.graph_iteration = currentIteration;

        theQueue = new Queue<TileScript>();
        
        theQueue.Enqueue(start);

        start.backPointer = null;
        start.graph_amanda_int1 = 0;

        TileScript shortestTile=null;
        int distance=999;

        while (theQueue.Count > 0)
        {
            TileScript current = theQueue.Dequeue();
            current.graph_iteration = currentIteration;

            //print(current.name + " " + current.graph_distance);

            if(distance > current.graph_distance)
            {
                distance = current.graph_distance;
                shortestTile = current;
            }

            if (current.graph_amanda_int1 < 2)
            {
                for (int i = 0; i < current.neighboringTiles.Count; i++)
                {
                    if (current.neighboringTiles[i].graph_iteration != currentIteration)
                    {
                        current.neighboringTiles[i].graph_iteration = currentIteration;
                        theQueue.Enqueue(current.neighboringTiles[i]);
                        current.neighboringTiles[i].backPointer = current;
                        current.neighboringTiles[i].graph_amanda_int1 = current.graph_amanda_int1 + 1;
                    }
                }
            }
        }

        print(shortestTile.name +" "+ distance);


        setPath(shortestTile, endNode);


        /*TileScript temp = endNode;
        while (temp != startNode)
        {
            path.Add(temp);
            temp = temp.backPointer;
        }
        path.Add(startNode);*/
    }

    private List<TileScript> path = new List<TileScript>();
}
