using BCrypt.Net;
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
using System.Windows.Shapes;

namespace password
{
    /// <summary>
    /// Логика взаимодействия для passwordChangeWindow.xaml
    /// </summary>
    public partial class passwordChangeWindow : Window
    {
        public int UserId;
        passwordCheckDBEntities db = new passwordCheckDBEntities();
        public passwordChangeWindow(int User)
        {
            var user = db.users.FirstOrDefault(x => x.id == User);
            InitializeComponent();
            UserId = User;
            loginLabel.Content = "Добро пожаловать, " + user.login;
        }

        private void passwordCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (oldPasswordBox.Visibility == Visibility.Visible)
            {
                oldPasswordBox.Visibility = Visibility.Hidden;
                newPasswordBox.Visibility = Visibility.Hidden;
                newPasswordConfirmBox.Visibility = Visibility.Hidden;
                oldPasswordCheckBox.Visibility = Visibility.Visible;
                newPasswordCheckBox.Visibility = Visibility.Visible;
                newPasswordConfirmCheckBox.Visibility = Visibility.Visible;
                oldPasswordCheckBox.Text = oldPasswordBox.Password;
                newPasswordCheckBox.Text = newPasswordBox.Password;
                newPasswordConfirmCheckBox.Text = newPasswordConfirmBox.Password;
            }
            else
            {
                oldPasswordBox.Visibility = Visibility.Visible;
                newPasswordBox.Visibility = Visibility.Visible;
                newPasswordConfirmBox.Visibility = Visibility.Visible;
                oldPasswordCheckBox.Visibility = Visibility.Hidden;
                newPasswordCheckBox.Visibility = Visibility.Hidden;
                newPasswordConfirmCheckBox.Visibility = Visibility.Hidden;
                oldPasswordBox.Password = oldPasswordCheckBox.Text;
                newPasswordBox.Password = newPasswordCheckBox.Text;
                newPasswordConfirmBox.Password = newPasswordConfirmCheckBox.Text;
            }
        }

        private void changePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = db.users.FirstOrDefault(x => x.id == UserId);
            if (oldPasswordBox.Visibility == Visibility.Hidden)
            {
                oldPasswordBox.Password = oldPasswordCheckBox.Text;
                newPasswordBox.Password = newPasswordCheckBox.Text;
                newPasswordConfirmBox.Password = newPasswordConfirmCheckBox.Text;
            }
            if (oldPasswordBox.Password != string.Empty && newPasswordBox.Password != string.Empty && newPasswordConfirmBox.Password != string.Empty)
            {
                if(BCrypt.Net.BCrypt.Verify(oldPasswordBox.Password, user.password))
                {
                    if(newPasswordBox.Password == newPasswordConfirmBox.Password)
                    {
                        if(BCrypt.Net.BCrypt.Verify(newPasswordBox.Password, user.password))
                        {
                            MessageBox.Show("Новый пароль должен отличаться от текущего.");
                        }
                        else
                        {               
                            if (ValidatePassword(newPasswordBox.Password))
                            {
                                var passListHistory = db.passwordHistory.Where(x => x.id_user == UserId).ToList();
                                var passList = passListHistory.OrderByDescending(x => x.id).Take(8).ToList();
                                bool passwordNotOld = false;
                                foreach (var p in passList)
                                {
                                    if(BCrypt.Net.BCrypt.Verify(newPasswordBox.Password, p.password))
                                    {
                                        passwordNotOld = false;
                                        break;
                                    }
                                    else
                                    {
                                        passwordNotOld = true;                            
                                    }
                                }
                                if(passwordNotOld)
                                {
                                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPasswordBox.Password, 12);
                                    user.password = hashedPassword;
                                    passwordHistory passwordHistory = new passwordHistory()
                                    {
                                        id_user = user.id,
                                        password = hashedPassword
                                    };
                                    db.passwordHistory.Add(passwordHistory);
                                    db.SaveChanges();
                                    MessageBox.Show("Пароль пользователя: " + user.login + " усепшно изменён!");
                                }
                                else
                                {
                                    MessageBox.Show("Нельзя использовать старый пароль.");
                                }                    
                            }
                            else
                            {
                                MessageBox.Show("Пароль не соответствует требованиям. Требования: не менее 8 символов;\r\nне более 128 символов;\r\nкак минимум одна заглавная и одна строчная буква;\r\nтолько латинские или кириллические буквы;\r\nкак минимум одна цифра;\r\nтолько арабские цифры;\r\nбез пробелов;\r\nДругие допустимые символы:~ ! ? @ # $ % ^ & * _ - + ( ) [ ] { } > < / \\ | \" ' . , : ;");
                            }
                        }                   
                    }
                    else
                    {
                        MessageBox.Show("Подтверждение пароля не совпадает.");
                    }
                }
                else
                {
                    MessageBox.Show("Старый пароль неверный.");
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

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            authorizationWindow authorizationWindow = new authorizationWindow();
            authorizationWindow.Show();
            this.Close();
        }
    }
}
