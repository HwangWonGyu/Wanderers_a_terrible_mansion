using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Player3DExample : MonoBehaviour {

	[Header("Character")]
	public float moveSpeed;
	public float walkSpeed;
	public float runSpeed;
	public float sitSpeed;
	private GlobalData.PlayerState playerState;
	public Vector3 moveVector;
	private NavMeshAgent navMeshAgent;
	[Range(1, 5)]
	public float rotateSensitive;
	public bool soundedState;
	private bool flag=false;
	private Coroutine soundedCoroutine;

	[Header("JoyStick")]
	public Joystick joystickLeft;
	[Range(0, 0.9f)]
	public float leftHandleThreshold;
	public Joystick right;
	public GameObject rightHandle;
	public GameObject rightBackGround;
	[Range(0, 0.9f)]
	public float rightHandleThreshold;
	private RectTransform rightHandleRectTransform;
	private RectTransform rightBackGroundRectTransform;
	private Vector2 curTouchPosforCharRot;
	private Vector2 prevTouchPosforCharRot;
	private Vector2 curTouchPosforCamRot;
	private Vector2 prevTouchPosforCamRot;

	[Header("Camera")]
	public Camera cam;
	[Range(1, 30)]
	public float camRotateSensitive;
	[Range(1, 40)]
	public float camUpBoundX;
	[Range(-40, -1)]
	public float camDownBoundX;
	private Quaternion m_CameraTargetRot;

	void Start()
	{
		playerState = GlobalData.PlayerState.Walk;
		curTouchPosforCharRot = prevTouchPosforCharRot = Vector2.zero;
		m_CameraTargetRot = cam.transform.localRotation;
		navMeshAgent = GetComponent<NavMeshAgent>();
		rightHandleRectTransform = rightHandle.GetComponent<Image>().rectTransform;
		rightBackGroundRectTransform = rightBackGround.GetComponent<Image>().rectTransform;
	}

	private void Update()
	{
		


		// 이동 (threshold 개념 적용)
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Space))
		{
			//UIManager.GetInstance.RunToggle();
		}

		

		if ((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) == false)
		{
			moveVector = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
			navMeshAgent.Move(moveVector * moveSpeed * Time.deltaTime);
			soundedState = true;
			if (flag)
			{
				StopCoroutine(soundedCoroutine);
				flag = false;
			}

		}
		else
		{
			if (soundedState)
			{
				if (!flag)
				{
					flag = true;
					soundedCoroutine = StartCoroutine("OnSoundedMode");
				}
			}


		}
#else
		if (((joystickLeft.Horizontal < leftHandleThreshold && joystickLeft.Horizontal > -leftHandleThreshold) &&
			(joystickLeft.Vertical < leftHandleThreshold && joystickLeft.Vertical > -leftHandleThreshold)) == false) 
		{
			moveVector = (transform.right * joystickLeft.Horizontal + transform.forward * joystickLeft.Vertical);
			navMeshAgent.Move(moveVector * moveSpeed * Time.deltaTime);
			soundedState = true;
			if (flag)
			{
				StopCoroutine(soundedCoroutine);
				flag = false;
			}
			
		}
		else
		{
			if (soundedState)
			{
				if (!flag)
				{
					flag = true;
					soundedCoroutine = StartCoroutine("OnSoundedMode");
				}
			}
			
			
		}
#endif

		// 좌우 캐릭터 회전 (bound 개념 적용)
		if (right.Horizontal != 0)
		{
			curTouchPosforCharRot = rightHandleRectTransform.localPosition;

			// 아직 아무것도 터치하지 않았을 경우 prevTouchPosforCharRot Vector2.zero이므로
			// 첫 터치시 curTouchPosforCharRot prevTouchPosforCharRot 차이가 매우 많이남
			// 그래서 터치 이후 차이값 범위인 -width 부터 width 까지를 이용
			if (prevTouchPosforCharRot.x != 0 &&
				((curTouchPosforCharRot.x - prevTouchPosforCharRot.x) <= rightBackGroundRectTransform.rect.width &&
				(curTouchPosforCharRot.x - prevTouchPosforCharRot.x) >= -rightBackGroundRectTransform.rect.width) )
			{
				transform.Rotate(Vector3.up, ((curTouchPosforCharRot.x - prevTouchPosforCharRot.x) * rotateSensitive * 0.1f));
			}

			prevTouchPosforCharRot = curTouchPosforCharRot;
		}
		else
		{
			curTouchPosforCharRot = prevTouchPosforCharRot = Vector2.zero;
		}

		// 상하 카메라 회전 (threshold, bound 개념 적용)
		if (right.Vertical > rightHandleThreshold || right.Vertical < -rightHandleThreshold)
		{
			curTouchPosforCamRot = rightHandleRectTransform.localPosition;

			// 아직 아무것도 터치하지 않았을 경우 prevTouchPosforCamRot Vector2.zero이므로
			// 첫 터치시 curTouchPosforCamRot prevTouchPosforCamRot 차이가 매우 많이남
			// 그래서 터치 이후 차이값 범위인 -height 부터 height 까지를 이용
			if (prevTouchPosforCamRot.y != 0 &&
				(curTouchPosforCamRot.y - prevTouchPosforCamRot.y) <= rightBackGroundRectTransform.rect.height &&
				(curTouchPosforCamRot.y - prevTouchPosforCamRot.y) >= -rightBackGroundRectTransform.rect.height )
			{
				float xRot = (curTouchPosforCamRot.y - prevTouchPosforCamRot.y) * camRotateSensitive * 0.1f;
				m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
				m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
				cam.transform.localRotation = m_CameraTargetRot;
			}

			prevTouchPosforCamRot = curTouchPosforCamRot;
		}
		else if (right.Vertical <= rightHandleThreshold && right.Vertical >= -rightHandleThreshold)
		{
			curTouchPosforCamRot = prevTouchPosforCamRot = Vector2.zero;
		}
		switch (playerState)
		{
			case GlobalData.PlayerState.Walk:
				moveSpeed = walkSpeed;
				//UIManager.GetInstance.stamina.value += 0.0025f;
				break;
			case GlobalData.PlayerState.Run:
				moveSpeed = runSpeed;

#if UNITY_EDITOR
				if ((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) == false)
#else
			if (((joystickLeft.Horizontal < leftHandleThreshold && joystickLeft.Horizontal > -leftHandleThreshold) &&
				(joystickLeft.Vertical < leftHandleThreshold && joystickLeft.Vertical > -leftHandleThreshold)) == false)
#endif
				{
					//UIManager.GetInstance.stamina.value -= 0.05f;
				}

				if (UIManager.GetInstance.stamina.value == UIManager.GetInstance.stamina.minValue)
				{
					moveSpeed = walkSpeed;
					playerState = GlobalData.PlayerState.Walk;
					//UIManager.GetInstance.isRun = false;
					//UIManager.GetInstance.runButtonText.text = "Run Off";
				}
				break;
			case GlobalData.PlayerState.Sit:
				break;
		}
	
		
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

		angleX = Mathf.Clamp(angleX, camDownBoundX, camUpBoundX);

		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
	


	IEnumerator OnSoundedMode()
	{
		yield return new WaitForSeconds(1.0f);
		soundedState = false;
		flag = false;
	}
}