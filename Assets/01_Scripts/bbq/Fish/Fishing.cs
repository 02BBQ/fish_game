using System;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    [SerializeField] private GameObject _aim;
    public Player player {get; private set;}
    private PlayerMovement playerMovement => player.playerMovement;
    
    public bool isFishing {get; private set;}

    void Awake()
    {
        _aim.SetActive(false);
        player = GetComponentInParent<Player>();
        if (player == null){
            Debug.LogWarning("Something's Wrong...");
            Destroy(gameObject);
        }

        player.playerInput.FishingDown += handleHoldStart;
        player.playerInput.FishingUp += handleHoldEnd;
        // player.playerInput
    }

    void handleHoldStart()
    {
        if (!isFishing)
            Aim();
    }

    private void Aim()
    {
        _aim.SetActive(true);
    }

    void handleHoldEnd()
    {
        if (!isFishing)
            Fish();
    }

    private void Fish()
    {
        isFishing = true;
    }
}
