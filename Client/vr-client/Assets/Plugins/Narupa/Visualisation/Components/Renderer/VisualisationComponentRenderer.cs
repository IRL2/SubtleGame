// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using UnityEngine;

namespace Narupa.Visualisation.Components.Renderer
{
    public abstract class VisualisationComponentRenderer<T> : VisualisationComponent<T>
        where T : new()
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Camera.onPreCull += Render;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Camera.onPreCull -= Render;
        }

        protected abstract void Render(Camera camera);

        private void Update()
        {
            if (!Application.isPlaying) UpdateInEditor();
        }

        protected virtual void UpdateInEditor()
        {
        }
    }
}