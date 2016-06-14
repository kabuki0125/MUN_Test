
using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

/// <summary>
/// Very basic NPC patrol between Waypoints
/// </summary>

public class NPC_Patrol : MonoBehaviour {
	
	public Transform[] WayPoints;
		
	// constants
	const float m_MaxSpeed = 3;
	const float m_SpeedDampTime = .25f;	
	const float m_DirectionDampTime = .25f;		
	
	int 		m_WayPointIndex = 0;	
	Animator  	m_Animator = null;	
	
		
	// Use this for initialization
	void Start () 
	{
        transform.parent.parent  = GameObject.Find("NPC_Spawner").transform.GetChild(0).transform;
        gameObject.SetActive(true);
        
		m_Animator = GetComponent<Animator>();		
	}
	
    
	void Update () 
	{
        // ホスト以外は処理をしない
        if( !MonobitEngine.MonobitNetwork.isHost )
        {
            return;
        }
        
		if(WayPoints.Length > 0)
		{			
			Transform target = WayPoints[m_WayPointIndex];
			if (target)
			{			
				if(Vector3.Distance(target.position,m_Animator.rootPosition) > 1.5f)
				{
					m_Animator.SetFloat("Speed",m_MaxSpeed,m_SpeedDampTime, Time.deltaTime);
					
					Vector3 curentDir = m_Animator.rootRotation * Vector3.forward;
					Vector3 wantedDir = (target.position - m_Animator.rootPosition).normalized;
		
					if(Vector3.Dot(curentDir,wantedDir) > 0)
					{
						m_Animator.SetFloat("Direction",Vector3.Cross(curentDir,wantedDir).y,m_DirectionDampTime, Time.deltaTime);
					}
					else
					{
	            		m_Animator.SetFloat("Direction", Vector3.Cross(curentDir,wantedDir).y > 0 ? 1 : -1, m_DirectionDampTime, Time.deltaTime);
					}
				}
				else // waypoint reached, change waypoint
				{
	            	m_Animator.SetFloat("Speed",0,m_SpeedDampTime, Time.deltaTime);
					m_WayPointIndex = (m_WayPointIndex+1) % WayPoints.Length;
				}
			}		
		}			
	}
}
