using System;
using Narupa.Visualisation.Properties.Collections;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Node.Calculator
{
    /// <summary>
    /// Calculates some 0-1 value based on indices.
    /// </summary>
    [Serializable]
    public abstract class GenericFractionNode : GenericOutputNode
    {
        private FloatArrayProperty fraction = new FloatArrayProperty();

        protected abstract override bool IsInputValid { get; }
        
        protected abstract override bool IsInputDirty { get; }

        protected abstract override void ClearDirty();
        
        protected abstract void GenerateArray(ref float[] array);

        protected override void UpdateOutput()
        {
            var array = fraction.HasValue ? fraction.Value : new float[0];
            GenerateArray(ref array);
            fraction.Value = array;
        }

        protected override void ClearOutput()
        {
            fraction.UndefineValue();
        }
    }
}