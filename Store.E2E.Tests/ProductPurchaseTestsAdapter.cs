using E2E.Load.Core.Attributes;
using E2E.Web.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace AdapterDesignPattern
{
    [TestClass]
    public class ProductPurchaseTestsAdapter : BaseTest
    {
        [TestMethod]
        [LoadTest]
        public void CompletePurchaseSuccessfully_WhenNewClient_TwoItems()
        {
            Driver.GoToUrl("http://demos.bellatrix.solutions/");

            var addToCartFalcon9 = Driver.FindElement(By.CssSelector("[data-product_id*='28']"));
            addToCartFalcon9.Click();
            var viewCartButton = Driver.FindElement(By.CssSelector("[class*='added_to_cart wc-forward']"));
            viewCartButton.Click();

            var couponCodeTextField = Driver.FindElement(By.Id("coupon_code"));
            couponCodeTextField.TypeText("happybirthday");
            var applyCouponButton = Driver.FindElement(By.CssSelector("[value*='Apply coupon']"));
            applyCouponButton.Click();
            var messageAlert = Driver.FindElement(By.CssSelector("[class*='woocommerce-message']"));
            ////Assert.AreEqual("Coupon code applied successfully.", messageAlert.Text);
            messageAlert.EnsuredTextIs("Coupon code applied successfully.");

            var quantityBox = Driver.FindElement(By.CssSelector("[class*='input-text qty text']"));
            quantityBox.TypeText("2");

            var updateCart = Driver.FindElement(By.CssSelector("[value*='Update cart']"));
            updateCart.Click();
            Driver.WaitForAjax();
            var totalSpan = Driver.FindElement(By.XPath("//*[@class='order-total']//span"));
            ////Assert.AreEqual("114.00€", totalSpan.Text);
            totalSpan.EnsuredTextIs("114.00€");

            var proceedToCheckout = Driver.FindElement(By.CssSelector("[class*='checkout-button button alt wc-forward']"));
            proceedToCheckout.Click();

            var billingFirstName = Driver.FindElement(By.Id("billing_first_name"));
            billingFirstName.TypeText("Anton");
            var billingLastName = Driver.FindElement(By.Id("billing_last_name"));
            billingLastName.TypeText("Angelov");
            var billingCompany = Driver.FindElement(By.Id("billing_company"));
            billingCompany.TypeText("Space Flowers");
            var billingCountryWrapper = Driver.FindElement(By.Id("select2-billing_country-container"));
            billingCountryWrapper.Click();
            var billingCountryFilter = Driver.FindElement(By.ClassName("select2-search__field"));
            billingCountryFilter.TypeText("Germany");
            var germanyOption = Driver.FindElement(By.XPath("//*[contains(text(),'Germany')]"));
            germanyOption.Click();
            var billingAddress1 = Driver.FindElement(By.Id("billing_address_1"));
            billingAddress1.TypeText("1 Willi Brandt Avenue Tiergarten");
            var billingAddress2 = Driver.FindElement(By.Id("billing_address_2"));
            billingAddress2.TypeText("Lützowplatz 17");
            var billingCity = Driver.FindElement(By.Id("billing_city"));
            billingCity.TypeText("Berlin");
            var billingZip = Driver.FindElement(By.Id("billing_postcode"));
            billingZip.TypeText("10115");
            var billingPhone = Driver.FindElement(By.Id("billing_phone"));
            billingPhone.TypeText("+00498888999281");
            var billingEmail = Driver.FindElement(By.Id("billing_email"));
            billingEmail.TypeText("info@berlinspaceflowers.com");
            Driver.WaitForAjax();
            var placeOrderButton = Driver.FindElement(By.Id("place_order"));
            placeOrderButton.Click();
            Driver.WaitForAjax();

            var receivedMessage = Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/main/div/header/h1"));
            ////Assert.AreEqual("Order received", receivedMessage.Text);
            receivedMessage.EnsuredTextIs("Order received");
        }

        [TestMethod]
        [LoadTest]
        public void CompletePurchaseSuccessfully_WhenNewClient_SingleItem()
        {
            Driver.GoToUrl("http://demos.bellatrix.solutions/");

            var addToCartFalcon9 = Driver.FindElement(By.CssSelector("[data-product_id*='28']"));
            addToCartFalcon9.Click();
            var viewCartButton = Driver.FindElement(By.CssSelector("[class*='added_to_cart wc-forward']"));
            viewCartButton.Click();

            var totalSpan = Driver.FindElement(By.XPath("//*[@class='order-total']//span"));
            ////Assert.AreEqual("60.00€", totalSpan.Text);
            totalSpan.EnsuredTextIs("60.00€");

            var proceedToCheckout = Driver.FindElement(By.CssSelector("[class*='checkout-button button alt wc-forward']"));
            proceedToCheckout.Click();

            var billingFirstName = Driver.FindElement(By.Id("billing_first_name"));
            billingFirstName.TypeText("Anton");
            var billingLastName = Driver.FindElement(By.Id("billing_last_name"));
            billingLastName.TypeText("Angelov");
            var billingCompany = Driver.FindElement(By.Id("billing_company"));
            billingCompany.TypeText("Space Flowers");
            var billingCountryWrapper = Driver.FindElement(By.Id("select2-billing_country-container"));
            billingCountryWrapper.Click();
            var billingCountryFilter = Driver.FindElement(By.ClassName("select2-search__field"));
            billingCountryFilter.TypeText("Germany");
            var germanyOption = Driver.FindElement(By.XPath("//*[contains(text(),'Germany')]"));
            germanyOption.Click();
            var billingAddress1 = Driver.FindElement(By.Id("billing_address_1"));
            billingAddress1.TypeText("1 Willi Brandt Avenue Tiergarten");
            var billingAddress2 = Driver.FindElement(By.Id("billing_address_2"));
            billingAddress2.TypeText("Lützowplatz 17");
            var billingCity = Driver.FindElement(By.Id("billing_city"));
            billingCity.TypeText("Berlin");
            var billingZip = Driver.FindElement(By.Id("billing_postcode"));
            billingZip.TypeText("10115");
            var billingPhone = Driver.FindElement(By.Id("billing_phone"));
            billingPhone.TypeText("+00498888999281");
            var billingEmail = Driver.FindElement(By.Id("billing_email"));
            billingEmail.TypeText("info@berlinspaceflowers.com");
            Driver.WaitForAjax();
            var placeOrderButton = Driver.FindElement(By.Id("place_order"));
            placeOrderButton.Click();
            Driver.WaitForAjax();

            var receivedMessage = Driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/main/div/header/h1"));
            ////Assert.AreEqual("Order received", receivedMessage.Text);
            receivedMessage.EnsuredTextIs("Order received");
        }
    }
}
