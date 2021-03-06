using MVP.Framework.Core;
using MVP.Framework.Views;

namespace MVP.Framework.Components.Windows
{
    public class Dialog : Component, ITrait
    {
        [Inspector]
        public bool animation;

        private Container container;

        public void Bind(Container container)
        {
            this.container = container;
            if (animation) container.PlayAnimation("open");
        }

        public TraitContext Adjust(TraitContext context)
        {
            return new TraitContext()
            {
                barrier = true,
                opaque = false,
            };
        }
    }
}