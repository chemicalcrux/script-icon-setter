using System;
using UnityEditor;
using UnityEngine;

namespace ChemicalCrux.ScriptIconSetter.Definitions
{
    [Serializable]
    public class ScriptAssetIconDefinition : IconDefinition
    {
        [SerializeField] private MonoScript script;

        public override bool Validate()
        {
            bool valid = base.Validate();

            if (script == null)
            {
                Debug.LogWarning("Missing script reference.");
                valid = false;
            }

            return valid;
        }

        public override ResolvedIcon Resolve()
        {
            return new ResolvedIcon(script.GetClass(), icon);
        }
    }
}