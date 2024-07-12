using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChemicalCrux.ScriptIconSetter
{
    public class IconSetterWindow : EditorWindow
    {
        [MenuItem("Window/Icon Setter")]
        public static void ShowWindow()
        {
            IconSetterWindow _ = GetWindow<IconSetterWindow>();
        }

        public void CreateGUI()
        {
            Debug.Log("Creating GUI");
            SerializedObject obj = new(IconSettings.instance);

            var property = obj.FindProperty(nameof(IconSettings.scriptAssetIcons));
            var list = new PropertyField(property);
            rootVisualElement.Add(list);

            list.Bind(obj);

            var folders = new PropertyField(obj.FindProperty(nameof(IconSettings.folders)));
            folders.Bind(obj);
            rootVisualElement.Add(folders);

            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            var validate = new Button();
            validate.text = "Validate";
            validate.clicked += () => IconSettings.instance.Validate();
            row.Add(validate);
            validate.style.flexGrow = 1;

            var resolve = new Button();
            resolve.text = "Resolve";
            resolve.clicked += Execute;
            row.Add(resolve);
            resolve.style.flexGrow = 1;

            rootVisualElement.Add(row);
        }

        void Execute()
        {
            List<ResolvedIcon> icons = new();

            if (!IconSettings.instance.TryResolve(icons))
                return;

            foreach (var icon in icons)
                Debug.Log(icon);

            List<string> paths = new();

            foreach (var folder in IconSettings.instance.folders)
            {
                paths.Add(AssetDatabase.GetAssetPath(folder));
            }
            
            foreach (var guid in AssetDatabase.FindAssets("t:monoscript", paths.ToArray()))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log(path);
                
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;

                foreach (var icon in icons)
                {
                    if (icon.type.IsAssignableFrom(script.GetClass()))
                    {
                        Debug.Log($"{icon.type} matched {script.GetClass()}");
                        importer.SetIcon(icon.icon);
                        importer.SaveAndReimport();
                        
                        break;
                    }
                }
            }

        }
    }
}