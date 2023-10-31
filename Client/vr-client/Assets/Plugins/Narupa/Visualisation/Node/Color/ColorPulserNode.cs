using System;
using Narupa.Visualisation.Properties;
using Narupa.Visualisation.Properties.Collections;
using Narupa.Visualisation.Property;
using UnityEngine;

namespace Narupa.Visualisation.Node.Color
{
    /// <summary>
    /// Applies an oscillating color change to a subset of particles.
    /// </summary>
    [Serializable]
    public class ColorPulserNode
    {
        [SerializeField]
        private ColorArrayProperty inputColors = new ColorArrayProperty();

        [SerializeField]
        private IntProperty count = new IntProperty();

        [SerializeField]
        private IntArrayProperty highlightFilter = new IntArrayProperty();

        private ColorArrayProperty outputColors = new ColorArrayProperty();

        private UnityEngine.Color[] cachedArray = new UnityEngine.Color[0];

        [SerializeField]
        private float speed = 6f;

        [SerializeField]
        private float maximum = 1f;

        [SerializeField]
        private float minimum = 0f;
        
        public bool IsInputDirty => inputColors.IsDirty
                                 || highlightFilter.IsDirty
                                 || count.IsDirty;

        private float strength = 1f;
        private float targetStrength = 1f;

        public void Refresh()
        {
            var isFilterNonZero = false;
            var isOutputValid = false;
            if (IsInputDirty)
            {
                isFilterNonZero = highlightFilter.HasNonEmptyValue();
                isOutputValid = inputColors.HasNonNullValue() || count.HasNonNullValue();

                if (inputColors.HasNonNullValue())
                {
                    Array.Resize(ref cachedArray, inputColors.Value.Length);
                    Array.Copy(inputColors.Value, cachedArray, inputColors.Value.Length);
                    
                    outputColors.Resize(cachedArray.Length);
                    Array.Copy(cachedArray, outputColors.Value, cachedArray.Length);
                    outputColors.MarkValueAsChanged();
                }
                else if (count.HasNonNullValue())
                {
                    Array.Resize(ref cachedArray, count.Value);
                    for (var i = 0; i < count.Value; i++)
                        cachedArray[i] = UnityEngine.Color.white;
                    
                    outputColors.Resize(cachedArray.Length);
                    Array.Copy(cachedArray, outputColors.Value, cachedArray.Length);
                    outputColors.MarkValueAsChanged();
                }
                else
                {
                    outputColors.UndefineValue();
                }

                highlightFilter.IsDirty = false;
                inputColors.IsDirty = false;
            }

            targetStrength = isFilterNonZero ? 1 : 0;

            if (isOutputValid && (strength != targetStrength || isFilterNonZero))
            {
                strength = Mathf.MoveTowards(strength, targetStrength, speed);
                var intensity = Mathf.Lerp(minimum, maximum,
                                           targetStrength *
                                           (0.5f + 0.5f * Mathf.Sin(speed * Time.time)));

                outputColors.Resize(cachedArray.Length);

                for (var i = 0; i < cachedArray.Length; i++)
                    outputColors.Value[i] = cachedArray[i];

                if (isFilterNonZero)
                {
                    var filter = highlightFilter.Value;
                    foreach (var i in filter)
                    {
                        outputColors.Value[i] += intensity * UnityEngine.Color.white;
                    }
                }

                outputColors.MarkValueAsChanged();
            }
        }
    }
}