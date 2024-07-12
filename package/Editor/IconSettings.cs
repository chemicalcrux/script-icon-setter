using System.Collections.Generic;
using ChemicalCrux.ScriptIconSetter.Definitions;
using UnityEditor;
using UnityEngine;

namespace ChemicalCrux.ScriptIconSetter
{
    [FilePath("chemicalcrux/script-icon-setter/icons.yaml", FilePathAttribute.Location.ProjectFolder)]
    public class IconSettings : ScriptableSingleton<IconSettings>
    {
        public List<ScriptAssetIconDefinition> scriptAssetIcons;
        public List<DefaultAsset> folders;
        
        public bool Validate()
        {
            bool valid = true;
            
            for (int i = 0; i < scriptAssetIcons.Count; ++i)
            {
                if (!scriptAssetIcons[i].Validate())
                {
                    valid = false;
                    Debug.LogWarning($"Prior warning is from script asset icon at index ${i}");
                }
            }

            return valid;
        }

        public bool TryResolve(List<ResolvedIcon> outputList)
        {
            outputList.Clear();

            if (!Validate())
            {
                Debug.LogWarning("Validation failed. Aborting.");
                return false;
            }
            
            foreach (var setting in scriptAssetIcons)
            {
                outputList.Add(setting.Resolve());
            }

            outputList.Sort();

            return true;
        }
    }
}
