using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace BandCenter.Maui.Controls
{
    public partial class ActivityTile : ContentView
    {
        public ActivityTile()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
            nameof(Subtitle), typeof(string), typeof(ActivityTile), string.Empty, propertyChanged: Subtitle_PropertyChanged);
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly BindableProperty MetricMarkupProperty = BindableProperty.Create(
            nameof(MetricMarkup), typeof(string), typeof(ActivityTile), string.Empty, propertyChanged: MetricMarkup_PropertyChanged);
        public string MetricMarkup
        {
            get => (string)GetValue(MetricMarkupProperty);
            set => SetValue(MetricMarkupProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter), typeof(object), typeof(ActivityTile), null);
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command), typeof(ICommand), typeof(ActivityTile), null);
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            nameof(IconSource), typeof(ImageSource), typeof(ActivityTile), null, propertyChanged: IconSource_PropertyChanged);
        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        private static void Subtitle_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // For some reason bindings just aren't working properly. ActivityTile gets the prop
            // changed event, but the Label never gets the new value.

            if (bindable is not ActivityTile tile)
                return;

            tile.SubtitleLabel.Text = newValue?.ToString();
        }

        private static void MetricMarkup_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // For some reason bindings just aren't working properly. ActivityTile gets the prop
            // changed event, but the MetricMarkupPresenter never gets the new value.

            if (bindable is not ActivityTile tile)
                return;

            tile.MarkupPresenter.MetricMarkup = newValue?.ToString();
        }

        private static void IconSource_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // For some reason bindings just aren't working properly. ActivityTile gets the prop
            // changed event, but the Image never gets the new value.

            if (bindable is not ActivityTile tile || newValue is not ImageSource source)
                return;

            tile.ImageElement.Source = source;
        }
    }
}