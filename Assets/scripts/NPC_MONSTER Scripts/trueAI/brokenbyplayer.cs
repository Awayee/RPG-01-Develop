using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenbyplayer : MonoBehaviour {
    private int value;

	// Use this for initialization
	void Start () {
        value = 100;

    }

    // Update is called once per frame

    public void beAttack(bool isamallsikill)
    {
        if (!isamallsikill)
        {
            Destroy(gameObject);
        }
        if (isamallsikill)
        {
            value = value - 26;

            if (value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "playerskill")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "playersmallskill")
        {
            value = value - 26;

            if (value <= 0)
            {
                Destroy(gameObject);
            }
        }


    }
}
