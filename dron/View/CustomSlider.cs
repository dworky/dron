using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace dron.View
{
    public class CustomSlider : Slider
    {

        public double FinalValue
        {
            get { return (double)GetValue(FinalValueProperty); }
            set { SetValue(FinalValueProperty, value); }
        }

        public static readonly DependencyProperty FinalValueProperty =
            DependencyProperty.Register(
                "FinalValue", typeof(double), typeof(CustomSlider),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            FinalValue = Value;
        }
    }
}
