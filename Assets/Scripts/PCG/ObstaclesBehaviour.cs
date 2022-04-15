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
                randomSetOfObstacles[i].SetActive(true);
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
