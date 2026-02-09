using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private ShootingRangeHealth srh;
    
    private PlayerHUD playerHUD;
    [SerializeField] private GameObject player;
    
    private int playerDistanceIndex = 1;
    [SerializeField] private GameObject playerPositions;
    private readonly List<Transform> distancePoints = new();
    
    private void Start()
    {
        playerHUD = player.GetComponent<PlayerHUD>();
        srh = FindFirstObjectByType<ShootingRangeHealth>();
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
        srh.UpdateDotSize(distancePoints[playerDistanceIndex].position.z);
        player.transform.position = distancePoints[playerDistanceIndex].position;
        playerHUD.Health("Distance: " + distancePoints[playerDistanceIndex].name);
    }
}
