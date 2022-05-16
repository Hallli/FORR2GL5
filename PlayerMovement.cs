using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public SunnyController controller;  // Held um SunnyController scriptuna
	public Animator animator;  // Held um inn anima

	public float runSpeed = 40f;  // Hraði leikmanns

	float horizontalMove = 0f;  // Set upp hreyfi og hopp breyturnar
	bool jump = false;

	private Rigidbody2D m_Rigidbody2D;  // Held um rigidbody-inn

	void Start()
    {
		m_Rigidbody2D = GetComponent<Rigidbody2D>();  // Tek inn rigidbody leikmanns
	}
	// Update is called once per frame
	void Update()
	{

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;  // Hreyfi leikmann með hraða

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); // Passa að hraðinn sé alltaf í plús tölu með því að nota Mathf.Abs

		if (Input.GetButtonDown("Jump"))  // Ef leikmaður ýtir á spacebar
		{
			jump = true;
			animator.SetBool("isJumping", true);
		}
	}

	public void OnLanding ()  // Klasi sem slekkur á hopp animation-inu
    {
		animator.SetBool("isJumping", false);
    }
	void FixedUpdate()
	{
		// Hreyfi leikmann
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
		jump = false;
	}
}