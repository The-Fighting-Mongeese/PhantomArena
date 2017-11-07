using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasedMaterial : MonoBehaviour {

    public Material physicalMaterial, phantomMaterial;
    public ParticleSystem physicalParticle, phantomParticle;

    private Renderer ren;
    private int physicalLayer, phantomLayer;


	void Start ()
    {
        ren = GetComponent<Renderer>();
        physicalLayer = LayerMask.NameToLayer("Physical");
        phantomLayer = LayerMask.NameToLayer("Phantom");
    }

    public void ShowPhase(int phase)
    {
        if (phase == physicalLayer)
        {
            ren.material = physicalMaterial;
            if (phantomParticle != null)
                phantomParticle.Stop();
            if (physicalParticle != null)
                physicalParticle.Play();
        }
        else if (phase == phantomLayer)
        {
            ren.material = phantomMaterial;
            if (physicalParticle != null)
                physicalParticle.Stop();
            if (phantomParticle != null)
                phantomParticle.Play();
        }
        else
        {
            Debug.Log("WARNING | PhasedMaterial: Switching to unknown phase - " + phase);
        }
    }
}
