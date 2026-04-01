using UnityEngine;

public class BroadcasterObject : MonoBehaviour
{
    public delegate void BroadcastDelegate();
    public static event BroadcastDelegate OnBroadCastOne;
    public static event BroadcastDelegate OnBroadCastTwo;
    public delegate void SpaceBarDelegate(float x, float y);

    public static event SpaceBarDelegate OnSpaceBarPress;


    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))OnSpaceBarPress(transform.position.x, transform.position.y);
        if (Input.GetKeyDown(KeyCode.W)) Debug.Log("W is pressed!");
        if (Input.GetKeyDown(KeyCode.A)) Debug.Log("A is pressed!");
        if (Input.GetKeyDown(KeyCode.S)) Debug.Log("S is pressed!");
        if (Input.GetKeyDown(KeyCode.D)) Debug.Log("D is pressed!");

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }
}
