using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.Frontend.Controls
{
    public partial class FormField : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(FormField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(FormField), new PropertyMetadata(null));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public FormField()
        { 
            InitializeComponent();
        }
    }
} 