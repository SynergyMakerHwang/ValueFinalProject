using UnityEngine;

using System.Collections;


public class MainConveyor : MonoBehaviour
{
    //속성 값

    [SerializeField] float speed;
    [SerializeField] bool ConveyorOn;
    [SerializeField] float WaterLevel;

    //생성개체들
    [SerializeField] GameObject Prefebs;
    [SerializeField] Transform PrefebsMom;
    [SerializeField] GameObject waters;
    [SerializeField] GameObject[] Fruits;
    [SerializeField] int FruitsCount;
    // 0 = Apple , 1= orange , 2=Strawberry


    public static MainConveyor instance;
    int CurrentNums = 0;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public bool IsOn()
    {
        return ConveyorOn;
    }

    void Start()
    {
        SpawnFruit(Fruits[2]);

    }

    private void Update()
    {
        if (CurrentNums == 0 && ConveyorOn)
            StartCoroutine(SpawnPrefabs());
    }


    private IEnumerator SpawnPrefabs()
    {



        for (int i = 0; i < 12; i++)
        {
            CurrentNums++;
            yield return new WaitForSeconds(0.55f); // 0.5초 대기
            Instantiate(Prefebs, PrefebsMom);

        }
        if (CurrentNums == 11)
            yield return null;


    }
    public IEnumerator WaterFlowPLC()
    {
        float moveDistance = 0.01f; // 이동 속도

        // Instantiate(waters); // Instantiate는 필요 없을 경우 주석 처리
        Vector3 originalPosition = waters.transform.localPosition; // 원래 위치 저장

        while (waters.transform.localPosition.y <= WaterLevel)
        {
            waters.transform.localPosition = new Vector3(originalPosition.x, waters.transform.localPosition.y + moveDistance, originalPosition.z);
            yield return new WaitForSeconds(0.01f);
        }


    }
   
 
    public bool MainConveyorOnPLC()
    {
        return ConveyorOn = true;
    }
    public bool MainConveyorOffPLC()
    {
        return ConveyorOn =false;
    }

    public void SpawnFruit(GameObject WhichFruit)
    {
        for (int i = 0; i < FruitsCount; i++)
        {
            // 랜덤 좌표 생성
            float Xrandom = Random.Range(waters.transform.position.x - 0.6f, waters.transform.position.x + 0.6f);
            float Yvalue = waters.transform.position.y + 1.4f;
            float Zrandom = Random.Range(waters.transform.position.z - 0.1f, waters.transform.position.z + 0.1f);
            Vector3 RandomSpot = new Vector3(Xrandom, Yvalue, Zrandom);

            Instantiate(WhichFruit, RandomSpot, Quaternion.identity);

        }
    }


}



