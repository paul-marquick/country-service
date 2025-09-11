using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CountryService.WpfAdminApp.Views.UserControls;

/// <summary>
/// Interaction logic for ClearableTextBox.xaml
/// </summary>
public partial class ClearableTextBox : UserControl
{
    public ClearableTextBox()
    {
        InitializeComponent();
    }

    private string placeHolderText;

    public string PlaceHolderText
    {
        get { return placeHolderText; }
        set 
        { 
            placeHolderText = value; 
            textBlockPlaceHolder.Text = placeHolderText;
        }
    }

    private void buttonClear_Click(object sender, RoutedEventArgs e)
    {
        textBoxInput.Clear();
        textBoxInput.Focus();
    }

    private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(textBoxInput.Text))
        {
            textBlockPlaceHolder.Visibility = Visibility.Visible;
        }
        else
        {
            textBlockPlaceHolder.Visibility = Visibility.Hidden;
        }
    }
}
