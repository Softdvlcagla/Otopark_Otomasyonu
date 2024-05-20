using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Otopark_Otomasyonu
{
    public partial class Form_Otopark : Form
    {
        public Form_Otopark()
        {
            InitializeComponent();
        }
                        string bagcum = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=c:otoparkotomasyonu.mdb"; // otoparkotomasyonu.mdb veri tabanımızı tanımladım

                        // global bağlantı değişkenlerimi buraya atadım
                        OleDbConnection bag;
                        OleDbConnection con;
                        OleDbDataAdapter da;
                        OleDbCommand cmd;
                        OleDbDataReader dr;
                        DataTable dt;

             void listele() // comboboxlara veri tabanından verileri çekiyorum 
        {
                        bag = new OleDbConnection(bagcum);  // burada bağlantıyı oluşturduk ve açtım
                        bag.Open();
                        da = new OleDbDataAdapter("select * from parkyerleri where durum = 'Boş'", bag); // park yerleri tablosunda durumu boş olanları verir
                        dt = new DataTable();
                        da.Fill(dt);

                        comboBox_Nereye.DataSource = dt; // comboBox_Nereye adlı comboya getiriyorum
                        comboBox_Nereye.ValueMember = "id";
                        comboBox_Nereye.DisplayMember = "id";
                        bag.Close();

                        bag = new OleDbConnection(bagcum);  // burada bağlantıyı oluşturduk ve açtım
                        bag.Open();
                        // Burda da durumu dolu olanları ve park_edilen_yeri park yerleri tablosunda id ye eşit olanları comboBox_Cikan_Arac combosuna listeledim
                        da = new OleDbDataAdapter("select * from parkyerleri,arac where parkyerleri.durum = 'Dolu' and arac.park_edilen_yer=parkyerleri.id", bag);
                        dt = new DataTable();
                        da.Fill(dt);

                        comboBox_Cikan_Arac.DataSource = dt;
                        comboBox_Cikan_Arac.ValueMember = "id";
                        comboBox_Cikan_Arac.DisplayMember = "arac_plaka";
                        bag.Close();
        }

                 void yenile() // yenile fonksiyonum
        {
                            foreach (Control item in panel3.Controls) // panel3 teki tüm elemanları gez
             {
                            if (item is Button) // buton olanların
                 {
                            if (item.Name.ToString()!= "button_programi_kapat") // Sağ üstteki kapat butonu hariç
                      {
                            item.BackColor = Color.DarkSlateGray; // arkaplanını DarkSlateGray
                            item.ForeColor = Color.DarkGray; // yazı rengini DarkGray
                            item.Text = item.Name.ToString(); // üstündeki yazıyı namesi olarak değiştir.
                            // namesi dediğim burda park yerleri tablosunda ki A1,A2 gibi numaralar
                        }
                    }
                 }
                            con = new OleDbConnection(bagcum); 
                            con.Open();
                            cmd = new OleDbCommand("select * from arac", con); // arac tablosundaki bilgileri getir
                            dr = cmd.ExecuteReader(); // bilgileri datareader ile oku
                            while (dr.Read())// bilgileri datareader ile oku
            {
                            foreach (Control item in panel3.Controls) // panel3 teki her iteme bak
                {
                            if (item is Button)
                    {
                             if (dr["park_edilen_yer"].ToString() == item.Name.ToString()) // eğer veritabanında park_edilen_yer ekrandaki butonlardan birinin namesine eşitse
                        {
                            item.BackColor = Color.Maroon; // arkaplanını Maroon
                            item.ForeColor = Color.FloralWhite; // arkaplanını Maroon
                            item.Text = dr["arac_plaka"].ToString(); //o butonunu üstündeki yazıyı plakaya çevir
                        }
                    }
                }
            }
            con.Close();
        }
        private void Form_Otopark_Load(object sender, EventArgs e)
        {
                    listele(); 
                    // form açılırken listele ve yenile fonk. çağırdım
                    yenile();
                    foreach (Control item in panel3.Controls) // panel 3 te ki butonlara tıklama eventı verdim
            {
                        if (item is Button)
                {
                        if (item.Name.ToString() != "button_programi_kapat") // kapat butonu hariç
                    {
                        item.Click += butonlara_tıklanınca; // tüm butonlara tıklama eventı verdim
                    }
                }
            }
        }

        private void butonlara_tıklanınca(object sender, EventArgs e) // butonlara tıklanınca
        {
            Button b = sender as Button; // hangi butona tıklantığını tespit ettik
                         if (b.BackColor==Color.DarkSlateGray) // arkaplanı DarkSlateGray yani oraya park edilmemişse
                {
                         for (int i = 0; i < comboBox_Nereye.Items.Count; i++) // combobox_nereye combosunda
                    {
                           comboBox_Nereye.SelectedIndex = i;
                        if (b.Name.ToString() == comboBox_Nereye.SelectedValue.ToString()) //seçilen butonun namesi atıyorum A1 comboboxta varsa onu seçili hale getiriyorum
                        {
                           comboBox_Nereye.SelectedIndex = i;
                           break;
                    }
                }           // yani bu komut tıkladığın butonu comboboxta seçiyor
            }
            else
            {
                        for (int i = 0; i < comboBox_Cikan_Arac.Items.Count; i++)
                    {   
                        comboBox_Cikan_Arac.SelectedIndex = i;
                        if (b.Name.ToString() == comboBox_Cikan_Arac.SelectedValue.ToString())
                    {
                            comboBox_Cikan_Arac.SelectedIndex = i;
                            break;
                    }
                }  // yani bu komut tıkladığın butonu comboboxta seçiyor tabi buda sadece arkaplanı Maroon olanlar için
            }
        }

        private void button_aracgirisi_Click(object sender, EventArgs e) // araç girişi yap butonuna tıklandığında
        {
            if (textBox_plaka.Text != "" || textBox_arac_sahibi.Text != "") // textboxlar boş değilse
            {
                try
                {
                        con = new OleDbConnection(bagcum);
                        con.Open();
                        cmd = new OleDbCommand("insert into arac(arac_plaka, arac_sahibi, arac_giris_saati, park_edilen_yer)values('" + textBox_plaka.Text.Trim() + "','" + textBox_arac_sahibi.Text.Trim() + "','" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "','" + comboBox_Nereye.SelectedValue.ToString() + "')", con);
                        cmd.ExecuteNonQuery();
                        // veri tabanına ekle textboxtaki bilgileri
                        con.Close();
                        con = new OleDbConnection(bagcum);
                        con.Open();
                        // eklediğimiz yerin yani park edilen numaranın boş mu dolu mu durumunu dolu yaptım
                        cmd = new OleDbCommand("update parkyerleri set durum='Dolu' where id='" + comboBox_Nereye.SelectedValue.ToString() +"'", con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                 }
                  catch (Exception ex)
                  {
                      MessageBox.Show("Hata oluştu" + ex);
                  }
                        textBox_arac_sahibi.Text = "";
                        textBox_plaka.Text = "";
                        yenile();
                        listele();
            }
                        else MessageBox.Show("Boş alan bırakmayınız. ");

                        listele();
                        yenile();
        }

        private void button_arac_cikisi_Click(object sender, EventArgs e)
           {
                    con = new OleDbConnection(bagcum);
                    con.Open();
                    cmd = new OleDbCommand("delete from arac where arac_plaka='" + comboBox_Cikan_Arac.Text + "'", con);
                    cmd.ExecuteNonQuery();
                    //arac tablosundan çıkış yaptığımız plakalı aracı sildim
                    con.Close();
                    con = new OleDbConnection(bagcum);
                    con.Open();
                    // park çıkışı yapılan yerin boş mu dolu mu durumunu Boş yaptım
                    cmd = new OleDbCommand("update parkyerleri set durum='Boş' where id='" + comboBox_Cikan_Arac.SelectedValue.ToString() + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    yenile();
                    listele();

        }

        private void comboBox_Cikan_Arac_SelectedIndexChanged(object sender, EventArgs e)
            {
                        con = new OleDbConnection(bagcum);
                        con.Open();
                        //arac saatini getirmek için aracların bilgilerini çektim
                        cmd = new OleDbCommand("select * from arac where arac_plaka='" + comboBox_Cikan_Arac.Text + "'", con);
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                  {
                        label_arac_giris_saati.Text = "Araç Giriş Saati \n" + dr["arac_giris_saati"].ToString(); // label arac saatine aracın saatini çektim
                 }
                        con.Close();
        }
        private void button_programi_kapat_Click(object sender, EventArgs e)
        {
                   this.Dispose(); // programı kapat butonu
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
