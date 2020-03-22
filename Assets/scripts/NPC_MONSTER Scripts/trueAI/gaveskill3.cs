using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaveskill3 : MonoBehaviour {
    private PlayerSkills playc;
    private GameObject player;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playc = player.GetComponent<PlayerSkills>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playc.skillIndex = 3;
            playc.SetCD();
            Destroy(gameObject);
        }
    }
}
