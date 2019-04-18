using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerJoystick : MonoBehaviour {
	public Vector3 moveVector;
	[Header("JoyStick Left")]
	public Joystick left;
	[Range(0, 0.9f)]
	public float leftHandleThreshold;

	[Header("JoyStick Right")]
	public Joystick right;
	private RectTransform rightHandleRectTransform;
	private RectTransform rightBackGroundRectTransform;
	private Vector2 curTouchPosforCharRot;
	private Vector2 prevTouchPosforCharRot;

	[Header("Horizontal Rotate Player")]
	[Range(1, 5)]
	public float horizontalRotateSensitive;

	[Header("Vertical Rotate Cam")]
	public Camera cam;
	[Range(1, 30)]
	public float verticalRotateSensitive;
	[Range(0.01f, 0.9f)]
	public float verticalRotateThreshold;
	[Range(1, 40)]
	public float camUpBoundX;
	[Range(-40, -1)]
	public float camDownBoundX;
	private Vector2 curTouchPosforCamRot;
	private Vector2 prevTouchPosforCamRot;
	private Quaternion m_CameraTargetRot;

	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(scene.name != GlobalData.SCENE_TITLE)
		{
#if UNITY_ANDROID || UNITY_IOS // || UNITY_EDITOR // UNITY EDITOR 는 에디터에서 모바일 조이스틱 테스트하려고 잠시 추가한것 (2018.11.26 원규)
			curTouchPosforCharRot = prevTouchPosforCharRot = Vector2.zero;

			// 아래의 Find 안쓰고 UIManager에서 가져올 수 있는지 확인해보기
			left = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
			right = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
			rightHandleRectTransform = right.handle;
			rightBackGroundRectTransform = right.background;
			cam = GameObject.Find("Player Camera").GetComponent<Camera>();
			m_CameraTargetRot = cam.transform.localRotation;

#endif
			horizontalRotateSensitive = SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive;
			verticalRotateThreshold = SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold;

		}
	}

	//void Start () {
	//	left = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
	//	right = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
	//	rightHandle = GameObject.Find("Floating Joystick Handle");
	//	rightBackGround = GameObject.Find("Floating Joystick Background");

	//	curTouchPosforCharRot = prevTouchPosforCharRot = Vector2.zero;
	//	m_CameraTargetRot = cam.transform.localRotation;
	//	rightHandleRectTransform = rightHandle.GetComponent<Image>().rectTransform;
	//	rightBackGroundRectTransform = rightBackGround.GetComponent<Image>().rectTransform;
	//}
	
	// Update is called once per frame
	void Update ()
	{
#region[MOVE]
		moveVector = Vector3.zero;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		if ((Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) == false)
		{
			moveVector = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"));
		}
#else
		if (((left.Horizontal < leftHandleThreshold && left.Horizontal > -leftHandleThreshold) &&
			(left.Vertical < leftHandleThreshold && left.Vertical > -leftHandleThreshold)) == false) 
		{
			moveVector = (transform.right * left.Horizontal + transform.forward * left.Vertical);
		}
#endif
		#endregion

		#region [Rotate]
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

#else
		// 좌우 캐릭터 회전 (bound 개념 적용)
		if (right.Horizontal != 0)
		{
			curTouchPosforCharRot = rightHandleRectTransform.localPosition;

			// 아직 아무것도 터치하지 않았을 경우 prevTouchPosforCharRot Vector2.zero이므로
			// 첫 터치시 curTouchPosforCharRot prevTouchPosforCharRot 차이가 매우 많이남
			// 그래서 터치 이후 차이값 범위인 -width 부터 width 까지를 이용
			if (prevTouchPosforCharRot.x != 0 &&
				((curTouchPosforCharRot.x - prevTouchPosforCharRot.x) <= rightBackGroundRectTransform.rect.width &&
				(curTouchPosforCharRot.x - prevTouchPosforCharRot.x) >= -rightBackGroundRectTransform.rect.width))
			{
				transform.Rotate(Vector3.up, ((curTouchPosforCharRot.x - prevTouchPosforCharRot.x) * horizontalRotateSensitive * 0.1f));
			}

			prevTouchPosforCharRot = curTouchPosforCharRot;
		}
		else
		{
			curTouchPosforCharRot = prevTouchPosforCharRot = Vector2.zero;
		}

		// 상하 카메라 회전 (threshold, bound 개념 적용)
		if (right.Vertical > verticalRotateThreshold || right.Vertical < -verticalRotateThreshold)
		{
			curTouchPosforCamRot = rightHandleRectTransform.localPosition;

			// 아직 아무것도 터치하지 않았을 경우 prevTouchPosforCamRot Vector2.zero이므로
			// 첫 터치시 curTouchPosforCamRot prevTouchPosforCamRot 차이가 매우 많이남
			// 그래서 터치 이후 차이값 범위인 -height 부터 height 까지를 이용
			if (prevTouchPosforCamRot.y != 0 &&
				(curTouchPosforCamRot.y - prevTouchPosforCamRot.y) <= rightBackGroundRectTransform.rect.height &&
				(curTouchPosforCamRot.y - prevTouchPosforCamRot.y) >= -rightBackGroundRectTransform.rect.height)
			{
				float xRot = (curTouchPosforCamRot.y - prevTouchPosforCamRot.y) * verticalRotateSensitive * 0.1f;
				m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
				m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
				cam.transform.localRotation = m_CameraTargetRot;
			}

			prevTouchPosforCamRot = curTouchPosforCamRot;
		}
		else if (right.Vertical <= verticalRotateThreshold && right.Vertical >= -verticalRotateThreshold)
		{
			curTouchPosforCamRot = prevTouchPosforCamRot = Vector2.zero;
		}
#endif
		#endregion


	}

	public Vector3 GetMoveVector()
	{
		return moveVector.normalized;
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
}
