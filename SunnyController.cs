using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class SunnyController : MonoBehaviour
{

	public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public Transform respawnPosition;
    public ParticleSystem hitParticle;
	public int health
		{
			get { return currentHealth; }
		}
    int currentHealth;
    float invincibleTimer;
    bool isInvincible;







	public GameObject projectilePrefab;   // skjota
	public AudioClip hitSound;
	public AudioClip shootingSound;
    
	Animator animator;
	Vector2 currentInput;
	Vector2 lookDirection = new Vector2(1, 0);

	public UnityEvent m_Jordinni;
	[SerializeField] private float m_JumpForce = 400f;                          // Kraftur hoppsins 
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // Hversu mikið hreyfingin sléttist
	[SerializeField] private bool m_AirControl = false;                         // Hvort að leikmaður geti hreyfst í loftinu eða ekki
	[SerializeField] private LayerMask m_WhatIsGround;                          // Sér um hvað er jörðin fyrir leikmanni
	[SerializeField] private Transform m_GroundCheck;                           // Kíki hvort að leikmaður er á jörðinni eða ekki.

	const float k_GroundedRadius = .2f; // Stærð radíusarins sem kíkir hvort að leikmðaur sé á jörðinni
	private bool m_Grounded;            // Hvort að leikmaður er á jörðinni eða ekki
	private Rigidbody2D m_Rigidbody2D;  // Held utan um rigidbody
	private bool m_FacingRight = true;  // Sér um hvort að leimaður snýr til hægri eða vinstri
	private Vector3 velocity = Vector3.zero;

	public static int count;  // Held utan um stigin
	public Text countText;  // Tek inn stigin á skjánum
	Rigidbody2D rigidbody2d;
	private void Start()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();  // Næ í rigidbody-inn


		invincibleTimer = -1.0f;
        currentHealth = maxHealth;
	}
void Update()
    {
		 if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }





		float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
                
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        currentInput = move;


        // ============== ANIMATION =======================


		     if (Input.GetKeyDown(KeyCode.C)){
            LaunchProjectile();}
			}

	private void FixedUpdate()
	{
 


		m_Grounded = false; // Byrja alltaf grounded á false áður en útreikningarnir byrja

		// Leikmaðurinn er á jörðinni ef að radíus hrings snertir eitthvað sem að er talið sem jörð.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;  // Læt þá grounded sem satt
		}

		if (m_GroundCheck.position.y < -10)  // Kíki hvort að leikmaður hafi dottið af borðinu
        {
			Enda();
        }
	}
public void ChangeHealth(int amount)
    {
        if (amount < 0)
        { 
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            animator.SetTrigger("Hit");
           // 	 audioSource.PlayOneShot(hitSound);

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        if(currentHealth == 0)
            Respawn();
        
        // UIHealthBar.Instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    void Respawn()
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition.position;
    }




	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag=="endir"){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
		}


		if (collision.gameObject.tag == "stig")  // Ef að leikmaður snertir pening
		{
			collision.gameObject.gameObject.SetActive(false);  // Læt peninginn hverfa
			count = count + 1;  
			Debug.Log("Nú er ég kominn með " + (count));
			SetCountText();  // Kalla á klasann sem bætir stigunum á skjáinn
		}
		if (collision.gameObject.tag == "Finish")
        {
			count = 0;
			SetCountText();
			SceneManager.LoadScene(0);   // Ef leikmaður klárar borðið byrjar leikurinn upp á nýtt
		}
		if (collision.gameObject.tag == "ovinur")
        {
			count = count - 1;
			SetCountText();
        }
	
        
	}

	public void Move(float move, bool jump)
	{

		// Mátt bara stjórna leikmanni ef að hann er á jörðinni eða stjórn í lofti er kveikt.
		if (m_Grounded || m_AirControl)
		{
			m_Jordinni.Invoke();  // Læt leikmann hætta að hoppa ef hann er á jörðinni

			// Hreyfi leikmann með því að finna targetVelocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// Slétti út hreyfinguna
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

			// Ef að spilari fer til hægri og leikmaður snýr til vinstri snýr leikmaður til hægri
			if (move > 0 && !m_FacingRight)
			{
				// Sný leikmanni við
				Flip();
			}
			// Sama hér
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
		}
		// Ef að spilarinn ætti að hoppa
		if (m_Grounded && jump)
		{
			Debug.Log("Hoppaðir");
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce)); // Bæti kraftinum við spilarann
		}
	}


	private void Flip()
	{
		// Breyti áttinni sem að spilari snýr
		m_FacingRight = !m_FacingRight;

		// Margfalda x-ás leikmanns við -1 til að snúa honum við
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void SetCountText()
	{
		count = Mathf.Clamp(count, 0, 5);
		countText.text = "Stig: " + count.ToString();  // Uppfæri stigunum á skjánum
	}

	private void Enda()
    {
		SceneManager.LoadScene(2);  // Hleð inn dauða senu
    }

    void LaunchProjectile()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        
        animator.SetTrigger("Launch");
       // audioSource.PlayOneShot(shootingSound);

    }
    
    // =============== SOUND ==========================

    //Allow to play a sound on the player sound source. used by Collectible



















}