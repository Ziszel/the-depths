using UnityEngine;

[CreateAssetMenu(fileName = "FileItem", menuName = "Scriptable Objects/FileItem")]
public class FileItem : PickupItem
{
    // Used to add the corresponding JSON FileData to the inventory
    [Tooltip("File ID (only used for files)")]
    public int fileId;
}
