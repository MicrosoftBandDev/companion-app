using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BandCenter.Maui.Controls
{
    public partial class ActivityTile : ContentView
    {
        public ActivityTile()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
            nameof(Subtitle), typeof(string), typeof(ActivityTile), string.Empty);
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

        private static void MetricMarkup_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // For some reason bindings just aren't working properly. ActivityTile gets the prop
            // changed event, but the MetricMarkupPresenter never gets the new value.

            if (bindable is not ActivityTile tile)
                return;

            tile.MarkupPresenter.MetricMarkup = newValue?.ToString();
        }
    }
}