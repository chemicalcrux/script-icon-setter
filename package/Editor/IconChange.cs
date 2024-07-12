using UnityEditor;
using UnityEngine;

namespace ChemicalCrux.ScriptIconSetter
{
    public class IconChange
    {
        public readonly Texture2D newIcon;
        public readonly MonoScript script;

        public IconChange(MonoScript script, Texture2D newIcon)
        {
            this.script = script;
            this.newIcon = newIcon;
        }

        public bool IsValid()
        {
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script)) as MonoImporter;
            return importer.GetIcon() != newIcon;
        }

        public void Apply()
        {
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script)) as MonoImporter;
            importer.SetIcon(newIcon);
            importer.SaveAndReimport();
        }
    }
}