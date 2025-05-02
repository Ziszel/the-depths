using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNodeManager : MonoBehaviour
{
    [Header("AI related")]
    [SerializeField] private List<Vector3> pathNodes;
    
    private Transform _playerTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerTransform = FindFirstObjectByType<PlayerController>().transform;
    }
    
    public List<Vector3> GetClosestPathNodesToPlayer(int numberOfNodes)
    {
        pathNodes = pathNodes.OrderBy(node => Vector3.Distance(_playerTransform.transform.position, node)).ToList();

        List<Vector3> closestNodes = new List<Vector3>();
        
        for (int i = 0; i < numberOfNodes; i++)
        {
            closestNodes.Add(pathNodes[i]);
        }
        
        return closestNodes;
    }
}
