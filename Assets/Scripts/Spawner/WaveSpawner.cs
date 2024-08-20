using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public void SpawnWave(Dictionary<GameObject, int> nextWave)
    {
        foreach(var wave in nextWave) 
        {
            for (int i = 0; i < wave.Value; i++)
            {
                //Instantiate(wave.Key);
            }
        }
    }
}
