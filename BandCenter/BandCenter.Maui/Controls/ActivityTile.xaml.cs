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
            nameof(MetricMarkup), typeof(string), typeof(ActivityTile), string.Empty);
        public string MetricMarkup
        {
            get => (string)GetValue(MetricMarkupProperty);
            set => SetValue(MetricMarkupProperty, value);
        }
    }
}