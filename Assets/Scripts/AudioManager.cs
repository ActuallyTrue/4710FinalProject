using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public List<AudioSource> sfx;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void leftStep()
    {
        sfx[5].PlayOneShot(sfx[5].clip);
        Debug.Log(sfx[5].clip.name);
    }
    public void rightStep()
    {
        sfx[6].PlayOneShot(sfx[6].clip);
        Debug.Log(sfx[6].clip.name);
    }
}
