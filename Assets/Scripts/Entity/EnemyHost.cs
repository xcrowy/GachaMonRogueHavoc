using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHost : Entity
{
    #region Object References
    public GameObject sign;
    private PlayerStateMachine playerStateMachine;

    #endregion

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        sign.SetActive(true);
        playerStateMachine = collision.tag == "Player" ? collision.GetComponent<Player>().PlayerStateMachine : null;
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        sign.SetActive(false);
    }

}
