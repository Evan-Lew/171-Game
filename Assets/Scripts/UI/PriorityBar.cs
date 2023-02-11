using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityBar : MonoBehaviour
{
    private Slider slider;
    private ParticleSystem particleSys;
    public float fillSpeed = 0.4f;
    public float targetPriority = 0.5f;

    // Start is called before the first frame update
    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        particleSys = GameObject.Find("PriorityBarParticles").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(slider.value < targetPriority - 0.05) {

            slider.value += fillSpeed * Time.deltaTime;
            if(!particleSys.isPlaying)
                particleSys.Play();

        } else if(slider.value > targetPriority + 0.05) {

            slider.value -= fillSpeed * Time.deltaTime;
            if(!particleSys.isPlaying)
                particleSys.Play();

        } else {
            particleSys.Stop();
        }
    }

    public void moveBar(double newValue){
        targetPriority = (float)newValue;
    }
}
