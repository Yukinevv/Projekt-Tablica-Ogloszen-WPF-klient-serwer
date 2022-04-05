﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            textBlock1.Text = string.Empty;
            foreach (string elements in Connect.SelectRecords())
            {
                textBlock1.Text += elements + "\n";
            }
        }

        private void LoginBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginBox.Text = "";
            LoginBox.TextAlignment = 0;
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            rejestracja.Visibility = (Visibility)1;
            logowanie.Visibility = (Visibility)0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZarejestrujSieDoGrida_Click(object sender, RoutedEventArgs e)
        {
            logowanie.Visibility = (Visibility)1;
            rejestracja.Visibility = (Visibility)0;
        }
    }
}
