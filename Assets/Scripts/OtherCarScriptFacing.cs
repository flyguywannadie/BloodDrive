using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCarScriptFacing : MonoBehaviour
{
    [SerializeField] Transform forward;
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriterend;

    // Start is called before the first frame update
    void Start()
    {
        spriterend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameradirection = Camera.main.transform.position - transform.position;

		float angle = Vector3.Dot(forward.forward, cameradirection.normalized);
        float sideAngle = Vector3.Dot(forward.right, cameradirection.normalized);

        //transform.localRotation = Quaternion.Euler(0, angle, 0);
        Debug.Log(cameradirection.ToString());

        if (spriterend.sprite != sprites[0] && spriterend.sprite != sprites[2])
        {
            if (sideAngle < 0)
            {
				spriterend.flipX = true;
			} else
            {
				spriterend.flipX = false;
			}
        }

        if (angle < 0)
        {
            if (angle < -0.9)
            {
				spriterend.sprite = sprites[2];
			}
            else
            {
				spriterend.sprite = sprites[1];
			}
		} else
        {
			if (angle > 0.9)
			{
				spriterend.sprite = sprites[0];
			}
			else
			{
				spriterend.sprite = sprites[3];
			}
		}
    }
}
