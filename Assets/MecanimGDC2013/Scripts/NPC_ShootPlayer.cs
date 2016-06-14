using UnityEngine;
using System.Collections;

/// <summary>
/// Makes NPC shoot at player
/// </summary>
public class NPC_ShootPlayer : MonobitEngine.MonoBehaviour {
	
	const float m_AttackDistance = 5;
	const float m_BulletSpeed = 15.0f;
	const float m_BulletDuration = 10.0f;
	
	public bool CheatRoot = false;
	public Transform BulletSpawnPoint;
	public GameObject Bullet;
	
	public Transform BulletParent;
	
	Animator  	    m_Animator = null;	
    GameObject[] 	m_Players = null;	 	
    Animator[] 	    m_PlayerAnimators = null;
    int             selectedPlayer = 0;
    Vector3         selectedPos = Vector3.zero;
	bool 		    m_HasShootInCycle;
	float 		    m_PrevStateTime;
	Vector3		    m_LookAtPosition = new Vector3();
	
	void Start () 
	{
        m_Animator = GetComponent<Animator>();
        m_Animator.logWarnings = false; // so we dont get warning when updating controller in live link ( undocumented/unsupported function!)
        m_Players = GameObject.FindGameObjectsWithTag("Player");
        m_PlayerAnimators = new Animator[m_Players.Length];
        for( int i = 0; i < m_Players.Length; i++ )
        {
            m_PlayerAnimators[i] = m_Players[i].GetComponent<Animator>();
        }
	}
		
	void Update ()
	{	
        // ホスト以外は処理をしない
        if( !MonobitEngine.MonobitNetwork.isHost )
        {
            return;
        }
        
		if(ShouldShootPlayer())
		{			
			m_Animator.SetBool("Shoot", true); // tell animator to shoot
			
			ManageShootCycle();
			
			if(BulletSpawnPoint && !m_HasShootInCycle)
			{								
				if( m_Animator.GetFloat("Throw") > 0.99f )  // additionnal curve in the shoot animation
				{					
                    monobitView.RPC("SpawnBullet", MonobitEngine.MonobitTargets.All, selectedPos);						
				}
			}					
		}
		else
		{
			m_Animator.SetBool("Shoot", false);
		}
	}
	
	
    bool ShouldShootPlayer()
    {
        // ルーム入室中のみ、プレイヤー情報の更新を行なう
        if(MonobitEngine.MonobitNetwork.isConnect && MonobitEngine.MonobitNetwork.inRoom)
        {
            if(m_Players.Length != MonobitEngine.MonobitNetwork.room.playerCount)
            {
                // プレイヤー情報の再取得
                m_Players = GameObject.FindGameObjectsWithTag("Player");
                m_PlayerAnimators = new Animator[m_Players.Length];
                for (int i = 0; i < m_Players.Length; i++)
                {
                    m_PlayerAnimators[i] = m_Players[i].GetComponent<Animator>();
                }
            }
        }

        for (int i = 0; i < m_Players.Length; i++)
        {
            float distanceToPlayer = Vector3.Distance(m_Players[i].transform.position, transform.position);
            if (distanceToPlayer < m_AttackDistance)
            {
                AnimatorStateInfo info = m_PlayerAnimators[i].GetCurrentAnimatorStateInfo(0);
                // real bears don't shoot at dead player!
                if (!info.IsName("Base Layer.Dying") && !info.IsName("Base Layer.Death") && !info.IsName("Base Layer.Reviving"))
                {
                    if(selectedPlayer < 0 )
                    {
                        selectedPlayer = i;
                        selectedPos = m_Players[i].transform.position;
                    }
                    return true;
                }
            }
        }

        return false;       
    }
	
	
	// Makes sure we only shoot once per cycle
	void ManageShootCycle()
	{
		AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		float time = Mathf.Repeat(stateInfo.normalizedTime,1.0f);
					
		if(time < m_PrevStateTime) // has looped
			m_HasShootInCycle = false;
		
		m_PrevStateTime = time;
	}
		
	void OnAnimatorMove()
	{
		if(CheatRoot)
		{
			if(!enabled || !GetComponent<CharacterController>().enabled) return;
			
			//  Cheat the root to align to player target.
			if(m_Animator.GetBool("Shoot"))
			{
                if(selectedPlayer >= 0 )
                {
                    m_LookAtPosition.Set(selectedPos.x, transform.position.y, selectedPos.z); // Kill Y.
                }
				
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_LookAtPosition-transform.position), Time.deltaTime * 5);
				m_Animator.rootRotation =  transform.rotation;
			}							
		
			GetComponent<CharacterController>().Move(m_Animator.deltaPosition);					
			transform.rotation = m_Animator.rootRotation;
		
			ForceOnFloor();		
		}
	}
	
    // Spawns bullet
    [MunRPC]
    void SpawnBullet(Vector3 targetPos) 
    {
        GameObject newBullet = Instantiate(Bullet, BulletSpawnPoint.position , Quaternion.Euler(0, 0, 0)) as GameObject;                                                
        Destroy(newBullet, m_BulletDuration);                           
        Vector3 direction = targetPos - BulletSpawnPoint.position;
        direction.y = 0;
        newBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(direction)* m_BulletSpeed;                               
        if(BulletParent)newBullet.transform.parent = BulletParent;
        m_HasShootInCycle = true;
        selectedPlayer = -1;
    }
		
	/// Make the NPC always on floor, even when colliding
	void ForceOnFloor()
	{		
		Vector3 position = transform.position;
  		position.y = 0;
  		transform.position = position;
	}
}
