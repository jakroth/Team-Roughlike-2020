using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    /* options for variables:
     * public - Show up in inspector and accessible by other scripts
     * [SerialiseField] private - Show up in inspector, not accessible by other scripts
     * [HideInInspector] public - Doesn't show in inspector, accessible by other scripts
     * private - Doesn't show in inspector, not accessible by other scripts
    */

    [Tooltip("How fast should this object move")]
    public float scrollSpeed;
    public const float ScrollWidth = 8;

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = pos.x - scrollSpeed * Time.deltaTime * GameController.speedModifier;
        if (transform.position.x < -ScrollWidth)
        {
            Offscreen(ref pos);
        }
        transform.position = pos;
    }

    protected virtual void Offscreen(ref Vector3 pos)
    {
        pos.x = pos.x + 2 * ScrollWidth;
    }

}
