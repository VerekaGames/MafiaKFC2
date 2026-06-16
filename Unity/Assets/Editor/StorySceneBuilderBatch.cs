#if UNITY_EDITOR
using UnityEditor;

public static class StorySceneBuilderBatch
{
    public static void Build()
    {
        StorySceneBuilder.BuildScene();
        EditorApplication.Exit(0);
    }
}
#endif
