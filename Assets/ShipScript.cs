using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{

    //direction ship travels in
    public Vector3 dir;
    //ship's speed
    public float speed = 5;

    //generates new bearing for ship
    private void calcShipDir(){
        float px = transform.localPosition.x;
        float py = transform.localPosition.y;
        float x;
        float y;
        if(px <= -50){
            x = Random.Range(0, 1.0f);
        }
        else if(px >= 50){
            x = Random.Range(-1.0f, 0);
        }
        else{
            x = Random.Range(1.0f, 1.0f);
        }
        if(py <= -50){
            y = Random.Range(0, 1.0f);
        }
        else if(py >= 50){
            y = Random.Range(-1.0f, 0);
        }
        else{
            y = Random.Range(1.0f, 1.0f);
        }
        dir = Vector3.Normalize(new Vector3(x, y, 0));
    }

    //checks if ship is still inside the map area
    private bool checkInsideCollider(){
        Vector2 point = new Vector2(transform.position.x, transform.position.y);
        Collider2D col = Physics2D.OverlapPoint(point);
        if(col == null){
            return false;
        }
        else{
            return true;
        }
    }

    // Start is called before the first frame update
    void Start(){
        //generates spawn point and target location to where it travells to
        int x = Random.Range(-50,50);
        int y = Random.Range(-50,50);
        transform.localPosition = new Vector3(x, y, 0);
        calcShipDir();
    }

    // Update is called once per frame
    void Update(){
        //checks if the ship is still inside the map
        if (checkInsideCollider() == false)
        {
            calcShipDir();
        }
        //moves ship
        transform.localPosition = transform.localPosition + dir * speed * Time.deltaTime;

    }
}
