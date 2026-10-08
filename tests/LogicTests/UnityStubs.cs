// Minimal stand-ins for the UnityEngine types used by the engine-independent scripts under test.
// Only what Codes/CodeProfiles.cs needs; the real types come from Unity in the actual project.
namespace UnityEngine
{
    public class Object
    {
    }

    public enum RuntimeInitializeLoadType
    {
        AfterSceneLoad,
        BeforeSceneLoad,
        AfterAssembliesLoaded,
        BeforeSplashScreen,
        SubsystemRegistration
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class TextAreaAttribute : System.Attribute
    {
        public TextAreaAttribute(int minLines, int maxLines)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public sealed class RuntimeInitializeOnLoadMethodAttribute : System.Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
        {
        }
    }
}

namespace UnityEngine.Serialization
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public sealed class FormerlySerializedAsAttribute : System.Attribute
    {
        public FormerlySerializedAsAttribute(string oldName)
        {
        }
    }
}
