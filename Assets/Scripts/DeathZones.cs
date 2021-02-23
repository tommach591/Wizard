using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZones : MonoBehaviour
{
	public bool inView;

    // Disable the behaviour when it becomes invisible...
    void OnBecameInvisible()
    {
        inView = false;
    }

    // ...and enable it again when it becomes visible.
    void OnBecameVisible()
    {
        inView = true;
    }
}
