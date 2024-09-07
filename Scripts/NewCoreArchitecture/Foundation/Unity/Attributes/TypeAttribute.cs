
using UnityEngine;
using System;

namespace PandasCanPlay.HexaWord.Utility
{
    public class TypeAttribute : PropertyAttribute
    {
        public Type[] types;
        public bool includeAbstracts;
        public bool showPartialName;
        public Type[] typesToExclude;

        public TypeAttribute(Type type, bool includeAbstracts, bool showPartialName, Type[] typesToExclude = null) : this(new Type[]{ type}, includeAbstracts, showPartialName)
        {

        }

        public TypeAttribute(Type[] types, bool includeAbstracts, bool showPartialName, Type[] typesToExclude = null)
        {
            this.types = types;
            this.includeAbstracts = includeAbstracts;
            this.showPartialName = showPartialName;
            this.typesToExclude = typesToExclude == null ? new Type[]{} : typesToExclude;
        }

        public TypeAttribute(Type type, bool includeAbstracts) : this(type, includeAbstracts, true)
        {
        }
    }
}