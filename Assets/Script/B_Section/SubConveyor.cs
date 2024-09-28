using System.Collections;
using UnityEngine;

public class SubConveyor : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float speed;
    [SerializeField] bool Power;
    [SerializeField] GameObject TottBox;
    [SerializeField] Transform SpawnPoint;

    public static SubConveyor Instance;
    Vector3 ResumePos;
    Coroutine Coroutine;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ResumePos = StartPos.localPosition; // 시작 위치 초기화
        Belt.localPosition = ResumePos; // 벨트 초기 위치 설정


        SpawnTottPLC();
    }

    // Update is called once per frame
    void Update()
    {




    }
    IEnumerator Moving()
    {


        while (true)
        {
            Vector3 from = ResumePos;
            Vector3 to = EndPos.localPosition;
            float Length = Vector3.Distance(from, to);
            float Journey = Length / speed;
            float currentTime = 0;

            while (currentTime < Journey)
            {
                currentTime += Time.deltaTime;
                Belt.localPosition = Vector3.Lerp(from, to, currentTime / Journey);

                yield return null; // 다음 프레임까지 대기
            }
            float Distance = Vector3.Distance(Belt.localPosition, EndPos.localPosition);
            if (Distance < 5)
                ResumePos = StartPos.localPosition;


            Belt.localPosition = StartPos.localPosition;
        }

    }
    public void SpawnTottPLC()
    {
        Instantiate(TottBox, SpawnPoint.position, Quaternion.identity);
    }
    public void SubConveyorOnPLC()
    {
        if (Coroutine == null)
        {
            Coroutine = StartCoroutine(Moving());
            // if (Belt.localPosition == EndPos.localPosition)
            // {
            //    ResumePos = StartPos.localPosition;
            // }


        }

    }
    public void SubConveyorOffPLC()
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            ResumePos = Belt.localPosition;
            Coroutine = null;
        }
    }
}


