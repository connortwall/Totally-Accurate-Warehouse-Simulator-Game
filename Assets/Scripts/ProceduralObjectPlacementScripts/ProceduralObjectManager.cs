using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralObjectManager : MonoBehaviour
{
    private List<Vector2> samples;
    
    public List<GameObject> obstacles;

    public Vector2 zone= Vector2.one;
    public float sparcity = 1;
    private int k = 2;

    public float scale = 1;


    private void Start()
    {
        samples = Poisson.GeneratePoint(sparcity, zone, k);
        if(samples != null)
        {
            foreach(Vector2 sample in samples)
            {
                
                int index = Random.Range(0, obstacles.Count);
                GameObject obstacle = Instantiate(obstacles[index], new Vector3(sample.x, 0, sample.y)+transform.position, Quaternion.identity)as GameObject;
                obstacle.transform.Rotate(0, Random.Range(0, 360), 0);
                obstacle.transform.localScale = Vector3.one * scale;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((new Vector3(zone.x, 0, zone.y) / 2)+transform.position, new Vector3(zone.x, 0, zone.y));
        
    }

}