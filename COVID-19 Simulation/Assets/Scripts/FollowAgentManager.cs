﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgentManager : MonoBehaviour
{

    private SimManager manager;

    public GameObject leader;
    public AgentManager leadManager;
    public NavMeshAgent navAgent;

    public bool isInfected;

    public Transform hit;
    public Transform infectHit;

    public Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Leader initialization.
        leader = gameObject.transform.parent.gameObject;
        leadManager = leader.GetComponent<AgentManager>();

        // * Tag and Color initialization.
        if (leader.tag == "GroupShopper") {
            color = new Color(1,0,1,0.5f);
            gameObject.tag = "GroupShopper";
        } else if (leader.tag == "GroupCommuter") {
            color = new Color(0,1,1,0.5f);
            gameObject.tag = "GroupCommuter";
        }

        gameObject.tag = leadManager.tag;
        color = leadManager.color;
        color.a = 0.5f;

        rend = GetComponent<Renderer>();
        rend.material.color = color;
        
        isInfected = leadManager.GetInfection();

        // * Radius initialization.
        float radius = leadManager.radius;
        Vector3 scaleChange = new Vector3(radius, 0, radius);
        gameObject.transform.GetChild(0).localScale += scaleChange;

        // * Speed initialization.
        navAgent.speed = leadManager.maxSpeed;
        navAgent.angularSpeed = leadManager.maxSpeed*10;
        navAgent.acceleration = leadManager.maxSpeed*10;
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.SetDestination(leader.transform.position);
        // TODO: Interaction and Infection checks.
    }

    // Collision detection code to count interactions between different agents.
    // Agents within the same group interacting with each other do not contribute to contact counts.
    // This is to model family and friend groups interactions with other groups.
    private void OnCollisionEnter(Collision other) {
        // TODO: infection only counts.
        // * Check if the other object is not the map and not a spawner object.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") && (other.gameObject.name != "Map"));  
        // * Check if other object is not the parent of this object.
        bool parentCheck = (transform.IsChildOf(other.transform));                 
        // * Check if siblings have the same parent.                         
        bool siblingCheck = (transform.parent == other.transform.parent);                                   
        if (environmentCheck && !parentCheck && !siblingCheck) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            if (leadScript != null && leadScript.GetInstanceID() > GetInstanceID()) {
                leadManager.TrackInteraction(other, isInfected, leadScript.isInfected);
                if (isInfected) { 
                    leadScript.SetColor(color); 
                    leadScript.SetInfection(isInfected);
                    leadScript.UpdateInfectionProportions();
                } else if (leadScript.isInfected) { 
                    SetColor(leadScript.GetColor()); 
                    SetInfection(leadScript.GetInfection());
                    leadScript.UpdateInfectionProportions();
                }
            } else if (followScript != null && followScript.GetInstanceID() > GetInstanceID()) {
                leadManager.TrackInteraction(other, isInfected, followScript.isInfected);
                if (isInfected) { 
                    followScript.SetColor(color); 
                    followScript.SetInfection(isInfected);
                    leadManager.UpdateInfectionProportions();
                } else if (followScript.isInfected) { 
                    SetColor(followScript.GetColor()); 
                    SetInfection(followScript.GetInfection());
                    leadManager.UpdateInfectionProportions();
                }  
            }

        }
    }

    //
    public Color GetColor() { return color; }
    public void SetColor(Color col) { rend.material.color = col;}

    //
    public bool GetInfection() { return isInfected; }
    public void SetInfection(bool infection) { isInfected = infection; }


}
