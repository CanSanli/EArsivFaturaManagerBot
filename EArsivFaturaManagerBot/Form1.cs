using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Windows.Forms;
using SeleniumExtras.WaitHelpers;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace EArsivFaturaManagerBot
{
    public partial class Form1 : Form
    {
        private const string LoginUrl = "https://earsivportal.efatura.gov.tr/intragiris.html";
        private const string DateFormat = "dd.MM.yyyy";
        static string ExcelFilePath = string.Empty;
        static string StartDate = string.Empty;
        static string FinishDate = string.Empty;
        static string VKN = string.Empty;
        public static string[,] UserList;

        private bool IsExcelFileSelected = false;
        private bool IsStartDateSelected = false;
        private bool IsFinishDateSelected = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            FormatDtp(dtpStartDate, DateFormat);
            FormatDtp(dtpFinishDate, DateFormat);

            
        }


        private void bBotuCalistir_Click(object sender, EventArgs e)
        {
            CreateTxtFile();
            string chromeDriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromedriver.exe");
            ChromeOptions chromeOptions = new ChromeOptions();

            ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromeDriverPath);
            service.HideCommandPromptWindow = true;


            StartDate = dtpStartDate.Text;
            FinishDate = dtpFinishDate.Text;

            Task task = Task.Run(() => GetUserListFromExcel());
            task.Wait();

            if (UserList != null)
            {
               
                for (int i = 0; i < UserList.GetLength(0); i++)
                {
                    lAccountCounter.Text = (i+1) + "/" + UserList.GetLength(0);

                    string username = UserList[i, 0];

                    string password = UserList[i, 1];



                    // WebDriver'ı başlat
                    IWebDriver driver = new ChromeDriver(service, chromeOptions);
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                    // E-Fatura Portalı sayfasını aç
                    driver.Navigate().GoToUrl(LoginUrl);

                    By elementLocator = By.Id("userid");
                    wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));
                    // Kullanıcı adı ve şifreyi doldur
                    driver.FindElement(By.Id("userid")).SendKeys(username);
                    driver.FindElement(By.Id("password")).SendKeys(password);

                    // Giriş yap butonuna tıkla
                    driver.FindElement(By.CssSelector("button[onclick='assosLogin()']")).Click();
                    Thread.Sleep(3000);


                    if (driver.Url.Contains("https://earsivportal.efatura.gov.tr/index.jsp"))
                    {
                        MessageBox.Show("Giriş Başarılı", "Oturum Açıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        System.Threading.Thread.Sleep(2000);
                        elementLocator = By.Id("gen__1006");
                        wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));

                        IWebElement selectElement = driver.FindElement(By.Id("gen__1006"));
                        selectElement.Click();
                        IList<IWebElement> options = selectElement.FindElements(By.TagName("option"));
                        options[1].Click();
                        System.Threading.Thread.Sleep(500);
                        //fatura bilgi sorgulama sayfa seçimi

                        IWebElement ulElement = driver.FindElement(By.Id("gen__1016-tree_ul"));

                        // soldaki menüden fatura sorgulamaya geldik
                        IList<IWebElement> liElements = ulElement.FindElements(By.TagName("li")); // Değiştirilecek TagName
                        System.Threading.Thread.Sleep(500);
                        // Belirli bir <li> elemanına tıkla (örneğin, 2. <li> elemanına tıkla)
                        liElements[1].Click();
                        IList<IWebElement> liElements2 = liElements[1].FindElements(By.TagName("li"));
                        liElements2[1].Click();

                        //Başlangıç tarihi ayarlandı
                        elementLocator = By.Id("date-gen__1024");
                        wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));

                        System.Threading.Thread.Sleep(1000);
                        var startDatePicker = driver.FindElement(By.Id("date-gen__1024"));
                        startDatePicker.Clear();
                        System.Threading.Thread.Sleep(500);
                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                        jsExecutor.ExecuteScript($"arguments[0].value = '{StartDate}';", startDatePicker);
                        System.Threading.Thread.Sleep(500);
                        //Bitiş tarihi ayarlandı
                        var finishDatePicker = driver.FindElement(By.Id("date-gen__1025"));
                        finishDatePicker.Clear();
                        System.Threading.Thread.Sleep(500);
                        jsExecutor.ExecuteScript($"arguments[0].value = '{FinishDate}';", finishDatePicker);
                        //Sorgula butonu tıkla
                        driver.FindElement(By.Id("gen__1026")).Click();
                        System.Threading.Thread.Sleep(2000);


                        //her sayfa için indirme işlemi
                        elementLocator = By.CssSelector("#gen__1036-div > span:nth-child(5)");
                        wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));
                        var tableFooter = driver.FindElement(By.CssSelector("#gen__1036-div > span:nth-child(5)"));

                        int pageCount = tableFooter.Text[1] - (byte)'0'; //tablodaki sayfa sayısını aldım

                        for (int j = 0; j < pageCount; j++)
                        {

                            Task task2 = Task.Run(() => DownloadPage(driver));
                            task2.Wait();
                            Thread.Sleep(1000);
                            if (j < pageCount - 1)
                            {
                                driver.FindElement(By.CssSelector("#gen__1036-div > span.csc-table-paging-btn.csc-table-seek-next")).Click();
                            }
                            Thread.Sleep(500);




                        }
                        Task t = Task.Run(() => { driver.FindElement(By.CssSelector("#gen__1008")).Click(); });
                        t.Wait();
                        driver.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Giriş başarısız! Kullanıcı verileri: {username} , {password}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        WriteTxtFile(username + " " + password);
                        driver.Close();
                    }
                }
            }

        }

        public static void DownloadPage(IWebDriver driver)
        {
            //Tablodaki satırları seç
            IWebElement table = driver.FindElement(By.Id("gen__1036-t"));

            IWebElement tbody = table.FindElement(By.TagName("tbody"));

            // Body içindeki satırları bul
            IList<IWebElement> rows = tbody.FindElements(By.TagName("tr"));
            System.Threading.Thread.Sleep(500);
            // Her satır için işlem yap
            foreach (var row in rows)
            {

                var deletedCheck = row;
                var iptalCheck = row;
                var vknCheck = row;
                // Satırdaki hücreleri bul
                IList<IWebElement> cells = row.FindElements(By.TagName("td"));
                System.Threading.Thread.Sleep(500);
                if (cells.Count > 1)
                {

                    iptalCheck = cells[7].FindElement(By.TagName("span"));


                    deletedCheck = cells[6].FindElement(By.TagName("i"));

                    vknCheck = cells[2].FindElement(By.TagName("span"));
                }



                foreach (var cell in cells)
                {

                    if (cell.GetAttribute("class") == "csc-table-select" && iptalCheck.Text == "----------" && deletedCheck.GetAttribute("class") != "fa fa-remove")
                    {
                        if (VKN != string.Empty && vknCheck.Text == VKN)
                        {
                            cell.FindElement(By.TagName("input")).Click();
                            System.Threading.Thread.Sleep(500);
                            driver.FindElement(By.Id("gen__1033")).Click();
                            System.Threading.Thread.Sleep(750);
                            cell.FindElement(By.TagName("input")).Click();
                            System.Threading.Thread.Sleep(500);
                        }
                        else if (VKN == string.Empty)
                        {
                            cell.FindElement(By.TagName("input")).Click();
                            System.Threading.Thread.Sleep(500);
                            driver.FindElement(By.Id("gen__1033")).Click();
                            System.Threading.Thread.Sleep(3000);
                            cell.FindElement(By.TagName("input")).Click();
                            System.Threading.Thread.Sleep(500);
                        }

                    }
                    // Burada hücre içeriğiyle yapmak istediğiniz işlemleri gerçekleştirebilirsiniz
                }


            }



        }

        private void FormatDtp(DateTimePicker dtp, string format)
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = format;

        }
        public static void GetUserListFromExcel()
        {

            // Excel dosyasının yolunu belirtin
            string excelFilePath = ExcelFilePath;

            if (File.Exists(excelFilePath))
            {
                using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs); // Excel dosyasını aç

                    ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını al
                    UserList = new string[sheet.LastRowNum, 2];
                    // Satırları dolaş
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            // Hücreleri dolaş
                            for (int j = 0; j < 2; j++)
                            {
                                ICell cell = row.GetCell(j);
                                if (cell != null)
                                {
                                    UserList[i - 1, j] = cell.ToString();

                                }
                            }

                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("Belirtilen excel dosyası bulunamadı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public static void CreateTxtFile()
        {
            string dosyaAdi = "BasarisizGirisler.txt";


            if (File.Exists(dosyaAdi))
            {
                File.WriteAllText(dosyaAdi, string.Empty);

            }
            else
            {
                File.Create(dosyaAdi).Close();

            }




        }
        public static void WriteTxtFile(string userData)
        {
            string dosyaAdi = "BasarisizGirisler.txt";

            File.AppendAllText(dosyaAdi, userData + Environment.NewLine);


        }

        private void bExcelDosyasiSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog dosyaSec = new OpenFileDialog();

            dosyaSec.Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm";

            // Kullanıcıya açılacak olan pencerenin başlığını belirleyebilirsiniz
            dosyaSec.Title = "Excel Dosyası Seç";

            if (dosyaSec.ShowDialog() == DialogResult.OK)
            {

                ExcelFilePath = dosyaSec.FileName;
                lAccountCounter.Text = ExcelFilePath;
                IsExcelFileSelected = true;
                CheckBotButtonState();
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            IsStartDateSelected = true;
            CheckBotButtonState();
        }

        private void dtpFinishDate_ValueChanged(object sender, EventArgs e)
        {
            IsFinishDateSelected = true;
            CheckBotButtonState();
        }

        private void CheckBotButtonState()
        {
            if (IsStartDateSelected && IsFinishDateSelected && IsExcelFileSelected)
            {
                bBotuCalistir.Enabled = true;
            }
        }

        private void cbVknFilterCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVknFilterCheck.Checked)
            {
                tbVKN.Enabled = true;

            }
            else
            {
                tbVKN.Enabled = false;
                tbVKN.Clear();

            }

        }

        private void tbVKN_TextChanged(object sender, EventArgs e)
        {
            VKN = tbVKN.Text;
        }
    }
}
