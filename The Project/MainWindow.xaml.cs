using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Project
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetAllButtonsToDisabled();

            // register events
            btn_login.Click += Btn_login_Click;
            btn_register.Click += Btn_register_Click;
            txtinput_username.TextChanged += Txtinput_username_TextChanged;
            txtinput_pswd.TextChanged += Txtinput_pswd_TextChanged;
            txtinput_confpswd.TextChanged += Txtinput_confpswd_TextChanged;
            txtinput_username.LostFocus += Txtinput_username_LostFocus;
            txtinput_pswd.LostFocus += Txtinput_pswd_LostFocus;
            txtinput_confpswd.LostFocus += Txtinput_confpswd_LostFocus;

            // add handler
            txtinput_username.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_username_MouseLeftButtonDown), true);
            txtinput_pswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_pswd_MouseLeftButtonDown), true);
            txtinput_confpswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_confpswd_MouseLeftButtonDown), true);
        }

        private void Txtinput_confpswd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtinput_confpswd.Text.Length < 1)
            {
                txtinput_confpswd.Text = "Confirm Password";
            }
        }

        private void Txtinput_pswd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtinput_pswd.Text.Length < 1)
            {
                txtinput_pswd.Text = "Password";
            }
        }

        private void Txtinput_username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtinput_username.Text.Length < 1)
            {
                txtinput_username.Text = "Username";
            }
        }

        private void Txtinput_pswd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtinput_username.Text.Length < 1)
            {
                SetAllButtonsToDisabled();
                return;
            }
            if (txtinput_pswd.Text.Length > 0 && txtinput_pswd.Text != "Password")
            {
                btn_login.IsEnabled = true;
                if (txtinput_confpswd.Text.Length > 0 && txtinput_confpswd.Text == txtinput_pswd.Text) btn_register.IsEnabled = true;
            }
            else
            {
                btn_login.IsEnabled = false;
            }
        }

        private void Txtinput_confpswd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtinput_pswd.Text.Length < 1)
            {
                SetAllButtonsToDisabled();
                return;
            }
            if (txtinput_pswd.Text == txtinput_confpswd.Text && (txtinput_confpswd.Text != "Confirm Password" || txtinput_confpswd.Text != "Password"))
            {
                btn_register.IsEnabled = true;
            }
            else
            {
                btn_register.IsEnabled = false;
            }
        }

        private void Txtinput_username_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Btn_register_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SetAllButtonsToDisabled()
        {
            btn_login.IsEnabled = false;
            btn_register.IsEnabled = false;
        }

        private void Txtinput_username_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtinput_username.Text = string.Empty;
        }

        private void Txtinput_pswd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtinput_pswd.Text = string.Empty;
        }

        private void Txtinput_confpswd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtinput_confpswd.Text = string.Empty;
        }
    }
}