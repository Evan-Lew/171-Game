using UnityEngine;

public class TextArea : MonoBehaviour
{
    [Header("Note")]
    [TextArea(minLines: 5, maxLines: 10)]
    public string Notes = "";
}
