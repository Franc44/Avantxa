using System.ComponentModel;
using Avantxa.iOS.Renderers;
using Avantxa.iOS.RenderersiOS;
using Avantxa.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEditorRenderer), typeof(BorderlessEditorRenderer))]
namespace Avantxa.iOS.Renderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        public new static void Init() { }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.Layer.BorderWidth = 0;
        }
    }
}