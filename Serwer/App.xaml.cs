using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Serwer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var bazaDanych = new MyDbContext();
            bazaDanych.Database.EnsureCreated();

            DataBaseLocator.Context = bazaDanych;
        }
    }
}
