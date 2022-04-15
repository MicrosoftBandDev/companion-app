using System;
using System.Text.RegularExpressions;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BandCenter.Uno.Controls
{
    public sealed partial class MetricMarkupPresenter : UserControl
    {
        public MetricMarkupPresenter()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty MetricMarkupProperty = DependencyProperty.Register(
            nameof(MetricMarkup), typeof(string), typeof(MetricMarkupPresenter), new(string.Empty, MetricMarkup_PropertyChanged));
        public string MetricMarkup
        {
            get => (string)GetValue(MetricMarkupProperty);
            set => SetValue(MetricMarkupProperty, value);
        }

        private static void MetricMarkup_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue || e.NewValue is not string markup || d is not MetricMarkupPresenter presenter)
                return;

            UpdateMetricMarkup(presenter, markup);
        }

        private enum Format
        {
            None, Small, Italic, Bold
        }

        private void CommitMarkupSegment(string segment, Format format)
        {
            Run label = new()
            {
                Text = segment,
                FontSize = 32,
            };

            switch (format)
            {
                case Format.Small:
                    label.FontSize = 14;
                    break;

                case Format.Italic:
                    label.FontStyle = FontStyle.Italic;
                    break;

                case Format.Bold:
                    label.FontWeight = FontWeights.Bold;
                    break;
            }

            TextContent.Inlines.Add(label);
        }

        private static void UpdateMetricMarkup(MetricMarkupPresenter presenter, string markup)
        {
            presenter.TextContent.Inlines.Clear();

            if (string.IsNullOrEmpty(markup))
                return;

            var matches = Regex.Matches(markup, @"<([nsib])>(.*?)<\/\1>");
            if (matches.Count > 0)
            {
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
        }
    }
}
