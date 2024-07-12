using System;
using UnityEditor;
using UnityEngine;

namespace ChemicalCrux.ScriptIconSetter
{
    public class ResolvedIcon : IComparable<ResolvedIcon>
    {
        public readonly GUID guid;
        public readonly Texture2D icon;
        public readonly Type type;

        public ResolvedIcon(Type type, Texture2D icon)
        {
            this.type = type;
            this.icon = icon;

            guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(icon));
        }

        public int CompareTo(ResolvedIcon other)
        {
            if (type == null && other.type == null)
                return 0;

            if (type == null)
                return -1;

            if (other.type == null)
                return 1;

            if (type.IsAssignableFrom(other.type))
                return 1;

            if (other.type.IsAssignableFrom(type))
                return -1;

            return 0;
        }

        public override string ToString()
        {
            return $"{type} - {icon.name}";
        }
    }
}