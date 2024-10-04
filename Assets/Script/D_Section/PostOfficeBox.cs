using System.Collections;
using UnityEngine;

public class PostOfficeBox : MonoBehaviour
{
    [SerializeField] Transform Side1;
    [SerializeField] Transform Side2;
    [SerializeField] Transform Side4;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            other.transform.SetParent(transform);
        }
        else if (other.name.Contains("Box Aligner"))
        {
            StartCoroutine(BoxAlign());
        }

    }
    IEnumerator BoxAlign()
    {
        float CurrentTime = 0;
        float duration = 0.2f;//정렬기 속도와 같이
        Quaternion StartPos1 = Side1.localRotation;
        Quaternion StartPos2 = Side2.localRotation;
        Quaternion StartPos4 = Side4.localRotation;
        Quaternion EndPos = Quaternion.Euler(0, 0, 0);
        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Side1.localRotation = Quaternion.Lerp(StartPos1, EndPos, CurrentTime / duration);
            Side2.localRotation = Quaternion.Lerp(StartPos2, EndPos, CurrentTime / duration);
            Side4.localRotation = Quaternion.Lerp(StartPos4, EndPos, CurrentTime / duration);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-90, 0, 0), CurrentTime / duration);
            yield return null;
        }


    }
}
