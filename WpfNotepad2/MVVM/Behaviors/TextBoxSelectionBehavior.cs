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
    bool isMouseDown = false;
    bool isScrollbarDragging = false;
    DispatcherTimer scrollTimer;
    const double ScrollSpeed = 75;
    ScrollViewer scrollViewer;
    ScrollBar verticalScrollBar;
    ScrollBar horizontalScrollBar;

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
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
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
        {
            PreviewKeyDownCommand.Execute(e);
        }
    }

    void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(TextChangedCommand?.CanExecute(e) == true)
        {
            TextChangedCommand.Execute(e);
        }
    }

    void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = true;
        if(!scrollTimer.IsEnabled && IsSelecting)
        {
            scrollTimer.Tag = ScrollSpeed;
            scrollTimer.Start();
        }
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
            if(mousePosition.Y < 0)
            {
                StartScrolling(-ScrollSpeed);
            }
            else if(mousePosition.Y > scrollViewer.ActualHeight)
            {
                StartScrolling(ScrollSpeed);
            }
            else
            {
                StopScrolling();
            }
        }
    }

    void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(SelectionChangedCommand?.CanExecute(e) == true)
        {
            SelectionChangedCommand.Execute(e);
        }
    }

    void ScrollBar_PreviewMouseDown(object sender, MouseButtonEventArgs e) => isScrollbarDragging = true;

    void ScrollBar_PreviewMouseUp(object sender, MouseButtonEventArgs e) => isScrollbarDragging = false;

    void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(isScrollbarDragging && scrollViewer != null)
        {
            scrollViewer.ScrollToVerticalOffset(e.NewValue);
        }
    }

    void ScrollTimer_Tick(object sender, EventArgs e)
    {
        if(!IsSelecting || isScrollbarDragging || scrollViewer == null) return;

        double speed = (double)scrollTimer.Tag;
        double newOffset = scrollViewer.VerticalOffset + (speed / 3.0);

        // Clamp the new offset
        newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));

        // Update the ScrollViewer
        scrollViewer.ScrollToVerticalOffset(newOffset);

        // Update the vertical scrollbar
        if(verticalScrollBar != null)
        {
            verticalScrollBar.Value = newOffset;
        }
    }

    void StartScrolling(double speed)
    {
        if(!scrollTimer.IsEnabled)
        {
            scrollTimer.Tag = speed;
            scrollTimer.Start();
        }
    }

    void StopScrolling() => scrollTimer?.Stop();

    bool IsSelecting => AssociatedObject.SelectionLength > 0 && isMouseDown;
}

