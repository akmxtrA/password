using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BCrypt.Net;

namespace password
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        passwordCheckDBEntities db = new passwordCheckDBEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void passwordCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if(passwordBox.Visibility == Visibility.Visible)
            {
                passwordBox.Visibility = Visibility.Hidden;
                passwordCheckBox.Visibility = Visibility.Visible;
                passwordConfirmBox.Visibility = Visibility.Hidden;
                passwordConfirmCheckBox.Visibility = Visibility.Visible;
                passwordCheckBox.Text = passwordBox.Password;
                passwordConfirmCheckBox.Text = passwordConfirmBox.Password;
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible;
                passwordCheckBox.Visibility = Visibility.Hidden;
                passwordConfirmBox.Visibility = Visibility.Visible;
                passwordConfirmCheckBox.Visibility = Visibility.Hidden;
                passwordBox.Password = passwordCheckBox.Text;
                passwordConfirmBox.Password = passwordConfirmCheckBox.Text;
            }
        }

        private void registrationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordCheckBox.Visibility == Visibility.Visible)
            {
                passwordBox.Password = passwordCheckBox.Text;
                passwordConfirmBox.Password = passwordConfirmCheckBox.Text;
            }
            if (passwordBox.Password != string.Empty && loginBox.Text != string.Empty && passwordConfirmBox.Password != string.Empty)
            {
                if (ValidatePassword(passwordBox.Password))
                {
                    var user = db.users.FirstOrDefault(x => x.login == loginBox.Text);
                    if (user == null)
                    {
                        if (passwordBox.Password == passwordConfirmBox.Password)
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordBox.Password, 12);
                            users users = new users()
                            {
                                login = loginBox.Text,
                                password = hashedPassword
                            };
                            db.users.Add(users);
                            db.SaveChanges();
                            var User = db.users.FirstOrDefault(x=> x.login == loginBox.Text);
                            passwordHistory passwordHistory = new passwordHistory()
                            {
                                id_user = User.id,
                                password = hashedPassword
                            };
                            db.passwordHistory.Add(passwordHistory);
                            db.SaveChanges();
                            passwordChangeWindow passwordChangeWindow = new passwordChangeWindow(User.id);
                            passwordChangeWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Пароль не совпадает.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователь уже зарегистрирован.");
                    }
                }     
            }
            else
            {
                MessageBox.Show("Заполните все поля и повторите попытку.");
            }     
        }

        private bool ValidatePassword(string password)
        {
            // Проверка длины
            if (password.Length < 8 || password.Length > 128)
            {
                MessageBox.Show("Пароль должен быть длинее 8 символов и короче 128.");
                return false;
            }

            // Проверка на пробелы
            if (password.Contains(" ")) 
            {
                MessageBox.Show("Пароль содержит пробел.");
                return false;
            }

            // Проверка на наличие хотя бы одной заглавной и одной строчной буквы
            if (!Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[a-z]")) 
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну строчную и заглавную букву.");
                return false;
            }

            // Проверка на наличие хотя бы одной цифры
            if (!Regex.IsMatch(password, "[0-9]")) 
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру.");
                return false;
            }


            // Проверка на допустимые символы (кроме букв и цифр)
            string allowedChars = "~!@#$%^&*_-+()[]{}<>/\\|.\",:;' ";
            string otherChars = new string(password.Where(c => !char.IsLetterOrDigit(c)).ToArray());
            foreach (char c in otherChars)
            {
                if (!allowedChars.Contains(c)) 
                {
                    MessageBox.Show("Пароль имеет недопустимые символы");
                    return false;
                }     
            }

            // Проверка на кириллицу и латиницу
            if (!Regex.IsMatch(password, "[А-Яа-яA-Za-z]")) 
            {
                MessageBox.Show("Пароль должен содержать только кириллицу или латиницу.");
                return false;
            }

            return true;
        }

        private void goToAuthorizationBtn_Click(object sender, RoutedEventArgs e)
        {
            authorizationWindow authorization = new authorizationWindow();
            authorization.Show();
            this.Close();
        }
    }
}
