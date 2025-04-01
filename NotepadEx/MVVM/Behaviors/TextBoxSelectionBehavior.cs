using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using NotepadEx.Util;
using Point = System.Windows.Point;

namespace NotepadEx.MVVM.Behaviors;

public class TextBoxSelectionBehavior : Behavior<TextBox>
{
    bool isMouseDown;
    bool isScrollbarDragging;
    DispatcherTimer scrollTimer;
    const double ScrollSpeed = 75;
    ScrollViewer scrollViewer;
    ScrollBar verticalScrollBar;
    ScrollBar horizontalScrollBar;
    double verticalOffset;
    double horizontalOffset;
    const double SCROLL_ZONE_PERCENTAGE = 0.15;

    #region Dependency Properties
    public static readonly DependencyProperty SelectionChangedCommandProperty =
        DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand),
            typeof(TextBoxSelectionBehavior));

    public static readonly DependencyProperty TextChangedCommandProperty =
        DependencyProperty.Register(nameof(TextChangedCommand), typeof(ICommand),
            typeof(TextBoxSelectionBehavior));

    public static readonly DependencyProperty PreviewKeyDownCommandProperty =
        DependencyProperty.Register(nameof(PreviewKeyDownCommand), typeof(ICommand),
            typeof(TextBoxSelectionBehavior));

    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    public ICommand TextChangedCommand
    {
        get => (ICommand)GetValue(TextChangedCommandProperty);
        set => SetValue(TextChangedCommandProperty, value);
    }

    public ICommand PreviewKeyDownCommand
    {
        get => (ICommand)GetValue(PreviewKeyDownCommandProperty);
        set => SetValue(PreviewKeyDownCommandProperty, value);
    }
    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        var textBox = AssociatedObject;
        textBox.Loaded += TextBox_Loaded;
        textBox.PreviewMouseDown += TextBox_MouseDown;
        textBox.PreviewMouseUp += TextBox_MouseUp;
        textBox.PreviewMouseMove += TextBox_MouseMove;
        textBox.SelectionChanged += TextBox_SelectionChanged;
        textBox.TextChanged += TextBox_TextChanged;
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

        scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        scrollTimer.Tick += ScrollTimer_Tick;
    }

    protected override void OnDetaching()
    {
        var textBox = AssociatedObject;
        if(textBox != null)
        {
            textBox.Loaded -= TextBox_Loaded;
            textBox.PreviewMouseDown -= TextBox_MouseDown;
            textBox.PreviewMouseUp -= TextBox_MouseUp;
            textBox.PreviewMouseMove -= TextBox_MouseMove;
            textBox.SelectionChanged -= TextBox_SelectionChanged;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
        }

        if(verticalScrollBar != null)
        {
            verticalScrollBar.PreviewMouseDown -= ScrollBar_PreviewMouseDown;
            verticalScrollBar.PreviewMouseUp -= ScrollBar_PreviewMouseUp;
            verticalScrollBar.ValueChanged -= VerticalScrollBar_ValueChanged;
        }

        if(horizontalScrollBar != null)
        {
            horizontalScrollBar.PreviewMouseDown -= ScrollBar_PreviewMouseDown;
            horizontalScrollBar.PreviewMouseUp -= ScrollBar_PreviewMouseUp;
            horizontalScrollBar.ValueChanged -= HorizontalScrollBar_ValueChanged;
        }

        scrollTimer?.Stop();
        scrollTimer = null;

        base.OnDetaching();
    }

    void TextBox_Loaded(object sender, RoutedEventArgs e) => InitializeScrollViewer();

    void InitializeScrollViewer()
    {
        try
        {
            scrollViewer = VisualTreeUtil.FindVisualChildren<ScrollViewer>(AssociatedObject).FirstOrDefault();
            if(scrollViewer == null) return;

            var grid = scrollViewer.Template.FindName("PART_Root", scrollViewer) as Grid;
            if(grid != null)
            {
                verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
                horizontalScrollBar = grid.FindName("PART_HorizontalScrollBar") as ScrollBar;

                if(verticalScrollBar != null)
                {
                    verticalScrollBar.PreviewMouseDown += ScrollBar_PreviewMouseDown;
                    verticalScrollBar.PreviewMouseUp += ScrollBar_PreviewMouseUp;
                    verticalScrollBar.ValueChanged += VerticalScrollBar_ValueChanged;
                }

                if(horizontalScrollBar != null)
                {
                    horizontalScrollBar.PreviewMouseDown += ScrollBar_PreviewMouseDown;
                    horizontalScrollBar.PreviewMouseUp += ScrollBar_PreviewMouseUp;
                    horizontalScrollBar.ValueChanged += HorizontalScrollBar_ValueChanged;
                }
            }
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing ScrollViewer: {ex}");
        }
    }

    void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if(PreviewKeyDownCommand?.CanExecute(e) == true)
            PreviewKeyDownCommand.Execute(e);
    }

    void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(TextChangedCommand?.CanExecute(e) == true)
            TextChangedCommand.Execute(e);
    }

    void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = true;
        if(scrollTimer != null && !scrollTimer.IsEnabled && IsSelecting)
            scrollTimer.Start();
    }

    void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = false;
        StopScrolling();
    }

    void TextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if(IsSelecting && scrollViewer != null)
        {
            Point mousePosition = e.GetPosition(scrollViewer);
            double zoneHeight = scrollViewer.ActualHeight * SCROLL_ZONE_PERCENTAGE;
            double zoneWidth = scrollViewer.ActualWidth * SCROLL_ZONE_PERCENTAGE;

            // Top scroll zone
            if(mousePosition.Y < zoneHeight)
            {
                double factor = 1.0 - (mousePosition.Y / zoneHeight);
                StartScrolling(-ScrollSpeed * factor * factor, 0);
            }
            // Bottom scroll zone
            else if(mousePosition.Y > scrollViewer.ActualHeight - zoneHeight)
            {
                double factor = (mousePosition.Y - (scrollViewer.ActualHeight - zoneHeight)) / zoneHeight;
                StartScrolling(ScrollSpeed * factor * factor, 0);
            }
            // Left scroll zone
            else if(mousePosition.X < zoneWidth)
            {
                double factor = 1.0 - (mousePosition.X / zoneWidth);
                StartScrolling(0, -ScrollSpeed * factor * factor);
            }
            // Right scroll zone
            else if(mousePosition.X > scrollViewer.ActualWidth - zoneWidth)
            {
                double factor = (mousePosition.X - (scrollViewer.ActualWidth - zoneWidth)) / zoneWidth;
                StartScrolling(0, ScrollSpeed * factor * factor);
            }
            else
                StopScrolling();
        }
    }

    void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(SelectionChangedCommand?.CanExecute(e) == true)
            SelectionChangedCommand.Execute(e);
    }

    void ScrollBar_PreviewMouseDown(object sender, MouseButtonEventArgs e) => isScrollbarDragging = true;

    void ScrollBar_PreviewMouseUp(object sender, MouseButtonEventArgs e) => isScrollbarDragging = false;

    void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        verticalOffset = e.NewValue;
        
        if(isScrollbarDragging && scrollViewer != null)
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
    }
    
    void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        horizontalOffset = e.NewValue;
        
        if(isScrollbarDragging && scrollViewer != null)
            scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
    }

    void ScrollTimer_Tick(object sender, EventArgs e)
    {
        if(!IsSelecting || isScrollbarDragging || scrollViewer == null) return;

        // Apply vertical scrolling if needed
        if(Math.Abs(verticalOffset) > 0.01)
        {
            // Calculate new vertical position
            double newVerticalOffset = scrollViewer.VerticalOffset + verticalOffset / 120.0;
            
            // Ensure it stays within bounds
            newVerticalOffset = Math.Max(0, Math.Min(newVerticalOffset, scrollViewer.ScrollableHeight));
            
            // Update both ScrollViewer and ScrollBar
            scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            
            if(verticalScrollBar != null)
                verticalScrollBar.Value = newVerticalOffset;
        }
        
        // Apply horizontal scrolling if needed
        if(Math.Abs(horizontalOffset) > 0.01)
        {
            // Calculate new horizontal position
            double newHorizontalOffset = scrollViewer.HorizontalOffset + horizontalOffset / 120.0;
            
            // Ensure it stays within bounds
            newHorizontalOffset = Math.Max(0, Math.Min(newHorizontalOffset, scrollViewer.ScrollableWidth));
            
            // Update both ScrollViewer and ScrollBar
            scrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            
            if(horizontalScrollBar != null)
                horizontalScrollBar.Value = newHorizontalOffset;
        }
    }

    void StartScrolling(double verticalSpeed, double horizontalSpeed)
    {
        this.verticalOffset = verticalSpeed;
        this.horizontalOffset = horizontalSpeed;
        
        if(scrollTimer != null && !scrollTimer.IsEnabled)
            scrollTimer.Start();
    }

    void StopScrolling()
    {
        if(scrollTimer != null)
        {
            scrollTimer.Stop();
            verticalOffset = 0;
            horizontalOffset = 0;
        }
    }

    bool IsSelecting => AssociatedObject.SelectionLength > 0 && isMouseDown;
}