using Microsoft.Data.Sqlite;
using System;
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
        public LoggingWindow DebugWindow { get; } = new();

        public MessagingHandler Handler { get; }
        public SqliteConnection? SQLConnection { get; }
        public Database.Tables.Tables Tables { get; }

        public MainWindow()
        {
            Handler = new();
            SQLConnection = Handler.Connection;
            Tables = Handler.Tables;

            //new Tables(SQLConnection).GetAndCreateAllTables();

            InitializeComponent();
            DisableAllButtons();

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
                DisableAllButtons();
                return;
            }
            if (txtinput_pswd.Password.Length > 0 && txtinput_pswd.Password != "Password")
            {
                btn_login.IsEnabled = true;
                Debug("Login Button Enabled");
                if (txtinput_confpswd.Password.Length > 0 && txtinput_confpswd.Password == txtinput_pswd.Password)
                {
                    btn_register.IsEnabled = true;
                    Debug("Register Button Enabled");
                }
            }
            else
            {
                btn_login.IsEnabled = false;
                Debug("Login Button Enabled");
            }
        }

        private void Txtinput_confpswd_TextChanged(object sender, RoutedEventArgs e)
        {
            btn_register.IsEnabled = false;
            if (txtinput_pswd.Password.Length < 1)
            {
                DisableAllButtons();
                return;
            }
            if (txtinput_confpswd.Password.ToLower() is "confirm password" or "password")
            {
                return;
            }

            btn_register.IsEnabled = txtinput_pswd.Password == txtinput_confpswd.Password;
            Debug("Enabled register button");
        }

        private void Txtinput_username_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Btn_register_Click(object sender, RoutedEventArgs e)
        {
            if (new Database.UserAccount(SQLConnection, Tables).GetAccount(txtinput_username.Text) is not null)
            {
                _ = new ErrorWindow().SetError("This account already exists!").Initialize();
                return;
            }

            Hashing Hash = new(this);

            // generate password hash
            string PasswordHash = Hash.Hash(txtinput_pswd.Password);
            DebugWindow.Debug($"Password Hash: {PasswordHash}");

            // create an account
            try
            {
                Account Account = new(txtinput_username.Text, PasswordHash, PasswordHash, SQLConnection, Tables);

                // register account to handler
                Handler.UserAccount = Account;
            }
            catch (Exception Error)
            {
                _ = new ErrorWindow().SetError(Error.Message).Initialize();
            }

            this.Content = new UserConnectionPage(this);
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            Hashing Hash = new(this);

            Debug("Registered LOGIN CLICK - Finding account");
            string PasswordHash = Hash.Hash(txtinput_pswd.Password);
            Debug($"Password hash: {PasswordHash}");
            Debug($"Hash length: {PasswordHash.Length}");
            try
            {
                Account account = new(txtinput_username.Text, PasswordHash, SQLConnection, Tables);
                Handler.UserAccount = account;
            }
            catch (Exception Error)
            {
                _ = new ErrorWindow().SetError(Error.Message).Initialize();
                return;
            }
            Debug("FOUND ACCOUNT!");

            UserConnectionPage UserConnectionPage = new(this);
            this.Content = UserConnectionPage;
        }

        public void DisableAllButtons()
        {
            btn_login.IsEnabled = false;
            btn_register.IsEnabled = false;
            Debug("All buttons disabled");
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

        public void Btn_DebugWindow_Click(object sender, RoutedEventArgs e)
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