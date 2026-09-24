using UnityEngine;

namespace AV.Framework.Core.Configuration
{
    public sealed class EnvironmentSettingsSO : ScriptableObject
    {
        [Header("Feature Toggles")]
        [SerializeField] private bool analyticsEnabled = true;
        [SerializeField] private bool srDebuggerEnabled = true;

        public bool AnalyticsEnabled => analyticsEnabled;
        public bool SRDebuggerEnabled => srDebuggerEnabled;
    }
}
