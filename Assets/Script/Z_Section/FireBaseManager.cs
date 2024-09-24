using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using Firebase.Extensions;
using static AuthManager;
using static FirebaseManager;
using System.Runtime.Remoting;
using System.Security.Policy;
using UnityEngine.Networking;
using Google.MiniJSON;

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


   public IEnumerator ReadDataWithNewtonJsonData(string key, System.Action<string> callback)
    {
           
        string userInfo = string.Empty;
        string json = "";
        if (instance != null)
        {
            DatabaseReference dbref = FirebaseDatabase.DefaultInstance.GetReference(key);          

            Task t = dbref.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                json = task.Result.GetRawJsonValue();
                print(json);            

                if (task.Exception != null)
                {
                    print(task.Exception);
                }
                callback(json);
            });
            
   
            yield return new WaitUntil(() => t.IsCompleted);

            

            print("������ �бⰡ �Ϸ�Ǿ����ϴ�.");
        }
    }
  
    public IEnumerator ReadDataWithNewtonJsonDataSnapshot(string key, System.Action<DataSnapshot> callback)
    {

        string userInfo = string.Empty;
        DataSnapshot snapshot;
        if (instance != null)
        {
            DatabaseReference dbref = FirebaseDatabase.DefaultInstance.GetReference(key);

            Task t = dbref.GetValueAsync().ContinueWithOnMainThread(task =>
            {

                snapshot = task.Result;
                print(snapshot);

                if (task.Exception != null)
                {
                    print(task.Exception);
                }
                callback(snapshot);
            });


            yield return new WaitUntil(() => t.IsCompleted);



            print("������ �бⰡ �Ϸ�Ǿ����ϴ�.");
        }
    }

}