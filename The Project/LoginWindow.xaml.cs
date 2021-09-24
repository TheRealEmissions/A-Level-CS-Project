using Microsoft.Data.Sqlite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using The_Project.Accounts;
using The_Project.Cryptography;

#nullable enable

namespace The_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoggingWindow DebugWindow = new();

        private readonly MessagingHandler Handler = new();
        public SqliteConnection? SQLConnection;

        public MainWindow()
        {
            SQLConnection = Handler.Connection;

            //new Tables(SQLConnection).GetAndCreateAllTables();

            InitializeComponent();
            SetAllButtonsToDisabled();

            // register events
            btn_login.Click += Btn_login_Click;
            btn_register.Click += Btn_register_Click;
            txtinput_username.TextChanged += Txtinput_username_TextChanged;
            txtinput_pswd.PasswordChanged += Txtinput_pswd_TextChanged;
            txtinput_confpswd.PasswordChanged += Txtinput_confpswd_TextChanged;

            // add handler
            txtinput_username.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_username_MouseLeftButtonDown), true);
            txtinput_pswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_pswd_MouseLeftButtonDown), true);
            txtinput_confpswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_confpswd_MouseLeftButtonDown), true);
        }

        public void Debug(string text)
        {
            DebugWindow.Debug(text);
        }

        private void Txtinput_pswd_TextChanged(object sender, RoutedEventArgs e)
        {
            if (txtinput_username.Text.Length < 1)
            {
                SetAllButtonsToDisabled();
                return;
            }
            if (txtinput_pswd.Password.Length > 0 && txtinput_pswd.Password != "Password")
            {
                btn_login.IsEnabled = true;
                if (txtinput_confpswd.Password.Length > 0 && txtinput_confpswd.Password == txtinput_pswd.Password)
                {
                    btn_register.IsEnabled = true;
                }
            }
            else
            {
                btn_login.IsEnabled = false;
            }
        }

        private void Txtinput_confpswd_TextChanged(object sender, RoutedEventArgs e)
        {
            if (txtinput_pswd.Password.Length < 1)
            {
                SetAllButtonsToDisabled();
                return;
            }
            btn_register.IsEnabled = txtinput_pswd.Password == txtinput_confpswd.Password && (txtinput_confpswd.Password != "Confirm Password" || txtinput_confpswd.Password != "Password");
        }

        private void Txtinput_username_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Btn_register_Click(object sender, RoutedEventArgs e)
        {

            Hashing Hash = new(this);

            // generate password hash
            string PasswordHash = Hash.Hash(txtinput_pswd.Password);

            // create an account
            Account Account = new(txtinput_username.Text, txtinput_pswd.Password, txtinput_confpswd.Password, SQLConnection);

            // register account to handler
            Handler.UserAccount = Account;
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            Hashing Hash = new(this);

            Debug("Registered LOGIN CLICK - Finding account");
            string PasswordHash = Hash.Hash(txtinput_pswd.Password);
            Debug($"Password hash: {PasswordHash}");
            Debug($"Hash length: {PasswordHash.Length}");
            Account account = new(txtinput_username.Text, txtinput_pswd.Password, SQLConnection);
            Handler.UserAccount = account;
            Debug("FOUND ACCOUNT!");

            UserConnectionPage UserConnectionPage = new();
            this.Content = UserConnectionPage;
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
            txtinput_pswd.Password = string.Empty;
        }

        private void Txtinput_confpswd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtinput_confpswd.Password = string.Empty;
        }

        private void Btn_DebugWindow_Click(object sender, RoutedEventArgs e)
        {
            if (DebugWindow.IsActive)
            {
                return;
            }

            DebugWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DebugWindow.Close();
        }
    }
}