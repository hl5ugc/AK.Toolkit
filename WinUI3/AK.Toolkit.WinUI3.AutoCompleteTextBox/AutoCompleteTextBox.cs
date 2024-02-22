using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.System;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

/// <remarks>
/// https://youtu.be/G17jbGSXLnk?si=jumeyg23f0zn-7UR
/// https://github.com/AndrewKeepCoding/AK.Toolkit.git
/// AK.Toolkit.WinUI3.AutoCompleteTextBox
/// AK.Toolkit.Utilities.RandomStringGenerator
/// </remarks>

namespace AK.Toolkit.WinUI3;

/// <summary>
/// A TextBox control that shows a suggestion based on input "inside it self".
/// The suggestion is shown inside the TextBox control by overriding the placeholder feater.
/// Suggestions need to be provided by the SuggestionsSource property.
/// </summary>

/// <remarks>
/// If you need to change the "FontFamily", use a "Monospaced" fonr.
/// Otherwise the suggestion might 
/// </remarks>

/// TextBlock가 중요한 역할을 하기 때문에 해당 컨트롤을 코드에서 직접 제어를 해야한다.
/// 그래서, TemplatePart라고 속성을 추가하고, 
/// OnApplyTemplate() 에서 각 내부 변수들에 GetTemplateChild를 이용해서 인스턴스를 땡겨온다.
///
[TemplatePart(Name = PlaceholderControlName,Type = typeof(TextBlock))]

public sealed class AutoCompleteTextBox : TextBox
{
    private const string PlaceholderControlName = "PlaceholderTextContentPresenter";

    /// <summary>
    /// Gets or sets a suffix of string as a source of suggestions
    /// </summary>
    public string SuggestionSuffix
    {
        get => (string)GetValue(SuggestionSuffixProperty);
        set => SetValue(SuggestionSuffixProperty, value);
    }
    // Using a DependencyProperty as the backing store for IsSuggestionCaseSensitive.
    // This enables animation, styling, binding, etc...
    /// <summary>
    /// Identifies the <see cref="SuggestionSuffix"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionSuffixProperty =
        DependencyProperty.Register(
            nameof(SuggestionSuffix), 
            typeof(string), 
            typeof(AutoCompleteTextBox), 
            new PropertyMetadata(string.Empty));
    /// <summary>
    /// Gets or sets a value indicating whether the suggestion is case sensitive
    /// </summary>
    public bool IsSuggestionCaseSensitive
    {
        get => (bool)GetValue(IsSuggestionCaseSensitiveProperty);
        set => SetValue(IsSuggestionCaseSensitiveProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="IsSuggestionCaseSensitive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSuggestionCaseSensitiveProperty =
        DependencyProperty.Register(
            nameof(IsSuggestionCaseSensitive), 
            typeof(bool), 
            typeof(AutoCompleteTextBox), 
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a collection of string as a source of suggestions
    /// </summary>
    public IEnumerable<string>  SuggestionsSource
    {
        get => (IEnumerable<string>)GetValue(SuggestionsSourceProperty);
        set => SetValue(SuggestionsSourceProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="SuggestionsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionsSourceProperty =
        DependencyProperty.Register(
            nameof(SuggestionsSource), 
            typeof(IEnumerable<string>), 
            typeof(AutoCompleteTextBox), 
            new PropertyMetadata(null));

    private string LastAcceptedSuggestion
    {
        get;
        set;
    } = string.Empty;

    private string OriginalPlacegolderText
    {
        get;
        set;
    } = string.Empty;

    private TextBlock? PlaceholderControl
    {
        get; set;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCompleteTextBox"/> class.
    /// </summary>
    public AutoCompleteTextBox()
    {
        DefaultStyleKey = typeof(AutoCompleteTextBox);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // 내부 변수들에 GetTemplateChild를 이용해서 인스턴스를 땡겨온다.
        PlaceholderControl = GetTemplateChild(PlaceholderControlName) as TextBlock;

        // 이벤트 핸들러를 추가 한다.
        if(PlaceholderControl != null)
        {
            OriginalPlacegolderText = PlaceholderControl.Text;

            TextChanged += (s, e) => UpdateSuggestion(acceptSuggestion: false);

            KeyDown += (s, e) =>
            {
                if (e.Key is VirtualKey.Right)
                {
                    UpdateSuggestion(acceptSuggestion: true);
                }
            };

            LostFocus += (s, e) =>
            {
                if (Text.Length > 0)
                {
                    PlaceholderControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UpdatePlaceholderControl(OriginalPlacegolderText,Visibility.Visible);
                }
            };

            GettingFocus += (s, e) => UpdateSuggestion(acceptSuggestion: false);
 
        }
    }
    /// <summary>
    /// suggestionsSource 콜렉션에 있는 문자열에서 input 조건맞는 문자열을 찾아서 반환한다.
    /// </summary>
    /// <param name="input"></param> 입력된 문자열
    /// <param name="ignoreCase"></param>
    /// <param name="suggestionsSource"></param> suggestions 콜렉션
    /// <returns></returns>
    private static string GetSuggestion(string input,bool ignoreCase,IEnumerable<string> suggestionsSource)
    {
        string? suggestion = string.Empty;

        if((input.Length > 0) && (suggestionsSource is not null))
        {
            /// FirstOrDefault() 함수는 컬렉션에서 첫 번째 요소를 반환하거나 또는 조건을 만족하는 여러 요소 중에서 첫 번째 요소를 반환합니다.
            /// string.StartsWith 지정한 문화권을 사용하여 비교할 때 이 문자열 인스턴스의 시작 부분과 지정한 문자열이 일치하는지를 확인합니다.
            var result = suggestionsSource.FirstOrDefault(x => x.StartsWith(input, ignoreCase, culture: null));
            if (result is not null)
            {
                suggestion = result;
            }
        }

        return suggestion;
    }
    /// <summary>
    /// text 를 PlaceholderControl.Text에 저장 and Visibility
    /// </summary>
    /// <param name="text"></param>
    /// <param name="visibility"></param>
    private void UpdatePlaceholderControl(string text, Visibility visibility)
    {
        if(PlaceholderControl != null)
        {
            PlaceholderControl.Text = text;
            PlaceholderControl.Visibility = visibility;
        }
    }
    // acceptSuggestion : true => 제안된 문자열을 받아들임
    private void UpdateSuggestion(bool acceptSuggestion)
    {
        if (Text.Length == 0 || LastAcceptedSuggestion.Equals(Text) is not true)
        {
            bool ignoreCase = (IsSuggestionCaseSensitive is false);
            string suggestion = GetSuggestion(Text, ignoreCase, SuggestionsSource);

            if (suggestion.Length > 0) // SuggestionsSource에서 Text를 찾았을 때
            {
                // PadRight("문자를 붙인 후 문자열 전체 길이수", 문자열 오른쪽에 붙이고 싶은 문자)
                // if Text is "K" then Text.Length = 1 ,if suggestion = "Kang" then suggestion.Length = 4
                // suggestion[1..].PadLeft(4) => suggestion = " ang"
                string text = suggestion[Text.Length..].PadLeft(suggestion.Length,'.');
                text += SuggestionSuffix;
                //
                UpdatePlaceholderControl(text, Visibility.Visible);
            }
            else if (Text.Length == 0)
            {
                UpdatePlaceholderControl(OriginalPlacegolderText, Visibility.Visible);
            }
            else
            {
                UpdatePlaceholderControl(OriginalPlacegolderText, Visibility.Collapsed);
            }
            //
            if(acceptSuggestion is true && suggestion.Length > 0)
            {
                UpdatePlaceholderControl(OriginalPlacegolderText, Visibility.Collapsed);
                Text = suggestion;
                LastAcceptedSuggestion = suggestion;
                SelectionStart = Text.Length;
            }
            else
            {
                LastAcceptedSuggestion = string.Empty;
            }
        }
    }

}
