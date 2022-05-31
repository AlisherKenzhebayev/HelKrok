using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleVisualizer : MonoBehaviour
{
    [SerializeField]
    private Transform grappleSpawn = null;
    [SerializeField]
    private float distanceSpawnLinks = 0.2f;
    [SerializeField]
    private GameObject chainLinkPrefab = null;
    [Tooltip("Grapple shake amount")]
    [SerializeField]
    private float shakeValue = 15f;
    [Tooltip("Animation speed")]
    [SerializeField]
    private float animationLaunchSpeed = 3f;
    [Tooltip("Animation time")]
    [SerializeField]
    private float animationPlaybackTime = 3f;
    [Tooltip("Animation distance")]
    [SerializeField]
    private float fixedAnimDist = 50f;

    [Header("Animation")]
    [Tooltip("Keypoints")]
    [SerializeField]
    private int keypoints;
    [Tooltip("Effect curve")]
    [SerializeField]
    private AnimationCurve effectTimeCurve;
    [Tooltip("Displacement curve")]
    [SerializeField]
    private AnimationCurve displacementCurve;
    [Tooltip("Curve")]
    [SerializeField]
    private AnimationCurve someCurve;

    private Vector3 tetherPoint;
    private bool isTethered = false;
    private List<GameObject> links;
    private List<Vector3> displacements;
    private Transform cameraTransform;
    private float grappleMaxOverlapTime;
    private float timeGrappledSince;
    private float timeGrappleOverlapGeometry;
    private Vector3 directionToGrapple;

    private void Start()
    {
        links = new List<GameObject>();
        displacements = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        Visualize();
    }

    private void Visualize()
    {
        if (isTethered)
        {
            if(displacements.Count == 0) {
                // Generate keypoints
                displacements = GenerateKeypoints();
            }

            // Spawn in chain links
            float maxDist = Vector3.Distance(grappleSpawn.position, tetherPoint);
            float maxTime = maxDist / animationLaunchSpeed;
            float animTime = fixedAnimDist / animationLaunchSpeed;
            float timeProportion = Mathf.Min(timeGrappledSince, maxTime) / maxTime;

            float totalDist = maxDist * timeProportion;
            directionToGrapple = Vector3.Normalize(tetherPoint - grappleSpawn.position);
            float numberOfSpawn = Mathf.RoundToInt(totalDist / distanceSpawnLinks);
            totalDist -= totalDist % distanceSpawnLinks;
            float distValue = 0;
            for (int i = 0; i < Mathf.Max(links.Count, numberOfSpawn); i++)
            {
                if (i > numberOfSpawn)
                {
                    if (links[i] != null)
                    {
                        GameObject.Destroy(links[i]);
                        links[i] = null;
                    }
                    continue;
                }

                // TODO: Weird thing with zeroes... Yeah, they exist.
                if (totalDist == 0.0f)
                {
                    totalDist += 0.01f;
                }

                //We get our lerpValue
                float lerpValue = precisionFloat(distValue / totalDist);

                float timeLerpValue = precisionFloat(timeGrappledSince / animationPlaybackTime);

                //Get the position
                Vector3 placePosition = EstimateDisplacedPosition(grappleSpawn.position, grappleSpawn.position + directionToGrapple * totalDist, distValue, totalDist, timeLerpValue);
                if (links.Count > i && links[i] != null)
                {
                    links[i].transform.position = placePosition;
                    Quaternion rotation = Quaternion.LookRotation(directionToGrapple);
                    rotation = Quaternion.Lerp(rotation * chainLinkPrefab.transform.rotation, rotation, lerpValue);

                    // Add shake when overlaps with something
                    Quaternion shake = Quaternion.Euler(0, Random.Range(-shakeValue, shakeValue), Random.Range(-shakeValue, shakeValue));
                    Quaternion localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), shake, precisionFloat(timeGrappleOverlapGeometry / grappleMaxOverlapTime));

                    if (i % 2 == 0)
                    {
                        localRotation *= Quaternion.Euler(0, 0, 90);
                    }

                    float maxRenderDistance = 50f;

                    Renderer rnd = links[i].GetComponent<Renderer>();
                    rnd.material.SetFloat("Distance_", Mathf.Clamp(distValue / maxRenderDistance, 0f, 1f));

                    links[i].transform.rotation = rotation;
                    links[i].transform.localRotation *= localRotation;
                    //links[i].transform.localScale = chainLinkPrefab.transform.lossyScale;
                }
                else
                {
                    if(i <= numberOfSpawn) { 
                        //Instantiate the object
                        GameObject newLink = Instantiate(chainLinkPrefab, placePosition, cameraTransform.rotation);
                        newLink.transform.parent = grappleSpawn.transform;
                        if (links.Count > i){
                            if(links[i] == null)
                                links[i] = newLink;
                        }
                        else {
                            links.Add(newLink);
                        }
                    }
                }

                distValue += distanceSpawnLinks;
            }
        }
        else
        {
            for (int i = 0; i < links.Count; i++)
            {
                GameObject.Destroy(links[i]);
            }
            links.Clear();
            displacements.Clear();
        }
    }

    private List<Vector3> GenerateKeypoints()
    {
        for (int i = 0; i < keypoints; i++)
        {
            if (displacements.Count > i)
            {
                displacements[i] = (Random.insideUnitSphere);
            }
            else {
                displacements.Add(Random.insideUnitSphere);
            }
        }

        return displacements;
    }
    
    private List<Vector3> KeypointsAtTime(float timeFrac)
    {
        var retVal = new List<Vector3>();

        for (int i = 0; i < keypoints; i++)
        {
            float evalCurve = effectTimeCurve.Evaluate(timeFrac);

            retVal.Add(displacements[i] * evalCurve);
        }

        return retVal;
    }

    private Vector3 GetDisplacement(List<Vector3> list, float posFrac) {
        var fValue = Mathf.Clamp(posFrac, 0.001f, 0.999f) * (keypoints - 1);
        var floorV = Mathf.FloorToInt(fValue);

        float evalCurve = displacementCurve.Evaluate(posFrac);

        return Vector3.Lerp(list[floorV], list[floorV], 1) * evalCurve;
    }

    private Vector3 EstimateDisplacedPosition(Vector3 startPosition, Vector3 endPosition, float distValue, float totalDist, float lerpTime)
    {
        float lerpTotalDist = precisionFloat(distValue / totalDist);
        var linePos = Vector3.Lerp(startPosition, endPosition, lerpTotalDist);

        //if (totalDist > fixedAnimDist)
        {
            if (distValue <= fixedAnimDist)
            {
                var keyPointsTime = KeypointsAtTime(lerpTime);
                float lerpAnimDist = precisionFloat(distValue / fixedAnimDist);
                linePos += GetDisplacement(keyPointsTime, lerpAnimDist);
            }
        //}
        //else {
        //    var keyPointsTime = KeypointsAtTime(lerpTime);
        //    linePos += GetDisplacement(keyPointsTime, lerpTotalDist);
        }

        return linePos;
    }

    public void UpdateGrappleVisualization(
            bool _isTethered, 
            Vector3 _tetherPoint, 
            Transform _cameraTransform, 
            float _grappleMaxOverlapTime, 
            float _timeGrappleOverlapGeometry,
            float _timeGrappledSince) {
        isTethered = _isTethered;
        tetherPoint = _tetherPoint;
        cameraTransform = _cameraTransform;
        grappleMaxOverlapTime = _grappleMaxOverlapTime;
        timeGrappleOverlapGeometry = _timeGrappleOverlapGeometry;
        timeGrappledSince = _timeGrappledSince;
    }

    public Vector3 DirectionToGrapple{
        get {
            if (isTethered)
            {
                return directionToGrapple;
            }
            else {
                return Vector3.zero;
            }
        }
    }

    private float precisionFloat(float fValue)
    {
        return Mathf.Round(fValue * 1000f) / 1000f;
    }
}
