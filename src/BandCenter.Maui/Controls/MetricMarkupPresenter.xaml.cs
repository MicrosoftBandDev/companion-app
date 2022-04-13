using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BandCenter.Maui.Controls
{
    public partial class MetricMarkupPresenter : ContentView
    {
        public MetricMarkupPresenter()
        {
            InitializeComponent();
        }

        // TODO: Currently, the way markup is converted to TextBlocks requires that the markup
        // be completely reparsed when any of the styles change. It also means the markup property
        // has to be set after the style properties, otherwise the markup will be parsed once
        // without any styles, and an additional four times for each style. This is horribly
        // inefficient, and we should probably be using a style selector instead of a C# switch.

        #region Style properties

        public static readonly BindableProperty NoneTextStyleProperty = BindableProperty.Create(
            nameof(NoneTextStyle), typeof(Style), typeof(MetricMarkupPresenter), null, propertyChanged: Style_PropertyChanged);
        public Style NoneTextStyle
        {
            get => (Style)GetValue(NoneTextStyleProperty);
            set => SetValue(NoneTextStyleProperty, value);
        }

        public static readonly BindableProperty SmallTextStyleProperty = BindableProperty.Create(
            nameof(SmallTextStyle), typeof(Style), typeof(MetricMarkupPresenter), null, propertyChanged: Style_PropertyChanged);
        public Style SmallTextStyle
        {
            get => (Style)GetValue(SmallTextStyleProperty);
            set => SetValue(SmallTextStyleProperty, value);
        }

        public static readonly BindableProperty ItalicTextStyleProperty = BindableProperty.Create(
            nameof(ItalicTextStyle), typeof(Style), typeof(MetricMarkupPresenter), null, propertyChanged: Style_PropertyChanged);
        public Style ItalicTextStyle
        {
            get => (Style)GetValue(ItalicTextStyleProperty);
            set => SetValue(ItalicTextStyleProperty, value);
        }

        public static readonly BindableProperty BoldTextStyleProperty = BindableProperty.Create(
            nameof(BoldTextStyle), typeof(Style), typeof(MetricMarkupPresenter), null, propertyChanged: Style_PropertyChanged);
        public Style BoldTextStyle
        {
            get => (Style)GetValue(BoldTextStyleProperty);
            set => SetValue(BoldTextStyleProperty, value);
        }

        #endregion

        public static readonly BindableProperty MetricMarkupProperty = BindableProperty.Create(
            nameof(MetricMarkup), typeof(string), typeof(MetricMarkupPresenter), string.Empty, propertyChanged: MetricMarkup_PropertyChanged);
        public string MetricMarkup
        {
            get => (string)GetValue(MetricMarkupProperty);
            set => SetValue(MetricMarkupProperty, value);
        }

        private static void MetricMarkup_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue || newValue is not string markup || bindable is not MetricMarkupPresenter presenter)
                return;

            UpdateMetricMarkup(presenter, markup);
        }

        private enum Format
        {
            None, Small, Italic, Bold
        }

        private void CommitMarkupSegment(string segment, Format format)
        {
            Span label = new()
            {
                Text = segment,
                Style = format switch
                {
                    Format.Small => SmallTextStyle,
                    Format.Italic => ItalicTextStyle,
                    Format.Bold => BoldTextStyle,

                    Format.None or
                    _ => NoneTextStyle
                }
            };
            TextContent.FormattedText.Spans.Add(label);
        }

        private static void Style_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var presenter = (MetricMarkupPresenter)bindable;
            UpdateMetricMarkup(presenter, presenter.MetricMarkup);
        }

        private static void UpdateMetricMarkup(MetricMarkupPresenter presenter, string markup)
        {
            presenter.TextContent.FormattedText.Spans.Clear();

            if (string.IsNullOrEmpty(markup))
                return;

            var matches = Regex.Matches(markup, @"<([nsib])>(.*?)<\/(?:\1)>");
            if (matches.Count > 0)
            {
                //IList<(int idx, int len)> specialRanges = matches.OrderBy(m => m.Index).Select(m => (m.Index, m.Length)).ToList();
                System.Text.StringBuilder curSegment = new();
                int specialRangeIdx = 0;
                Match curSpecialRange = matches[specialRangeIdx];
                for (int i = 0; i < markup.Length;)
                {
                    if (curSpecialRange.Index == i)
                    {
                        // We're now in a marked up segment
                        // Commit the previous segment
                        if (curSegment != null)
                        {
                            presenter.CommitMarkupSegment(curSegment.ToString(), Format.None);
                            curSegment.Clear();
                        }

                        // Commit the marked up segment
                        var format = matches[specialRangeIdx].Groups[1].Value switch
                        {
                            "n" => Format.None,
                            "s" => Format.Small,
                            "i" => Format.Italic,
                            "b" => Format.Bold,
                            _ => throw new FormatException($"Invalid format type.")
                        };
                        presenter.CommitMarkupSegment(curSpecialRange.Groups[2].Value, format);
                        // Jump to end of segment
                        i += curSpecialRange.Length;

                        // Select next markup segment
                        if (++specialRangeIdx < matches.Count)
                            curSpecialRange = matches[specialRangeIdx];
                    }
                    else
                    {
                        // We're in a plain-text segment
                        // Add character to current segment
                        curSegment.Append(markup[i++]);
                    }
                }

                // Make sure last segment is committed
                if (curSegment.Length > 0)
                {
                    presenter.CommitMarkupSegment(curSegment.ToString(), Format.None);
                    curSegment.Clear();
                }
            }
            else
            {
                // No special markup
                presenter.CommitMarkupSegment(markup, Format.None);
            }

            // Ugly hack to make sure the Label updates
            presenter.TextContent.FormattedText = presenter.TextContent.FormattedText;
        }
    }
}