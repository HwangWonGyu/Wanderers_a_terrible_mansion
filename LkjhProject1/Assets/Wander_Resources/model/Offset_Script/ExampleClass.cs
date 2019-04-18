using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public float scrollSpeed = 0.5F;
    public Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material/*sharedMaterial*/.SetTextureOffset("_MainTex", new Vector2(0, -offset));
		//2018.11.27 원규
		//http://rapapa.net/?p=2472
		//Script에서 Material을 사용할 때 Render.material 호출은 Material을 Copy하여 생성하는 것이므로 Draw Call이 추가 발생하게 된다.
		//대신 Render.sharedMaterial을 사용하여 Batching이 발생하도록 공유하는 형태로 사용하는 것이 좋다.
		//따라서 나중에 바꿔서 테스트해볼것

		//2018.11.28 원규 테스트 결과
		// 드로우콜은 영향없음, 대신 유리병에 유리창처럼 반짝이는거 적용돼서 당장은 sharedMaterial 안쓰기로함, material은 혹시 모르니 메모리 점검해볼것
		// 유리창처럼 반짝이는거 다른 해결법으로 메테리얼 분리도 고려중
	}
}