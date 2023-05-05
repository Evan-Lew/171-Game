using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaScale : MonoBehaviour
{

    public SpriteRenderer ScaleLeft;
    public SpriteRenderer ScaleRight;
    public SpriteRenderer ScaleBar;

    private float ScaleBarXSize;
    private float ScaleYSize;
    private Vector3 ScaleBarPosition;

    public Character player;
    public Character enemy;
    private float priorityDifference = 0.0f;

    public GameObject playerOrbPrefab;
    public Transform  playerOrbSpawn;
    public GameObject enemyOrbPrefab;
    public Transform  enemyOrbSpawn;    
    private float OrbDifference = 0.0f;

    //lerp starting value
    private float lerpMin = 0.0f;
    private float lerpMax = 0.0f;
    static float t = 0.0f; 
    private float angle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        ScaleBarXSize = ScaleBar.size.x;
        // ScaleBarXSize = 24;
        ScaleYSize = ScaleLeft.size.y;
        ScaleBarPosition = ScaleBar.transform.position;

    }

    // Takes the karma difference and rotates the scales based on it
    public void MoveScales(float newRotation){
        // float newRotation = 10*karmaDiff;

        newRotation = newRotation > 60 ? 60 : newRotation;
        newRotation = newRotation < -60 ? -60 : newRotation;

        float newY = ScaleBarXSize * Mathf.Sin(newRotation*Mathf.PI/180);
        float newX = ScaleBarXSize * Mathf.Cos(newRotation*Mathf.PI/180);

        ScaleLeft.transform.localPosition = new Vector3(-newX, -newY-ScaleYSize, 0);
        ScaleRight.transform.localPosition = new Vector3(newX, newY-ScaleYSize, 0);

        Quaternion rotation = Quaternion.Euler(0, 0, newRotation);
        ScaleBar.transform.localRotation = rotation;
    }

    public void setRotation(float karmaDiff){

        float temp = angle;
        angle = 10.0f*karmaDiff;

        if(angle != temp) { 
            lerpMax = angle;
            lerpMin = temp;
            t = 0.0f;
            if(OrbDifference > karmaDiff) {
                StartCoroutine(spawnOrb(playerOrbSpawn, playerOrbPrefab));
            } else if (OrbDifference < karmaDiff) {
                StartCoroutine(spawnOrb(enemyOrbSpawn, enemyOrbPrefab));
            }
        }

    }

    private void Update(){

        priorityDifference = (float)(enemy.Priority_Current - player.Priority_Current);
        setRotation(priorityDifference);

        if (t < 1.0f){

            t += 0.5f * Time.deltaTime;
            if(t > 1.0f){
                t = 1.0f;
            }
            MoveScales(Mathf.Lerp(lerpMin, lerpMax, t));
            
        }
    }

    private IEnumerator spawnOrb(Transform spawnPoint, GameObject orbType){
        yield return new WaitForSeconds(1.0f);

        Vector3 pos = spawnPoint.position;
        Debug.Log("Spawning new orb");
        GameObject newOrb = Instantiate(orbType, pos, Quaternion.identity, spawnPoint);

        if (OrbDifference > priorityDifference) {
            OrbDifference -= 1;
            if (OrbDifference != priorityDifference){
                StartCoroutine(spawnOrb(playerOrbSpawn, playerOrbPrefab));
            }
        } else if (OrbDifference < priorityDifference) {
            OrbDifference += 1;
            if (OrbDifference != priorityDifference){
                StartCoroutine(spawnOrb(enemyOrbSpawn, enemyOrbPrefab));
            }
        }
    }

}
