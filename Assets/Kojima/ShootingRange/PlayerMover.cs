using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // pegar o player
    // setar a vida como distancia
    // ter um alvo que mostra onde foram os tiros
    // adicionar mensagens com o sistema de effects no player (tipo bleeding tals) pra falar os botoes de controle
    //  R pra resetar a board
    // fazer algo pra calcular o MOA e spread dependendo dos tiros na board
    
    [SerializeField] private GameObject player;
    private PlayerHUD playerHUD;
    
    private int playerDistanceIndex = 1;
    [SerializeField] private GameObject playerPositions;
    private readonly List<Transform> distancePoints = new();
    
    private void Start()
    {
        playerHUD = player.GetComponent<PlayerHUD>();
        foreach (Transform point in playerPositions.transform) distancePoints.Add(point);
        MovePlayer();
    }

    private void Update()
    {
        // F to toggle snap positions in the future
        if (Input.GetKeyDown(KeyCode.W)) MovePlayerForward();
        if (Input.GetKeyDown(KeyCode.S)) MovePlayerBackward();
    }
    
    private void MovePlayerForward()
    {
        if (playerDistanceIndex <= 0) return;
        
        playerDistanceIndex--;
        MovePlayer();
    }
    private void MovePlayerBackward()
    {
        if (playerDistanceIndex >= distancePoints.Count-1) return;
        
        playerDistanceIndex++;
        MovePlayer();
    }

    private void MovePlayer()
    {
        player.transform.position = distancePoints[playerDistanceIndex].position;
        playerHUD.Health("Distance: " + distancePoints[playerDistanceIndex].name);
    }
}
