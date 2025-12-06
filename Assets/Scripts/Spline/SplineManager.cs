using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SplineManager : MonoBehaviour
{

    [SerializeField] private float tIncrement = 0.05f;
    [SerializeField] private float influence = 2f;

    [SerializeField] private GameObject controlPointPrefab;

    [SerializeField] private GameObject[] controlPoints;

    [SerializeField] private int cpCount = 0;

    [SerializeField] private bool showSpline = false;
    [SerializeField] private bool moveTrain = false;
    [SerializeField] private Vector3[] interpolatedPoints;

    [SerializeField] private GameObject trainGO;
    [SerializeField] private GameObject trackPrefab;

    [SerializeField] private List<GameObject> tracks = new List<GameObject>();

    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotSpeed = 10f;


    private void Start()
    {
        controlPoints = new GameObject[4];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (cpCount < 4)
            {
                //create cp
                CreateControlPoint();
                cpCount++;
            }
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            if (cpCount == 4)
            {
                //create spline
                interpolatedPoints = CatmullRomSpline.CreateCatmullRomSpline(controlPoints, tIncrement, influence);
                showSpline = true;
            }
        }

        if (showSpline)
        {
            VisualiseSpline(interpolatedPoints);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            BuildTrack();
            print(tracks.Count);
        }

        if (Input.GetKey(KeyCode.F) && showSpline)
        {
            moveTrain = true;

        }

        if (moveTrain)
        {
            TestMovement();
        }
    }

 
    

    private void BuildTrack()
    {
        float trackSpacing = 2f; // distance between each track piece
        float distanceCovered = 0f;
        Vector3 lastPlacedTrackPos = interpolatedPoints[0]; // start at the first point

        for (int i = 1; i < interpolatedPoints.Length; i++)
        {
            float segmentLength = Vector3.Distance(interpolatedPoints[i - 1], interpolatedPoints[i]);
            distanceCovered += segmentLength;

            if (distanceCovered >= trackSpacing)
            {
                Vector3 targetPos = interpolatedPoints[i];
                Vector3 rot = interpolatedPoints[i] - lastPlacedTrackPos;
                Quaternion targetRot = Quaternion.LookRotation(rot);

                GameObject trackGO = Instantiate(trackPrefab, targetPos, targetRot);
                tracks.Add(trackGO);
                lastPlacedTrackPos = targetPos; //update last placed position
                distanceCovered = 0f; //reset distance tracker
            }
        }
    }

    private void VisualiseSpline(Vector3[] intPoints)
    {
        for (int i = 0; i < intPoints.Length; i++)
        {
            if (i == intPoints.Length - 1) return;
            
            Debug.DrawLine(intPoints[i], intPoints[i + 1], Color.red);
        }
    }

    private void TestMovement()
    {
        // t is what moves it
        tIncrement += Time.deltaTime * speed / interpolatedPoints.Length;
        tIncrement = Mathf.Clamp01(tIncrement);

        
        int segment = Mathf.FloorToInt(tIncrement * (interpolatedPoints.Length - 1));
        float segmentT = (tIncrement * (interpolatedPoints.Length - 1)) - segment;

        Vector3 p0 = interpolatedPoints[segment];
        Vector3 p1 = interpolatedPoints[Mathf.Min(segment + 1, interpolatedPoints.Length - 1)];

        
        trainGO.transform.position = Vector3.Lerp(p0, p1, segmentT);

        Vector3 direction = p1 - p0;

        if (direction != Vector3.zero) // rotate train
        {
            
            if (direction.magnitude > 0.1f)
            {
                
                
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                
                trainGO.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotSpeed);
            }
        }
    }


    private void CreateControlPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            
            GameObject cpGo = Instantiate(controlPointPrefab, hit.point, Quaternion.identity);
            controlPoints[cpCount] = cpGo;
        }
    }
}
