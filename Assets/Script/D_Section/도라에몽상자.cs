using UnityEngine;

public class 도라에몽상자 : MonoBehaviour
{
    public GameObject pouchPrefab; // 파우치로 변경할 프리팹
    int Cnt;
    private void OnTriggerEnter(Collider other)
    {

        // 태그가 "Apples"인 경우
        if (other.CompareTag("Apples"))
        {
            Cnt++;
            // 사과 오브젝트를 파우치로 변경
            if (Cnt == 2)
            {
                Cnt = 0;
                ChangeToPouch(other.gameObject);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("토트박스"))
        {
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
        Quaternion rotation = apple.transform.rotation;

        // 기존 사과 오브젝트를 파우치로 대체
        Destroy(apple); // 기존 사과 오브젝트 파괴
        GameObject pouch = Instantiate(pouchPrefab, position, rotation); // 파우치 인스턴스 생성

        // 필요에 따라 추가적인 작업 수행
        Debug.Log("사과가 파우치로 변경되었습니다.");
    }

}