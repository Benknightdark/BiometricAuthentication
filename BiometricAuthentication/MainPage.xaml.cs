﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BiometricAuthentication
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        string AuthType;
        public MainPage()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                AuthType = DependencyService.Get<IBiometricAuthenticateService>().GetAuthenticationType();
                if (!AuthType.Equals("None"))
                {
                    lbl.Text = "Please use " + AuthType + " to unlock";
                    if (AuthType.Equals("TouchId") || AuthType.Equals("FaceId"))
                    {
                        GetAuthResults();
                    }
                }
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                bool res = DependencyService.Get<IBiometricAuthenticateService>().fingerprintEnabled();
                if (res)
                {
                    //check for user have gives access to finger print for this app
                    //app permision stored locally
                    fingerprintDroid.IsVisible = true;
                    DependencyService.Get<IBiometricAuthenticateService>().AuthenticateUserIDWithTouchID();

                    MessagingCenter.Subscribe<string>("Auth", "Success", (sender) =>
                    {
                        contentPage.BackgroundColor = Color.Green;
                        lbl.Text = "TouchID authentication success";
                    });
                    MessagingCenter.Subscribe<string>("Auth", "Fail", (sender) =>
                    {
                        contentPage.BackgroundColor = Color.Red;
                        lbl.Text = "TouchId authentication fail";
                    });

                }
                else
                {
                    contentPage.BackgroundColor = Color.Red;
                    lbl.Text = "Biometric not supported on this device or no fingerprint enrolled";
                    fingerprintDroid.IsVisible = false;
                }
            }
        }

        private async Task GetAuthResults()
        {
            //todo according to Auth type change the authenticationmethod in interface if face id or touch id
            //string AuthType = DependencyService.Get<IFingerprintAuthService>().GetAuthenticationType();
            var result = await DependencyService.Get<IBiometricAuthenticateService>().AuthenticateUserIDWithTouchID();
            if (result)
            {
                if (AuthType.Equals("TouchId"))
                {
                    lbl.Text = "TouchID authentication success";
                }
                else if (AuthType.Equals("FaceId"))
                {
                    lbl.Text = "FaceID authentication success";
                }
            }
        }
    }
}
