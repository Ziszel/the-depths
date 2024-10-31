using UnityEngine;

public class InteractHelpUI : MonoBehaviour
{
    public Transform[] ChildGameObjects;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChildGameObjects = GetComponentsInChildren<Transform>();
        SetWhetherChildrenActive(false);
    }

    public void SetWhetherChildrenActive(bool shouldBeActive)
    {
        foreach (Transform child in ChildGameObjects)
        {
            child.gameObject.SetActive(shouldBeActive);
        }
    }
}
