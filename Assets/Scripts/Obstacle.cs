using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class Obstacle : MonoBehaviourPunCallbacks
{
    public Rigidbody2D rb;
    public float speed;
    public int pointValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, Random.Range(-120, -65)));
    }

    [PunRPC]
    private void Update()
    {
        speed = PlayerPrefs.GetInt("level") * 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView && photonView.IsMine)
            {
                // Se o jogador é local, chama o método AddPoints no ScoreManager usando um RPC
                GameController.Instance.photonView.RPC("SubScore", RpcTarget.AllBuffered, photonView.ViewID, pointValue);
            }
            Destroy(gameObject);
        }
    }
}
