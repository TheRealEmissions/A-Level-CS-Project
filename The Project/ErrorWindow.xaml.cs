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
using System.Windows.Shapes;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private string Error;

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow SetError(string Error)
        {
            this.Error = Error;
            txtblock_error.Text = Error;
            return this;
        }

        public ErrorWindow Initialize()
        {
            this.Show();
            return this;
        }
    }
}
