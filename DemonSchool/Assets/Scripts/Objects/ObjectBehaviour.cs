using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{

    // for grabbing these components from this object or the DungeonManager instance
    private ObjectGenerator objectGenerator;
    private SpriteRenderer spriteRenderer;

    // the spriteID for this enemy
    public int spriteID;

    // the location of this enemy on the map
    public Vector2Int pos;

    // temp variable used to pass collision test in PlayerBehaviour
    public bool isObject = true, isGirl;
    public Animator studentAni;

    void Start()
    {
        if(this.gameObject.tag == "boyStudent")
        {
            isGirl = false;
            studentAni.SetBool("isGirl", isGirl);
        }
        else if(this.gameObject.tag == "girlStudent")
        {
            isGirl = true;
            studentAni.SetBool("isGirl", isGirl);
        }
    }

    public void setObject(int spriteID, Vector2Int pos, int type)
    {
        // make sure these links exist
        if (objectGenerator == null)
        {
            objectGenerator = GameObject.Find("DungeonManager").GetComponent<ObjectGenerator>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //set sprite and position
        this.spriteID = spriteID;
        
        

        this.pos = pos;

        // update sprite on object
        if (type == 0)
        {
            spriteRenderer.sprite = objectGenerator.objectSprites[spriteID];

            // give the object a name in the Hierarchy
            gameObject.name = "Object (" + pos.x + "," + pos.y + "): " + spriteID;
        }
        else if(type == 1)
        {
            spriteRenderer.sprite = objectGenerator.studentSprites[spriteID];

            // give the student a name in the Hierarchy
            gameObject.name = "Student (" + pos.x + "," + pos.y + "): " + spriteID;
            if (this.spriteID == 0)
            {
                this.gameObject.tag = "boyStudent";
            }
            else if(this.spriteID == 1)
            {
                this.gameObject.tag = "girlStudent";
            }
        }

    }

   


}
