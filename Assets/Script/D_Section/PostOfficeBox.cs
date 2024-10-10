using System.Collections;
using TMPro;
using UnityEngine;

public class PostOfficeBox : MonoBehaviour
{
    [SerializeField] Transform Side1;
    [SerializeField] Transform Side2;
    [SerializeField] Transform Side4;

    [Header("접히는부분")]
    [SerializeField] Transform BoxUnderWing1;
    [SerializeField] Transform BoxUnderWing2;
    [SerializeField] Transform BoxUnderWing3;
    [SerializeField] Transform BoxUnderWing4;
    [SerializeField] Transform BoxUpperWing1;
    [SerializeField] Transform BoxUpperWing2;
    [SerializeField] Transform BoxUpperWing3;
    [SerializeField] Transform BoxUpperWing4;

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
        else if (other.name.StartsWith("BOXFOLDER"))
        {
            StartCoroutine(Rotate(BoxUnderWing4, Quaternion.Euler(-90, 0, 0), 0.5f));
        }
        else if (other.name.StartsWith("Bong"))
        {
            StartCoroutine(Rotate(BoxUnderWing1, Quaternion.Euler(0, 0, -90), 2f));
            StartCoroutine(Rotate(BoxUnderWing2, Quaternion.Euler(0, 0, 90), 2f));
        }
        else if (other.name.StartsWith("Another"))
        {
            StartCoroutine(Rotate(BoxUpperWing1, Quaternion.Euler(0, 0, 90), 0.7f));
            StartCoroutine(Rotate(BoxUpperWing2, Quaternion.Euler(0, 0, -90), 0.7f));
        }
        else if (other.name.StartsWith("Wing"))
        {

            StartCoroutine(Rotate(BoxUpperWing4, Quaternion.Euler(90, 0, 0), 0.7f));
        }
        else if (other.name.StartsWith("Hand"))
            StartCoroutine(Rotate(BoxUpperWing3, Quaternion.Euler(-90, 0, 0), 0.7f));
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
    IEnumerator Rotate(Transform Which, Quaternion EndRot, float duration)
    {
        float CurrentTime = 0;

        Quaternion StartPos = Which.transform.localRotation;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Which.localRotation = Quaternion.Lerp(StartPos, EndRot, CurrentTime / duration);
            yield return null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("BOXFOLDER"))
        {
            StartCoroutine(Rotate(BoxUnderWing3, Quaternion.Euler(90, 0, 0), 2f));
        }
        else if (other.name.StartsWith("Box Aligner"))
        {
            Rigidbody rb = transform.GetComponent<Rigidbody>();



            rb.constraints = RigidbodyConstraints.None; // 모든 제약 해제

        }
    }
}

