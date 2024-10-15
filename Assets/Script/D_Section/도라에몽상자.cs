using UnityEngine;

public class 도라에몽상자 : MonoBehaviour
{
    public GameObject pouchPrefab; // 파우치로 변경할 프리팹
    public bool IsTotePLC;
    public static 도라에몽상자 instance;
    public void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {

        // 태그가 "Apples"인 경우
        if (other.CompareTag("Apples"))
        {
            ChangeToPouch(other.gameObject);
        }
        else if (other.CompareTag("토트박스"))
        {
            IsTotePLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("토트박스"))
        {
            IsTotePLC = false;
            Destroy(other.gameObject);
        }
    }
    private void DestroyTottBox(GameObject TottBox)
    {
        Destroy(TottBox);
    }

    private void ChangeToPouch(GameObject apple)
    {
        // 사과 오브젝트의 위치와 회전을 기억
        Vector3 position = apple.transform.position;


        // 기존 사과 오브젝트를 파우치로 대체
        Destroy(apple); // 기존 사과 오브젝트 파괴
        GameObject pouch = Instantiate(pouchPrefab, position, Quaternion.identity); // 파우치 인스턴스 생성


    }

}