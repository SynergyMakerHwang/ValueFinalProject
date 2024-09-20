using UnityEngine;

public class ProcessClass : MonoBehaviour
{
    static public ProcessClass instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null) { 
            instance = new ProcessClass();
        }
    }

    public class Process
    {
        public string processid;
        public string processname;
        public string section;
        public string startposition;
        public string endposition;
        public string starthight;
        public string endhight;
        public string startrotation;
        public string endrotation;
    }

}
