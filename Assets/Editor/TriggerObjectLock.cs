using UnityEditor;

static class TriggerObjectLock
{

    [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + L
    static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}