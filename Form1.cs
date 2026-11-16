using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NTP_KeyEventLogger
{
    public partial class Form1 : Form
    {
        // Arayüz elemanlarýný (nesneleri) tanýmlýyoruz
        private ListBox lstKeys;
        private Button btnStart, btnStop, btnClear, btnSave;
        private Label lblStatus;

        // Kayýt durumunu tutacak deðiþken
        private bool isRecording = false;

        public Form1()
        {
            InitializeComponent();
            ArayuzuKur(); // Kendi yazdýðýmýz arayüz metodunu çaðýrýyoruz
        }

        private void ArayuzuKur()
        {
            // Formun temel özellikleri
            this.Text = "YMH219 - Eðitsel Keylogger";
            this.Size = new Size(420, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ÇOK ÖNEMLÝ: Formun tuþ vuruþlarýný diðer nesnelerden önce yakalamasýný saðlar
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // ListBox (Tuþlarýn listeleneceði alan)
            lstKeys = new ListBox() { Location = new Point(20, 20), Size = new Size(360, 250) };

            // Baþlat Butonu
            btnStart = new Button() { Text = "Baþlat", Location = new Point(20, 280), Size = new Size(80, 30) };
            btnStart.Click += (s, e) => { isRecording = true; lblStatus.Text = "Durum: Kaydediyor..."; lblStatus.ForeColor = Color.Green; };

            // Durdur Butonu
            btnStop = new Button() { Text = "Durdur", Location = new Point(110, 280), Size = new Size(80, 30) };
            btnStop.Click += (s, e) => { isRecording = false; lblStatus.Text = "Durum: Bekliyor"; lblStatus.ForeColor = Color.Red; };

            // Temizle Butonu
            btnClear = new Button() { Text = "Temizle", Location = new Point(200, 280), Size = new Size(80, 30) };
            btnClear.Click += (s, e) => lstKeys.Items.Clear();

            // Dosyaya Kaydet Butonu
            btnSave = new Button() { Text = "Kaydet", Location = new Point(290, 280), Size = new Size(90, 30) };
            btnSave.Click += BtnSave_Click;

            // Durum Göstergesi
            lblStatus = new Label() { Text = "Durum: Bekliyor", Location = new Point(20, 330), ForeColor = Color.Red, AutoSize = true };

            // Oluþturduðumuz nesneleri ekrana (Form'a) ekliyoruz
            this.Controls.Add(lstKeys);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnStop);
            this.Controls.Add(btnClear);
            this.Controls.Add(btnSave);
            this.Controls.Add(lblStatus);
        }

        // Klavye tuþuna basýldýðýnda tetiklenen olay
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isRecording)
            {
                // Tarih/Saat formatý ve basýlan tuþun adý
                string kayit = $"[{DateTime.Now:HH:mm:ss}] - Basýlan Tuþ: {e.KeyCode}";
                lstKeys.Items.Add(kayit);

                // Listeye yeni eleman eklendikçe otomatik olarak en aþaðý kaydýr
                lstKeys.TopIndex = lstKeys.Items.Count - 1;
            }
        }

        // Föydeki StreamWriter ile Dosyaya Kaydetme Gereksinimi
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (lstKeys.Items.Count == 0)
            {
                MessageBox.Show("Kaydedilecek tuþ verisi bulunamadý!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Programýn çalýþtýðý klasöre 'TusKayitlari.txt' adýnda dosya oluþturur
                string dosyaYolu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TusKayitlari.txt");

                using (StreamWriter sw = new StreamWriter(dosyaYolu, true))
                {
                    sw.WriteLine($"\n--- Yeni Kayýt Oturumu: {DateTime.Now} ---");
                    foreach (var item in lstKeys.Items)
                    {
                        sw.WriteLine(item.ToString());
                    }
                }
                MessageBox.Show($"Kayýtlar baþarýyla dosyaya eklendi!\nKonum: {dosyaYolu}", "Baþarýlý", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dosya kaydedilirken hata oluþtu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

