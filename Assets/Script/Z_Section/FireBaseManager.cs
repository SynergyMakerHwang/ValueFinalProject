using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Firebase Realtime Database�� ����, �����͸� �а� ����.
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [Serializable]
    public class User
    {
        public string id;
        public string pw;
        public string role;
        public string products;
        public string history;
    }


    [SerializeField] string dbURL = "";    
    public DatabaseReference dbRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
  

  
    void Start()
    {

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

     
    }


    //������ ��������
   /* public string ReadDataWithNewtonJson()
    {
        string result = "";
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
               
                foreach (var item in snapshot.Children)
                {
                    string json = item.GetRawJsonValue();
                    print(json);

                    product = JsonConvert.DeserializeObject<Product>(json);                    
                }

                print("�����͸� �� �޾ҽ��ϴ�.");
                
            }
        });
        return result;
    }*/
    public IEnumerable<DataSnapshot> ReadDataWithNewtonJsonDataSnapshot()
    {
        IEnumerable<DataSnapshot> result = null;
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;                
                result = snapshot.Children;               
                print("�����͸� �� �޾ҽ��ϴ�.");
                
            }
        });
        return result;
    }


}