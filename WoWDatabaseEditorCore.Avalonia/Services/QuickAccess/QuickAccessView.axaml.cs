using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using WDE.MVVM.Observable;
using WoWDatabaseEditorCore.Services.QuickAccess;

namespace WoWDatabaseEditorCore.Avalonia.Services.QuickAccess;

public class QuickAccessView : UserControl
{
    private TextBox searchBox = null!;
    private ListBox resultsList = null!;
    private SelectingItemsControlSelectionAdapter adapter = null!;
    private bool searchBoxMoveToEnd = false;
    
    public QuickAccessView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        searchBox = this.FindControl<TextBox>("SearchBox");
        resultsList = this.FindControl<ListBox>("ResultsList");
        adapter = new SelectingItemsControlSelectionAdapter(resultsList);
        adapter.Commit += ResultCommit;
        searchBox.GotFocus += (_, _) =>
        {
            if (searchBoxMoveToEnd)
                searchBox.SelectionEnd = searchBox.SelectionStart = searchBox.Text.Length;
            else
               searchBox.SelectAll();
        };
        searchBox.KeyDown += SearchBox_KeyDown;

        this.GetObservable(IsVisibleProperty).SubscribeAction(@is =>
        {
            if (@is)
            {
                DispatcherTimer.RunOnce(() =>
                {
                    searchBox?.Focus();
                }, TimeSpan.FromMilliseconds(1));
                DispatcherTimer.RunOnce(() =>
                {
                    if (searchBox != null && searchBox.Text != null)
                        searchBox.SelectionEnd = searchBox.SelectionStart = searchBox.Text.Length;
                }, TimeSpan.FromMilliseconds(2));
            }
        });
    }

    private void ResultCommit(object? sender, RoutedEventArgs e)
    {
        if (DataContext is QuickAccessViewModel vm)
        {
            var item = adapter.SelectedItem as QuickAccessItemViewModel;
            if (item == null)
            {
                if (vm.Items.Count == 1)
                    item = vm.Items[0];
            }

            if (item != null)
            {
                vm.Commit(item);
                searchBoxMoveToEnd = true;
                searchBox.Focus();
                searchBox.SelectionEnd = searchBox.SelectionStart = searchBox.Text.Length;
                searchBoxMoveToEnd = false;   
            }
        }
    }

    private void SearchBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down || e.Key == Key.Up)
        {
            adapter.HandleKeyDown(e);
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            ResultCommit(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            if (DataContext is QuickAccessViewModel vm)
                vm.CloseSearch();
            e.Handled = true;
        }
    }
}