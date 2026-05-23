using UnityEngine;

public class BackGroundFollow : MonoBehaviour
{
    [SerializeField] private GameObject[] backGrounds;
    [SerializeField] private float[] moveSpeeds;
    private GameObject player;
    private Rigidbody2D playerRb;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < backGrounds.Length; i++)
        {
            if (i > 1)
            {
                backGrounds[i].transform.position += Vector3.right * playerRb.velocity.x * moveSpeeds[i] * Time.fixedDeltaTime;
            }
            else {
                backGrounds[i].transform.position += Vector3.right * moveSpeeds[i] * Time.fixedDeltaTime;
            }
            if (Mathf.Abs(player.transform.position.x - backGrounds[i].transform.position.x) >= 32)
            {
                backGrounds[i].transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            }
        }
    }
}