using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    public Vector3 movementVector;
    public Rigidbody rb;
    public float speed;
    public RecordValues recordValues;
    public RecordTransform recordTransform;
    public int index;

    // Checks for settings
    private bool isClass;
    private bool startRecording;
    private bool replaying;
    private int frameCount;
    private string FilePath;
    string ClassPath ;
    string ListPath ;

  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 10;
        frameCount = 0;
        recordValues = new RecordValues();
        recordTransform = new RecordTransform();
        replaying = false;
    }
    public void SetFilePaths()
    {
        ClassPath = Application.persistentDataPath + "/ClassData" + index.ToString() + ".json";
        ListPath = Application.persistentDataPath + "/ListData" + index.ToString() + ".json";
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();        
        Record();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void CalculateMovement()
    {
        if (replaying)
            return;
        movementVector = new Vector3(Input.GetAxis("Horizontal"), rb.velocity.y, Input.GetAxis("Vertical"));
       
    }
    private void SetRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), 0.25f);
    }

    private void Move()
    {
        if (replaying)
            return;
        rb.velocity = new Vector3(movementVector.x * speed, movementVector.y, movementVector.z * speed);
        if (movementVector != Vector3.zero)
        {          
            SetRotation();
        }
    }

    public void Record()
    {
        if (!startRecording)
            return;
        else
        {   
            if(isClass)
            {
                TransformClass t = new TransformClass();
                t.position = transform.position;
                t.rotation = transform.rotation.eulerAngles;
                recordTransform.transforms.Add(t);
            }
            else
            {
                recordValues.positions.Add(transform.position);
                recordValues.rotations.Add(transform.rotation.eulerAngles);
               
            }
        }
       
    }

    public void Replay()
    {
        Debug.Log("Replaying");
        isClass = PlayerPrefs.GetInt("Class") == 0;
        replaying = true;
        LoadData();
        StartCoroutine(LoadReplay());
    }

    IEnumerator LoadReplay()
    {
        if(isClass)
        {
            for (int i = 0; i < recordTransform.transforms.Count; i++)
            {
                Debug.Log("Reading Class");
                transform.position = recordTransform.transforms[i].position;
                transform.rotation = Quaternion.Euler(recordTransform.transforms[i].rotation);
                yield return new WaitForEndOfFrame();
            }
           
        }
        else
        {
            for (int i = 0; i < recordValues.positions.Count; i++)
            {
                Debug.Log("Reading List");
                transform.position = recordValues.positions[i];
                transform.rotation = Quaternion.Euler(recordValues.rotations[i]);
                yield return new WaitForEndOfFrame();
            }
        }
      
        
    }


    public void StartRecording()
    {
        startRecording = true;
        isClass = PlayerPrefs.GetInt("Class") == 0;
    }

    public void EndRecording()
    {
        startRecording = false;
        replaying = false;
        StopAllCoroutines();
        SaveIntoJson();
    }

    public void SaveIntoJson()
    {
        string data = " ";
        if(isClass)
        {
            data = JsonUtility.ToJson(recordTransform);
            File.WriteAllText(ClassPath, data);
        }
        else
        {
            data = JsonUtility.ToJson(recordValues);
            File.WriteAllText(ListPath, data);
        }
        recordValues = new RecordValues();
        recordTransform = new RecordTransform();
    }

    public void LoadData()
    {
        if(isClass)
        {
            if(File.Exists(ClassPath))
            {
                string fileContents = File.ReadAllText(ClassPath);
                recordTransform = JsonUtility.FromJson<RecordTransform>(fileContents);
            }
            else
            {
                Debug.Log("File not found");
            }
        }
        else
        {
            if (File.Exists(ListPath))
            {
                string fileContents = File.ReadAllText(ListPath);
                recordValues = JsonUtility.FromJson<RecordValues>(fileContents);
            }
            else
                Debug.Log("File not found");
        }
      
    }
}

//Classes

[System.Serializable]
public class RecordValues
{
    public List<Vector3> positions;
    public List<Vector3> rotations;

    public RecordValues() { 
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
    }   
}
[System.Serializable]
public class TransformClass
{
    public Vector3 position;
    public Vector3 rotation;
}
[System.Serializable]
public class RecordTransform
{
    public List<TransformClass> transforms;

    public RecordTransform()
    {
        transforms = new List<TransformClass>();
    }
}
