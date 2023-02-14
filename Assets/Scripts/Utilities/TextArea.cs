using UnityEngine;

public class TextArea : MonoBehaviour
{
    [Header("Read this Dummy!!")]
    [TextArea(minLines: 5, maxLines: 10)]
    public string Notes = "";
}
