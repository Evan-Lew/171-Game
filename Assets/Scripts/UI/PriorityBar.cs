using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class PriorityBar : MonoBehaviour
{
    private Slider slider;
    private ParticleSystem particleSys;
    public float fillSpeed = 2.0f;
    public double targetPriority = 10.0f;
    public Image mask;
    public Character player;
    public Character enemy;


    // Start is called before the first frame update
    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        //particleSys = GameObject.Find("PriorityBarParticles").GetComponent<ParticleSystem>();
        mask.fillAmount = slider.value/20;
    }

    // Update is called once per frame
    void Update()
    {
        moveBar();

        if (slider.value < targetPriority) {

            slider.value += fillSpeed * Time.deltaTime;
            mask.fillAmount += fillSpeed/20 * Time.deltaTime;
            // if (!particleSys.isPlaying)
            //     particleSys.Play();

        } else if(slider.value > targetPriority + 0.05) {

            slider.value -= fillSpeed * Time.deltaTime;
            mask.fillAmount -= fillSpeed/20 * Time.deltaTime;
            // if (!particleSys.isPlaying)
            //     particleSys.Play();

        } else {
            // particleSys.Stop();
        }
    }

    public void moveBar(){
        targetPriority = enemy.Priority_Current - player.Priority_Current + 10;
    }
}
