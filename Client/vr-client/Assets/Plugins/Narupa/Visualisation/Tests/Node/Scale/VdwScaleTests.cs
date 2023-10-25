using Narupa.Core.Science;
using Narupa.Visualisation.Node.Scale;
using Narupa.Visualisation.Properties.Collections;
using Narupa.Visualisation.Property;
using NUnit.Framework;

namespace Narupa.Visualisation.Tests.Node.Scale
{
    public class VdwScaleTests
    {
        private Element[] ValidElements = {Element.Carbon, Element.Hydrogen};
        
        [Test]
        public void NullScale()
        {
            var node = new VdwScaleNode();

            node.Elements.Value = ValidElements;
            node.Scale.UndefineValue();

            node.Refresh();
            
            Assert.IsFalse(node.Scales.HasNonNullValue());
        }
        
        [Test]
        public void NullElements()
        {
            var node = new VdwScaleNode();

            node.Scale.Value = 1f;
            node.Elements.UndefineValue();

            node.Refresh();
            
            Assert.IsFalse(node.Scales.HasNonNullValue());
        }
        
        [Test]
        public void EmptyElements()
        {
            var node = new VdwScaleNode();

            node.Elements.Value = new Element[0];
            node.Scale.Value = 1f;

            node.Refresh();
            
            Assert.IsFalse(node.Scales.HasNonNullValue());
        }
        
        [Test]
        public void ValidInput()
        {
            var node = new VdwScaleNode();

            node.Elements.Value = ValidElements;
            node.Scale.Value = 1f;

            node.Refresh();
            
            Assert.IsTrue(node.Scales.HasNonNullValue());
        }
        
        [Test]
        public void OnlyRefreshOnce()
        {
            var node = new VdwScaleNode();
            var linked = new FloatArrayProperty
            {
                LinkedProperty = node.Scales, 
                IsDirty = false
            };
            Assert.IsFalse(linked.IsDirty);

            node.Elements.Value = ValidElements;
            node.Scale.Value = 1f;

            node.Refresh();

            Assert.IsTrue(linked.IsDirty);
            linked.IsDirty = false;
            
            node.Refresh();
            
            Assert.IsFalse(linked.IsDirty);
        }
    }
}