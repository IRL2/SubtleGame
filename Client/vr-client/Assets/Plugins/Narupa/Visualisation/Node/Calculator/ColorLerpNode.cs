using System;
using Narupa.Visualisation.Properties;
using Narupa.Visualisation.Properties.Collections;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Node.Calculator
{
    [Serializable]
    public class ColorLerpNode : LerpNode<UnityEngine.Color, ColorArrayProperty>
    {
        [SerializeField]
        private FloatProperty speed = new FloatProperty()
        {
            Value = 1f
        };

        private float Delta => Application.isPlaying ? speed * Time.deltaTime : 999f;

        protected override UnityEngine.Color MoveTowards(UnityEngine.Color current, UnityEngine.Color target)
        {
            return new UnityEngine.Color(
                Mathf.MoveTowards(current.r, target.r, Delta),
                Mathf.MoveTowards(current.g, target.g, Delta),
                Mathf.MoveTowards(current.b, target.b, Delta),
                Mathf.MoveTowards(current.a, target.a, Delta)
            );
        }
    }
}