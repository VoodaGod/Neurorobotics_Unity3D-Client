using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankTest : MonoBehaviour {


    public Transform[] plankSpawns;
    public GameObject plank;
    public Material plankMaterial;
    Color[] colors = { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.white, Color.yellow };
    public Rigidbody[] plankTargets;
    public UnityEngine.UI.Image leftImage, rightImage, centerImage;
    public Sprite[] wrongSprites;
    public Sprite correctSprite;

    public SteamVR_TrackedObject leftHand, rightHand;
    SteamVR_Controller.Device rightDevice, leftDevice;

    public TrackedObjectAssigner trackedObjectAssigner;

    // Use this for initialization
    void Start () {
	}

    public float totalDiscrepancyTime = 0;
    public void AddTime(float time)
    {
        totalDiscrepancyTime += time;
    }


    public float timeSwitchImages = 2;
    float time = 0;
    bool rightCorrect = false;
    bool leftCorrect = false;
    private void Update()
    {
        leftHand = trackedObjectAssigner.leftHandTrackedObject;
        rightHand = trackedObjectAssigner.righthandTrackedObject;

        time += Time.deltaTime;
        if (time > timeSwitchImages)
        {
            time = 0;
            leftCorrect = false;
            rightCorrect = false;
            centerImage.enabled = false;
            leftImage.sprite = wrongSprites[Random.Range(0, wrongSprites.Length)];
            rightImage.sprite = wrongSprites[Random.Range(0, wrongSprites.Length)];
            float p = Random.Range(0f, 1f);
            if (p < 0.3f)
            {
                p = Random.Range(0f, 1f);
                if (p > 0.5f)
                {
                    rightImage.sprite = correctSprite;
                    rightCorrect = true;
                }
                else
                {
                    leftImage.sprite = correctSprite;
                    leftCorrect = true;
                }
            }
        }

        if (leftHand != null && rightHand != null)
        {
            leftDevice = SteamVR_Controller.Input((int)leftHand.index);
            rightDevice = SteamVR_Controller.Input((int)rightHand.index);
            if (rightCorrect && rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && !leftDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("correct");
                centerImage.enabled = true;
            }
            if (leftCorrect && !rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && leftDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("correct");
                centerImage.enabled = true;
            }
        }
    }

    float delay = 0;
    public float timeBetweenPlankSpawns = 0.75f;
    public float plankSpeed = 0.05f;
    public bool started = false;
    // Update is called once per frame
    void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            started = !started;
        }
        if (started) { 
        delay -= Time.fixedDeltaTime;
            if (delay <= 0)
            {
                delay = 0;
                /*
                for (int i = 0; i < plankSpawns.Length; i++)
                {
                    //Instantiate(plank, plankSpawns[i]).GetComponent<Plank>().target = plankTargets[i];
                    StartCoroutine(SpawnAfterSeconds(plank, plankSpawns[i], plankTargets[i], delay));
                    StartCoroutine(SpawnAfterSeconds(plank, plankSpawns[plankSpawns.Length - 1 - i], plankTargets[plankTargets.Length - 1 - i], delay));
                    delay += timeBetween;
                }
                for (int i = plankSpawns.Length - 1; i >= 0; i--)
                {
                    //Instantiate(plank, plankSpawns[i]).GetComponent<Plank>().target = plankTargets[i];
                    StartCoroutine(SpawnAfterSeconds(plank, plankSpawns[i], plankTargets[i], delay));
                    delay += timeBetween;
                }
                */

                List<int> indexesRandomized = new List<int>();
                for (int i = 0; i < plankSpawns.Length; i++)
                {
                    indexesRandomized.Add(i);
                }
                for (int i = 0; i < indexesRandomized.Count; i++)
                {
                    int temp = indexesRandomized[i];
                    int randomIndex = Random.Range(i, indexesRandomized.Count);
                    indexesRandomized[i] = indexesRandomized[randomIndex];
                    indexesRandomized[randomIndex] = temp;
                }


                for (int j = 0; j < indexesRandomized.Count; j++)
                {
                    int i = indexesRandomized[j];
                    StartCoroutine(SpawnAfterSeconds(plank, plankSpawns[i], plankTargets[i], delay));
                    StartCoroutine(SpawnAfterSeconds(plank, plankSpawns[plankSpawns.Length - 1 - i], plankTargets[plankTargets.Length - 1 - i], delay));
                    delay += timeBetweenPlankSpawns;
                }


            }
        }
	}
 

    IEnumerator SpawnAfterSeconds(GameObject plank, Transform spawnPos, Rigidbody target, float delay)
    {
        yield return new WaitForSeconds(delay);
        Plank newPlank = Instantiate(plank, spawnPos).GetComponent<Plank>();
        newPlank.target = target;
        newPlank.speed = plankSpeed;
        Material newMat = Instantiate(plankMaterial);
        newMat.color = colors[Random.Range(0, colors.Length)];
        newPlank.GetComponent<MeshRenderer>().material = newMat;


    }
}
