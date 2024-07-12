using System;
using UnityEngine;

namespace ChemicalCrux.ScriptIconSetter.Definitions
{
    [Serializable]
    public abstract class IconDefinition
    {
        [SerializeField] protected Texture2D icon;

        public virtual bool Validate()
        {
            bool valid = true;

            if (icon == null)
            {
                Debug.LogWarning("Missing icon reference.");
                valid = false;
            }

            return valid;
        }

        public abstract ResolvedIcon Resolve();
    }
}