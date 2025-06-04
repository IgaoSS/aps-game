using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.Simple;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float speed = 3f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Player photonPlayer;
    private int idPlayer;

    [PunRPC]
    public void Initialize(Player player)
    {
        Debug.Log(player);

        photonPlayer = player;
        idPlayer = player.ActorNumber;

        Debug.Log("photon: " + photonPlayer);
        Debug.Log("id: " + idPlayer);
        Debug.Log(this);

        PhotonNetwork.Instantiate("Player", new Vector3(1f, 0, 0), Quaternion.identity);
        //GameController.Instance.Players.Add(this);
        //GameController.Instance.Players.Clear();

        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Vector3 movePosition = (speed * Time.fixedDeltaTime * moveDirection.normalized) + rb.position;
            rb.MovePosition(movePosition);
        }
    }
    // Testar no cel
    /*private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if(t.phase == TouchPhase.Moved)
            {
                transform.position += (Vector3)t.deltaPosition/600;
            }
        }
    }*/

    //Testar no pc
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical);

        //Celular
        if (photonView.IsMine)
        {

            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Moved)
                {
                    transform.position += (Vector3)t.deltaPosition / 200;
                }
            }
        }
    }
}
