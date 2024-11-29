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
using BCrypt.Net;

namespace password
{
    /// <summary>
    /// Логика взаимодействия для authorizationWindow.xaml
    /// </summary>
    public partial class authorizationWindow : Window
    {
        passwordCheckDBEntities db = new passwordCheckDBEntities();
        public authorizationWindow()
        {
            InitializeComponent();
        }

        private void passwordCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Visibility == Visibility.Visible)
            {
                passwordBox.Visibility = Visibility.Hidden;
                passwordCheckBox.Visibility = Visibility.Visible;
                passwordCheckBox.Text = passwordBox.Password;
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible;
                passwordCheckBox.Visibility = Visibility.Hidden;
                passwordBox.Password = passwordCheckBox.Text;
            }
        }

        private void authorizationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordCheckBox.Visibility == Visibility.Visible)
            {
                passwordBox.Password = passwordCheckBox.Text;
            }
            if (loginBox.Text != string.Empty && passwordBox.Password != string.Empty)
            {
                var user = db.users.FirstOrDefault(x=> x.login == loginBox.Text);
                if(user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(passwordBox.Password, user.password))
                    {
                        passwordChangeWindow passwordChangeWindow = new passwordChangeWindow(user.id);
                        passwordChangeWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Пароль неверный.");
                    }
                }
                else
                {
                    MessageBox.Show("Пользователя не существует.");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля и повторите попытку.");
            }
        }

        private void goToRegistrationBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
