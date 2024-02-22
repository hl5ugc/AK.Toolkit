using System;
using System.Collections.Generic;
using AK.Toolkit.Utilities;
using Microsoft.UI.Xaml;
using static AK.Toolkit.Utilities.RandomStringGenerator;


namespace AK.Toolkit.Samples;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private List<string> DemoSuggestions
    {
        get;
    } = new();

    public MainWindow()
    {
        this.InitializeComponent();
    }

    private void UpdateDemoSuggestions()
    {
        DemoSuggestions.Clear();

        if(int.TryParse(DemoSuggestionsCount.Text,out var suggestionsCount) is true) 
        { 
            for(var i=0; i < suggestionsCount; i++)
            {
                var suggestion = RandomStringGenerator.GeneraterString(
                    OutputType.AlphaNumbers, 3, 10);
                DemoSuggestions.Add(suggestion);

            }
        }

        string[]? addionals = AdditionalSuggestions.Text.Split('\u002C');
        Random random = new Random();

        foreach (string item in addionals)
        {
            int index = random.Next(0,DemoSuggestions.Count);
            DemoSuggestions.Insert(index, item);
        }

    }
    private void UpdateSuggestionsButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateDemoSuggestions();
    }
}
