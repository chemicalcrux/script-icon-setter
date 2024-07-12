using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChemicalCrux.ScriptIconSetter
{
    public class IconSetterWindow : EditorWindow
    {
        private void OnEnable()
        {
            var icon = Resources.Load<Texture2D>("Script Icon Setter/window-icon");
            titleContent = new GUIContent(icon);
            titleContent.text = "Icon Setter";
        }

        void OnDestroy()
        {
            IconSettings.instance.SaveChanges();
        }

        public void CreateGUI()
        {
            SerializedObject obj = new(IconSettings.instance);

            SerializedProperty property = obj.FindProperty(nameof(IconSettings.scriptAssetIcons));
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

            var dryRun = new Button();
            dryRun.text = "Dry Run";
            dryRun.clicked += DryRun;
            row.Add(dryRun);
            dryRun.style.flexGrow = 1;

            var execute = new Button();
            execute.text = "Execute";
            execute.clicked += Execute;
            row.Add(execute);
            execute.style.flexGrow = 1;

            rootVisualElement.Add(row);
            rootVisualElement.TrackSerializedObjectValue(obj, Save);
        }

        [MenuItem("Window/Icon Setter")]
        public static void ShowWindow()
        {
            _ = GetWindow<IconSetterWindow>();
        }

        private List<IconChange> GetPlan()
        {
            List<ResolvedIcon> icons = new();

            if (!IconSettings.instance.TryResolve(icons))
                return new List<IconChange>();

            List<string> paths = new();

            foreach (DefaultAsset folder in IconSettings.instance.folders)
                paths.Add(AssetDatabase.GetAssetPath(folder));

            List<IconChange> plan = new();

            foreach (string guid in AssetDatabase.FindAssets("t:monoscript", paths.ToArray()))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                foreach (ResolvedIcon icon in icons)
                    if (icon.type.IsAssignableFrom(script.GetClass()))
                    {
                        plan.Add(new IconChange(script, icon.icon));
                        break;
                    }
            }

            return plan;
        }

        private void DryRun()
        {
            var plan = GetPlan();

            int validSteps = plan.Count(step => step.IsValid());

            Debug.Log($"{validSteps}/{plan.Count} icons will be changed.");
        }

        private void Execute()
        {
            var plan = GetPlan();

            int steps = 0;
            foreach (IconChange step in plan.Where(step => step.IsValid()))
            {
                step.Apply();
                ++steps;
            }

            Debug.Log($"Updated {steps} icon(s).");
        }
        
        void Save(SerializedObject _)
        {
            IconSettings.instance.SaveChanges();
        }
    }
}