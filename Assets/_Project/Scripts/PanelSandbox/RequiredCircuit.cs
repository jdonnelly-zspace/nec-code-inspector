using UnityEngine.Serialization;
using System;
using UnityEngine;

namespace NECInspector.PanelSandbox
{
    [Serializable]
    public class RequiredCircuit
    {
        public string circuitName;
        public int ampsRequired;
        public string wireGauge;       // e.g., "12 AWG"
        public int poleCount = 1;      // 1 for 120V, 2 for 240V
        public bool requiresGFCI;
        public bool requiresAFCI;
        [FormerlySerializedAs("necReference")]
        public string codeReference;   // e.g., "210.11(C)(3)"
        public bool isRequired = true;
        public string room;            // room type the circuit serves (see ScopeVocabulary), so its GFCI/AFCI flags can be checked against the code's scope
        public string appliance;       // the appliance it serves, if any

        [TextArea(1, 2)]
        public string description;
    }
}
