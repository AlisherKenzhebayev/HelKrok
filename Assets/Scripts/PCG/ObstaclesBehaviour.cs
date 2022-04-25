using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesBehaviour : MonoBehaviour
{
    [System.Serializable]
    public struct ListWrapper<T>
    {
        public List<T> list;
    }

    // does not work
    [System.Serializable]
    public struct DictionaryWrapper<TKey, TValue>
    {
        [SerializeField]
        public Dictionary<TKey, TValue> dictionary;
    }




    //helper structure
    [System.Serializable]
    public struct RandomSelection
    {
        public int minValue;
        public int maxValue;
        public float probability;

        public RandomSelection(int minValue, int maxValue, float probability)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.probability = probability;
        }

        public int GetValue() { return Random.Range(minValue, maxValue + 1); }
    }


    int GetRandomValue(List<RandomSelection> selections)
    {
        float rand = Random.value;
        float currentProb = 0;
        foreach (var selection in selections)
        {
            currentProb += selection.probability;
            if (rand <= currentProb)
                return selection.GetValue();
        }

        //will happen if the input's probabilities sums to less than 1
        //throw error here if that's appropriate
        return -1;
    }

    public List<RandomSelection> listOfRandomRanges;

    public List<ListWrapper<GameObject>> listOfObstacleSets;
    public List<ListWrapper<float>> listOfObstacleSetsProbabilies;

    bool collisionDetect(Transform input)
    {
        Transform upWallPosition = transform.parent.Find("Entrances").Find("Up Wall");
        Transform downWallPosition = transform.parent.Find("Entrances").Find("Down Wall");
        Transform rightWallPosition = transform.parent.Find("Entrances").Find("Right Wall");
        Transform leftWallPosition = transform.parent.Find("Entrances").Find("Left Wall");
        print(upWallPosition);
        Transform[] listOfTransforms = { upWallPosition, downWallPosition, rightWallPosition, leftWallPosition };

        for (int i = 0; i < listOfTransforms.Length; i++)
        {
            if (listOfTransforms[i] != null && (Mathf.Abs(listOfTransforms[i].position.x - input.position.x) < 0.3 || Mathf.Abs(listOfTransforms[i].position.z - input.position.z) < 0.3))
            {
                return true;
            }
        }
        return false;

    }

    // Start is called before the first frame update
    void Start()
    {
        //int numberOfSets = listOfObstacleSets.Count;
        //int randomSetIndex = Random.Range(0, numberOfSets);
        int randomSetIndex = GetRandomValue(listOfRandomRanges);
        List<GameObject> randomSetOfObstacles = listOfObstacleSets[randomSetIndex].list;
        List<float> randomSetOfObstaclesProbabilities = listOfObstacleSetsProbabilies[randomSetIndex].list;
        for (int i = 0; i < randomSetOfObstacles.Count; i++)
        {
            if (Random.value <= randomSetOfObstaclesProbabilities[i])
            {
                //print("here-1");
                if (collisionDetect(randomSetOfObstacles[i].transform))
                {
                    //print("here0");
                    randomSetOfObstacles[i].SetActive(false);
                }
                else
                {
                    //print("here1");
                    randomSetOfObstacles[i].SetActive(true);

                }
            }
            else
            {
                randomSetOfObstacles[i].SetActive(false);
            }
        }

        for (int i = 0; i < listOfObstacleSets.Count; i++)
        {
            if (i != randomSetIndex)
            {
                for (int j = 0; j < listOfObstacleSets[i].list.Count; j++)
                {
                    listOfObstacleSets[i].list[j].SetActive(false);
                }
            }
        }

        //setsOfObstacles = new GameObject[3, 4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
