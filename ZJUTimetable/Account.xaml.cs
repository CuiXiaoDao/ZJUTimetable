﻿using ZJUTimetable.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;
using ZJUTimetable.DataModel;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace ZJUTimetable
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Account : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Account()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //PasswordVault value = new PasswordVault();
            //if (value.FindAllByResource("zju") != null)
            //{
            //    var passwordCredential = value.FindAllByResource("zju").First();
            //    UserName.Text = passwordCredential.UserName;
            //    MyPasswordBox.Password = passwordCredential.Password;
            //}
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            //change the seasonlabel
            int month = System.DateTime.Now.Month;

            if (month >= 2 && month <=8)
            {
                FirstTerm.Content = "春";
                SecondTerm.Content = "夏";
            }
            else
            {
                FirstTerm.Content = "秋";
                SecondTerm.Content = "冬";
            }

            //load account info
            Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localsettings.Values.ContainsKey("userName") && localsettings.Values.ContainsKey("password"))
            {
                UserName.Text = localsettings.Values["userName"].ToString();
                MyPasswordBox.Password = localsettings.Values["password"].ToString();
                SaveButton.Content = "删除";
            }

            //load the season and weekNumber
            if (localsettings.Values.ContainsKey("season"))
            {
                string season = localsettings.Values["season"].ToString();
                Season.SelectedIndex = season=="春"|| season=="秋"?0:1;
            }
            if (localsettings.Values.ContainsKey("weekNumber")&& localsettings.Values.ContainsKey("weekDate"))
            {
                int weekDate = (int)localsettings.Values["weekDate"];            
                CurrentWeekNumber.SelectedIndex = Math.Abs((int)localsettings.Values["weekNumber"]+ (System.DateTime.Today.DayOfYear- weekDate)/7)%8;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
           
            Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localsettings.Values["season"] = ((ComboBoxItem)Season.SelectedItem).Content;
            localsettings.Values["weekNumber"] = CurrentWeekNumber.SelectedIndex;
            localsettings.Values["weekDate"] = System.DateTime.Today.DayOfYear-(System.DateTime.Today.DayOfWeek==0?6:(int)System.DateTime.Today.DayOfWeek-1);//the day in year of this week's monday
        }

        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            UserName.SelectAll();
        }

        private void MyPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MyPasswordBox.SelectAll();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //PasswordVault value = new PasswordVault();
            Windows.Storage.ApplicationDataContainer localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (SaveButton.Content.ToString() == "确认")
            {
                if (UserName.Text != "" && MyPasswordBox.Password != "")
                {
                    //PasswordCredential passwordCredential = new PasswordCredential("zju", UserName.Text, MyPasswordBox.Password);                  
                    //value.Add(passwordCredential);

                    localsettings.Values["userName"] = UserName.Text;
                    localsettings.Values["password"] = MyPasswordBox.Password;
                    SaveButton.Content = "删除";
                }
                else
                {
                    var message = new Windows.UI.Popups.MessageDialog("不要卖萌，没有数据怎么确认~");
                    await message.ShowAsync();
                }

            }
            else //the buton is a delete button
            {
                //var passwordCredentials = value.FindAllByResource("zju");

                //foreach (var credential in passwordCredentials) //删除用户账户，防止可能有错误的用户密码，可能用户保存多次
                //{
                //    value.Remove(credential);
                //}
                localsettings.Values.Remove("userName");
                localsettings.Values.Remove("password");
                await DataHelper.DeleteDatabaseAsync(); //删除用户数据
                UserName.Text = "";
                MyPasswordBox.Password = "";
                SaveButton.Content = "确认";
            }
        }

        private void Account_TextChanged(object sender, RoutedEventArgs e)
        {
            SaveButton.Content = "确认";
        }

        #endregion
    }
}