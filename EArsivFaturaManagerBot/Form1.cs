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
using System.IO.Compression;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.Diagnostics;

//OpenQA.Selenium.Chrome
//SeleniumExtras
//NPOI
//OpenXML
//XML Linq

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
        //xmltoexcel
        public static string startFolder = Directory.GetCurrentDirectory(); // Programın başlangıç dizini
        static string sourceFolder = Path.Combine(startFolder, "Faturalar"); // Kaynak klasör
        static string targetFolder = Path.Combine(startFolder, "xmls"); // Hedef klasör
        static List<string> XmlFiles;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            FormatDtp(dtpStartDate, DateFormat);
            FormatDtp(dtpFinishDate, DateFormat);
            bListele.Enabled = false;


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
                    lAccountCounter.Text = (i + 1) + "/" + UserList.GetLength(0);

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

                    if (cell.GetAttribute("class") == "csc-table-select" && iptalCheck.Text == "----------"
                        && deletedCheck.GetAttribute("class") != "fa fa-remove")
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
                tbVKN.Clear();
                tbVKN.Enabled = false;


            }

        }

        private void tbVKN_TextChanged(object sender, EventArgs e)
        {
            VKN = tbVKN.Text;
        }

        private void bAyıkla_Click(object sender, EventArgs e)
        {
            bKaydet.Enabled = true;
            
            string[] zipFiles = Directory.GetFiles(sourceFolder, "*.zip");
            if (zipFiles.Length==0)
            {
                MessageBox.Show("Faturalar klasörü boş", "Fatura Bulunamadı", MessageBoxButtons.OK,MessageBoxIcon.Error);
                bKaydet.Enabled = false;
                bListele.Enabled = false;
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder); // "xmls" klasörünü oluştur
            }

            foreach (string zipFile in zipFiles)
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFile))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            string destinationPath = Path.Combine(targetFolder, Path.GetFileName(entry.FullName));
                            if (!File.Exists(destinationPath))
                            {
                                entry.ExtractToFile(destinationPath);
                            }


                        }
                    }
                }
            }
            MessageBox.Show("XML dosyaları başarıyla çıkartıldı ve 'xmls' klasörüne yerleştirildi.");
            bListele.Enabled = true;
        }

        private void bListele_Click(object sender, EventArgs e)
        {

            lbFaturalar.Items.Clear();
            if (Directory.Exists(targetFolder))
            {
                string[] xmlFiles = Directory.GetFiles(targetFolder, "*.xml");
                XmlFiles = new List<string>();
                if (xmlFiles.Length > 0)
                {
                    foreach (string xmlFile in xmlFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(xmlFile); // Dosya adını al (uzantısız)
                        XmlFiles.Add(xmlFile);
                        lbFaturalar.Items.Add(fileName); // Dosya yollarını bir ListBox veya başka bir kontrolde listeleyebilirsiniz.
                    }
                }
                else
                {
                    MessageBox.Show("Klasörde .xml uzantılı dosya bulunamadı.");
                }
            }
            else
            {
                MessageBox.Show("Belirtilen klasör bulunamadı.");
            }
            if (lbFaturalar.Items.Count>0)
            {
                lbFaturalar.SelectedIndex = 0;
            }
            lXmlSayisi.Text = lbFaturalar.Items.Count.ToString();
        }
        private Fatura GetFatura(XNamespace cac, XNamespace cbc, XDocument xmlDoc) ///!!!sil
        {

            Fatura fatura = new Fatura();
            fatura.FaturaHatlari = new List<FaturaHatti>();
            var supplierParty = xmlDoc.Descendants(cac + "AccountingSupplierParty").FirstOrDefault();
            var customerParty = xmlDoc.Descendants(cac + "AccountingCustomerParty").FirstOrDefault();
            fatura.tevkifatTutarı = "0";
            fatura.faturaNo = (string)xmlDoc.Descendants(cbc + "ID").FirstOrDefault();
            fatura.faturaTarihi = (string)xmlDoc.Descendants(cbc + "IssueDate").FirstOrDefault();
            DateTime parsedDate = DateTime.Parse(fatura.faturaTarihi);
            string formattedDate = parsedDate.ToString("dd.MM.yyyy"); ///!!!!!!!!
            fatura.faturaTarihi = formattedDate;
            fatura.faturaTipi = (string)xmlDoc.Descendants(cbc + "InvoiceTypeCode").FirstOrDefault();
            fatura.supplierAd = (string)supplierParty.Descendants(cac + "Person").Descendants(cbc + "FirstName").FirstOrDefault();
            fatura.supplierSoyad = (string)supplierParty.Descendants(cac + "Person").Descendants(cbc + "FamilyName").FirstOrDefault();
            fatura.supplierTcKimlikNo = (string)supplierParty.Descendants(cac + "PartyIdentification")
                .Where(e => e.Element(cbc + "ID")?.Attribute("schemeID")?.Value == "TCKN")
                .Select(e => (string)e.Element(cbc + "ID")).FirstOrDefault();
            fatura.customerCompanyName = (string)customerParty.Descendants(cac + "PartyName").Descendants(cbc + "Name").FirstOrDefault();
            fatura.customerVKN = (string)customerParty.Descendants(cac + "PartyIdentification")
                .Where(e => e.Element(cbc + "ID")?.Attribute("schemeID")?.Value == "VKN")
                .Select(e => (string)e.Element(cbc + "ID")).FirstOrDefault();

            foreach (var invoiceLine in xmlDoc.Descendants(cac + "InvoiceLine"))
            {
                FaturaHatti faturaHatti = new FaturaHatti();
                faturaHatti.Id = (string)invoiceLine.Descendants(cbc + "ID").FirstOrDefault();
                faturaHatti.Name = (string)invoiceLine.Descendants(cac + "Item").Descendants(cbc + "Name").FirstOrDefault();
                faturaHatti.TotalAmount = (string)invoiceLine.Descendants(cac + "TaxSubtotal").Descendants(cbc + "TaxableAmount").FirstOrDefault();
                var withholdingTaxTotal = invoiceLine.Descendants(cac + "WithholdingTaxTotal").FirstOrDefault();
                if (withholdingTaxTotal != null)
                {
                    faturaHatti.Tevkifat = true;
                    fatura.tevkifatTutarı = (Convert.ToDouble(fatura.tevkifatTutarı, new CultureInfo("en-US")) + 
                        Convert.ToDouble(faturaHatti.TotalAmount, new CultureInfo("en-US"))).ToString(new CultureInfo("en-US"));
                }
                else
                {
                    faturaHatti.Tevkifat = false;
                }

                fatura.FaturaHatlari.Add(faturaHatti);
            }
            fatura.taxInclusiveAmount = (string)xmlDoc.Descendants(cbc + "TaxInclusiveAmount").FirstOrDefault();
            fatura.payableAmount = (string)xmlDoc.Descendants(cbc + "PayableAmount").FirstOrDefault();
            fatura.kdvUcreti = (string)xmlDoc.Descendants(cac + "WithholdingTaxTotal").Descendants(cbc + "TaxAmount").FirstOrDefault();
            fatura.kdvDahilIslemUcreti = (string)xmlDoc.Descendants(cac + "WithholdingTaxTotal")
                .Descendants(cac + "TaxSubtotal").Descendants(cbc + "TaxableAmount").FirstOrDefault();
            fatura.kdvTevkifatUcreti = (string)xmlDoc.Descendants(cac + "WithholdingTaxTotal")
                .Descendants(cbc + "TaxAmount").FirstOrDefault();
            fatura.hesaplananKDV = (string)xmlDoc.Descendants(cac + "TaxTotal").Descendants(cac + "TaxSubtotal")
                .Descendants(cbc + "TaxAmount").Where(element => (string)element.Attribute("currencyID") == "TRY").FirstOrDefault();
            fatura.toplamUcret = (string)xmlDoc.Descendants(cac + "TaxTotal")
                .Descendants(cac + "TaxSubtotal").Descendants(cbc + "TaxableAmount").Where(element => 
                (string)element.Attribute("currencyID") == "TRY").FirstOrDefault();
            return fatura;
        }

        private void bKaydet_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(startFolder, "Faturalar.xlsx");

            using (var document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                // Çalışma kitabı ve çalışma sayfasını oluşturun
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Çalışma sayfasını çalışma kitabına ekleyin
                var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Faturalar"
                });

                // Çalışma sayfasının verilerini alın
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Başlık satırı oluşturun ve verileri ekleyin
                var row = new Row();
                row.Append(
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("FATURA DÜZENLEYEN UNVAN") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("FATURA DÜZENLEYEN VERGİ KİMLİK NO/TC KİMLİK NO") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("FATURA TARİHİ") },
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("FATURA NO") },
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("FATURA TİPİ") },
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("ADINA FATURA DÜZENLENEN UNVAN") },
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("ADINA FATURA DÜZENLENEN VERGİ KİMLİK NO/TC KİMLİK NO") }
                );
                for (int i = 0; i < 10; i++)
                {
                    row.Append(
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue("MAL / HİZMET ADI  " + (i + 1)) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue("MAL / HİZMET TOPLAM TUTARI " + (i + 1)) }
                        );
                }
                row.Append(
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("HESAPLANAN KDV") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("HESAPLANAN KDV TEVKİFAT") },
                    new Cell() { DataType = CellValues.String, CellValue = 
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("TEVKİFATA TABİ İŞLEM TUTARI") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("VERGİLER DAHİL TOPLAM TUTAR") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("MAL/HİZMET TOPLAM TUTAR") },
                    new Cell() { DataType = CellValues.String, CellValue =
                    new DocumentFormat.OpenXml.Spreadsheet.CellValue("ÖDENECEK TUTAR") }
                );

                // Başlık satırını çalışma sayfasına ekleyin
                sheetData.AppendChild(row);

                // Veri satırı oluşturun ve verileri ekleyin
                for (int i = 0; i < XmlFiles.Count; i++)
                {


                    XDocument xmlDoc = XDocument.Load(XmlFiles[i]);
                    XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                    XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                    Fatura fatura = GetFatura(cac, cbc, xmlDoc);

                    //Excele ekle

                    row = new Row();
                    row.Append(
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.supplierAd + " " + fatura.supplierSoyad) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.supplierTcKimlikNo) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.faturaTarihi) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.faturaNo) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.faturaTipi) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.customerCompanyName) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.customerVKN) }
                    );
                    for (int j = 0; j < fatura.FaturaHatlari.Count; j++)
                    {
                        row.Append(
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.FaturaHatlari[j].Name) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.FaturaHatlari[j].TotalAmount) }
                        );
                    }
                    if (fatura.FaturaHatlari.Count < 10)
                    {
                        int counter = 10 - fatura.FaturaHatlari.Count;
                        for (int j = 0; j < counter; j++)
                        {
                            row.Append(
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue("") },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue("") }
                        );
                        }
                    }
                    row.Append(
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.hesaplananKDV) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.kdvTevkifatUcreti) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.tevkifatTutarı) },
                        new Cell() { DataType = CellValues.String, CellValue =
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.taxInclusiveAmount) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.toplamUcret) },
                        new Cell() { DataType = CellValues.String, CellValue = 
                        new DocumentFormat.OpenXml.Spreadsheet.CellValue(fatura.payableAmount) }
                    );

                    // Veri satırını çalışma sayfasına ekleyin
                    sheetData.AppendChild(row);


                }



            }
            MessageBox.Show("Excel dosyası oluşturuldu.");
        }

        private void lbFaturalar_SelectedIndexChanged(object sender, EventArgs e)
        {


            List<FaturaHatti> Faturalar = new List<FaturaHatti>();
            XDocument xmlDoc = XDocument.Load(XmlFiles[lbFaturalar.SelectedIndex]);
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            Fatura fatura = GetFatura(cac, cbc, xmlDoc); ///!!!sil

            lFaturaNo.Text = fatura.faturaNo;
            lFaturaTip.Text = fatura.faturaTipi;
            lFaturaTarih.Text = fatura.faturaTarihi;

            lTedarikciUnvan.Text = fatura.supplierAd + " " + fatura.supplierSoyad;
            lTedarikciTC.Text = fatura.supplierTcKimlikNo;

            lMusteriUnvan.Text = fatura.customerCompanyName;
            lMusteriVKN.Text = fatura.customerVKN;

            lbFaturaHatti.Items.Clear();
            foreach (var item in fatura.FaturaHatlari)
            {
                lbFaturaHatti.Items.Add(item.Id + "| " + item.Name + "| " + item.TotalAmount + " Tl");
            }

            lVergiDahilToplam.Text = fatura.taxInclusiveAmount;
            lToplamOdenecek.Text = fatura.payableAmount;
            lTevkifataTabiTutar.Text = fatura.tevkifatTutarı;
            lHesaplananKdvTevkifat.Text = fatura.kdvTevkifatUcreti;
            lHesaplananKDV.Text = fatura.hesaplananKDV;
            lMalHizmetToplam.Text = fatura.toplamUcret;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Directory.Exists(targetFolder))
            {
                try
                {
                    Directory.Delete(targetFolder, true); // Klasörü ve içeriğini sil
                    MessageBox.Show("xmls klasörü silindi");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: " + ex.Message);
                }
            }
        }

        private void bFaturalarKlasoru_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(sourceFolder))
            {
                Directory.CreateDirectory(sourceFolder);
            }
            Process.Start("explorer.exe",sourceFolder);
            bAyıkla.Enabled = true;
            bListele.Enabled = true;
        }
    }
}
