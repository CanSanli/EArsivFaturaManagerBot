
namespace EArsivFaturaManagerBot
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbVKN = new System.Windows.Forms.TextBox();
            this.cbVknFilterCheck = new System.Windows.Forms.CheckBox();
            this.bExcelDosyasiSec = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFinishDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lAccountCounter = new System.Windows.Forms.Label();
            this.bBotuCalistir = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(396, 450);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.LightGray;
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.tbVKN);
            this.tabPage1.Controls.Add(this.cbVknFilterCheck);
            this.tabPage1.Controls.Add(this.bExcelDosyasiSec);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.dtpFinishDate);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.dtpStartDate);
            this.tabPage1.Controls.Add(this.lAccountCounter);
            this.tabPage1.Controls.Add(this.bBotuCalistir);
            this.tabPage1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(388, 417);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Fatura Bot";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.MintCream;
            this.tabPage2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 417);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Xml to Excel Converter";
            // 
            // tbVKN
            // 
            this.tbVKN.Enabled = false;
            this.tbVKN.Location = new System.Drawing.Point(9, 310);
            this.tbVKN.Name = "tbVKN";
            this.tbVKN.PlaceholderText = "VKN";
            this.tbVKN.Size = new System.Drawing.Size(249, 27);
            this.tbVKN.TabIndex = 20;
            // 
            // cbVknFilterCheck
            // 
            this.cbVknFilterCheck.AutoSize = true;
            this.cbVknFilterCheck.Location = new System.Drawing.Point(9, 279);
            this.cbVknFilterCheck.Name = "cbVknFilterCheck";
            this.cbVknFilterCheck.Size = new System.Drawing.Size(105, 24);
            this.cbVknFilterCheck.TabIndex = 19;
            this.cbVknFilterCheck.Text = "VKN filtresi";
            this.cbVknFilterCheck.UseVisualStyleBackColor = true;
            // 
            // bExcelDosyasiSec
            // 
            this.bExcelDosyasiSec.Location = new System.Drawing.Point(9, 244);
            this.bExcelDosyasiSec.Name = "bExcelDosyasiSec";
            this.bExcelDosyasiSec.Size = new System.Drawing.Size(249, 29);
            this.bExcelDosyasiSec.TabIndex = 18;
            this.bExcelDosyasiSec.Text = "Excel Dosyası Seç";
            this.bExcelDosyasiSec.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(250, 44);
            this.label3.TabIndex = 17;
            this.label3.Text = "Giriş bilgilerinin bulunduğu excel dosyasını seçin:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "Bitiş Tarihi:";
            // 
            // dtpFinishDate
            // 
            this.dtpFinishDate.Location = new System.Drawing.Point(8, 153);
            this.dtpFinishDate.Name = "dtpFinishDate";
            this.dtpFinishDate.Size = new System.Drawing.Size(250, 27);
            this.dtpFinishDate.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Başlangıç Tarihi:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(8, 100);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(250, 27);
            this.dtpStartDate.TabIndex = 13;
            // 
            // lAccountCounter
            // 
            this.lAccountCounter.AutoSize = true;
            this.lAccountCounter.Enabled = false;
            this.lAccountCounter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lAccountCounter.ForeColor = System.Drawing.Color.OrangeRed;
            this.lAccountCounter.Location = new System.Drawing.Point(143, 381);
            this.lAccountCounter.Name = "lAccountCounter";
            this.lAccountCounter.Size = new System.Drawing.Size(42, 28);
            this.lAccountCounter.TabIndex = 12;
            this.lAccountCounter.Text = "0/0";
            // 
            // bBotuCalistir
            // 
            this.bBotuCalistir.Enabled = false;
            this.bBotuCalistir.Location = new System.Drawing.Point(8, 380);
            this.bBotuCalistir.Name = "bBotuCalistir";
            this.bBotuCalistir.Size = new System.Drawing.Size(129, 29);
            this.bBotuCalistir.TabIndex = 11;
            this.bBotuCalistir.Text = "Botu Çalıştır";
            this.bBotuCalistir.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Palatino Linotype", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(8, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(331, 50);
            this.label4.TabIndex = 21;
            this.label4.Text = "E-Arşiv Fatura Bot";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "E-Arşiv Fatura Manager Bot";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbVKN;
        private System.Windows.Forms.CheckBox cbVknFilterCheck;
        private System.Windows.Forms.Button bExcelDosyasiSec;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFinishDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lAccountCounter;
        private System.Windows.Forms.Button bBotuCalistir;
        private System.Windows.Forms.TabPage tabPage2;
    }
}

