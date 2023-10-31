// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Core.Science;
using Narupa.Visualisation.Node.Color;
using UnityEditor;
using UnityEngine;

namespace Narupa.Visualisation.Editor
{
    /// <summary>
    /// Property drawer to render element palette assignments in the editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(ElementColorMapping.ElementColorAssignment))]
    public sealed class ElementColorAssignmentPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var atomicNumberProperty =
                property.FindPropertyRelative(
                    nameof(ElementColorMapping.ElementColorAssignment.atomicNumber));

            var colorProperty =
                property.FindPropertyRelative(
                    nameof(ElementColorMapping.ElementColorAssignment.color));

            // Get the element name to display
            var elementId = atomicNumberProperty.intValue;
            Element? element = null;
            if (Enum.IsDefined(typeof(Element), elementId))
                element = (Element) elementId;

            // Prefix each line with the element name label
            position = EditorGUI.PrefixLabel(position,
                                             new GUIContent(element.ToString() ?? ""));

            var numberBoxWidth = Mathf.Min(72f, position.width / 2f);

            var numberRect = new Rect(position.x,
                                      position.y,
                                      numberBoxWidth,
                                      position.height);
            var colorRect = new Rect(position.x + numberBoxWidth + 8,
                                     position.y,
                                     position.width - 8 - numberBoxWidth,
                                     position.height);

            EditorGUI.PropertyField(numberRect,
                                    atomicNumberProperty,
                                    GUIContent.none);

            EditorGUI.PropertyField(colorRect,
                                    colorProperty,
                                    GUIContent.none);
        }
    }
}