﻿using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeFramework.iOS.Utils;
using Cirrious.CrossCore;
using CodeHub.Core.Services;

namespace CodeHub.iOS.Views.App
{
    public partial class EnableEnterpriseViewController : UIViewController
    {
        private IHud _hud;

        public EnableEnterpriseViewController() : base("EnableEnterpriseViewController", null)
        {
        }

        public event EventHandler Dismissed;

        private void OnDismissed()
        {
            var handler = Dismissed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.AutosizesSubviews = true;

            ImageView.Image = Images.Logos.Enterprise;
            ImageView.Layer.CornerRadius = 24f;
            ImageView.Layer.MasksToBounds = true;

            CancelButton.SetBackgroundImage(Images.Buttons.GreyButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);
            CancelButton.TintColor = UIColor.Black;
            CancelButton.Layer.ShadowColor = UIColor.Black.CGColor;
            CancelButton.Layer.ShadowOffset = new SizeF(0, 1);
            CancelButton.Layer.ShadowOpacity = 0.3f;
            CancelButton.TouchUpInside += (sender, e) => DismissViewController(true, OnDismissed);

            EnableButton.SetBackgroundImage(Images.Buttons.BlackButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);
            EnableButton.TintColor = UIColor.White;
            EnableButton.Layer.ShadowColor = UIColor.Black.CGColor;
            EnableButton.Layer.ShadowOffset = new SizeF(0, 1);
            EnableButton.Layer.ShadowOpacity = 0.3f;
            EnableButton.TouchUpInside += EnablePushNotifications;

            _hud = new Hud(View);

            GetPrices();
        }

        private async void GetPrices()
        {
            try
            {
                var productData = await InAppPurchases.RequestProductData(FeatureIds.EnterpriseSupport);
                if (productData.Products == null || productData.Products.Length == 0)
                    return;
                EnableButton.SetTitle("Yes! (" + productData.Products[0].LocalizedPrice() + ")", UIControlState.Normal);
            }
            catch
            {
            }
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            ContainerView.Frame = new RectangleF(View.Bounds.Width / 2 - ContainerView.Frame.Width / 2, 
                View.Bounds.Height / 2 - ContainerView.Frame.Height / 2, 
                ContainerView.Frame.Width, ContainerView.Frame.Height);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            InAppPurchases.Instance.PurchaseSuccess += HandlePurchaseSuccess;
            InAppPurchases.Instance.PurchaseError += HandlePurchaseError;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            InAppPurchases.Instance.PurchaseSuccess -= HandlePurchaseSuccess;
            InAppPurchases.Instance.PurchaseError -= HandlePurchaseError;
        }

        void HandlePurchaseError (object sender, Exception e)
        {
            _hud.Hide();
        }

        void HandlePurchaseSuccess (object sender, string e)
        {
            _hud.Hide();
            DismissViewController(true, OnDismissed);
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait;
            return base.GetSupportedInterfaceOrientations();
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientation.Portrait;
            return base.PreferredInterfaceOrientationForPresentation();
        }

        private void EnablePushNotifications(object sender, EventArgs e)
        {
            _hud.Show("Enabling...");
            var featureService = Mvx.Resolve<IFeaturesService>();
            featureService.Activate(FeatureIds.EnterpriseSupport);
        }
    }
}

