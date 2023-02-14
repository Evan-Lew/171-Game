using UnityEditor;

static class TriggerObjectLock
{
    //To create a hotkey, use the following special characters: % (ctrl on Windows and Linux, cmd on macOS), ^ (ctrl on Windows, Linux, and macOS), # (shift), & (alt). If no special modifier key combinations are required, the key can be given after an underscore. For example, to create a menu with the hotkey Shift+Alt+G, use "MyMenu/Do Something #&g". To create a menu with hotkey G and no key modifiers pressed, use "MyMenu/Do Something _g".
    [MenuItem("Tools/Toggle Inspector Lock _1")] // #5 is numberpad 5, ctrl + l is %l, # is shift
    static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}