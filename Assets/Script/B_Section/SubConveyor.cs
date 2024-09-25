using System.Collections;
using UnityEngine;

public class SubConveyor : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float duration;
    [SerializeField] bool Power;
    [SerializeField] GameObject TottBox;
    [SerializeField] Transform SpawnPoint;

    public static SubConveyor Instance;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //초기설정
        Belt.localPosition = StartPos.localPosition;
        SpawnTottPLC();
    }
    // Update is called once per frame
    void Update()
    {

        if (Power && Belt.localPosition == StartPos.localPosition)
        {
            StartCoroutine(Moving(StartPos.localPosition, EndPos.localPosition));
        }


    }
    IEnumerator Moving(Vector3 from, Vector3 to)
    {
        float CurrentTIme = 0;
        Belt.localPosition = from;
        while (CurrentTIme < duration)
        {
            if (Power)
            {
                CurrentTIme += Time.deltaTime;
                Belt.localPosition = Vector3.Lerp(from, to, CurrentTIme / duration);
                yield return null;
            }
            else if (!Power)
            {
                Belt.localPosition = StartPos.localPosition;
                yield break;
            }
        }
        Belt.localPosition = to;
        Belt.localPosition = from;

    }
    public void SpawnTottPLC()
    {
        Instantiate(TottBox, SpawnPoint.position, Quaternion.identity);
    }
    public bool SubConveyorOnPLC()
    {
        return Power = true;
    }
    public bool SubConveyorOffPLC()
    {
        return Power = false;
    }
}

