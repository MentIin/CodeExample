using System;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data
{
    public class NamedArrayAttribute : PropertyAttribute {
        public Type TargetEnum;
        public NamedArrayAttribute(Type TargetEnum) {
            this.TargetEnum = TargetEnum;
        }
    }
}