using Avantxa.Controls;
using Avantxa.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedListView), typeof(ExtendedListViewRenderer))]
namespace Avantxa.iOS.Renderers
{
    public class ExtendedListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control != null)
                {
                    Control.AllowsSelection = false;
                    Control.AlwaysBounceVertical = false;
                    Control.Bounces = false;
                }
            }
        }
    }
}
