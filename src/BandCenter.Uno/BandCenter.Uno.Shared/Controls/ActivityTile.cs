using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BandCenter.Uno.Controls
{
    public sealed partial class ActivityTile : Control
    {
        public ActivityTile()
        {
            this.DefaultStyleKey = typeof(ActivityTile);
        }

        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
           nameof(Subtitle), typeof(string), typeof(ActivityTile), new(string.Empty));
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly DependencyProperty MetricMarkupProperty = DependencyProperty.Register(
            nameof(MetricMarkup), typeof(string), typeof(ActivityTile), new(string.Empty));
        public string MetricMarkup
        {
            get => (string)GetValue(MetricMarkupProperty);
            set => SetValue(MetricMarkupProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon), typeof(IconSource), typeof(ActivityTile), new(null));
        public IconSource Icon
        {
            get => (IconSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
