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
    public partial class MainWindow
    {
        public LoggingWindow DebugWindow { get; } = new();

        public MessagingHandler Handler { get; }
        public SqliteConnection? SqliteConnection { get; }
        public Database.Tables.Tables Tables { get; }

        public MainWindow()
        {
            Handler = new MessagingHandler();
            SqliteConnection = Handler.Connection;
            Tables = Handler.Tables;

            //new Tables(SqliteConnection).GetAndCreateAllTables();

            InitializeComponent();
            DisableAllButtons();

            // register events
            BtnLogin.Click += Btn_login_Click;
            BtnRegister.Click += Btn_register_Click;
            TxtinputUsername.TextChanged += Txtinput_username_TextChanged;
            TxtinputPswd.PasswordChanged += Txtinput_pswd_TextChanged;
            TxtinputConfpswd.PasswordChanged += Txtinput_confpswd_TextChanged;

            // add handler
            TxtinputUsername.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_username_MouseLeftButtonDown), true);
            TxtinputPswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_pswd_MouseLeftButtonDown), true);
            TxtinputConfpswd.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_confpswd_MouseLeftButtonDown), true);
        }

        public void Debug(string text)
        {
            DebugWindow.Debug(text);
        }

        private void Txtinput_pswd_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TxtinputUsername.Text.Length < 1)
            {
                DisableAllButtons();
                return;
            }
            if (TxtinputPswd.Password.Length > 0 && TxtinputPswd.Password != "Password")
            {
                BtnLogin.IsEnabled = true;
                Debug("Login Button Enabled");

                if (TxtinputConfpswd.Password.Length <= 0 || TxtinputConfpswd.Password != TxtinputPswd.Password)
                {
                    return;
                }

                BtnRegister.IsEnabled = true;
                Debug("Register Button Enabled");
            }
            else
            {
                BtnLogin.IsEnabled = false;
                Debug("Login Button Enabled");
            }
        }

        private void Txtinput_confpswd_TextChanged(object sender, RoutedEventArgs e)
        {
            BtnRegister.IsEnabled = false;
            if (TxtinputPswd.Password.Length < 1)
            {
                DisableAllButtons();
                return;
            }
            if (TxtinputConfpswd.Password.ToLower() is "confirm password" or "password")
            {
                return;
            }

            BtnRegister.IsEnabled = TxtinputPswd.Password == TxtinputConfpswd.Password;
            Debug("Enabled register button");
        }

        private void Txtinput_username_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Btn_register_Click(object sender, RoutedEventArgs e)
        {
            if (new Database.UserAccount(SqliteConnection, Tables).GetAccount(TxtinputUsername.Text) is not null)
            {
                _ = new ErrorWindow().SetError("This account already exists!").Initialize();
                return;
            }

            Hashing hash = new(this);

            // generate password hash
            string passwordHash = hash.Hash(TxtinputPswd.Password);
            DebugWindow.Debug($"Password Hash: {passwordHash}");

            // create an account
            try
            {
                Account userAccount = new(TxtinputUsername.Text, passwordHash, passwordHash, SqliteConnection, Tables);

                // register account to handler
                Handler.UserAccount = userAccount;
            }
            catch (Exception exception)
            {
                _ = new ErrorWindow().SetError(exception.Message).Initialize();
            }

            Content = new UserConnectionPage(this);
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            Hashing hash = new(this);

            Debug("Registered LOGIN CLICK - Finding account");
            string passwordHash = hash.Hash(TxtinputPswd.Password);
            Debug($"Password hash: {passwordHash}");
            Debug($"Hash length: {passwordHash.Length}");
            try
            {
                Account account = new(TxtinputUsername.Text, passwordHash, SqliteConnection, Tables);
                Handler.UserAccount = account;
            }
            catch (Exception exception)
            {
                _ = new ErrorWindow().SetError(exception.Message).Initialize();
                return;
            }
            Debug("FOUND ACCOUNT!");

            UserConnectionPage userConnectionPage = new(this);
            Content = userConnectionPage;
        }

        public void DisableAllButtons()
        {
            BtnLogin.IsEnabled = false;
            BtnRegister.IsEnabled = false;
            Debug("All buttons disabled");
        }

        private void Txtinput_username_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TxtinputUsername.Text = string.Empty;
        }

        private void Txtinput_pswd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TxtinputPswd.Password = string.Empty;
        }

        private void Txtinput_confpswd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TxtinputConfpswd.Password = string.Empty;
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